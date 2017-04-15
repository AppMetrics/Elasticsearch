// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using App.Metrics.Internal;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayload
    {
        private readonly List<MetricsDocument> _documents;
        private readonly JsonSerializer _serializer;
        private readonly string _indexName;

        public BulkPayload(JsonSerializer serializer, string indexName)
        {
            if (indexName.IsMissing())
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _indexName = indexName;
            _documents = new List<MetricsDocument>();
        }

        public void Add(MetricsDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            _documents.Add(document);
        }

        public void Write(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException(nameof(textWriter));
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
