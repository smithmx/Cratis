// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Projections.Expressions;

namespace Aksio.Cratis.Projections;

/// <summary>
/// Represents a default implementation of <see cref="IKeyBuilder"/> that works with <see cref="IEventValueExpression"/>.
/// </summary>
public class KeyBuilder : IKeyBuilder
{
    readonly IEventValueExpression _expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyBuilder"/> class.
    /// </summary>
    /// <param name="expression"><see cref="IEventValueExpression"/> to use.</param>
    public KeyBuilder(IEventValueExpression expression) => _expression = expression;

    /// <inheritdoc/>
    public string Build() => _expression.Build();
}
