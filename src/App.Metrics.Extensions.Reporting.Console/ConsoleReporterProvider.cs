// <copyright file="ConsoleReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.Console
{
    public class ConsoleReporterProvider : IReporterProvider
    {
        private readonly ConsoleReporterSettings _settings;

        public ConsoleReporterProvider(ConsoleReporterSettings settings, IFilterMetrics filter)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;

            Filter = filter;
        }

        public IFilterMetrics Filter { get; }

        public IMetricReporter CreateMetricReporter(string name, ILoggerFactory loggerFactory)
        {
            return new ConsoleReporter(name, _settings.ReportInterval, loggerFactory);
        }
    }
}