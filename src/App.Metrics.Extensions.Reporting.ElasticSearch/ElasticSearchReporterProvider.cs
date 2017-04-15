// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Internal;
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

            return new ElasticSearchReporter(
                lineProtocolClient,
                payloadBuilder,
                _settings.ReportInterval,
                name,
                loggerFactory,
                _settings.MetricNameFormatter);
        }
    }
}