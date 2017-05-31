// <copyright file="ElasticSearchReporterSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Reporting;
using EsConstants = App.Metrics.Formatting.ElasticSearch.Constants;

namespace App.Metrics.Extensions.Reporting.ElasticSearch
{
    public class ElasticSearchReporterSettings : IReporterSettings
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElasticSearchReporterSettings" /> class.
        /// </summary>
        public ElasticSearchReporterSettings()
        {
            DataKeys = new MetricValueDataKeys(
                EsConstants.ElasticsearchDefaults.CustomHistogramDataKeys,
                EsConstants.ElasticsearchDefaults.CustomMeterDataKeys);

            ElasticSearchSettings = new ElasticSearchSettings(new Uri("http://localhost:9200"), "metrics");
            HttpPolicy = new HttpPolicy
                         {
                             FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                             BackoffPeriod = Constants.DefaultBackoffPeriod,
                             Timeout = Constants.DefaultTimeout
                         };
            ReportInterval = TimeSpan.FromSeconds(5);
            MetricNameFormatter = EsConstants.ElasticsearchDefaults.MetricNameFormatter;
            MetricTagValueFormatter = EsConstants.ElasticsearchDefaults.MetricTagValueFormatter;
        }

        /// <inheritdoc />
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public MetricValueDataKeys DataKeys { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        /// <summary>
        ///     Gets or sets the ElasticSearch database settings.
        /// </summary>
        /// <value>
        ///     The ElasticSearch database settings.
        /// </value>
        public ElasticSearchSettings ElasticSearchSettings { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP policy settings which allows circuit breaker configuration to be adjusted
        /// </summary>
        /// <value>
        ///     The HTTP policy.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public HttpPolicy HttpPolicy { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        /// <summary>
        ///     Gets or sets the metric name formatter func which takes the metric context and name and returns a formatted string
        ///     which will be reported to ElasticSearch as the measurement
        /// </summary>
        /// <value>
        ///     The metric name formatter.
        /// </value>
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public Func<string, string, string> MetricNameFormatter { get; set; }
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        ///     Gets or sets the metric tag value formatter func which takes the metric name and returns a formatted string
        ///     which will be used when reporting tag values to elasticsearch
        /// </summary>
        /// <value>
        ///     The metric tag value formatter.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public Func<string, string> MetricTagValueFormatter { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        /// <summary>
        ///     Gets or sets the report interval for which to flush metrics to ElasticSearch.
        /// </summary>
        /// <value>
        ///     The report interval.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public TimeSpan ReportInterval { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
    }
}