// <copyright file="TextFileReporterExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Abstractions.Filtering;
using App.Metrics.Reporting.Abstractions;

namespace App.Metrics.Extensions.Reporting.TextFile
{
    public static class TextFileReporterExtensions
    {
        public static IReportFactory AddTextFile(
            this IReportFactory factory,
            TextFileReporterSettings settings,
            IFilterMetrics filter = null)
        {
            factory.AddProvider(new TextFileReporterProvider(settings, filter));
            return factory;
        }

        public static IReportFactory AddTextFile(this IReportFactory factory, IFilterMetrics filter = null)
        {
            var settings = new TextFileReporterSettings();
            factory.AddTextFile(settings, filter);
            return factory;
        }
    }
}