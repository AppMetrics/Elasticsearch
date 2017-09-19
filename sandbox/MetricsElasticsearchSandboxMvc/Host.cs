// <copyright file="Host.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics;
using App.Metrics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace MetricsElasticsearchSandboxMvc
{
    public static class Host
    {
        private static readonly string ElasticsearchIndex = "metricselasticsearchsandboxmvc";
        private static readonly string ElasticsearchUri = "http://localhost:9200";

        public static IWebHost BuildWebHost(string[] args)
        {
            ConfigureLogging();

            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureMetricsWithDefaults(
                              builder =>
                              {
                                  builder.Report.ToElasticsearch(ElasticsearchUri, ElasticsearchIndex); // TODO: allow load from config
                              })
                          .UseMetrics()
                          .UseSerilog()
                          .UseStartup<Startup>()
                          .Build();
        }

        public static void Main(string[] args) { BuildWebHost(args).Run(); }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .WriteTo.LiterateConsole()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}