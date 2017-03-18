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
        private ElasticSearchSettings _settings;
        private JsonSerializer _serializer;

        public BulkPayload Payload { get; private set; }

        public BulkPayloadBuilder(ElasticSearchSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
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

        public BulkPayloadBuilder Pack(string name, object value, MetricTags tags)
        {
            Payload.Add(new MetricsDocument
            {
                MeasurementName = name,
                Fields = new Dictionary<string, object> { { "value", value } },
                Tags = tags.ToDictionary()
            });

            return this;
        }

        public BulkPayloadBuilder Pack(
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data })
                                .ToDictionary(pair => pair.column, pair => pair.data);
            Payload.Add(new MetricsDocument
            {
                MeasurementName = name,
                Fields = fields,
                Tags = tags.ToDictionary()
            });

            return this;
        }
    }
}
