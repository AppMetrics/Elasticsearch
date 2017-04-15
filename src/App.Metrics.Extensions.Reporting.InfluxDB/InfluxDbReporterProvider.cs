// <copyright file="InfluxDbReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Internal;
using App.Metrics.Reporting;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.InfluxDB
{
    public class InfluxDbReporterProvider : IReporterProvider
    {
        private readonly InfluxDBReporterSettings _settings;

        public InfluxDbReporterProvider(InfluxDBReporterSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = new NoOpMetricsFilter();
        }

        public InfluxDbReporterProvider(InfluxDBReporterSettings settings, IFilterMetrics fitler)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = fitler ?? new NoOpMetricsFilter();
        }

        public IFilterMetrics Filter { get; }

        public IMetricReporter CreateMetricReporter(string name, ILoggerFactory loggerFactory)
        {
            var lineProtocolClient = new DefaultLineProtocolClient(
                loggerFactory,
                _settings.InfluxDbSettings,
                _settings.HttpPolicy);
            var payloadBuilder = new LineProtocolPayloadBuilder();

            return new ReportRunner<LineProtocolPayload>(
                async p =>
                {
                    var result = await lineProtocolClient.WriteAsync(p.Payload());
                    return result.Success;
                },
                payloadBuilder,
                _settings.ReportInterval,
                name,
                loggerFactory,
                _settings.MetricNameFormatter,
                _settings.DataKeys);
        }
    }
}