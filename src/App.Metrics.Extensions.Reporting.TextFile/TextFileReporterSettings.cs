// <copyright file="TextFileReporterSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Reporting;

namespace App.Metrics.Extensions.Reporting.TextFile
{
    public class TextFileReporterSettings : IReporterSettings
    {
        public string FileName { get; set; }

        /// <inheritdoc />
        public MetricValueDataKeys DataKeys { get; set; }

        public TimeSpan ReportInterval { get; set; } = TimeSpan.FromSeconds(5);
    }
}