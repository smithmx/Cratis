// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Aksio.Cratis.Execution;

namespace Aksio.Cratis.Kernel.Grains.Observation.for_Observer.when_rewinding;

public class and_has_three_events_in_sequence : given.an_observer_and_two_event_types
{
    EventSourceId event_source_id;
    AppendedEvent first_appended_event;
    AppendedEvent second_appended_event;
    AppendedEvent third_appended_event;

    List<AppendedEvent> appended_events;

    async Task Establish()
    {
        event_sequence_storage_provider.Setup(_ => _.GetHeadSequenceNumber(event_sequence_id, event_types, null)).Returns(Task.FromResult(EventSequenceNumber.First));
        event_sequence_storage_provider.Setup(_ => _.GetTailSequenceNumber(event_sequence_id, event_types, null)).Returns(Task.FromResult((EventSequenceNumber)2));

        await observer.Subscribe<ObserverSubscriber>(event_types);
        state.RunningState = ObserverRunningState.Active;

        state.NextEventSequenceNumber = EventSequenceNumber.First;

        event_source_id = Guid.NewGuid().ToString();

        storage.Invocations.Clear();

        first_appended_event = new AppendedEvent(
            new(EventSequenceNumber.First, event_types.ToArray()[0]),
            new(event_source_id, EventSequenceNumber.First, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, TenantId.Development, CorrelationId.New(), CausationId.System, CausedBy.System),
            new ExpandoObject());

        second_appended_event = new AppendedEvent(
            new(EventSequenceNumber.First + 1, event_types.ToArray()[0]),
            new(event_source_id, EventSequenceNumber.First + 1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, TenantId.Development, CorrelationId.New(), CausationId.System, CausedBy.System),
            new ExpandoObject());

        third_appended_event = new AppendedEvent(
            new(EventSequenceNumber.First + 2, event_types.ToArray()[0]),
            new(event_source_id, EventSequenceNumber.First + 1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, TenantId.Development, CorrelationId.New(), CausationId.System, CausedBy.System),
            new ExpandoObject());

        state.LastHandled = EventSequenceNumber.First + 2;
        appended_events = new();
        subscriber.Setup(_ => _.OnNext(
            IsAny<AppendedEvent>())).Returns(
                (AppendedEvent @event) =>
                {
                    appended_events.Add(@event);
                    return Task.FromResult(ObserverSubscriberResult.Ok);
                });
    }

    async Task Because()
    {
        await observer.Rewind();
        await observers[1].OnNextAsync(first_appended_event);
        await observers[1].OnNextAsync(second_appended_event);
        await observers[1].OnNextAsync(third_appended_event);
    }

    [Fact] void should_set_head_of_replay_as_event_observation_state_for_first_event() => appended_events[0].Context.ObservationState.ShouldEqual(EventObservationState.HeadOfReplay | EventObservationState.Replay);
    [Fact] void should_set_replay_as_event_observation_state_for_second_event() => appended_events[1].Context.ObservationState.ShouldEqual(EventObservationState.Replay);
    [Fact] void should_set_tail_of_replay_as_event_observation_state_for_third_event() => appended_events[2].Context.ObservationState.ShouldEqual(EventObservationState.TailOfReplay | EventObservationState.Replay);
    [Fact] void should_set_state_to_active() => state_on_write.RunningState.ShouldEqual(ObserverRunningState.Active);
}
