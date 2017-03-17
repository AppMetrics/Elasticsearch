using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkPayload
    {
        private List<MetricsDocument> _documents;
        private JsonSerializer _serializer;
        private string _indexName;
        private const string _typeName = "common";

        public BulkPayload(JsonSerializer serializer, string indexName)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (String.IsNullOrEmpty(indexName)) throw new ArgumentNullException(nameof(indexName));

            _serializer = serializer;
            _indexName = indexName;
            _documents = new List<MetricsDocument>();
        }

        public void Add(MetricsDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            _documents.Add(document);
        }

        public void Write(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));

            foreach (var document in _documents)
            {
                _serializer.Serialize(textWriter, new BulkDocumentMetaData(_indexName, _typeName));
                textWriter.Write('\n');
                _serializer.Serialize(textWriter, document);
            }
        }
    }
}
