// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Aksio.Cratis.Events;
using Aksio.Cratis.Projections;
using Aksio.Cratis.Properties;

namespace Aksio.Cratis.Kernel.Engines.Projections;

/// <summary>
/// Represents the implementation of <see cref="IProjection"/>.
/// </summary>
public class Projection : IProjection
{
    readonly ISubject<ProjectionEventContext> _subject = new Subject<ProjectionEventContext>();
    IDictionary<EventType, KeyResolver> _eventTypesToKeyResolver = new Dictionary<EventType, KeyResolver>();

    /// <inheritdoc/>
    public ProjectionId Identifier { get; }

    /// <inheritdoc/>
    public ExpandoObject InitialModelState { get; }

    /// <inheritdoc/>
    public ProjectionName Name { get; }

    /// <inheritdoc/>
    public ProjectionPath Path { get; }

    /// <inheritdoc/>
    public PropertyPath ChildrenPropertyPath { get; }

    /// <inheritdoc/>
    public Model Model { get; }

    /// <inheritdoc/>
    public bool IsRewindable { get; }

    /// <inheritdoc/>
    public IObservable<ProjectionEventContext> Event { get; }

    /// <inheritdoc/>
    public IEnumerable<EventType> EventTypes { get; private set; } = Array.Empty<EventType>();

    /// <inheritdoc/>
    public IEnumerable<EventType> OwnEventTypes { get; private set; } = Array.Empty<EventType>();

    /// <inheritdoc/>
    public IEnumerable<IProjection> ChildProjections { get; }

    /// <inheritdoc/>
    public bool HasParent => Parent != default;

    /// <inheritdoc/>
    public IProjection? Parent { get; private set; }

    /// <inheritdoc/>
    public IEnumerable<EventTypeWithKeyResolver> EventTypesWithKeyResolver { get; private set; } = Array.Empty<EventTypeWithKeyResolver>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Projection"/> class.
    /// </summary>
    /// <param name="identifier">The unique identifier of the projection.</param>
    /// <param name="initialModelState">The initial state to use for new model instances.</param>
    /// <param name="name">The name of the projection.</param>
    /// <param name="path">The qualified path of the projection.</param>
    /// <param name="childrenPropertyPath">The fully qualified path of the array that holds the children, if this is a child projection.</param>
    /// <param name="model">The target <see cref="Model"/>.</param>
    /// <param name="rewindable">Whether or not the projection is rewindable.</param>
    /// <param name="childProjections">Collection of <see cref="IProjection">child projections</see>, if any.</param>
    public Projection(
        ProjectionId identifier,
        ExpandoObject initialModelState,
        ProjectionName name,
        ProjectionPath path,
        PropertyPath childrenPropertyPath,
        Model model,
        bool rewindable,
        IEnumerable<IProjection> childProjections)
    {
        Identifier = identifier;
        InitialModelState = initialModelState;
        Name = name;
        Model = model;
        IsRewindable = rewindable;
        Event = FilterEventTypes(_subject);
        Path = path;
        ChildrenPropertyPath = childrenPropertyPath;
        ChildProjections = childProjections;
    }

    /// <inheritdoc/>
    public IObservable<ProjectionEventContext> FilterEventTypes(IObservable<ProjectionEventContext> observable) => observable.Where(_ => EventTypes.Any(et => et.Id == _.Event.Metadata.Type.Id));

    /// <inheritdoc/>
    public IObservable<AppendedEvent> FilterEventTypes(IObservable<AppendedEvent> observable) => observable.Where(_ => EventTypes.Any(et => et.Id == _.Metadata.Type.Id));

    /// <inheritdoc/>
    public void OnNext(ProjectionEventContext context)
    {
        _subject.OnNext(context);
    }

    /// <inheritdoc/>
    public bool Accepts(EventType eventType) => _eventTypesToKeyResolver.Keys.Any(_ => _.Id == eventType.Id);

    /// <inheritdoc/>
    public bool HasKeyResolverFor(EventType eventType) => _eventTypesToKeyResolver.ContainsKey(new(eventType.Id, eventType.Generation));

    /// <inheritdoc/>
    public KeyResolver GetKeyResolverFor(EventType eventType)
    {
        // We only care about the actual event type identifier and generation, any other properties should be the default
        eventType = new(eventType.Id, eventType.Generation);
        ThrowIfMissingKeyResolverForEventType(eventType);
        return _eventTypesToKeyResolver[eventType];
    }

    /// <inheritdoc/>
    public void SetEventTypesWithKeyResolvers(IEnumerable<EventTypeWithKeyResolver> eventTypesWithKeyResolver, IEnumerable<EventType> ownEventTypes)
    {
        EventTypesWithKeyResolver = eventTypesWithKeyResolver;
        EventTypes = eventTypesWithKeyResolver.Select(_ => new EventType(_.EventType.Id, _.EventType.Generation)).ToArray();
        _eventTypesToKeyResolver = eventTypesWithKeyResolver.ToDictionary(
            _ => new EventType(_.EventType.Id, _.EventType.Generation),
            _ => _.KeyResolver);

        OwnEventTypes = ownEventTypes;
    }

    /// <inheritdoc/>
    public void SetParent(IProjection projection) => Parent = projection;

    void ThrowIfMissingKeyResolverForEventType(EventType eventType)
    {
        if (!_eventTypesToKeyResolver.ContainsKey(eventType))
        {
            throw new MissingKeyResolverForEventType(eventType);
        }
    }
}
