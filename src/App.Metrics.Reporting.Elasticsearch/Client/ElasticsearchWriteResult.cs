// <copyright file="ElasticsearchWriteResult.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

namespace App.Metrics.Reporting.Elasticsearch.Client
{
    public struct ElasticsearchWriteResult
    {
        public ElasticsearchWriteResult(bool success)
        {
            Success = success;
            ErrorMessage = null;
        }

        public ElasticsearchWriteResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public bool Success { get; }
    }
}