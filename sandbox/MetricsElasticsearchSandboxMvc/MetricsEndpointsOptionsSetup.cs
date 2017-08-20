// <copyright file="MetricsEndpointsOptionsSetup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.Formatters.Elasticsearch;
using Microsoft.Extensions.Options;

namespace MetricsElasticsearchSandboxMvc
{
    /// <summary>
    ///     Sets up the metrics web hosting options for this application
    /// </summary>
    public class MetricsEndpointsOptionsSetup : IConfigureOptions<MetricsEndpointsOptions>
    {
        private readonly IOptions<MetricsOptions> _metricsOptionsAccessor;

        public MetricsEndpointsOptionsSetup(IOptions<MetricsOptions> metricsOptionsAccessor) { _metricsOptionsAccessor = metricsOptionsAccessor; }

        public void Configure(MetricsEndpointsOptions options)
        {
            options.MetricsEndpointOutputFormatter =
                _metricsOptionsAccessor.Value.OutputMetricsFormatters.GetType<MetricsElasticsearchOutputFormatter>();
        }
    }
}