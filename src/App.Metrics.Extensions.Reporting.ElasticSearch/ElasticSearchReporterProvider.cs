// <copyright file="ElasticSearchReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Internal;
using App.Metrics.Reporting;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.ElasticSearch
{
    public class ElasticSearchReporterProvider : IReporterProvider
    {
        private readonly ElasticSearchReporterSettings _settings;

        public ElasticSearchReporterProvider(ElasticSearchReporterSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = new NoOpMetricsFilter();
        }

        public ElasticSearchReporterProvider(ElasticSearchReporterSettings settings, IFilterMetrics fitler)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = fitler ?? new NoOpMetricsFilter();
        }

        public IFilterMetrics Filter { get; }

        public IMetricReporter CreateMetricReporter(string name, ILoggerFactory loggerFactory)
        {
            var lineProtocolClient = new ElasticSearchBulkClient(
                loggerFactory,
                _settings.ElasticSearchSettings,
                _settings.HttpPolicy);
            var payloadBuilder = new BulkPayloadBuilder(_settings.ElasticSearchSettings, _settings.MetricTagValueFormatter);

            return new ReportRunner<BulkPayload>(
                async p => await lineProtocolClient.WriteAsync(p.Payload()),
                payloadBuilder,
                _settings.ReportInterval,
                name,
                loggerFactory,
                _settings.MetricNameFormatter,
                _settings.DataKeys);
        }
    }
}