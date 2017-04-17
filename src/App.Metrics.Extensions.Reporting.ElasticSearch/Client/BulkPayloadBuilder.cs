// <copyright file="BulkPayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayloadBuilder : IMetricPayloadBuilder<BulkPayload>
    {
        private readonly Func<string, string> _metricTagValueFormatter;
        private readonly JsonSerializer _serializer;
        private readonly ElasticSearchSettings _settings;
        private BulkPayload _payload;

        public BulkPayloadBuilder(ElasticSearchSettings settings, Func<string, string> metricTagValueFormatter)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _metricTagValueFormatter = metricTagValueFormatter;
            _serializer = JsonSerializer.Create();
        }

        public void Clear() { _payload = null; }

        public void Init() { _payload = new BulkPayload(_serializer, _settings.Index); }

        public void Pack(string name, object value, MetricTags tags)
        {
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);

            _payload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = name,
                    Fields = new Dictionary<string, object> { { "value", value } },
                    Tags = tagKeyValues
                });
        }

        public void Pack(
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);
            var tagKeyValues = tags.ToDictionary(_metricTagValueFormatter);

            _payload.Add(
                new MetricsDocument
                {
                    MeasurementType = tagKeyValues["mtype"],
                    MeasurementName = name,
                    Fields = fields,
                    Tags = tagKeyValues
                });
        }

        public BulkPayload Payload() { return _payload; }

        public string PayloadFormatted()
        {
            var result = new StringWriter();
            _payload.Format(result);
            return result.ToString();
        }
    }
}