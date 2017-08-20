// <copyright file="ElasticsearchFormatterConstants.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Metrics.Formatters.Elasticsearch.Internal
{
    public static class ElasticsearchFormatterConstants
    {
        public class ElasticsearchDefaults
        {
            public static readonly Dictionary<HistogramValueDataKeys, string> CustomHistogramDataKeys = new Dictionary<HistogramValueDataKeys, string>
                                                                                                        {
                                                                                                            {
                                                                                                                HistogramValueDataKeys.Count,
                                                                                                                "countHist"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserLastValue,
                                                                                                                "userLast"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserMinValue,
                                                                                                                "userMin"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserMaxValue,
                                                                                                                "userMax"
                                                                                                            }
                                                                                                        };

            public static readonly Dictionary<MeterValueDataKeys, string> CustomMeterDataKeys = new Dictionary<MeterValueDataKeys, string>
                                                                                                {
                                                                                                    { MeterValueDataKeys.Count, "countMeter" },
                                                                                                    { MeterValueDataKeys.RateMean, "rateMean" }
                                                                                                };

            public static readonly string[] SpecialChars = { @"\", @"/", " ", "-", "+", "=", "{", "}", "[", "]", ":", "&", "^", "~", "?", "!", "," };

            public static readonly Func<string, string, string> MetricNameFormatter =
                (metricContext, metricName) => string.IsNullOrWhiteSpace(metricContext)
                    ? SpecialChars.Aggregate(metricName, (current, @char) => current.Replace(@char, "_")).ToLowerInvariant()
                    : SpecialChars.Aggregate($"{metricContext}__{metricName}", (current, @char) => current.Replace(@char, "_")).ToLowerInvariant();

            public static readonly Func<string, string> MetricTagValueFormatter = tagValue =>
            {
                return SpecialChars.Aggregate(tagValue, (current, @char) => current.Replace(@char, "_")).ToLowerInvariant();
            };
        }
    }
}