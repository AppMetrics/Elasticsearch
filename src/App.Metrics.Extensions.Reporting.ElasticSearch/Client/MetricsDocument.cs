using App.Metrics.Tagging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Metrics.Extensions.Reporting.ElasticSearch.Client
{
    public class MetricsDocument
    {
        [JsonProperty("name")]
        public string MeasurementName { get; set; }

        [JsonProperty("fields")]
        public IDictionary<string, object> Fields { get; set; }

        [JsonProperty("tags")]
        public IDictionary<string, string> Tags { get; set; }
    }
}
