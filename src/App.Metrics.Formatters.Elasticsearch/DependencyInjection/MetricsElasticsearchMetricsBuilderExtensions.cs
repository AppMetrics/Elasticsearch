// <copyright file="MetricsElasticsearchMetricsBuilderExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Elasticsearch;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Extension methods for setting up App Metrics Elasticsearch formatting services in an <see cref="IMetricsBuilder" />.
    /// </summary>
    public static class MetricsElasticsearchMetricsBuilderExtensions
    {
        /// <summary>
        ///     Adds Elasticsearch formatters to the specified <see cref="IMetricsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsBuilder" /> to add services to.</param>
        /// <param name="elasticsearchIndex">The Elasticsearch Index name</param>
        /// <returns>An <see cref="IMetricsBuilder"/> that can be used to further configure the App Metrics services.</returns>
        public static IMetricsBuilder AddElasticsearchFormatters(this IMetricsBuilder builder, string elasticsearchIndex)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddElasticsearchFormatterServices(elasticsearchIndex);
            builder.Services.AddDefaultFormatterOptions();

            return builder;
        }

        /// <summary>
        ///     Adds Elasticsearch options to the specified <see cref="IMetricsBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMetricsBuilder" /> to add services to.</param>
        /// <param name="setupAction">
        ///     An <see cref="Action" /> to configure the provided <see cref="MetricsElasticsearchDocumentFormattingOptions" />.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure the App Metrics services.
        /// </returns>
        public static IMetricsBuilder AddElasticsearchOptions(
            this IMetricsBuilder builder,
            Action<MetricsElasticsearchDocumentFormattingOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure(setupAction);

            return builder;
        }
    }
}