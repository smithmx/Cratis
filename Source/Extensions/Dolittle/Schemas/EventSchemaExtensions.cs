// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Schema;

namespace Cratis.Extensions.Dolittle.Schemas
{
    /// <summary>
    /// Extension methods for working with <see cref="EventSchema"/>.
    /// </summary>
    public static class EventSchemaExtensions
    {
        /// <summary>
        /// Convert to a <see cref="EventSchemaMongoDB">MongoDB</see> representation.
        /// </summary>
        /// <param name="schema"><see cref="EventSchema"/> to convert.</param>
        /// <returns>Converted <see cref="EventSchemaMongoDB"/>.</returns>
        public static EventSchemaMongoDB ToMongoDB(this EventSchema schema)
        {
            return new EventSchemaMongoDB
            {
                EventType = schema.EventType.Id,
                Generation = schema.EventType.Generation,
                Schema = schema.Schema.ToString()
            };
        }

        /// <summary>
        /// Convert to <see cref="EventSchema"/> from <see cref="EventSchemaMongoDB"/>.
        /// </summary>
        /// <param name="schema"><see cref="EventSchemaMongoDB"/> to convert from.</param>
        /// <returns>Converted <see cref="EventSchema"/>.</returns>
        public static EventSchema ToEventSchema(this EventSchemaMongoDB schema)
        {
            return new EventSchema(
               new global::Dolittle.SDK.Events.EventType(
                   schema.EventType,
                   schema.Generation),
               JSchema.Parse(schema.Schema));
        }
    }
}
