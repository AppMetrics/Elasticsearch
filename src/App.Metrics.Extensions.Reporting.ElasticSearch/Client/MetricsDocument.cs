// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class MetricsDocument
    {
        [JsonProperty("name")]
        public string MeasurementName { get; set; }

        [JsonIgnore]
        public string MeasurementType { get; internal set; }

        [JsonProperty("fields")]
        public IDictionary<string, object> Fields { get; set; }

        [JsonProperty("tags")]
        public IDictionary<string, string> Tags { get; set; }

        [JsonProperty("@timestamp")]
        public DateTime WrittenOn { get; set; } = DateTime.UtcNow;
    }
}
