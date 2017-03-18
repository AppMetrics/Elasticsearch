// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public enum ElasticSearchAuthorizationSchemas
    {
        /// <summary>
        /// Accessible without authorization
        /// </summary>
        Anonymous,

        /// <summary>
        /// Basic HTTP authorization
        /// </summary>
        Basic,

        /// <summary>
        /// Bearer token based authorization (e.g. JWT token)
        /// </summary>
        BearerToken
    }
}
