// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Reporting;

namespace App.Metrics.Extensions.Reporting.Console
{
    public class ConsoleReporterSettings : IReporterSettings
    {
        /// <inheritdoc />
        public MetricValueDataKeys DataKeys { get; set; }

        public TimeSpan ReportInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}