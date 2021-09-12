﻿// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts.given;

namespace Cratis.Concepts.for_ConceptExtensions
{
    public class when_getting_the_value_from_a_long_concept : concepts
    {
        static LongConcept value;
        static long primitive_value = 10;
        static object returned_value;

        void Establish() => value = new LongConcept(primitive_value);

        void Because() => returned_value = value.GetConceptValue();

        [Fact] void should_get_the_value_of_the_primitive() => returned_value.ShouldEqual(primitive_value);
    }
}
