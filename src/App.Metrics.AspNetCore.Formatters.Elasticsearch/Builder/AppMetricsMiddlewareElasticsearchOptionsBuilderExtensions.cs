// <copyright file="AppMetricsMiddlewareElasticsearchOptionsBuilderExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Builder;
using App.Metrics.Formatters.ElasticSearch;
using App.Metrics.Middleware;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    public static class AppMetricsMiddlewareElasticsearchOptionsBuilderExtensions
    {
        /// <summary>
        /// Enables Elasticsearch serialization on the metric endpoint's response
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>The metrics host builder</returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddMetricsElasticsearchFormatters(this IAppMetricsMiddlewareOptionsBuilder options, string index)
        {
            options.AppMetricsBuilder.Services.Replace(ServiceDescriptor.Transient<IMetricsResponseWriter>(provider => new ElasticsearchMetricsResponseWriter(index)));

            return options;
        }

        /// <summary>
        /// Enables Elasticsearch serialization on the metric endpoint's response
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>
        /// The metrics host builder
        /// </returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddMetricsTextElasticsearchFormatters(this IAppMetricsMiddlewareOptionsBuilder options, string index)
        {
            options.AppMetricsBuilder.Services.Replace(ServiceDescriptor.Transient<IMetricsTextResponseWriter>(provider => new ElasticsearchTextResponseWriter(index)));

            return options;
        }

        /// <summary>
        /// Enables Elasticsearch serialization on the metrics and metrics-text responses
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <param name="index">The elasticsearch index.</param>
        /// <returns>The metrics host builder</returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddElasticsearchFormatters(this IAppMetricsMiddlewareOptionsBuilder options, string index)
        {
            options.AddMetricsElasticsearchFormatters(index);
            options.AddMetricsTextElasticsearchFormatters(index);

            return options;
        }
    }
}