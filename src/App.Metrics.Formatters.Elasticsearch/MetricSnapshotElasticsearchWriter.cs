// <copyright file="MetricSnapshotElasticsearchWriter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Formatters.Elasticsearch.Internal;
using App.Metrics.Serialization;
using App.Metrics.Tagging;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Elasticsearch
{
    public class MetricSnapshotElasticsearchWriter : IMetricSnapshotWriter
    {
        private readonly BulkPayload _bulkPayload;
        private readonly Func<string, string, string> _metricNameFormatter;
        private readonly Func<string, string> _metricTagValueFormatter;
        private readonly TextWriter _textWriter;

        public MetricSnapshotElasticsearchWriter(
            TextWriter textWriter,
            string elasticsearchIndex,
            Func<string, string, string> metricNameFormatter = null,
            Func<string, string> metricTagValueFormatter = null,
            GeneratedMetricNameMapping dataKeys = null)
        {
            if (string.IsNullOrWhiteSpace(elasticsearchIndex))
            {
                throw new ArgumentNullException(nameof(elasticsearchIndex), "The elasticsearch index name cannot be null or whitespace");
            }

            _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            var serializer = JsonSerializer.Create();
            _bulkPayload = new BulkPayload(serializer, elasticsearchIndex);
            _metricNameFormatter = metricNameFormatter ?? ElasticsearchFormatterConstants.ElasticsearchDefaults.MetricNameFormatter;
            _metricTagValueFormatter = metricTagValueFormatter ?? ElasticsearchFormatterConstants.ElasticsearchDefaults.MetricTagValueFormatter;

            MetricNameMapping = dataKeys ?? new GeneratedMetricNameMapping(
                                    histogram: ElasticsearchFormatterConstants.ElasticsearchDefaults.CustomHistogramDataKeys,
                                    meter: ElasticsearchFormatterConstants.ElasticsearchDefaults.CustomMeterDataKeys);
        }

        /// <inheritdoc />
        public GeneratedMetricNameMapping MetricNameMapping { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Write(string context, string name, object value, MetricTags tags, DateTime timestamp)
        {
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);
            var measurement = _metricNameFormatter(context, name);

            _bulkPayload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = measurement,
                    Fields = new Dictionary<string, object> { { "value", value } },
                    Tags = tagKeyValues
                });
        }

        /// <inheritdoc />
        public void Write(string context, string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags, DateTime timestamp)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);
            var measurement = _metricNameFormatter(context, name);

            _bulkPayload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = measurement,
                    Fields = fields,
                    Tags = tagKeyValues
                });
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bulkPayload.Write(_textWriter);
                _textWriter?.Close();
                _textWriter?.Dispose();
            }
        }
    }
}