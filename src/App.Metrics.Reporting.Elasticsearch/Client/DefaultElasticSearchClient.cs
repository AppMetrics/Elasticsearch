// <copyright file="DefaultElasticSearchClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Extensions.Reporting.ElasticSearch;
using App.Metrics.Logging;

namespace App.Metrics.Reporting.Elasticsearch.Client
{
    public class DefaultElasticSearchClient : IElasticsearchClient
    {
        private static readonly ILog Logger = LogProvider.For<DefaultElasticSearchClient>();

        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private static TimeSpan _backOffPeriod;

        private readonly HttpClient _httpClient;

        public DefaultElasticSearchClient(
            HttpPolicy httpPolicy,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _backOffPeriod = httpPolicy?.BackoffPeriod ?? throw new ArgumentNullException(nameof(httpPolicy));
            _failuresBeforeBackoff = httpPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
        }

        public async Task<ElasticsearchWriteResult> WriteAsync(
            string payload,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return new ElasticsearchWriteResult(true);
            }

            if (NeedToBackoff())
            {
                return new ElasticsearchWriteResult(false, "Too many failures in writing to Elasticsearch, Circuit Opened");
            }

            try
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/_bulk", content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Interlocked.Increment(ref _failureAttempts);

                    var errorMessage = $"Failed to write to Elasticsearch - StatusCode: {response.StatusCode} Reason: {response.ReasonPhrase}";
                    Logger.Error(errorMessage);

                    return new ElasticsearchWriteResult(false, errorMessage);
                }

                Logger.Trace("Successful write to Elasticsearch");

                return new ElasticsearchWriteResult(true);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failureAttempts);
                Logger.Error(ex, "Failed to write to Elasticsearch");
                return new ElasticsearchWriteResult(false, ex.ToString());
            }
        }

        private bool NeedToBackoff()
        {
            if (Interlocked.Read(ref _failureAttempts) < _failuresBeforeBackoff)
            {
                return false;
            }

            Logger.Error($"Elasticsearch write backoff for {_backOffPeriod.Seconds} secs");

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