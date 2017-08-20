// <copyright file="MetricsElasticsearchOutputFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Serialization;

namespace App.Metrics.Formatters.Elasticsearch
{
    public class MetricsElasticsearchOutputFormatter : IMetricsOutputFormatter
    {
        private readonly MetricsElasticsearchDocumentFormattingOptions _documentFormattingOptions;

        public MetricsElasticsearchOutputFormatter() { _documentFormattingOptions = new MetricsElasticsearchDocumentFormattingOptions(); }

        public MetricsElasticsearchOutputFormatter(MetricsElasticsearchDocumentFormattingOptions documentFormattingOptions)
        {
            _documentFormattingOptions = documentFormattingOptions ?? throw new ArgumentNullException(nameof(documentFormattingOptions));
        }

        /// <inheritdoc />
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue("text", "vnd.appmetrics.metrics.elasticsearch", "v1", "plain");

        /// <inheritdoc />
        public Task WriteAsync(
            Stream output,
            MetricsDataValueSource metricsData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var serializer = new MetricSnapshotSerializer();

            using (var streamWriter = new StreamWriter(output))
            {
                using (var textWriter = new MetricSnapshotElasticsearchWriter(
                    streamWriter,
                    _documentFormattingOptions.Index,
                    _documentFormattingOptions.MetricNameFormatter,
                    _documentFormattingOptions.MetricTagFormatter,
                    _documentFormattingOptions.MetricNameMapping))
                {
                    serializer.Serialize(textWriter, metricsData);
                }
            }

            return Task.CompletedTask;
        }
    }
}