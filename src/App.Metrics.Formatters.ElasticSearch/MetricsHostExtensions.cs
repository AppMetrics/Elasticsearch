// <copyright file="MetricsHostExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Extensions.Middleware.Abstractions;
using App.Metrics.Formatters.ElasticSearch;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    public static class MetricsHostExtensions
    {
        /// <summary>
        /// Enables Elasticsearch serialization on the metric endpoint's response
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>The metrics host builder</returns>
        public static IMetricsHostBuilder AddElasticsearchMetricsSerialization(this IMetricsHostBuilder host, string index)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricsResponseWriter>(provider => new ElasticsearchMetricsResponseWriter(index)));

            return host;
        }

        /// <summary>
        /// Enables Elasticsearch serialization on the metric endpoint's response
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>
        /// The metrics host builder
        /// </returns>
        public static IMetricsHostBuilder AddElasticsearchMetricsTextSerialization(this IMetricsHostBuilder host, string index)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricsTextResponseWriter>(provider => new ElasticsearchTextResponseWriter(index)));

            return host;
        }

        /// <summary>
        /// Enables Elasticsearch serialization on the metrics and metrics-text responses
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>The metrics host builder</returns>
        public static IMetricsHostBuilder AddDefaultElasticsearchSerialization(this IMetricsHostBuilder host, string index)
        {
            host.AddElasticsearchMetricsSerialization(index);
            host.AddElasticsearchMetricsTextSerialization(index);
            return host;
        }
    }
}