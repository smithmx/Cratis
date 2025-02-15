// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable disable

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Observation;

namespace Aksio.Cratis.Kernel.Grains.Observation;

/// <summary>
/// Represents the state used for an observer.
/// </summary>
public class ObserverState
{
    /// <summary>
    /// The name of the storage provider used for working with this type of state.
    /// </summary>
    public const string StorageProvider = "observer-state";

    /// <summary>
    /// Gets or sets the identifier of the observer state.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event types the observer is observing.
    /// </summary>
    public IEnumerable<EventType> EventTypes { get; set; } = Array.Empty<EventType>();

    /// <summary>
    /// Gets or sets the <see cref="EventSequenceId"/> the state is for.
    /// </summary>
    public EventSequenceId EventSequenceId { get; set; } = EventSequenceId.Unspecified;

    /// <summary>
    /// Gets or sets the <see cref="EventSequenceId"/> the state is for.
    /// </summary>
    public ObserverId ObserverId { get; set; } = ObserverId.Unspecified;

    /// <summary>
    /// Gets or sets a friendly name for the observer.
    /// </summary>
    public ObserverName Name { get; set; } = ObserverName.NotSpecified;

    /// <summary>
    /// Gets or sets the <see cref="ObserverType"/>.
    /// </summary>
    public ObserverType Type { get; set; } = ObserverType.Unknown;

    /// <summary>
    /// Gets or sets the expected next event sequence number into the event log.
    /// </summary>
    public EventSequenceNumber NextEventSequenceNumber { get; set; } = EventSequenceNumber.First;

    /// <summary>
    /// Gets or sets the last handled event in the event log, ever. This value will never reset during a rewind.
    /// </summary>
    public EventSequenceNumber LastHandled { get; set; } = EventSequenceNumber.First;

    /// <summary>
    /// Gets or sets the running state.
    /// </summary>
    public ObserverRunningState RunningState { get; set; } = ObserverRunningState.New;

    /// <summary>
    /// Gets or sets the failed partitions for the observer.
    /// </summary>
    public IEnumerable<FailedObserverPartition> FailedPartitions
    {
        get => _failedPartitions;
        set => _failedPartitions = new(value);
    }

    /// <summary>
    /// Gets or sets the failed partitions for the observer.
    /// </summary>
    public IEnumerable<RecoveringFailedObserverPartition> RecoveringPartitions
    {
        get => _partitionsBeingRecovered;
        set => _partitionsBeingRecovered = new(value);
    }

    /// <summary>
    /// Gets whether or not there are any failed partitions.
    /// </summary>
    public bool HasFailedPartitions => _failedPartitions.Count > 0;

    /// <summary>
    /// Gets whether or not there are any partitions being recovered.
    /// </summary>
    public bool IsRecoveringAnyPartition => _partitionsBeingRecovered.Count > 0;

    /// <summary>
    /// Gets whether or not the observer is in disconnected state. Meaning that there is no subscriber to it.
    /// </summary>
    public bool IsDisconnected => RunningState == ObserverRunningState.Disconnected;

    List<FailedObserverPartition> _failedPartitions = new();
    List<RecoveringFailedObserverPartition> _partitionsBeingRecovered = new();

    /// <summary>
    /// Check whether or not a partition is being recovered.
    /// </summary>
    /// <param name="eventSourceId">Partition to check.</param>
    /// <returns>True if being recovered, false if not.</returns>
    public bool IsRecoveringPartition(EventSourceId eventSourceId) => _partitionsBeingRecovered.Any(_ => _.EventSourceId == eventSourceId);

    /// <summary>
    /// Fail a partition. If the partition is already failed, it will update it with the details.
    /// </summary>
    /// <param name="eventSourceId">The partition to fail.</param>
    /// <param name="sequenceNumber">Sequence number it failed at.</param>
    /// <param name="messages">Messages describing the failure.</param>
    /// <param name="stackTrace">Stack trace, if any.</param>
    public void FailPartition(EventSourceId eventSourceId, EventSequenceNumber sequenceNumber, string[] messages, string stackTrace)
    {
        var partition = _failedPartitions.Find(_ => _.EventSourceId == eventSourceId);
        if (partition is null)
        {
            partition = new();
            _failedPartitions.Add(partition);
        }
        partition.EventSourceId = eventSourceId;
        partition.SequenceNumber = sequenceNumber;
        partition.Messages = messages;
        partition.StackTrace = stackTrace;
        AddAttemptToFailedPartition(eventSourceId);
    }

    /// <summary>
    /// Start the recovery of a failed partition.
    /// </summary>
    /// <param name="eventSourceId">Partition to start recovery on.</param>
    /// <remarks>If the partition is not failed, it will not register it for recovering.</remarks>
    public void StartRecoveringPartition(EventSourceId eventSourceId)
    {
        if (!IsPartitionFailed(eventSourceId) || IsRecoveringPartition(eventSourceId))
        {
            return;
        }

        var failedPartition = GetFailedPartition(eventSourceId);
        _partitionsBeingRecovered.Add(new()
        {
            EventSourceId = eventSourceId,
            SequenceNumber = failedPartition.SequenceNumber,
            StartedRecoveryAt = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Recover a failed partition.
    /// </summary>
    /// <param name="eventSourceId">Partition to recover.</param>
    public void PartitionRecovered(EventSourceId eventSourceId)
    {
        var partition = _failedPartitions.Find(_ => _.EventSourceId == eventSourceId);
        if (partition is not null)
        {
            _failedPartitions.Remove(partition);
        }
        var recoveringPartition = _partitionsBeingRecovered.Find(_ => _.EventSourceId == eventSourceId);
        if (recoveringPartition is not null)
        {
            _partitionsBeingRecovered.Remove(recoveringPartition);
        }
    }

    /// <summary>
    /// Get the recovery information for a partition being recovered.
    /// </summary>
    /// <param name="eventSourceId">Partition to get for.</param>
    /// <returns>Recovery information.</returns>
    public RecoveringFailedObserverPartition GetPartitionRecovery(EventSourceId eventSourceId) => _partitionsBeingRecovered.Find(_ => _.EventSourceId == eventSourceId);

    /// <summary>
    /// Check whether or not a partition is failed.
    /// </summary>
    /// <param name="eventSourceId">Partition to check.</param>
    /// <returns>True if failed, false if not.</returns>
    public bool IsPartitionFailed(EventSourceId eventSourceId) => _failedPartitions.Any(_ => _.EventSourceId == eventSourceId);

    /// <summary>
    /// Gets a failed partition by its partition identifier.
    /// </summary>
    /// <param name="eventSourceId">Partition to get.</param>
    /// <returns>The failed partition.</returns>
    public FailedObserverPartition GetFailedPartition(EventSourceId eventSourceId) => _failedPartitions.Find(_ => _.EventSourceId == eventSourceId);

    /// <summary>
    /// Add an attempt to a failed partition.
    /// </summary>
    /// <param name="eventSourceId">Partition to add to.</param>
    public void AddAttemptToFailedPartition(EventSourceId eventSourceId)
    {
        var partition = _failedPartitions.Find(_ => _.EventSourceId == eventSourceId);
        if (partition is not null)
        {
            partition.Attempts++;
            partition.LastAttempt = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    /// Clear any failed partitions.
    /// </summary>
    public void ClearFailedPartitions()
    {
        _failedPartitions.Clear();
    }

    /// <summary>
    /// Clear any recovering partitions.
    /// </summary>
    public void ClearRecoveringPartitions()
    {
        _partitionsBeingRecovered.Clear();
    }
}
