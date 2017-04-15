// <copyright file="DefaultLineProtocolClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.InfluxDB.Client
{
    public class DefaultLineProtocolClient : ILineProtocolClient
    {
        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private static TimeSpan _backOffPeriod;

        private readonly HttpClient _httpClient;
        private readonly InfluxDBSettings _influxDbSettings;
        private readonly ILogger<DefaultLineProtocolClient> _logger;

        public DefaultLineProtocolClient(ILoggerFactory loggerFactory, InfluxDBSettings influxDbSettings)
            : this(
                loggerFactory,
                influxDbSettings,
#pragma warning disable SA1118
                new HttpPolicy
                {
                    FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                    BackoffPeriod = Constants.DefaultBackoffPeriod,
                    Timeout = Constants.DefaultTimeout
                })
        {
        }

#pragma warning disable SA1118

        public DefaultLineProtocolClient(
            ILoggerFactory loggerFactory,
            InfluxDBSettings influxDbSettings,
            HttpPolicy httpPolicy,
            HttpMessageHandler httpMessageHandler = null)
        {
            if (influxDbSettings == null)
            {
                throw new ArgumentNullException(nameof(influxDbSettings));
            }

            if (httpPolicy == null)
            {
                throw new ArgumentNullException(nameof(httpPolicy));
            }

            _httpClient = CreateHttpClient(influxDbSettings, httpPolicy, httpMessageHandler);
            _influxDbSettings = influxDbSettings;
            _backOffPeriod = httpPolicy.BackoffPeriod;
            _failuresBeforeBackoff = httpPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
            _logger = loggerFactory.CreateLogger<DefaultLineProtocolClient>();
        }

        public async Task<LineProtocolWriteResult> WriteAsync(
            LineProtocolPayload payload,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (NeedToBackoff())
            {
                return new LineProtocolWriteResult(false, "Too many failures in writing to InfluxDB, Circuit Opened");
            }

            var payloadText = new StringWriter();
            payload.Format(payloadText);
            var content = new StringContent(payloadText.ToString(), Encoding.UTF8);

            try
            {
                var response = await _httpClient.PostAsync(_influxDbSettings.Endpoint, content, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Interlocked.Increment(ref _failureAttempts);

                    var errorMessage = $"Failed to write to InfluxDB - StatusCode: {response.StatusCode} Reason: {response.ReasonPhrase}";
                    _logger.LogError(LoggingEvents.InfluxDbWriteError, errorMessage);

                    return new LineProtocolWriteResult(false, errorMessage);
                }

                _logger.LogTrace("Successful write to InfluxDB");

                return new LineProtocolWriteResult(true);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failureAttempts);
                _logger.LogError(LoggingEvents.InfluxDbWriteError, "Failed to write to InfluxDB", ex);
                return new LineProtocolWriteResult(false, ex.ToString());
            }
        }

        private static HttpClient CreateHttpClient(
            InfluxDBSettings influxDbSettings,
            HttpPolicy httpPolicy,
            HttpMessageHandler httpMessageHandler = null)
        {
            var client = httpMessageHandler == null
                ? new HttpClient()
                : new HttpClient(httpMessageHandler);

            client.BaseAddress = influxDbSettings.BaseAddress;
            client.Timeout = httpPolicy.Timeout;

            if (influxDbSettings.UserName.IsMissing() || influxDbSettings.Password.IsMissing())
            {
                return client;
            }

            var byteArray = Encoding.ASCII.GetBytes($"{influxDbSettings.UserName}:{influxDbSettings.Password}");
            client.BaseAddress = influxDbSettings.BaseAddress;
            client.Timeout = httpPolicy.Timeout;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;
        }

        private bool NeedToBackoff()
        {
            if (Interlocked.Read(ref _failureAttempts) < _failuresBeforeBackoff)
            {
                return false;
            }

            _logger.LogError($"InfluxDB write backoff for {_backOffPeriod.Seconds} secs");

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