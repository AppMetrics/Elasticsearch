// <copyright file="ElasticsearchReporter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Filters;
using App.Metrics.Formatters;
using App.Metrics.Formatters.Elasticsearch;
using App.Metrics.Logging;
using App.Metrics.Reporting.Elasticsearch.Client;

namespace App.Metrics.Reporting.Elasticsearch
{
    public class ElasticsearchReporter : IReportMetrics
    {
        private static readonly ILog Logger = LogProvider.For<ElasticsearchReporter>();
        private readonly IElasticsearchClient _elasticsearchClient;

        public ElasticsearchReporter(
            MetricsReportingElasticsearchOptions options,
            IElasticsearchClient elasticsearchClient)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.FlushInterval < TimeSpan.Zero)
            {
                throw new InvalidOperationException($"{nameof(MetricsReportingElasticsearchOptions.FlushInterval)} must not be less than zero");
            }

            if (string.IsNullOrWhiteSpace(options.Elasticsearch.Index))
            {
                throw new InvalidOperationException($"{nameof(MetricsReportingElasticsearchOptions)} Elasticsearch Index is required");
            }

            _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));

            Formatter = options.MetricsOutputFormatter ?? new MetricsElasticsearchOutputFormatter(options.Elasticsearch.Index);

            FlushInterval = options.FlushInterval > TimeSpan.Zero
                ? options.FlushInterval
                : AppMetricsConstants.Reporting.DefaultFlushInterval;

            Filter = options.Filter;

            Logger.Info($"Using Metrics Reporter {this}. Url: {options.Elasticsearch.BaseUri} Index: {options.Elasticsearch.Index} FlushInterval: {FlushInterval}");
        }

        /// <inheritdoc />
        public IFilterMetrics Filter { get; set; }

        /// <inheritdoc />
        public TimeSpan FlushInterval { get; set; }

        /// <inheritdoc />
        public IMetricsOutputFormatter Formatter { get; set; }

        /// <inheritdoc />
        public async Task<bool> FlushAsync(MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            Logger.Trace("Flushing metrics snapshot");

            ElasticsearchWriteResult result;

            using (var stream = new MemoryStream())
            {
                await Formatter.WriteAsync(stream, metricsData, cancellationToken);

                result = await _elasticsearchClient.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()), cancellationToken);
            }

            if (result.Success)
            {
                Logger.Trace("Flushed metrics snapshot");
                return true;
            }

            Logger.Error(result.ErrorMessage);

            return false;
        }
    }
}