using System;
using Nest;

namespace App.Metrics.Health.Reporting.Elastic.Builder
{
    public static class ElasticHealthAlerterBuilderExtensions
    {
        public static IHealthBuilder ToElastic(
            this IHealthReportingBuilder healthReportingBuilder,
            Action<ElasticHealthAlertOptions> optionsSetup)
        {
            var options = new ElasticHealthAlertOptions();

            optionsSetup(options);
            
            if (string.IsNullOrEmpty(options.Uri))
            {
                throw new ArgumentNullException(nameof(options.Uri),"Please provide a URI that we need to write health checks to");
            }

            var elasticClient = new ElasticClient(new Uri(options.Uri));
            healthReportingBuilder.Using(new ElasticHealthAlerter(options, elasticClient));

            return healthReportingBuilder.Builder;
        }

        public static IHealthBuilder ToElastic(
            this IHealthReportingBuilder healthReportingBuilder,
            ElasticHealthAlertOptions options)
        {
            if (string.IsNullOrEmpty(options.Uri))
            {
                throw new ArgumentNullException(nameof(options.Uri),"Please provide a URI that we need to write health checks to");
            }

            var elasticClient = new ElasticClient(new Uri(options.Uri));
            healthReportingBuilder.Using(new ElasticHealthAlerter(options, elasticClient));

            return healthReportingBuilder.Builder;
        }
    }
}