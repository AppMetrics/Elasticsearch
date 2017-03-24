// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using App.Metrics.Internal;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddIfNotNanOrInfinity(this IDictionary<string, object> values, string key, double value)
        {
            if (!double.IsNaN(value) && !double.IsInfinity(value))
            {
                values.Add(key, value);
            }
        }

        public static void AddIfPresent(this IDictionary<string, object> values, string key, string value)
        {
            if (value.IsPresent())
            {
                values.Add(key, value);
            }
        }
    }
}