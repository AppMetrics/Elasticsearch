// <copyright file="BulkPayloadTest.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using App.Metrics.Formatters.Elasticsearch.Internal;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace App.Metrics.Reporting.Elasticsearch.Facts
{
    public class BulkPayloadTest
    {
        [Fact]
        public void Can_format_payload_correctly()
        {
            // Arrange
            var textWriter = new StringWriter();
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var documentId = Guid.NewGuid();
            var point = new MetricsDocument
                        {
                            MeasurementName = "measurement",
                            Fields = fields,
                            WrittenOn = timestamp,
                            MeasurementType = "counter"
                        };

            // Act
            var bulkPayload = new BulkPayload(JsonSerializer.Create(), "index");
            bulkPayload.Add(point);
            bulkPayload.Write(textWriter, documentId);

            // Assert
            textWriter.ToString().Should().Be("{\"index\":{\"_index\":\"index.counter\",\"_type\":\"counter\",\"_id\":\"" + documentId.ToString("D") + "\"}}\n{\"name\":\"measurement\",\"fields\":{\"key\":\"value\"},\"tags\":null,\"@timestamp\":\"2017-01-01T01:01:01Z\"}\n");
        }

        [Fact]
        public void Can_format_payload_with_multiple_fields_correctly()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var textWriter = new StringWriter();
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var fields = new Dictionary<string, object>
                         {
                             { "field1key", "field1value" },
                             { "field2key", 2 },
                             { "field3key", false }
                         };
            var point = new MetricsDocument
                        {
                            MeasurementName = "measurement",
                            Fields = fields,
                            WrittenOn = timestamp,
                            MeasurementType = "counter"
                        };

            // Act
            var bulkPayload = new BulkPayload(JsonSerializer.Create(), "index");
            bulkPayload.Add(point);
            bulkPayload.Write(textWriter, documentId);

            // Assert
            textWriter.ToString().Should().Be("{\"index\":{\"_index\":\"index.counter\",\"_type\":\"counter\",\"_id\":\"" + documentId.ToString("D") + "\"}}\n{\"name\":\"measurement\",\"fields\":{\"field1key\":\"field1value\",\"field2key\":2,\"field3key\":false},\"tags\":null,\"@timestamp\":\"2017-01-01T01:01:01Z\"}\n");
        }

        [Fact]
        public void Can_format_payload_with_tags_correctly()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var textWriter = new StringWriter();
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var tags = new MetricTags("tagkey", "tagvalue");
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var point = new MetricsDocument
                        {
                            MeasurementName = "measurement",
                            Fields = fields,
                            WrittenOn = timestamp,
                            MeasurementType = "counter",
                            Tags = tags.ToDictionary()
                        };

            // Act
            var bulkPayload = new BulkPayload(JsonSerializer.Create(), "index");
            bulkPayload.Add(point);
            bulkPayload.Write(textWriter, documentId);

            // Assert
            textWriter.ToString().Should().Be("{\"index\":{\"_index\":\"index.counter\",\"_type\":\"counter\",\"_id\":\"" + documentId.ToString("D") + "\"}}\n{\"name\":\"measurement\",\"fields\":{\"key\":\"value\"},\"tags\":{\"tagkey\":\"tagvalue\"},\"@timestamp\":\"2017-01-01T01:01:01Z\"}\n");
        }

        [Fact]
        public void Measurement_type_cannot_be_empty()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var fields = new Dictionary<string, object> { { string.Empty, "value" } };
            var textWriter = new StringWriter();
            Action action = () =>
            {
                var point = new MetricsDocument
                            {
                                MeasurementName = "measurement",
                                Fields = fields,
                                WrittenOn = timestamp,
                                MeasurementType = string.Empty
                            };

                // Act
                var bulkPayload = new BulkPayload(JsonSerializer.Create(), "index");
                bulkPayload.Add(point);
                bulkPayload.Write(textWriter, documentId);
            };

            // Assert
            action.ShouldThrow<ArgumentException>();
        }
    }
}