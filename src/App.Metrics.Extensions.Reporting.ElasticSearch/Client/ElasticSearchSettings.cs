// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Internal;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class ElasticSearchSettings
    {
        public Uri Address { get; }

        public string Index { get; }

        public ElasticSearchAuthorizationSchemas AuthorizationSchema { get; }

        public string UserName { get; }

        public string Password { get; }

        public string BearerToken { get; }

        public ElasticSearchSettings(Uri address, string indexName)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Index = indexName ?? throw new ArgumentNullException(nameof(indexName));

            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException("Cannot be empty", nameof(indexName));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.Basic;
        }

        public ElasticSearchSettings(Uri address, string indexName, string userName, string password)
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

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.Anonymous;
        }

        public ElasticSearchSettings(Uri address, string indexName, string bearerToken)
            : this(address, indexName)
        {
            BearerToken = bearerToken ?? throw new ArgumentNullException(nameof(bearerToken));

            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                throw new ArgumentException("Cannot be empty", nameof(bearerToken));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.BearerToken;
        }
    }
}
