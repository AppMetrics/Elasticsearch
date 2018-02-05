// <copyright file="MetricsElasticsearchReporterBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using App.Metrics.Builder;
using App.Metrics.Reporting.Elasticsearch;
using App.Metrics.Reporting.Elasticsearch.Client;

// ReSharper disable CheckNamespace
namespace App.Metrics
    // ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Builder for configuring metrics Elasticsearch reporting using an
    ///     <see cref="IMetricsReportingBuilder" />.
    /// </summary>
    public static class MetricsElasticsearchReporterBuilder
    {
        /// <summary>
        ///     Add the <see cref="ElasticsearchReporter" /> allowing metrics to be reported to Elasticsearch.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="options">The Elasticsearch reporting options to use.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToElasticsearch(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            MetricsReportingElasticsearchOptions options)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            var httpClient = CreateClient(options.Elasticsearch, options.HttpPolicy);
            var reporter = new ElasticsearchReporter(options, httpClient);

            return metricReporterProviderBuilder.Using(reporter);
        }

        /// <summary>
        ///     Add the <see cref="ElasticsearchReporter" /> allowing metrics to be reported to Elasticsearch.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="setupAction">The Elasticsearch reporting options to use.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToElasticsearch(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            Action<MetricsReportingElasticsearchOptions> setupAction)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            var options = new MetricsReportingElasticsearchOptions();

            setupAction?.Invoke(options);

            var httpClient = CreateClient(options.Elasticsearch, options.HttpPolicy);
            var reporter = new ElasticsearchReporter(options, httpClient);

            return metricReporterProviderBuilder.Using(reporter);
        }

        /// <summary>
        ///     Add the <see cref="ElasticsearchReporter" /> allowing metrics to be reported to Elasticsearch.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="url">The base url where Elasticsearch is hosted.</param>
        /// <param name="elasticsearchIndex">The Elasticsearch where metrics should be flushed.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToElasticsearch(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            string url,
            string elasticsearchIndex)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException($"{nameof(url)} must be a valid absolute URI");
            }

            var options = new MetricsReportingElasticsearchOptions
                          {
                              Elasticsearch =
                              {
                                  BaseUri = uri,
                                  Index = elasticsearchIndex
                              }
                          };

            var httpClient = CreateClient(options.Elasticsearch, options.HttpPolicy);
            var reporter = new ElasticsearchReporter(options, httpClient);

            var builder = metricReporterProviderBuilder.Using(reporter);
            builder.OutputMetrics.AsElasticsearch(elasticsearchIndex);

            return builder;
        }

        /// <summary>
        ///     Add the <see cref="ElasticsearchReporter" /> allowing metrics to be reported to Elasticsearch.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="url">The base url where Elasticsearch is hosted.</param>
        /// <param name="elasticsearchIndex">The Elasticsearch where metrics should be flushed.</param>
        /// <param name="flushInterval">
        ///     The <see cref="T:System.TimeSpan" /> interval used if intended to schedule metrics
        ///     reporting.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToElasticsearch(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            string url,
            string elasticsearchIndex,
            TimeSpan flushInterval)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException($"{nameof(url)} must be a valid absolute URI");
            }

            var options = new MetricsReportingElasticsearchOptions
                          {
                              FlushInterval = flushInterval,
                              Elasticsearch =
                              {
                                  BaseUri = uri,
                                  Index = elasticsearchIndex
                              }
                          };

            var httpClient = CreateClient(options.Elasticsearch, options.HttpPolicy);
            var reporter = new ElasticsearchReporter(options, httpClient);

            var builder = metricReporterProviderBuilder.Using(reporter);
            builder.OutputMetrics.AsElasticsearch(elasticsearchIndex);

            return builder;
        }

        internal static IElasticsearchClient CreateClient(
            ElasticsearchOptions options,
            HttpPolicy httpPolicy,
            HttpMessageHandler httpMessageHandler = null)
        {
            var httpClient = httpMessageHandler == null
                ? new HttpClient()
                : new HttpClient(httpMessageHandler);

            httpClient.BaseAddress = options.BaseUri;
            httpClient.Timeout = httpPolicy.Timeout;

            if (string.IsNullOrWhiteSpace(options.UserName) || string.IsNullOrWhiteSpace(options.Password))
            {
                return new DefaultElasticSearchClient(httpPolicy, httpClient);
            }

            var byteArray = Encoding.ASCII.GetBytes($"{options.UserName}:{options.Password}");
            httpClient.BaseAddress = options.BaseUri;
            httpClient.Timeout = httpPolicy.Timeout;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return new DefaultElasticSearchClient(httpPolicy, httpClient);
        }
    }
}