// <copyright file="BulkDocumentMetaData.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class BulkDocumentMetaData
    {
        [JsonProperty("index")]
        public DocumentMetaData MetaData { get; set; }

        public BulkDocumentMetaData(string indexName, string typeName)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            MetaData = new DocumentMetaData
            {
                IndexName = indexName,
                TypeName = typeName,
                DocumentId = Guid.NewGuid().ToString("D")
            };
        }

        public class DocumentMetaData
        {
            [JsonProperty("_index")]
            public string IndexName { get; set; }

            [JsonProperty("_type")]
            public string TypeName { get; set; }

            [JsonProperty("_id")]
            public string DocumentId { get; set; }
        }
    }
}
