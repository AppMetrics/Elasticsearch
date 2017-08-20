// <copyright file="MetricsElasticsearchFormattingServiceCollectionExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics;
using App.Metrics.Formatters.Elasticsearch;
using App.Metrics.Formatters.Elasticsearch.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Extension methods for setting up App Metrics Elasticsearch formatting services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class MetricsElasticsearchFormattingServiceCollectionExtensions
    {
        internal static void AddElasticsearchFormatterServices(this IServiceCollection services, string elasticsearchIndex)
        {
            var elasticsearchSetupOptionsDescriptor =
                ServiceDescriptor.Transient<IConfigureOptions<MetricsOptions>, MetricsElasticsearchDocumentFormattingOptionsSetup>(
                    provider => new MetricsElasticsearchDocumentFormattingOptionsSetup(elasticsearchIndex, provider.GetRequiredService<IOptions<MetricsElasticsearchDocumentFormattingOptions>>()));
            services.TryAddEnumerable(elasticsearchSetupOptionsDescriptor);
        }

        internal static void AddDefaultFormatterOptions(this IServiceCollection services)
        {
            services.Configure<MetricsOptions>(
                options =>
                {
                    if (options.DefaultOutputMetricsFormatter == null)
                    {
                        options.DefaultOutputMetricsFormatter = options.OutputMetricsFormatters.GetType<MetricsElasticsearchOutputFormatter>();
                    }
                });
        }
    }
}