// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.Core.Abstractions;
using App.Metrics.Counter;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Meter;
using App.Metrics.Tagging;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Extensions
{
    public static class PackMetricValueSourceExtensions
    {
        private static readonly string MetricSetItemSuffix = "  items";

        public static void PackCounterSetItems<T>(
            this BulkPayloadBuilder payloadBuilder,
            string type,
            Func<string, string, string> metricNameFormatter,
            string context,
            MetricValueSourceBase<T> valueSource,
            CounterValue.SetItem setItem,
            CounterValueSource counterValueSource)
        {
            var itemData = new Dictionary<string, object> { { "total", setItem.Count } };

            if (counterValueSource.ReportItemPercentages)
            {
                itemData.Add("percent", setItem.Percent);
            }

            var keys = itemData.Keys.ToList();
            var values = keys.Select(k => itemData[k]);
            var tags = MetricTags.Concat(valueSource.Tags, setItem.Tags);

            if (valueSource.IsMultidimensional)
            {
                payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.MultidimensionalName + MetricSetItemSuffix),
                    keys,
                    values,
                    tags);
                return;
            }

            payloadBuilder.Pack(type, metricNameFormatter(context, valueSource.Name + MetricSetItemSuffix), keys, values, tags);
        }

        public static void PackMeterSetItems<T>(
            this BulkPayloadBuilder payloadBuilder,
            string type,
            Func<string, string, string> metricNameFormatter,
            string context,
            MetricValueSourceBase<T> valueSource,
            MeterValue.SetItem setItem)
        {
            var itemData = new Dictionary<string, object>();

            setItem.Value.AddMeterValues(itemData);
            itemData.Add("percent", setItem.Percent);

            var keys = itemData.Keys.ToList();
            var values = keys.Select(k => itemData[k]);

            var tags = MetricTags.Concat(valueSource.Tags, setItem.Tags);

            if (valueSource.IsMultidimensional)
            {
                payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.IsMultidimensional + MetricSetItemSuffix),
                    keys,
                    values,
                    tags);
                return;
            }

            payloadBuilder.Pack(
                type,
                metricNameFormatter(context, valueSource.Name + MetricSetItemSuffix),
                keys,
                values,
                setItem.Tags);
        }

        public static void PackValueSource<T>(
            this BulkPayloadBuilder payloadBuilder,
            string type,
            Func<string, string, string> metricNameFormatter,
            string context,
            MetricValueSourceBase<T> valueSource,
            Dictionary<string, object> data)
        {
            var keys = data.Keys.ToList();
            var values = keys.Select(k => data[k]);

            if (valueSource.IsMultidimensional)
            {
                payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.MultidimensionalName),
                    keys,
                    values,
                    valueSource.Tags);
                return;
            }

            payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.Name),
                    keys,
                    values,
                    valueSource.Tags);
        }

        public static void PackValueSource(
            this BulkPayloadBuilder payloadBuilder,
            string type,
            Func<string, string, string> metricNameFormatter,
            string context,
            MetricValueSourceBase<double> valueSource)
        {
            if (valueSource.IsMultidimensional)
            {
                payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.MultidimensionalName),
                    valueSource.Value,
                    valueSource.Tags);
                return;
            }

            payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.Name),
                    valueSource.Value,
                    valueSource.Tags);
        }

        public static void PackValueSource(
            this BulkPayloadBuilder payloadBuilder,
            string type,
            Func<string, string, string> metricNameFormatter,
            string context,
            MetricValueSourceBase<CounterValue> valueSource,
            CounterValueSource counterValueSource)
        {
            var count = valueSource.ValueProvider.GetValue(resetMetric: counterValueSource.ResetOnReporting).Count;

            if (valueSource.IsMultidimensional)
            {
                payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.MultidimensionalName),
                    count,
                    valueSource.Tags);
                return;
            }

            payloadBuilder.Pack(
                    type,
                    metricNameFormatter(context, valueSource.Name),
                    count,
                    valueSource.Tags);
        }
    }
}