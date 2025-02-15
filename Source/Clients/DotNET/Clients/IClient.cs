// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Commands;
using Aksio.Cratis.Queries;

namespace Aksio.Cratis.Clients;

/// <summary>
/// Represents a connection.
/// </summary>
public interface IClient
{
    /// <summary>
    /// Gets the unique <see cref="ConnectionId"/> for the client.
    /// </summary>
    ConnectionId ConnectionId { get; }

    /// <summary>
    /// Gets whether or not the client is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connect to the kernel.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Connect();

    /// <summary>
    /// Perform a command.
    /// </summary>
    /// <param name="route">Route of the command.</param>
    /// <param name="command">Optional command payload.</param>
    /// <returns><see cref="CommandResult"/> of the operation.</returns>
    Task<CommandResult> PerformCommand(string route, object? command = default);

    /// <summary>
    /// Perform a query.
    /// </summary>
    /// <param name="route">Route of the command.</param>
    /// <param name="queryString">Optional querystring.</param>
    /// <returns><see cref="QueryResult"/> of the operation.</returns>
    Task<QueryResult> PerformQuery(string route, IDictionary<string, string>? queryString = default);
}
