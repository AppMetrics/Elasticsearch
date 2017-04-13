// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Apdex;
using App.Metrics.Core.Abstractions;
using App.Metrics.Counter;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Health;
using App.Metrics.Histogram;
using App.Metrics.Infrastructure;
using App.Metrics.Meter;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;
using App.Metrics.Timer;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.InfluxDB
{
    public class InfluxDbReporter : IMetricReporter
    {
        private readonly ILineProtocolClient _lineProtocolClient;
        private readonly ILogger<InfluxDbReporter> _logger;
        private readonly Func<string, string, string> _metricNameFormatter;
        private readonly MetricValueDataKeys _dataKeys;
        private readonly IMetricPayloadBuilder<LineProtocolPayload> _payloadBuilder;
        private bool _disposed;

        public InfluxDbReporter(
            ILineProtocolClient lineProtocolClient,
            IMetricPayloadBuilder<LineProtocolPayload> payloadBuilder,
            TimeSpan reportInterval,
            ILoggerFactory loggerFactory,
            Func<string, string, string> metricNameFormatter,
            MetricValueDataKeys customDataKeys = null)
            : this(
                lineProtocolClient,
                payloadBuilder,
                reportInterval,
                typeof(InfluxDbReporter).Name,
                loggerFactory,
                metricNameFormatter,
                customDataKeys)
        {
        }

        public InfluxDbReporter(
            ILineProtocolClient lineProtocolClient,
            IMetricPayloadBuilder<LineProtocolPayload> payloadBuilder,
            TimeSpan reportInterval,
            string name,
            ILoggerFactory loggerFactory,
            Func<string, string, string> metricNameFormatter,
            MetricValueDataKeys customDataKeys = null)
        {
            ReportInterval = reportInterval;
            Name = name;

            _payloadBuilder = payloadBuilder;
            _metricNameFormatter = metricNameFormatter;
            _dataKeys = customDataKeys ?? new MetricValueDataKeys();
            _logger = loggerFactory.CreateLogger<InfluxDbReporter>();
            _lineProtocolClient = lineProtocolClient;
        }

        public string Name { get; }

        public TimeSpan ReportInterval { get; }

        public void Dispose() { Dispose(true); }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    _payloadBuilder.Clear();
                }
            }

            _disposed = true;
        }

        public async Task<bool> EndAndFlushReportRunAsync(IMetrics metrics)
        {
            _logger.LogTrace($"Ending {Name} Run");

            var result = await _lineProtocolClient.WriteAsync(_payloadBuilder.Payload());

            _payloadBuilder.Clear();

            return result.Success;
        }

        public void ReportEnvironment(EnvironmentInfo environmentInfo) { }

        public void ReportHealth(
            GlobalMetricTags globalTags,
            IEnumerable<HealthCheck.Result> healthyChecks,
            IEnumerable<HealthCheck.Result> degradedChecks,
            IEnumerable<HealthCheck.Result> unhealthyChecks)
        {
            // Health checks are reported as metrics as well
        }

        public void ReportMetric<T>(string context, MetricValueSourceBase<T> valueSource)
        {
            _logger.LogTrace($"Packing Metric {typeof(T)} for {Name}");

            if (typeof(T) == typeof(double))
            {
                ReportGauge(context, valueSource as MetricValueSourceBase<double>);
                return;
            }

            if (typeof(T) == typeof(CounterValue))
            {
                ReportCounter(context, valueSource as MetricValueSourceBase<CounterValue>);
                return;
            }

            if (typeof(T) == typeof(MeterValue))
            {
                ReportMeter(context, valueSource as MetricValueSourceBase<MeterValue>);
                return;
            }

            if (typeof(T) == typeof(TimerValue))
            {
                ReportTimer(context, valueSource as MetricValueSourceBase<TimerValue>);
                return;
            }

            if (typeof(T) == typeof(HistogramValue))
            {
                ReportHistogram(context, valueSource as MetricValueSourceBase<HistogramValue>);
                return;
            }

            if (typeof(T) == typeof(ApdexValue))
            {
                ReportApdex(context, valueSource as MetricValueSourceBase<ApdexValue>);
                return;
            }

            _logger.LogTrace($"Finished Packing Metric {typeof(T)} for {Name}");
        }

        public void StartReportRun(IMetrics metrics)
        {
            _logger.LogTrace($"Starting {Name} Report Run");

            _payloadBuilder.Init();
        }

        private void ReportApdex(string context, MetricValueSourceBase<ApdexValue> valueSource)
        {
            _payloadBuilder.PackApdex(_metricNameFormatter, context, valueSource, _dataKeys.Apdex);
        }

        private void ReportCounter(string context, MetricValueSourceBase<CounterValue> valueSource)
        {
            var counterValueSource = valueSource as CounterValueSource;
            _payloadBuilder.PackCounter(_metricNameFormatter, context, valueSource, counterValueSource, _dataKeys.Counter);
        }

        private void ReportGauge(string context, MetricValueSourceBase<double> valueSource)
        {
            _payloadBuilder.PackGauge(_metricNameFormatter, context, valueSource);
        }

        private void ReportHistogram(string context, MetricValueSourceBase<HistogramValue> valueSource)
        {
            _payloadBuilder.PackHistogram(_metricNameFormatter, context, valueSource, _dataKeys.Histogram);
        }

        private void ReportMeter(string context, MetricValueSourceBase<MeterValue> valueSource)
        {
            _payloadBuilder.PackMeter(_metricNameFormatter, context, valueSource, _dataKeys.Meter);
        }

        private void ReportTimer(string context, MetricValueSourceBase<TimerValue> valueSource)
        {
            _payloadBuilder.PackTimer(_metricNameFormatter, context, valueSource, _dataKeys.Meter, _dataKeys.Histogram);
        }
    }
}