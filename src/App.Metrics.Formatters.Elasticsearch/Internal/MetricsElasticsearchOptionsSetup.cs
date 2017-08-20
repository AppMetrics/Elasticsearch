// <copyright file="MetricsElasticsearchOptionsSetup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Options;

namespace App.Metrics.Formatters.Elasticsearch.Internal
{
    /// <summary>
    ///     Sets up default Elasticsearch documentFormattingOptions for <see cref="MetricsOptions"/>.
    /// </summary>
    public class MetricsElasticsearchOptionsSetup : IConfigureOptions<MetricsOptions>
    {
        private readonly MetricsElasticsearchDocumentFormattingOptions _elasticsearchDocumentFormattingOptions;

        public MetricsElasticsearchOptionsSetup(IOptions<MetricsElasticsearchDocumentFormattingOptions> elasticsearchOptionsAccessor)
        {
            _elasticsearchDocumentFormattingOptions = elasticsearchOptionsAccessor.Value ?? throw new ArgumentNullException(nameof(elasticsearchOptionsAccessor));
        }

        /// <inheritdoc/>
        public void Configure(MetricsOptions options)
        {
            var formatter = new MetricsElasticsearchOutputFormatter(_elasticsearchDocumentFormattingOptions);

            options.OutputMetricsFormatters.Add(formatter);
        }
    }
}