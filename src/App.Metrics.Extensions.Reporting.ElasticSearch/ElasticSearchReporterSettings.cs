// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;

namespace App.Metrics.Extensions.Reporting.ElasticSearch
{
    // ReSharper disable InconsistentNaming
    public class ElasticSearchReporterSettings : IReporterSettings
        // ReSharper restore InconsistentNaming
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElasticSearchReporterSettings" /> class.
        /// </summary>
        public ElasticSearchReporterSettings()
        {
            ElasticSearchSettings = new ElasticSearchSettings(new Uri("http://localhost:9200"), "metrics");
            HttpPolicy = new HttpPolicy
                         {
                             FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                             BackoffPeriod = Constants.DefaultBackoffPeriod,
                             Timeout = Constants.DefaultTimeout
                         };
            ReportInterval = TimeSpan.FromSeconds(5);
            MetricNameFormatter = (metricContext, metricName) => metricContext.IsMissing()
                ? $"{metricName}".Replace(' ', '_').ToLowerInvariant()
                : $"{metricContext}__{metricName}".Replace(' ', '_').ToLowerInvariant();
        }

        /// <summary>
        ///     Gets or sets the HTTP policy settings which allows circuit breaker configuration to be adjusted
        /// </summary>
        /// <value>
        ///     The HTTP policy.
        /// </value>
        public HttpPolicy HttpPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the ElasticSearch database settings.
        /// </summary>
        /// <value>
        ///     The ElasticSearch database settings.
        /// </value>
        public ElasticSearchSettings ElasticSearchSettings { get; set; }

        /// <summary>
        ///     Gets or sets the metric name formatter func which takes the metric context and name and returns a formatted string
        ///     which will be reported to ElasticSearch as the measurement
        /// </summary>
        /// <value>
        ///     The metric name formatter.
        /// </value>
        public Func<string, string, string> MetricNameFormatter { get; set; }

        /// <summary>
        ///     Gets or sets the report interval for which to flush metrics to ElasticSearch.
        /// </summary>
        /// <value>
        ///     The report interval.
        /// </value>
        public TimeSpan ReportInterval { get; set; }
    }
}