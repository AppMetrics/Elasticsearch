// <copyright file="BulkPayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayload
    {
        private readonly List<MetricsDocument> _documents;
        private readonly string _indexName;
        private readonly JsonSerializer _serializer;

        public BulkPayload(JsonSerializer serializer, string indexName)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));

            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException("Cannot be empty", nameof(indexName));
            }

            _documents = new List<MetricsDocument>();
        }

        public void Add(MetricsDocument document)
        {
            if (document == null)
            {
                return;
            }

            _documents.Add(document);
        }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                return;
            }

            foreach (var document in _documents)
            {
                _serializer.Serialize(
                    textWriter,
                    new BulkDocumentMetaData(_indexName, document.MeasurementType));
                textWriter.Write('\n');
                _serializer.Serialize(textWriter, document);
                textWriter.Write('\n');
            }
        }
    }
}