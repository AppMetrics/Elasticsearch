// <copyright file="ILineProtocolClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.Extensions.Reporting.InfluxDB.Client
{
    public interface ILineProtocolClient
    {
        Task<LineProtocolWriteResult> WriteAsync(
            LineProtocolPayload payload,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}