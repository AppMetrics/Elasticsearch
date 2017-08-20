// <copyright file="MetricsReportingElasticsearchMetricsReportingBuilderExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Reporting.Elasticsearch;
using Microsoft.Extensions.Configuration;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Extension methods for setting up App Metrics Elastisearch Reporting in an <see cref="IMetricsReportingBuilder" />.
    /// </summary>
    public static class MetricsReportingElasticsearchMetricsReportingBuilderExtensions
    {
        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingBuilder" /> to add services to.</param>
        /// <param name="elasticsearchBaseUri">The base URI of the Elastisearch API.</param>
        /// <param name="elasticsearchIndex">The Elastisearch database name used to report metrics.</param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingBuilder AddElasticsearch(
            this IMetricsReportingBuilder builder,
            Uri elasticsearchBaseUri,
            string elasticsearchIndex)
        {
            builder.Services.AddElasticsearchFormatterServices();

            builder.Services.AddElasticsearchCore(elasticsearchBaseUri, elasticsearchIndex);

            return builder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingBuilder" /> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> from where to load <see cref="MetricsReportingElasticsearchOptions" />.</param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingBuilder AddElasticsearch(
            this IMetricsReportingBuilder builder,
            IConfiguration configuration)
        {
            builder.Services.AddElasticsearchFormatterServices();

            builder.Services.AddElasticsearchCore(configuration);

            return builder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingBuilder" /> to add services to.</param>
        /// <param name="elasticsearchBaseUri">The base URI of the Elastisearch API.</param>
        /// <param name="elasticsearchIndex">The Elastisearch index name used to report metrics.</param>
        /// <param name="setupAction">
        ///     An <see cref="Action" /> to configure the provided <see cref="MetricsReportingElasticsearchOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingBuilder AddElasticsearch(
            this IMetricsReportingBuilder builder,
            Uri elasticsearchBaseUri,
            string elasticsearchIndex,
            Action<MetricsReportingElasticsearchOptions> setupAction)
        {
            var reportingBuilder = builder.AddElasticsearch(elasticsearchBaseUri, elasticsearchIndex);

            builder.Services.Configure(setupAction);

            return reportingBuilder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingBuilder" /> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> from where to load <see cref="MetricsReportingElasticsearchOptions" />.</param>
        /// <param name="setupAction">
        ///     An <see cref="Action" /> to configure the provided <see cref="MetricsReportingElasticsearchOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingBuilder AddElasticsearch(
            this IMetricsReportingBuilder builder,
            IConfiguration configuration,
            Action<MetricsReportingElasticsearchOptions> setupAction)
        {
            var reportingBuilder = builder.AddElasticsearch(configuration);

            builder.Services.Configure(setupAction);

            return reportingBuilder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingCoreBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingCoreBuilder" /> to add services to.</param>
        /// <param name="elasticsearchBaseUri">The base URI of the Elastisearch API.</param>
        /// <param name="elasticsearchIndex">The Elastisearch index name used to report metrics.</param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingCoreBuilder AddElasticsearch(
            this IMetricsReportingCoreBuilder builder,
            Uri elasticsearchBaseUri,
            string elasticsearchIndex)
        {
            builder.Services.AddElasticsearchCore(elasticsearchBaseUri, elasticsearchIndex);

            return builder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingCoreBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingCoreBuilder" /> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> from where to load <see cref="MetricsReportingElasticsearchOptions" />.</param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingCoreBuilder AddElasticsearch(
            this IMetricsReportingCoreBuilder builder,
            IConfiguration configuration)
        {
            builder.Services.AddElasticsearchCore(configuration);

            return builder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingCoreBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingCoreBuilder" /> to add services to.</param>
        /// <param name="elasticsearchBaseUri">The base URI of the Elastisearch API.</param>
        /// <param name="elasticsearchIndex">The Elastisearch index name used to report metrics.</param>
        /// <param name="setupAction">
        ///     An <see cref="Action" /> to configure the provided <see cref="MetricsReportingElasticsearchOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingCoreBuilder AddElasticsearch(
            this IMetricsReportingCoreBuilder builder,
            Uri elasticsearchBaseUri,
            string elasticsearchIndex,
            Action<MetricsReportingElasticsearchOptions> setupAction)
        {
            var reportingBuilder = builder.AddElasticsearch(elasticsearchBaseUri, elasticsearchIndex);

            builder.Services.Configure(setupAction);

            return reportingBuilder;
        }

        /// <summary>
        ///     Adds App Metrics elasticsearch reporting metrics services to the specified <see cref="IMetricsReportingCoreBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsReportingCoreBuilder" /> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> from where to load <see cref="MetricsReportingElasticsearchOptions" />.</param>
        /// <param name="setupAction">
        ///     An <see cref="Action" /> to configure the provided <see cref="MetricsReportingElasticsearchOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsReportingBuilder" /> that can be used to further configure the App Metrics Reporting services.
        /// </returns>
        public static IMetricsReportingCoreBuilder AddElasticsearch(
            this IMetricsReportingCoreBuilder builder,
            IConfiguration configuration,
            Action<MetricsReportingElasticsearchOptions> setupAction)
        {
            var reportingBuilder = builder.AddElasticsearch(configuration);

            builder.Services.Configure(setupAction);

            return reportingBuilder;
        }
    }
}