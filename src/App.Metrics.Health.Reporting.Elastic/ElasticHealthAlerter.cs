using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health.Logging;
using App.Metrics.Health.Reporting.Elastic.Internal;
using Nest;

namespace App.Metrics.Health.Reporting.Elastic
{
    public class ElasticHealthAlerter : IReportHealthStatus
    {
        private static readonly HashSet<string> LastUnhealthyCheckCache = new HashSet<string>();
        private static readonly HashSet<string> LastDegradedCheckCache = new HashSet<string>();
        private static readonly ILog Logger = LogProvider.For<ElasticHealthAlerter>();
        private static int _runs = 0;
        private readonly IElasticClient _client;
        private readonly ElasticHealthAlertOptions _options;

        public ElasticHealthAlerter(ElasticHealthAlertOptions options, IElasticClient client)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            ReportInterval = options.ReportInterval > TimeSpan.Zero
                ? options.ReportInterval
                : HealthConstants.Reporting.DefaultReportInterval;

            Logger.Trace($"Using Metrics Reporter {this}. Index: {options.Index} ReportInterval: {ReportInterval}");
        }

        /// <inheritdoc />
        public TimeSpan ReportInterval { get; set; }

        /// <inheritdoc />
        public async Task ReportAsync(HealthOptions options, HealthStatus status,
            CancellationToken cancellationToken = default)
        {
            var resetRuns = false;
            _runs++;

            if (!_options.Enabled || !options.Enabled)
            {
                Logger.Trace($"Health Status Reporter `{this}` disabled, not reporting.");

                return;
            }

            Logger.Trace($"Health Status Reporter `{this}` reporting health status.");

            var healthCheckItems = new List<ElasticHealthCheckDocument>();

            var newUnhealthyChecks = status.Results.Where(r =>
                r.Check.Status == HealthCheckStatus.Unhealthy && !LastUnhealthyCheckCache.Contains(r.Name)).ToList();

            if (newUnhealthyChecks.Any())
            {
                AddHealthChecks(HealthCheckStatus.Unhealthy, newUnhealthyChecks, healthCheckItems);
            }

            if (_runs > _options.RunsBeforeReportExistingFailures)
            {
                resetRuns = true;

                var existingUnHealthyChecks = status.Results
                    .Where(r => r.Check.Status == HealthCheckStatus.Unhealthy &&
                                LastUnhealthyCheckCache.Contains(r.Name) &&
                                newUnhealthyChecks.All(c => c.Name != r.Name))
                    .ToList();

                if (existingUnHealthyChecks.Any())
                {
                    AddHealthChecks(HealthCheckStatus.Unhealthy, existingUnHealthyChecks, healthCheckItems);
                }
            }

            if (_options.AlertOnDegradedChecks)
            {
                var degradedChecks = status.Results.Where(r =>
                    r.Check.Status == HealthCheckStatus.Degraded && !LastDegradedCheckCache.Contains(r.Name)).ToList();

                if (degradedChecks.Any())
                {
                    AddHealthChecks(HealthCheckStatus.Degraded, degradedChecks, healthCheckItems);
                }

                if (_runs > _options.RunsBeforeReportExistingFailures)
                {
                    resetRuns = true;

                    var existingDegradedChecks = status.Results
                        .Where(r => r.Check.Status == HealthCheckStatus.Degraded &&
                                    LastDegradedCheckCache.Contains(r.Name) &&
                                    degradedChecks.All(c => c.Name != r.Name))
                        .ToList();

                    if (existingDegradedChecks.Any())
                    {
                        AddHealthChecks(HealthCheckStatus.Degraded, existingDegradedChecks, healthCheckItems);
                    }
                }
            }

            if (resetRuns)
            {
                _runs = 0;
            }

            if (healthCheckItems.Any())
            {
                try
                {
                    var bulkRequest = new BulkRequest(_options.Index) {Operations = new List<IBulkOperation>()};

                    foreach (var item in healthCheckItems)
                    {
                        bulkRequest.Operations.Add(new BulkIndexOperation<ElasticHealthCheckDocument>(item));
                    }

                    var response = await _client.BulkAsync(bulkRequest, cancellationToken);

                    if (response.IsValid)
                    {
                        Logger.Trace($"Health Status Reporter `{this}` successfully reported health status.");
                    }
                    else
                    {
                        Logger.Error(
                            $"Health Status Reporter `{this}` failed to reported health status with status code: Invalid and reason phrase: `{response.OriginalException}`");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Health Status Reporter `{this}` failed to reported health status");
                }
            }

            await AlertStatusChangeChecks(status, cancellationToken);
        }

        private void AddHealthChecks(
            HealthCheckStatus checkStatus,
            IReadOnlyCollection<HealthCheck.Result> checks,
            List<ElasticHealthCheckDocument> documents)
        {
            foreach (var result in checks)
            {
                if (result.Check.Status == HealthCheckStatus.Unhealthy)
                {
                    LastDegradedCheckCache.Remove(result.Name);
                    LastUnhealthyCheckCache.Add(result.Name);
                }
                else if (result.Check.Status == HealthCheckStatus.Degraded)
                {
                    LastUnhealthyCheckCache.Remove(result.Name);
                    LastDegradedCheckCache.Add(result.Name);
                }

                var doc = new ElasticHealthCheckDocument(_options.ApplicationName, result.Name)
                {
                    CheckStatus = result.Check.Status.ToString(),
                    Message = result.Check.Message
                };

                documents.Add(doc);
            }
        }

        private async Task AlertStatusChangeChecks(HealthStatus status,
            CancellationToken cancellationToken)
        {
            var healthyChecks = status.Results.Where(r => r.Check.Status == HealthCheckStatus.Healthy).ToList();

            if (!healthyChecks.Any())
            {
                return;
            }

            var recoveredChecks = new List<HealthCheck.Result>();

            foreach (var check in healthyChecks)
            {
                if (LastUnhealthyCheckCache.Contains(check.Name))
                {
                    recoveredChecks.Add(check);
                    LastUnhealthyCheckCache.Remove(check.Name);
                    LastDegradedCheckCache.Remove(check.Name);
                }
                else if (LastDegradedCheckCache.Contains(check.Name))
                {
                    recoveredChecks.Add(check);
                    LastDegradedCheckCache.Remove(check.Name);
                    LastUnhealthyCheckCache.Remove(check.Name);
                }
            }

            if (recoveredChecks.Any())
            {
                try
                {
                    var bulkRequest = new BulkRequest(_options.Index) {Operations = new List<IBulkOperation>()};

                    foreach (var check in recoveredChecks)
                    {
                        bulkRequest.Operations.Add(new BulkIndexOperation<ElasticHealthCheckDocument>(
                            new ElasticHealthCheckDocument(_options.ApplicationName, check.Name)
                            {
                                CheckStatus = check.Check.Status.ToString(),
                                Message = check.Check.Message
                            }));
                    }

                    var response = await _client.BulkAsync(bulkRequest, cancellationToken);

                    if (response.IsValid)
                    {
                        Logger.Trace($"Health Status Reporter `{this}` successfully reported health status changes.");

                        await AlertStatusChangeChecks(status, cancellationToken);
                    }
                    else
                    {
                        Logger.Error(
                            $"Health Status Reporter `{this}` failed to reported health status changes with status code: Invalid and reason phrase: `{response.OriginalException}`");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Health Status Reporter `{this}` failed to reported health status changes.");
                }
            }
        }
    }
}