// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Applications.Validation;

namespace Aksio.Cratis.Applications.Commands;

/// <summary>
/// Represents the base type for a validator of commands.
/// </summary>
/// <typeparam name="T">Type of command.</typeparam>
public class CommandValidator<T> : DiscoverableValidator<T>
{
}
