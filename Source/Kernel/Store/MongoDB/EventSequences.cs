// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Aksio.Cratis.DependencyInversion;
using Aksio.Cratis.Execution;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aksio.Cratis.Events.Store.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IEventSequences"/> for MongoDB.
/// </summary>
[SingletonPerMicroserviceAndTenant]
public class EventSequences : IEventSequences
{
    readonly ILogger<EventSequences> _logger;
    readonly IExecutionContextManager _executionContextManager;
    readonly IEventStoreDatabase _eventStoreDatabase;

    /// <summary>
    /// Initializes a new instance of <see cref="EventSequences"/>.
    /// </summary>
    /// <param name="eventStoreDatabase"><see cref="ProviderFor{T}">Provider for</see> <see cref="IMongoDatabase"/>.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for getting current <see cref="ExecutionContext"/>.</param>
    public EventSequences(IEventStoreDatabase eventStoreDatabase, ILogger<EventSequences> logger, IExecutionContextManager executionContextManager)
    {
        _eventStoreDatabase = eventStoreDatabase;
        _logger = logger;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public async Task Append(EventSequenceId eventSequenceId, EventSequenceNumber sequenceNumber, EventSourceId eventSourceId, EventType eventType, JsonObject content)
    {
        try
        {
            _logger.Appending(sequenceNumber);
            var @event = new Event(
                sequenceNumber,
                _executionContextManager.Current.CorrelationId,
                _executionContextManager.Current.CausationId,
                _executionContextManager.Current.CausedBy,
                eventType.Id,
                DateTimeOffset.UtcNow,
                eventSourceId,
                new Dictionary<string, BsonDocument>
                {
                        { eventType.Generation.ToString(), BsonDocument.Parse(content.ToJsonString()) }
                },
                Array.Empty<EventCompensation>());
            await GetCollectionFor(eventSequenceId).InsertOneAsync(@event);
        }
        catch (Exception ex)
        {
            _logger.AppendFailure(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task Compensate(EventSequenceId eventSequenceId, EventSequenceNumber sequenceNumber, EventType eventType, JsonObject content) => throw new NotImplementedException();

    IMongoCollection<Event> GetCollectionFor(EventSequenceId eventSequenceId) => _eventStoreDatabase.GetEventSequenceCollectionFor(eventSequenceId);
}
