// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export type CircularMenuItemClicked = (item: CircularMenuItem) => void;

export interface CircularMenuItem {
    color?: string;
    icon?: string;
    label?: string;
    iconColor?: string;
    onClick?: CircularMenuItemClicked;
}
