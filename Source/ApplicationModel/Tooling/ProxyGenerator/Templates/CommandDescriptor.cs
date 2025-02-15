// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Applications.ProxyGenerator.Templates;

/// <summary>
/// Describes a command for templating purposes.
/// </summary>
/// <param name="Route">API route for the command.</param>
/// <param name="Name">Name of the command.</param>
/// <param name="Properties">Properties on the command.</param>
/// <param name="Imports">Additional import statements.</param>
/// <param name="Arguments">Arguments for the request - typically in the route or query string.</param>
/// <param name="HasResponse">Whether or not there is a response from the command.</param>
/// <param name="ResponseType">The details about the response type.</param>
public record CommandDescriptor(
    string Route,
    string Name,
    IEnumerable<PropertyDescriptor> Properties,
    IEnumerable<ImportStatement> Imports,
    IEnumerable<RequestArgumentDescriptor> Arguments,
    bool HasResponse,
    ModelDescriptor ResponseType);
