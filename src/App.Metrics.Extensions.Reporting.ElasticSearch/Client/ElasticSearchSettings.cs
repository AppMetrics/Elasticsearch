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
            if (indexName.IsMissing())
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.Basic;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Index = indexName;
        }

        public ElasticSearchSettings(Uri address, string indexName, string userName, string password)
            : this(address, indexName)
        {
            if (userName.IsMissing())
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (password.IsMissing())
            {
                throw new ArgumentNullException(nameof(password));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.Anonymous;
            UserName = userName;
            Password = password;
        }

        public ElasticSearchSettings(Uri address, string indexName, string bearerToken)
            : this(address, indexName)
        {
            if (bearerToken.IsMissing())
            {
                throw new ArgumentNullException(nameof(bearerToken));
            }

            AuthorizationSchema = ElasticSearchAuthorizationSchemas.BearerToken;
            BearerToken = bearerToken;
        }
    }
}
