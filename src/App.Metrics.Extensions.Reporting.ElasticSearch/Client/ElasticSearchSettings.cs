// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class ElasticSearchSettings
    {
        public Uri Address { get; private set; }

        public string Index { get; private set; }

        public ElasticSearchAuthorizationSchemas AuthorizationSchema { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string BearerToken { get; private set; }

        public ElasticSearchSettings(Uri address, string indexName)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            this.AuthorizationSchema = ElasticSearchAuthorizationSchemas.Basic;
            this.Address = address;
            this.Index = indexName;
        }

        public ElasticSearchSettings(Uri address, string indexName, string userName, string password)
            : this(address, indexName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            this.AuthorizationSchema = ElasticSearchAuthorizationSchemas.Anonymous;
            this.UserName = userName;
            this.Password = password;
        }

        public ElasticSearchSettings(Uri address, string indexName, string bearerToken)
            : this(address, indexName)
        {
            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new ArgumentNullException(nameof(bearerToken));
            }

            this.AuthorizationSchema = ElasticSearchAuthorizationSchemas.BearerToken;
            this.BearerToken = bearerToken;
        }
    }
}
