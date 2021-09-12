// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Events.Store
{
    /// <summary>
    /// Represents the context in which an event exists - typically what it was committed with.
    /// </summary>
    /// <param name="EventSourceId">The <see cref="EventSourceId"/>.</param>
    /// <param name="Occurred"><see cref="DateTimeOffset">When</see> it occurred.</param>
    public record EventContext(EventSourceId EventSourceId, DateTimeOffset Occurred);
}
