// <copyright file="Host.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.AspNetCore.Reporting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace App.Metrics.Elasticsearch.Sandbox
{
    public static class Host
    {
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureServices(AddMetricsOptions)
                .UseMetrics()
                .UseMetricsReporting(ConfigureMetricsReportingOptions())
                .UseStartup<Startup>()
                .Build();
        }

        public static void Main(string[] args) { BuildWebHost(args).Run(); }

        private static void AddMetricsOptions(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MetricsEndpointsOptions>, MetricsEndpointsOptionsSetup>());
        }

        private static Action<WebHostBuilderContext, MetricsReportingWebHostOptions> ConfigureMetricsReportingOptions()
        {
            return (context, options) =>
            {
                options.ReportingBuilder = reportingBuilder =>
                {
                    reportingBuilder.AddElasticsearch(context.Configuration);
                };
            };
        }
    }
}