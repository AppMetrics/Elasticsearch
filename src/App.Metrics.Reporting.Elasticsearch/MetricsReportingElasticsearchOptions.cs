// <copyright file="MetricsReportingElasticsearchOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Extensions.Reporting.ElasticSearch;
using App.Metrics.Filters;
using App.Metrics.Formatters;
using App.Metrics.Reporting.Elasticsearch.Client;

namespace App.Metrics.Reporting.Elasticsearch
{
    /// <summary>
    ///     Provides programmatic configuration for Elasticsearch Reporting in the App Metrics framework.
    /// </summary>
    public class MetricsReportingElasticsearchOptions
    {
        public MetricsReportingElasticsearchOptions()
        {
            ReportInterval = TimeSpan.FromSeconds(10);
            HttpPolicy = new HttpPolicy
            {
                FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                BackoffPeriod = Constants.DefaultBackoffPeriod,
                Timeout = Constants.DefaultTimeout
            };
            Elasticsearch = new ElasticsearchOptions();
        }

        /// <summary>
        ///     Gets or sets the <see cref="IFilterMetrics" /> to use for just this reporter.
        /// </summary>
        /// <value>
        ///     The <see cref="IFilterMetrics" /> to use for this reporter.
        /// </value>
        public IFilterMetrics Filter { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP policy settings which allows circuit breaker configuration to be adjusted
        /// </summary>
        /// <value>
        ///     The HTTP policy.
        /// </value>
        public HttpPolicy HttpPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the available options for Elasticsearch connectivity.
        /// </summary>
        /// <value>
        ///     The <see cref="ElasticsearchOptions" />.
        /// </value>
        public ElasticsearchOptions Elasticsearch { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IMetricsOutputFormatter" /> used to write metrics.
        /// </summary>
        /// <value>
        ///     The <see cref="IMetricsOutputFormatter" /> used to write metrics.
        /// </value>
        public IMetricsOutputFormatter MetricsOutputFormatter { get; set; }

        /// <summary>
        ///     Gets or sets the flush metrics interval
        /// </summary>
        /// <remarks>
        ///     This <see cref="TimeSpan" /> will apply to all configured reporters unless overriden by a specific reporters
        ///     options.
        /// </remarks>
        /// <value>
        ///     The <see cref="TimeSpan" /> to wait between reporting metrics
        /// </value>
        public TimeSpan ReportInterval { get; set; }
    }
}