// <copyright file="ElasticSearchAuthorizationSchemes.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

namespace App.Metrics.Reporting.Elasticsearch.Client
{
    public enum ElasticSearchAuthorizationSchemes
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
