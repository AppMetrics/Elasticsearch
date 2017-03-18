// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace App.Metrics.Internal
{
    internal static class StringExtensions
    {
        [DebuggerStepThrough]
        internal static bool IsPresent(this string value) { return !string.IsNullOrWhiteSpace(value); }

        [DebuggerStepThrough]
        internal static bool IsMissing(this string value) { return string.IsNullOrWhiteSpace(value); }
    }
}
