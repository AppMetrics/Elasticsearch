// <copyright file="MetricsElasticsearchDocumentFormattingOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Elasticsearch.Internal;

namespace App.Metrics.Formatters.Elasticsearch
{
    /// <summary>
    ///     Provides programmatic configuration for Elasticsearch document formatting options in the App Metrics framework.
    /// </summary>
    public class MetricsElasticsearchDocumentFormattingOptions
    {
        public MetricsElasticsearchDocumentFormattingOptions()
        {
            MetricNameMapping = new GeneratedMetricNameMapping(
                histogram: ElasticsearchFormatterConstants.ElasticsearchDefaults.CustomHistogramDataKeys,
                meter: ElasticsearchFormatterConstants.ElasticsearchDefaults.CustomMeterDataKeys);
            MetricTagFormatter = ElasticsearchFormatterConstants.ElasticsearchDefaults.MetricTagValueFormatter;
            MetricNameFormatter = ElasticsearchFormatterConstants.ElasticsearchDefaults.MetricNameFormatter;
        }

        public Func<string, string, string> MetricNameFormatter { get; set; }

        public Func<string, string> MetricTagFormatter { get; set; }

        public GeneratedMetricNameMapping MetricNameMapping { get; set; }
    }
}
