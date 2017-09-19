// <copyright file="MetricsElasticsearchFormatterBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Elasticsearch;

// ReSharper disable CheckNamespace
namespace App.Metrics
    // ReSharper restore CheckNamespace
{
    public static class MetricsElasticsearchFormatterBuilder
    {
        /// <summary>
        ///     Add the <see cref="MetricsElasticsearchFormatterBuilder" /> allowing metrics to optionally be reported to
        ///     Elasticsearch.
        /// </summary>
        /// <param name="metricFormattingBuilder">s
        ///     The <see cref="IMetricsOutputFormattingBuilder" /> used to configure Elasticsearch formatting
        ///     options.
        /// </param>
        /// <param name="elasticsearchIndex">The elastic search index name.</param>
        /// <param name="setupAction">The Elasticsearch formatting options to use.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder AsElasticsearch(
            this IMetricsOutputFormattingBuilder metricFormattingBuilder,
            string elasticsearchIndex,
            Action<MetricsElasticsearchDocumentFormattingOptions> setupAction)
        {
            if (metricFormattingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricFormattingBuilder));
            }

            if (string.IsNullOrWhiteSpace(elasticsearchIndex))
            {
                throw new ArgumentException("An elasticsearch index name must be provided", nameof(elasticsearchIndex));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var options = new MetricsElasticsearchDocumentFormattingOptions();

            setupAction.Invoke(options);

            var formatter = new MetricsElasticsearchOutputFormatter(elasticsearchIndex, options);

            return metricFormattingBuilder.Using(formatter, false);
        }

        /// <summary>
        ///     Add the <see cref="MetricsElasticsearchFormatterBuilder" /> allowing metrics to optionally be reported to
        ///     Elasticsearch.
        /// </summary>
        /// <param name="metricFormattingBuilder">s
        ///     The <see cref="IMetricsOutputFormattingBuilder" /> used to configure Elasticsearch formatting
        ///     options.
        /// </param>
        /// <param name="elasticsearchIndex">The elastic search index name.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder AsElasticsearch(
            this IMetricsOutputFormattingBuilder metricFormattingBuilder,
            string elasticsearchIndex)
        {
            if (metricFormattingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricFormattingBuilder));
            }

            if (string.IsNullOrWhiteSpace(elasticsearchIndex))
            {
                throw new ArgumentException("An elasticsearch index name must be provided", nameof(elasticsearchIndex));
            }

            var formatter = new MetricsElasticsearchOutputFormatter(elasticsearchIndex);

            return metricFormattingBuilder.Using(formatter, false);
        }
    }
}