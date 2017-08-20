// <copyright file="ElasticsearchOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;

namespace App.Metrics.Reporting.Elasticsearch
{
    public class ElasticsearchOptions
    {
        public ElasticsearchOptions()
        {
            AuthorizationSchema = ElasticSearchAuthorizationSchemes.Anonymous;
        }

        public ElasticsearchOptions(Uri address, string indexName)
        {
            BaseUri = address ?? throw new ArgumentNullException(nameof(address));
            Index = indexName ?? throw new ArgumentNullException(nameof(indexName));

            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException("Cannot be empty", nameof(indexName));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemes.Basic;
        }

        public ElasticsearchOptions(Uri address, string indexName, string userName, string password)
            : this(address, indexName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Cannot be empty", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Cannot be empty", nameof(password));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemes.Anonymous;
        }

        public ElasticsearchOptions(Uri address, string indexName, string bearerToken)
            : this(address, indexName)
        {
            BearerToken = bearerToken ?? throw new ArgumentNullException(nameof(bearerToken));

            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                throw new ArgumentException("Cannot be empty", nameof(bearerToken));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemes.BearerToken;
        }

        public Uri BaseUri { get; set; }

        public ElasticSearchAuthorizationSchemes AuthorizationSchema { get; set; }

        public string BearerToken { get; set; }

        public string Index { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}