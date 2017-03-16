// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Reporting.Abstractions;

// ReSharper disable CheckNamespace
namespace App.Metrics.Reporting.Interfaces
{
    // ReSharper restore CheckNamespace
    public static class ElasticSearchReporterExtensions
    {
        public static IReportFactory AddElasticSearch(
            this IReportFactory factory,
            ElasticSearchReporterSettings settings,
            IFilterMetrics filter = null)
        {
            factory.AddProvider(new ElasticSearchReporterProvider(settings, filter));
            return factory;
        }

        public static IReportFactory AddElasticSearch(
            this IReportFactory factory,
            string database,
            Uri baseAddress,
            IFilterMetrics filter = null)
        {
            var settings = new ElasticSearchBReporterSettings
            {
                ElasticSearchSettings = new ElasticSearchSettings(database, baseAddress)
                           };

            factory.AddElasticSearch(settings, filter);
            return factory;
        }
    }
}