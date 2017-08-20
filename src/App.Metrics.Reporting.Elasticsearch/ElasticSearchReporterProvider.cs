// <copyright file="ElasticsearchReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Filters;
using App.Metrics.Reporting.Elasticsearch.Client;
using Microsoft.Extensions.Options;

namespace App.Metrics.Reporting.Elasticsearch
{
    public class ElasticsearchReporterProvider : IReporterProvider
    {
        private readonly IOptions<MetricsReportingElasticsearchOptions> _elasticsearchOptionsAccessor;
        private readonly IElasticsearchClient _elasticsearchClient;

        public ElasticsearchReporterProvider(
            IOptions<MetricsReportingOptions> optionsAccessor,
            IOptions<MetricsReportingElasticsearchOptions> elasticsearchReportingOptionsAccessor,
            IElasticsearchClient elasticsearchClient)
        {
            _elasticsearchOptionsAccessor = elasticsearchReportingOptionsAccessor ?? throw new ArgumentNullException(nameof(elasticsearchReportingOptionsAccessor));
            _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
            Filter = elasticsearchReportingOptionsAccessor.Value.Filter ?? optionsAccessor.Value.Filter;
            ReportInterval = elasticsearchReportingOptionsAccessor.Value.ReportInterval;
        }

        /// <inheritdoc />
        public IFilterMetrics Filter { get; }

        /// <inheritdoc />
        public TimeSpan ReportInterval { get; }

        /// <inheritdoc />
        public async Task<bool> FlushAsync(MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = new MemoryStream())
            {
                await _elasticsearchOptionsAccessor.Value.MetricsOutputFormatter.WriteAsync(stream, metricsData, cancellationToken);

                await _elasticsearchClient.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()), cancellationToken);
            }

            return true;
        }
    }
}