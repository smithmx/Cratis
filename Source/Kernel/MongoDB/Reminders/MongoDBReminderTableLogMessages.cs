// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.MongoDB.Reminders;

/// <summary>
/// Holds log messages for <see cref="MongoDBReminderTable"/>.
/// </summary>
internal static partial class MongoDBReminderTableLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "Upserting reminder with key '{Key}'")]
    internal static partial void Upserting(this ILogger<MongoDBReminderTable> logger, string key);

    [LoggerMessage(1, LogLevel.Trace, "Removing reminder with key '{Key}'")]
    internal static partial void Removing(this ILogger<MongoDBReminderTable> logger, string key);

    [LoggerMessage(2, LogLevel.Critical, "Failed upserting reminder with key '{Key}'")]
    internal static partial void FailedUpserting(this ILogger<MongoDBReminderTable> logger, string key, Exception exception);

    [LoggerMessage(3, LogLevel.Critical, "Failed removing reminder with key '{Key}'")]
    internal static partial void FailedRemoving(this ILogger<MongoDBReminderTable> logger, string key, Exception exception);

    [LoggerMessage(4, LogLevel.Trace, "Reading all reminders for '{Key}'")]
    internal static partial void ReadingAllRemindersForGrain(this ILogger<MongoDBReminderTable> logger, string key);

    [LoggerMessage(5, LogLevel.Trace, "Reading reminder '{ReminderName}' for '{Grain}'")]
    internal static partial void ReadingSpecificReminderForGrain(this ILogger<MongoDBReminderTable> logger, string grain, string reminderName);

    [LoggerMessage(6, LogLevel.Trace, "Reading reminders in range '{Begin}' for '{End}'")]
    internal static partial void ReadingRemindersInRange(this ILogger<MongoDBReminderTable> logger, uint begin, uint end);
}
