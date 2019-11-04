// <copyright file="MetricsElasticsearchOutputFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if !NETSTANDARD1_6
using App.Metrics.Internal;
#endif
using App.Metrics.Serialization;

namespace App.Metrics.Formatters.Elasticsearch
{
    public class MetricsElasticsearchOutputFormatter : IMetricsOutputFormatter
    {
        private readonly string _elasticsearchIndex;
        private readonly MetricsElasticsearchDocumentFormattingOptions _options;

        public MetricsElasticsearchOutputFormatter(string elasticsearchIndex)
        {
            if (string.IsNullOrEmpty(elasticsearchIndex))
            {
                throw new ArgumentNullException(nameof(elasticsearchIndex));
            }

            _elasticsearchIndex = elasticsearchIndex;
            _options = new MetricsElasticsearchDocumentFormattingOptions();
            MetricFields = new MetricFields();
        }

        public MetricsElasticsearchOutputFormatter(
            string elasticsearchIndex,
            MetricsElasticsearchDocumentFormattingOptions options)
        {
            if (string.IsNullOrEmpty(elasticsearchIndex))
            {
                throw new ArgumentNullException(nameof(elasticsearchIndex));
            }

            _elasticsearchIndex = elasticsearchIndex;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            MetricFields = new MetricFields();
        }

        /// <inheritdoc />
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue("text", "vnd.appmetrics.metrics.elasticsearch", "v1", "plain");

        /// <inheritdoc />
        public MetricFields MetricFields { get; set; }

        /// <inheritdoc />
        public Task WriteAsync(
            Stream output,
            MetricsDataValueSource metricsData,
            CancellationToken cancellationToken = default)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var serializer = new MetricSnapshotSerializer();

            using (var streamWriter = new StreamWriter(output))
            {
                using var textWriter = new MetricSnapshotElasticsearchWriter(streamWriter, _elasticsearchIndex);
                serializer.Serialize(textWriter, metricsData, MetricFields);
            }

            return Task.CompletedTask;
        }
    }
}