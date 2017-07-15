// <copyright file="ElasticSearchReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Core.Filtering;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Filters;
using App.Metrics.Formatting.ElasticSearch;
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
            var elasticSearchBulkClient = new ElasticSearchBulkClient(
                loggerFactory,
                _settings.ElasticSearchSettings,
                _settings.HttpPolicy);

            var payloadBuilder = new BulkPayloadBuilder(
                _settings.ElasticSearchSettings.Index,
                _settings.MetricNameFormatter,
                _settings.MetricTagValueFormatter,
                _settings.DataKeys);

            return new ReportRunner<BulkPayload>(
                async p =>
                {
                    var result = await elasticSearchBulkClient.WriteAsync(p.Payload());
                    return result;
                },
                payloadBuilder,
                _settings.ReportInterval,
                name,
                loggerFactory);
        }
    }
}