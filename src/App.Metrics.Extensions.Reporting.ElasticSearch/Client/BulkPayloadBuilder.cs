// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.Tagging;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayloadBuilder
    {
        private readonly Func<string, string> _metricTagValueFormatter;
        private readonly ElasticSearchSettings _settings;
        private readonly JsonSerializer _serializer;

        public BulkPayload Payload { get; private set; }

        public BulkPayloadBuilder(ElasticSearchSettings settings, Func<string, string> metricTagValueFormatter)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _metricTagValueFormatter = metricTagValueFormatter;
            _serializer = JsonSerializer.Create();
        }

        public void Clear()
        {
            Payload = null;
        }

        public BulkPayloadBuilder Init()
        {
            Payload = new BulkPayload(_serializer, _settings.Index);
            return this;
        }

        public BulkPayloadBuilder Pack(string type, string name, object value, MetricTags tags)
        {
            Payload.Add(new MetricsDocument
            {
                MeasurementType = type,
                MeasurementName = name,
                Fields = new Dictionary<string, object> { { "value", value } },
                Tags = tags.ToDictionary(_metricTagValueFormatter)
            });

            return this;
        }

        public BulkPayloadBuilder Pack(
            string type,
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data })
                                .ToDictionary(pair => pair.column, pair => pair.data);

            Payload.Add(new MetricsDocument
            {
                MeasurementType = type,
                MeasurementName = name,
                Fields = fields,
                Tags = tags.ToDictionary(_metricTagValueFormatter)
            });

            return this;
        }
    }
}
