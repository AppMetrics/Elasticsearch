// <copyright file="TextFileReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.TextFile
{
    public class TextFileReporterProvider : IReporterProvider
    {
        private readonly TextFileReporterSettings _settings;

        public TextFileReporterProvider(TextFileReporterSettings settings, IFilterMetrics fitler)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
            Filter = fitler;
        }

        public IFilterMetrics Filter { get; }

        public IMetricReporter CreateMetricReporter(string name, ILoggerFactory loggerFactory)
        {
            return new TextFileReporter(name, _settings.FileName, _settings.ReportInterval, loggerFactory);
        }
    }
}