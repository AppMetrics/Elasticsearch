// <copyright file="MetricsElasticsearchDocumentFormattingOptionsSetup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Options;

namespace App.Metrics.Formatters.Elasticsearch.Internal
{
    /// <summary>
    ///     Sets up default Elasticsearch documentFormattingOptions for <see cref="MetricsOptions"/>.
    /// </summary>
    public class MetricsElasticsearchDocumentFormattingOptionsSetup : IConfigureOptions<MetricsOptions>
    {
        private readonly string _elasticsearchIndex;
        private readonly MetricsElasticsearchDocumentFormattingOptions _elasticsearchDocumentFormattingOptions;

        public MetricsElasticsearchDocumentFormattingOptionsSetup(
            string elasticsearchIndex,
            IOptions<MetricsElasticsearchDocumentFormattingOptions> elasticsearchOptionsAccessor)
        {
            if (string.IsNullOrEmpty(elasticsearchIndex))
            {
                throw new ArgumentNullException(nameof(elasticsearchIndex));
            }

            _elasticsearchIndex = elasticsearchIndex;
            _elasticsearchDocumentFormattingOptions = elasticsearchOptionsAccessor.Value ?? throw new ArgumentNullException(nameof(elasticsearchOptionsAccessor));
        }

        /// <inheritdoc/>
        public void Configure(MetricsOptions options)
        {
            var formatter = new MetricsElasticsearchOutputFormatter(_elasticsearchIndex, _elasticsearchDocumentFormattingOptions);

            options.OutputMetricsFormatters.Add(formatter);
        }
    }
}