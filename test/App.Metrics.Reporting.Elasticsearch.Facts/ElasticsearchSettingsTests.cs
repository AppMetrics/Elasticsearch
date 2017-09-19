// <copyright file="ElasticsearchSettingsTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Reporting.Elasticsearch.Client;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Reporting.Elasticsearch.Facts
{
    // ReSharper disable InconsistentNaming
    public class ElasticsearchSettingsTests
        // ReSharper restore InconsistentNaming
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Index_name_cannot_be_null_or_whitespace(string index)
        {
            // Arrange
            Action action = () =>
            {
                // Act
                var settings = new ElasticsearchOptions(new Uri("http://localhost"), index);
            };

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Index_name_cannot_be_null_or_whitespace_when_specifing_creds(string index)
        {
            // Arrange
            Action action = () =>
            {
                // Act
                var settings = new ElasticsearchOptions(new Uri("http://localhost"), index, "username", "password");
            };

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Index_name_cannot_be_null_or_whitespace_when_specifing_token(string index)
        {
            // Arrange
            Action action = () =>
            {
                // Act
                var settings = new ElasticsearchOptions(new Uri("http://localhost"), index, "token");
            };

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Scheme_should_be_anonymous_when_no_creds()
        {
            // Arrange
            var settings = new ElasticsearchOptions(new Uri("http://localhost"), "index");

            // Act
            var scheme = settings.AuthorizationSchema;

            // Assert
            scheme.Should().Be(ElasticSearchAuthorizationSchemes.Anonymous);
        }

        [Fact]
        public void Scheme_should_be_basic_when_specifing_creds()
        {
            // Arrange
            var settings = new ElasticsearchOptions(new Uri("http://localhost"), "index", "username", "password");

            // Act
            var scheme = settings.AuthorizationSchema;

            // Assert
            scheme.Should().Be(ElasticSearchAuthorizationSchemes.Basic);
        }

        [Fact]
        public void Scheme_should_be_bearer_token_when_specifing_token()
        {
            // Arrange
            var settings = new ElasticsearchOptions(new Uri("http://localhost"), "index", "token");

            // Act
            var scheme = settings.AuthorizationSchema;

            // Assert
            scheme.Should().Be(ElasticSearchAuthorizationSchemes.BearerToken);
        }
    }
}