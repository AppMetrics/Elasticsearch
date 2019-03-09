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
            public static readonly Dictionary<HistogramFields, string> CustomHistogramDataKeys = new Dictionary<HistogramFields, string>
                                                                                                        {
                                                                                                            {
                                                                                                                HistogramFields.Count,
                                                                                                                "countHist"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramFields.UserLastValue,
                                                                                                                "userLast"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramFields.UserMinValue,
                                                                                                                "userMin"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramFields.UserMaxValue,
                                                                                                                "userMax"
                                                                                                            }
                                                                                                        };

            public static readonly Dictionary<MeterFields, string> CustomMeterDataKeys = new Dictionary<MeterFields, string>
                                                                                                {
                                                                                                    { MeterFields.Count, "countMeter" },
                                                                                                    { MeterFields.RateMean, "rateMean" }
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