using System;

namespace App.Metrics.Health.Reporting.Elastic
{
    public class ElasticHealthAlertOptions
    {
        public bool AlertOnDegradedChecks { get; set; } = true;

        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets the health status reporting interval.
        /// </summary>
        /// <remarks>
        ///     If not set reporting interval will be set to the <see cref="HealthConstants.Reporting.DefaultReportInterval" />.
        /// </remarks>
        /// <value>
        ///     The <see cref="TimeSpan" /> to wait between reporting health status.
        /// </value>
        public TimeSpan ReportInterval { get; set; }

        /// <summary>
        ///     Gets or sets the number of report runs before re-alerting checks that have re-failed. If set to 0, failed checks will only be reported once until healthy again.
        /// </summary>
        /// <remarks>
        ///     If not set number of runs will be set to the <see cref="HealthConstants.Reporting.DefaultNumberOfRunsBeforeReAlerting" />.
        /// </remarks>
        public int RunsBeforeReportExistingFailures { get; set; } = HealthConstants.Reporting.DefaultNumberOfRunsBeforeReAlerting;

        public string ApplicationName { get; set; }

        public string Index { get; set; }
        
        public string Uri { get; set; }
    }
}