// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Aksio.Cratis.Events;
using Aksio.Cratis.Events.Projections;
using Aksio.Cratis.Execution;
using Aksio.Cratis.Schemas;
using Orleans;

namespace Aksio.Cratis.Integration;

/// <summary>
/// Represents an implementation of <see cref="IAdapterProjectionFactory"/>.
/// </summary>
public class AdapterProjectionFactory : IAdapterProjectionFactory
{
    readonly IEventTypes _eventTypes;
    readonly IJsonSchemaGenerator _schemaGenerator;
    readonly JsonSerializerOptions _jsonSerializerOptions;
    readonly IClusterClient _clusterClient;
    readonly IExecutionContextManager _executionContextManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdapterProjectionFactory"/> class.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes"/> to use.</param>
    /// <param name="schemaGenerator">The <see cref="IJsonSchemaGenerator"/> for generating schemas.</param>
    /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/> for serialization.</param>
    /// <param name="clusterClient">Orleans <see cref="IClusterClient"/>.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with the execution context.</param>
    public AdapterProjectionFactory(
        IEventTypes eventTypes,
        IJsonSchemaGenerator schemaGenerator,
        JsonSerializerOptions jsonSerializerOptions,
        IClusterClient clusterClient,
        IExecutionContextManager executionContextManager)
    {
        _eventTypes = eventTypes;
        _schemaGenerator = schemaGenerator;
        _jsonSerializerOptions = jsonSerializerOptions;
        _clusterClient = clusterClient;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public Task<IAdapterProjectionFor<TModel>> CreateFor<TModel, TExternalModel>(IAdapterFor<TModel, TExternalModel> adapter)
    {
        var projectionBuilder = new ProjectionBuilderFor<TModel>(adapter.Identifier.Value, _eventTypes, _schemaGenerator, _jsonSerializerOptions);
        adapter.DefineModel(projectionBuilder);
        var projectionDefinition = projectionBuilder.Build();
        return Task.FromResult<IAdapterProjectionFor<TModel>>(new AdapterProjectionFor<TModel>(projectionDefinition, _clusterClient, _jsonSerializerOptions, _executionContextManager));
    }
}
