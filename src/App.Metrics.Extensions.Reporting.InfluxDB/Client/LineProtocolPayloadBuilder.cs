// <copyright file="LineProtocolPayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;

namespace App.Metrics.Extensions.Reporting.InfluxDB.Client
{
    public class LineProtocolPayloadBuilder : IMetricPayloadBuilder<LineProtocolPayload>
    {
        private LineProtocolPayload _payload;

        public void Clear() { _payload = null; }

        public void Init()
        {
            _payload = new LineProtocolPayload();
        }

        public void Pack(string name, object value, MetricTags tags)
        {
            _payload.Add(new LineProtocolPoint(name, new Dictionary<string, object> { { "value", value } }, tags));
        }

        public void Pack(
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data })
                                .ToDictionary(pair => pair.column, pair => pair.data);

            _payload.Add(new LineProtocolPoint(name, fields, tags));
        }

        public LineProtocolPayload Payload() { return _payload; }

        public string PayloadFormatted()
        {
            var result = new StringWriter();
            _payload.Format(result);
            return result.ToString();
        }
    }
}