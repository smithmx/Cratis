// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@aksio/cratis-fundamentals';
import { ICommandResult } from './ICommandResult';
import { ValidationError } from '../validation/ValidationError';
import { Constructor, JsonSerializer } from '@aksio/cratis-fundamentals';

/**
 * Represents the result from executing a {@link ICommand}.
 */
export class CommandResult<TResponse = {}> implements ICommandResult<TResponse> {

    static empty: CommandResult = new CommandResult({
        correlationId: Guid.empty.toString(),
        isSuccess: true,
        isAuthorized: true,
        isValid: true,
        hasExceptions: false,
        validationErrors: [],
        exceptionMessages: [],
        exceptionStackTrace: '',
        response: null
    }, Object, false);

    /** @inheritdoc */
    readonly correlationId: Guid;

    /** @inheritdoc */
    readonly isSuccess: boolean;

    /** @inheritdoc */
    readonly isAuthorized: boolean;

    /** @inheritdoc */
    readonly isValid: boolean;

    /** @inheritdoc */
    readonly hasExceptions: boolean;

    /** @inheritdoc */
    readonly validationErrors: ValidationError[];

    /** @inheritdoc */
    readonly exceptionMessages: string[];

    /** @inheritdoc */
    readonly exceptionStackTrace: string;

    /** @inheritdoc */
    readonly response?: TResponse;

    /**
     * Creates an instance of command result.
     * @param {*} result The JSON/any representation of the command result;
     * @param {Constructor} responseInstanceType The {@see Constructor} that represents the type of response, if any. Defaults to {@see Object}.
     * @param {boolean} isResponseTypeEnumerable Whether or not the response type is an enumerable or not.
     */
    constructor(result: any, responseInstanceType: Constructor = Object, isResponseTypeEnumerable: boolean) {
        this.correlationId = Guid.parse(result.correlationId);
        this.isSuccess = result.isSuccess;
        this.isAuthorized = result.isAuthorized;
        this.isValid = result.isValid;
        this.hasExceptions = result.hasExceptions;
        this.validationErrors = result.validationErrors.map(_ => new ValidationError(_.message, _.memberNames));
        this.exceptionMessages = result.exceptionMessages;
        this.exceptionStackTrace = result.exceptionStackTrace;

        if (result.response) {
            if (isResponseTypeEnumerable) {
                this.response = JsonSerializer.deserializeArrayFromInstance(responseInstanceType, result.response) as any;
            } else {
                this.response = JsonSerializer.deserializeFromInstance(responseInstanceType, result.response) as any;
            }
        }
    }
}
