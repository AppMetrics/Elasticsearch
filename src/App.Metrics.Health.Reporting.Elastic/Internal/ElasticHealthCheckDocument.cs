using System;
using System.Text.RegularExpressions;

namespace App.Metrics.Health.Reporting.Elastic.Internal
{
    internal class ElasticHealthCheckDocument
    {
        private readonly string _healthCheckName;
        private readonly string _applicationName;

        private string SanitizePattern => "[^a-zA-Z0-9-]+";

        public ElasticHealthCheckDocument(string applicationName, string healthCheckName, string type = null)
        {
            _healthCheckName = healthCheckName ?? throw new ArgumentNullException(nameof(healthCheckName));
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));;
        }

        public string Id => string.Join("-", ApplicationName, HealthCheckName);

        private string HealthCheckName => Regex.Replace(_healthCheckName, SanitizePattern, "_").ToLowerInvariant();

        private string ApplicationName => Regex.Replace(_applicationName, SanitizePattern, "_").ToLowerInvariant();

        public string CheckStatus { get; set; }
        
        public string Message { get; set; }
    }
}