// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Changes.for_CollectionExtensions;

public class when_finding_known_object_by_key : Specification
{
    record Item(string Key, string Value);
    IEnumerable<Item> items;
    Item result;

    void Establish() => items = new[]
    {
        new Item("First", "First Value"),
        new Item("Second", "Second Value")
    };

    void Because() => result = items.FindByKey(nameof(Item.Key), "Second");

    [Fact] void should_return_correct_item() => result.ShouldEqual(items.ToArray()[1]);
}


