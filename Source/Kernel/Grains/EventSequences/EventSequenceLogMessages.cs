// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Execution;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Grains.EventSequences;

/// <summary>
/// Holds log messages for <see cref="EventSequence"/>.
/// </summary>
internal static partial class EventSequenceLogMessages
{
    [LoggerMessage(0, LogLevel.Debug, "Appending '{EventName}-{EventType}' for EventSource {EventSource} with sequence number {SequenceNumber} to event sequence '{EventSequenceId} for microservice {MicroserviceId} on tenant {TenantId}")]
    internal static partial void Appending(this ILogger<EventSequence> logger, MicroserviceId microserviceId, TenantId tenantId, EventSequenceId eventSequenceId, EventType eventType, string eventName, EventSourceId eventSource, ulong sequenceNumber);

    [LoggerMessage(1, LogLevel.Debug, "Compensating event @ {SequenceNumber} in event sequence {EventSequenceId} - event type '{EventType}' for microservice '{MicroserviceId}' on tenant {TenantId}")]
    internal static partial void Compensating(this ILogger<EventSequence> logger, MicroserviceId microserviceId, TenantId tenantId, EventType eventType, EventSequenceId eventSequenceId, ulong sequenceNumber);

    [LoggerMessage(2, LogLevel.Critical, "Failed appending event type '{EventType}' at sequence {SequenceNumber} for event source {EventSourceId} to stream {StreamId} for microservice '{MicroserviceId}' on tenant {TenantId}")]
    internal static partial void FailedAppending(this ILogger<EventSequence> logger, MicroserviceId microserviceId, TenantId tenantId, EventType eventType, Guid streamId, string eventSourceId, ulong sequenceNumber, Exception exception);

    [LoggerMessage(3, LogLevel.Error, "Error when appending event at sequence {SequenceNumber} for event source {EventSourceId} to event sequence {EventSequenceId} for microservice {MicroserviceId} on tenant {TenantId}")]
    internal static partial void ErrorAppending(this ILogger<EventSequence> logger, MicroserviceId microserviceId, TenantId tenantId, EventSequenceId eventSequenceId, string eventSourceId, ulong sequenceNumber, Exception exception);
}
