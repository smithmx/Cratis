// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Clients;

internal static partial class OrleansAzureTableStoreKernelClientLogMessages
{
    [LoggerMessage(0, LogLevel.Information, "Getting Silo hosts from Azure Storage.")]
    internal static partial void GettingSilosFromStorage(this ILogger<OrleansAzureTableStoreKernelClient> logger);

    [LoggerMessage(1, LogLevel.Information, "Using Silo endpoints : {Endpoints}")]
    internal static partial void UsingEndpoints(this ILogger<OrleansAzureTableStoreKernelClient> logger, string endpoints);
}
