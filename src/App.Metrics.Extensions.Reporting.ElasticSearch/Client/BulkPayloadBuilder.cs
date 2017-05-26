// <copyright file="BulkPayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayloadBuilder : IMetricPayloadBuilder<BulkPayload>
    {
        private readonly string _index;
        private readonly Func<string, string> _metricTagValueFormatter;
        private readonly Func<string, string, string> _metricNameFormatter;
        private readonly JsonSerializer _serializer;
        private BulkPayload _payload;

        public BulkPayloadBuilder(
            string index,
            Func<string, string, string> metricNameFormatter,
            Func<string, string> metricTagValueFormatter,
            MetricValueDataKeys dataKeys)
        {
            _index = index;
            _metricTagValueFormatter = metricTagValueFormatter;
            _metricNameFormatter = metricNameFormatter;
            _serializer = JsonSerializer.Create();
            DataKeys = dataKeys;
        }

        /// <inheritdoc />
        public MetricValueDataKeys DataKeys { get; }

        /// <inheritdoc />
        public void Clear() { _payload = null; }

        /// <inheritdoc />
        public void Init() { _payload = new BulkPayload(_serializer, _index); }

        /// <inheritdoc />
        public void Pack(string context, string name, object value, MetricTags tags)
        {
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);
            var measurement = _metricNameFormatter(context, name);

            _payload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = measurement,
                    Fields = new Dictionary<string, object> { { "value", value } },
                    Tags = tagKeyValues
                });
        }

        /// <inheritdoc />
        public void Pack(
            string context,
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);
            var measurement = _metricNameFormatter(context, name);

            _payload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = measurement,
                    Fields = fields,
                    Tags = tagKeyValues
                });
        }

        /// <inheritdoc />
        public BulkPayload Payload() { return _payload; }

        /// <inheritdoc />
        public string PayloadFormatted()
        {
            var result = new StringWriter();
            _payload.Format(result);
            return result.ToString();
        }
    }
}