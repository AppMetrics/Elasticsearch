// <copyright file="MetricsReportingElasticsearchOptionsSetup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Elasticsearch;
using Microsoft.Extensions.Options;

namespace App.Metrics.Reporting.Elasticsearch.Internal
{
    /// <summary>
    ///     Sets up default Elasticsearch Reporting options for <see cref="MetricsReportingElasticsearchOptions"/>.
    /// </summary>
    public class MetricsReportingElasticsearchOptionsSetup : IConfigureOptions<MetricsReportingElasticsearchOptions>
    {
        private readonly string _elasticsearchIndex;
        private readonly Uri _elasticsearchBaseUri;
        private readonly MetricsOptions _metricsOptions;

        public MetricsReportingElasticsearchOptionsSetup(
            IOptions<MetricsOptions> metricsOptionsAccessor,
            Uri elasticsearchBaseUri,
            string elasticsearchIndex)
        {
            if (string.IsNullOrWhiteSpace(elasticsearchIndex))
            {
                throw new ArgumentException("An Elasticsearch Index name is required.", nameof(elasticsearchIndex));
            }

            _elasticsearchIndex = elasticsearchIndex;

            _elasticsearchBaseUri = elasticsearchBaseUri ?? throw new ArgumentNullException(nameof(elasticsearchBaseUri));
            _metricsOptions = metricsOptionsAccessor.Value ?? throw new ArgumentNullException(nameof(metricsOptionsAccessor));
        }

        /// <inheritdoc/>
        public void Configure(MetricsReportingElasticsearchOptions options)
        {
            options.Elasticsearch.BaseUri = _elasticsearchBaseUri;
            options.Elasticsearch.Index = _elasticsearchIndex;

            if (options.MetricsOutputFormatter == null)
            {
                options.MetricsOutputFormatter = _metricsOptions.OutputMetricsFormatters.GetType<MetricsElasticsearchOutputFormatter>();
            }
        }
    }
}