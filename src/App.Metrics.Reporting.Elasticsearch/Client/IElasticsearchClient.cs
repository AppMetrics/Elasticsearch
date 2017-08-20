// <copyright file="IElasticsearchClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.Reporting.Elasticsearch.Client
{
    public interface IElasticsearchClient
    {
        Task<ElasticsearchWriteResult> WriteAsync(
            string payload,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}