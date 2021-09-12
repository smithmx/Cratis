// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Events.Store
{
    /// <summary>
    /// Defines an immutable event log.
    /// </summary>
    public interface IEventLogs
    {
        /// <summary>
        /// Commit a single event to the event store.
        /// </summary>
        /// <param name="eventLogId">The <see cref="EventLogId"/> representing the event log to commit to.</param>
        /// <param name="sequenceNumber">The unique <see cref="EventLogSequenceNumber">sequence number</see> within the event log.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> to commit for.</param>
        /// <param name="eventType">The <see cref="EventType">type of event</see> to commit.</param>
        /// <param name="content">The JSON payload of the event.</param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task Commit(EventLogId eventLogId, EventLogSequenceNumber sequenceNumber, EventSourceId eventSourceId, EventType eventType, string content);

        /// <summary>
        /// Find <see cref="CommittedEvent">committed events</see> from the <see cref="IEventLog"/>.
        /// </summary>
        /// <param name="eventLogId">The <see cref="EventLogId"/> representing the event log to find from.</param>
        /// <param name="eventSourceId"><see cref="EventSourceId"/> to find for.</param>
        /// <returns>Awaitable <see cref="Task"/> with <see cref="IEventStoreFindResult"/>.</returns>
        Task<IEventStoreFindResult> FindFor(EventLogId eventLogId, EventSourceId eventSourceId);
    }
}
