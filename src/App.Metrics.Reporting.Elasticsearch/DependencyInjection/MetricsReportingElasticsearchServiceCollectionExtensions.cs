// <copyright file="MetricsReportingElasticsearchServiceCollectionExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using App.Metrics;
using App.Metrics.Extensions.Reporting.ElasticSearch;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Elasticsearch;
using App.Metrics.Reporting.Elasticsearch.Client;
using App.Metrics.Reporting.Elasticsearch.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Extension methods for setting up essential App Metrics Elastisearch reporting services in an
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    public static class MetricsReportingElasticsearchServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds Essential App Metrics Elastisearch reporting metrics services to the specified
        ///     <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="elastisearchBaseUri">The base URI of the Elastisearch API.</param>
        /// <param name="elasticsearchIndex">The Elastisearch index name used to report metrics.</param>
        /// <returns>
        ///     An <see cref="IServiceCollection" /> that can be used to further configure the App Metrics services.
        /// </returns>
        internal static IServiceCollection AddElasticsearchCore(
            this IServiceCollection services,
            Uri elastisearchBaseUri,
            string elasticsearchIndex)
        {
            AddElasticsearchReportingServices(services, elastisearchBaseUri, elasticsearchIndex);

            return services;
        }

        /// <summary>
        ///     Adds Essential App Metrics Elastisearch reporting metrics services to the specified
        ///     <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configuration">
        ///     The <see cref="IConfiguration" /> from where to load
        ///     <see cref="MetricsReportingElasticsearchOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IServiceCollection" /> that can be used to further configure the App Metrics services.
        /// </returns>
        internal static IServiceCollection AddElasticsearchCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var elasticsearchReportingOptinos = new MetricsReportingElasticsearchOptions();
            configuration.Bind(nameof(MetricsReportingElasticsearchOptions), elasticsearchReportingOptinos);

            AddElasticsearchReportingServices(
                services,
                elasticsearchReportingOptinos.Elasticsearch.BaseUri,
                elasticsearchReportingOptinos.Elasticsearch.Index);

            return services;
        }

        internal static void AddElasticsearchReportingServices(IServiceCollection services, Uri elasticsearchBaseUri, string elasticsearchIndex)
        {
            if (elasticsearchBaseUri == default(Uri))
            {
                throw new InvalidOperationException(
                    "MetricsReportingElasticsearchOptions.Elasticsearch.BaseUri is required, check the application's startup code and/or configuration");
            }

            if (string.IsNullOrWhiteSpace(elasticsearchIndex))
            {
                throw new InvalidOperationException(
                    "MetricsReportingElasticsearchOptions.Elasticsearch.Index is required, check the application's startup code and/or configuration");
            }

            //
            // Options
            //
            var optionsSetupDescriptor =
                ServiceDescriptor.Transient<IConfigureOptions<MetricsReportingElasticsearchOptions>, MetricsReportingElasticsearchOptionsSetup>(
                    provider =>
                    {
                        var optionsAccessor = provider.GetRequiredService<IOptions<MetricsOptions>>();
                        return new MetricsReportingElasticsearchOptionsSetup(optionsAccessor, elasticsearchBaseUri, elasticsearchIndex);
                    });

            services.TryAddEnumerable(optionsSetupDescriptor);

            //
            // Elasticsearch Reporting Infrastructure
            //
            var elasticsearchReportProviderDescriptor = ServiceDescriptor.Transient<IReporterProvider, ElasticsearchReporterProvider>();
            services.TryAddEnumerable(elasticsearchReportProviderDescriptor);
            services.TryAddSingleton<IElasticsearchClient>(
                provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var optionsAccessor = provider.GetRequiredService<IOptions<MetricsReportingElasticsearchOptions>>();
                    var httpClient = CreateHttpClient(optionsAccessor.Value.Elasticsearch, optionsAccessor.Value.HttpPolicy);

                    return new DefaultElasticSearchClient(
                        loggerFactory.CreateLogger<DefaultElasticSearchClient>(),
                        optionsAccessor.Value.HttpPolicy,
                        httpClient);
                });
        }

        internal static HttpClient CreateHttpClient(
            ElasticsearchOptions elasticsearchOptions,
            HttpPolicy httpPolicy,
            HttpMessageHandler httpMessageHandler = null)
        {
            var client = httpMessageHandler == null
                ? new HttpClient()
                : new HttpClient(httpMessageHandler);

            SetElasticsearchClientAuthHeader(elasticsearchOptions, client);
            client.BaseAddress = elasticsearchOptions.BaseUri;
            client.Timeout = httpPolicy.Timeout;

            var byteArray = Encoding.ASCII.GetBytes($"{elasticsearchOptions.UserName}:{elasticsearchOptions.Password}");
            client.BaseAddress = elasticsearchOptions.BaseUri;
            client.Timeout = httpPolicy.Timeout;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;
        }

        private static void SetElasticsearchClientAuthHeader(ElasticsearchOptions elasticsearchOptions, HttpClient client)
        {
            if (elasticsearchOptions.AuthorizationSchema == ElasticSearchAuthorizationSchemes.Basic &&
                (string.IsNullOrWhiteSpace(elasticsearchOptions.UserName) || string.IsNullOrWhiteSpace(elasticsearchOptions.Password)))
            {
                throw new InvalidOperationException($"The specified schema {elasticsearchOptions.AuthorizationSchema} requires a Username and Password");
            }

            if (elasticsearchOptions.AuthorizationSchema == ElasticSearchAuthorizationSchemes.BearerToken &&
                string.IsNullOrWhiteSpace(elasticsearchOptions.BearerToken))
            {
                throw new InvalidOperationException($"The specified schema {elasticsearchOptions.AuthorizationSchema} requires a BearerToken");
            }

            switch (elasticsearchOptions.AuthorizationSchema)
            {
                case ElasticSearchAuthorizationSchemes.Anonymous:
                    break;
                case ElasticSearchAuthorizationSchemes.Basic:
                    var authHeader = Encoding.ASCII.GetBytes($"{elasticsearchOptions.UserName}:{elasticsearchOptions.Password}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authHeader));
                    break;
                case ElasticSearchAuthorizationSchemes.BearerToken:
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", elasticsearchOptions.BearerToken);
                    break;
                default:
                    throw new NotImplementedException($"The specified schema {elasticsearchOptions.AuthorizationSchema} is not implemented");
            }
        }
    }
}