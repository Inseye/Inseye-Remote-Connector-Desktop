// Module name: Shared
// File name: StringExtensions.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

namespace EyeTrackerStreaming.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Concatenates first and second sentence.
    /// </summary>
    /// <param name="firstSentence">First sentence to concatenate.</param>
    /// <param name="secondSentence">Second sentence to concatenate.</param>
    /// <param name="separator">Separator used between sentences</param>
    /// <returns>
    ///     First sentence concatenated with second sentence with separator in between. If second sentence is null then
    ///     first sentence is returned.
    /// </returns>
    public static string ConcatStrings(this string firstSentence, string? secondSentence, string separator = " ")
    {
        ArgumentException.ThrowIfNullOrEmpty(firstSentence, nameof(firstSentence));
        ArgumentNullException.ThrowIfNull(separator);
        if (string.IsNullOrEmpty(secondSentence))
            return firstSentence;
        using var handle = StringBuilderPool.Shared.GetAutoDisposing();
        var sb = handle.Object;
        return sb.Append(firstSentence).Append(separator).Append(secondSentence).ToString();
    }
}