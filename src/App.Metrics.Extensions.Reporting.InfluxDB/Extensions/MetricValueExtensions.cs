// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using App.Metrics.Apdex;
using App.Metrics.Histogram;
using App.Metrics.Meter;

namespace App.Metrics.Extensions.Reporting.InfluxDB.Extensions
{
    internal static class MetricValueExtensions
    {
        public static void AddApdexValues(this ApdexValue apdex, IDictionary<string, object> values)
        {
            values.Add("samples", apdex.SampleSize);
            values.AddIfNotNanOrInfinity("score", apdex.Score);
            values.Add("satisfied", apdex.Satisfied);
            values.Add("tolerating", apdex.Tolerating);
            values.Add("frustrating", apdex.Frustrating);
        }

        public static void AddHistogramValues(this HistogramValue histogram, IDictionary<string, object> values)
        {
            values.Add("samples", histogram.SampleSize);
            values.AddIfNotNanOrInfinity("last", histogram.LastValue);
            values.Add("count.hist", histogram.Count);
            values.Add("sum", histogram.Sum);
            values.AddIfNotNanOrInfinity("min", histogram.Min);
            values.AddIfNotNanOrInfinity("max", histogram.Max);
            values.AddIfNotNanOrInfinity("mean", histogram.Mean);
            values.AddIfNotNanOrInfinity("median", histogram.Median);
            values.AddIfNotNanOrInfinity("stddev", histogram.StdDev);
            values.AddIfNotNanOrInfinity("p999", histogram.Percentile999);
            values.AddIfNotNanOrInfinity("p99", histogram.Percentile99);
            values.AddIfNotNanOrInfinity("p98", histogram.Percentile98);
            values.AddIfNotNanOrInfinity("p95", histogram.Percentile95);
            values.AddIfNotNanOrInfinity("p75", histogram.Percentile75);
            values.AddIfPresent("user.last", histogram.LastUserValue);
            values.AddIfPresent("user.min", histogram.MinUserValue);
            values.AddIfPresent("user.max", histogram.MaxUserValue);
        }

        public static void AddMeterValues(this MeterValue meter, IDictionary<string, object> values)
        {
            values.Add("count.meter", meter.Count);
            values.AddIfNotNanOrInfinity("rate1m", meter.OneMinuteRate);
            values.AddIfNotNanOrInfinity("rate5m", meter.FiveMinuteRate);
            values.AddIfNotNanOrInfinity("rate15m", meter.FifteenMinuteRate);
            values.AddIfNotNanOrInfinity("rate.mean", meter.MeanRate);
        }
    }
}