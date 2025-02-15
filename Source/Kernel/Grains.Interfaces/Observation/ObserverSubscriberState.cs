// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Grains.Observation;

/// <summary>
/// Represents the state of an observer subscriber.
/// </summary>
public enum ObserverSubscriberState
{
    /// <summary>
    /// Unknown state.
    /// </summary>
    None = 0,

    /// <summary>
    /// The observer subscriber is ok.
    /// </summary>
    Ok = 1,

    /// <summary>
    /// The observer subscriber is errored.
    /// </summary>
    Error = 2,

    /// <summary>
    /// The observer is disconnected.
    /// </summary>
    Disconnected = 3
}
