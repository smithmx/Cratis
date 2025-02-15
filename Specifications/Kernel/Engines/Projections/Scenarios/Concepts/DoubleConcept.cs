// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Concepts;

namespace Aksio.Cratis.Kernel.Engines.Projections.Scenarios.Concepts;

public record DoubleConcept(double Value): ConceptAs<double>(Value)
{
    public static implicit operator DoubleConcept(double value) => new(value);
}
