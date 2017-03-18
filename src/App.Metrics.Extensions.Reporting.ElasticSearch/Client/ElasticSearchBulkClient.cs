// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class ElasticSearchBulkClient
    {
        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private static TimeSpan _backOffPeriod;

        private readonly HttpClient _httpClient;
        private readonly ILogger<ElasticSearchBulkClient> _logger;

        public ElasticSearchBulkClient(
            ILoggerFactory loggerFactory,
            ElasticSearchSettings elasticSearchSettings)
            : this(
                loggerFactory,
                elasticSearchSettings,
                new HttpPolicy())
        {
        }

        public ElasticSearchBulkClient(
            ILoggerFactory loggerFactory,
            ElasticSearchSettings elasticSearchSettings,
            HttpPolicy httpPolicy,
            HttpMessageHandler httpMessageHandler = null)
        {
            if (elasticSearchSettings == null)
            {
                throw new ArgumentNullException(nameof(elasticSearchSettings));
            }

            if (httpPolicy == null)
            {
                throw new ArgumentNullException(nameof(httpPolicy));
            }

            _httpClient = CreateClient(elasticSearchSettings, httpPolicy);
            _backOffPeriod = httpPolicy.BackoffPeriod;
            _failuresBeforeBackoff = httpPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
            _logger = loggerFactory.CreateLogger<ElasticSearchBulkClient>();
        }

        public async Task<bool> WriteAsync(
            BulkPayload payload,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (NeedToBackoff())
            {
                _logger.LogTrace("Too many failures in writing to ElasticSearch, Circuit Opened");
                return false;
            }

            using (var stream = new MemoryStream())
            {
                payload.Write(new StreamWriter(stream));
                var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    var response = await _httpClient.PostAsync(
                        "/_bulk",
                        content,
                        cancellationToken).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        Interlocked.Increment(ref _failureAttempts);

                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var errorMessage = $"Failed to write to ElasticSearch - StatusCode: {response.StatusCode} Reason: {body}";
                        _logger.LogError(LoggingEvents.ElasticSearchWriteError, errorMessage);

                        return false;
                    }

                    _logger.LogTrace("Successful write to ElasticSearch");

                    return true;
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref _failureAttempts);
                    _logger.LogError(LoggingEvents.ElasticSearchWriteError, "Failed to write to ElasticSearch", ex);
                    return false;
                }
            }
        }

        private HttpClient CreateClient(ElasticSearchSettings settings, HttpPolicy httpPolicy)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = settings.Address,
                Timeout = httpPolicy.Timeout
            };

            switch (settings.AuthorizationSchema)
            {
                case ElasticSearchAuthorizationSchemas.Anonymous:
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", settings.BearerToken);
                    break;
                case ElasticSearchAuthorizationSchemas.Basic:
                    var byteArray = Encoding.ASCII.GetBytes($"{settings.UserName}:{settings.Password}");
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    break;
                case ElasticSearchAuthorizationSchemas.BearerToken:
                    break;
                default:
                    throw new NotImplementedException($"The specified schema {settings.AuthorizationSchema} is not implemented");
            }

            return httpClient;
        }

        private bool NeedToBackoff()
        {
            if (Interlocked.Read(ref _failureAttempts) < _failuresBeforeBackoff)
            {
                return false;
            }

            _logger.LogError($"ElasticSearch client write backoff for {_backOffPeriod.Seconds} seconds");

            if (Interlocked.Read(ref _backOffTicks) == 0)
            {
                Interlocked.Exchange(ref _backOffTicks, DateTime.UtcNow.Add(_backOffPeriod).Ticks);
            }

            if (DateTime.UtcNow.Ticks <= Interlocked.Read(ref _backOffTicks))
            {
                return true;
            }

            Interlocked.Exchange(ref _failureAttempts, 0);
            Interlocked.Exchange(ref _backOffTicks, 0);

            return false;
        }
    }
}
