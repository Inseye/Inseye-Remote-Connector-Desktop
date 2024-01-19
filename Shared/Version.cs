// Module name: Shared
// File name: Version.cs
// Last edit: 2024-2-20 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared
{
    /// <summary>
    ///     Represents version of components in Inseye SDK.
    /// </summary>
    // ReSharper disable once RedundantNameQualifier
    public readonly struct Version : System.IEquatable<Version>
    {
        /// <summary>
        ///     Version major component. Changes in major version are braking from API point of view.
        /// </summary>
        public readonly int Major;

        /// <summary>
        ///     Version minor component. Changes in minor version introduce new features in non breaking way.
        /// </summary>
        public readonly int Minor;

        /// <summary>
        ///     Version patch component. Changes in patch version introduce bug fixes and minor improvements.
        /// </summary>
        public readonly int Patch;

        /// <summary>
        ///     Optional string identifier.
        /// </summary>
        public readonly string Extra;

        /// <summary>
        ///     Creates new version.
        /// </summary>
        /// <param name="major">Version major value. Must not be less than zero.</param>
        /// <param name="minor">Version minor value. Must not be less than zero.</param>
        /// <param name="patch">Version patch value. Must not be less than zero.</param>
        /// <param name="extra">Extra information, any string.</param>
        /// <exception cref="System.ArgumentException">Thrown when any value is breaking constrain.</exception>
        public Version(int major, int minor, int patch, string extra)
        {
            if (major < 0)
                throw new ArgumentException("Major version cannot be less than zero");
            if (minor < 0)
                throw new ArgumentException("Minor version cannot be less than zero");
            if (patch < 0)
                throw new ArgumentException("Patch version cannot be less than zero");
            Major = major;
            Minor = minor;
            Patch = patch;
            Extra = extra ?? "";
        }

        /// <summary>
        ///     Creates new version.
        /// </summary>
        /// <param name="major">Version major value.</param>
        /// <param name="minor">Version minor value.</param>
        /// <param name="patch">Version patch value.</param>
        /// <exception cref="System.ArgumentException">Thrown when any numeric value of the version is less than zero</exception>
        public Version(int major, int minor, int patch) : this(major, minor, patch, string.Empty)
        {
        }

        /// <summary>
        ///     Parses string in format {major}.{minor}.{patch} or {major}.{minor}.{patch}-{extra}.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Component version.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input string is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when input string is empty.</exception>
        /// <exception cref="System.FormatException">Thrown when input string is in invalid format.</exception>
        public static Version Parse(string str)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            if (str.Length == 0)
                throw new ArgumentException("String is empty");
            return Parse(str.AsSpan());
        }

        /// <summary>
        ///     Parses span of characters in format {major}.{minor}.{patch} or {major}.{minor}.{patch}-{extra}.
        /// </summary>
        /// <param name="charSpan">Span of characters to parse.</param>
        /// <returns>Component version.</returns>
        /// <exception cref="System.ArgumentException">Thrown when input span is empty.</exception>
        /// <exception cref="System.FormatException">Thrown when input span is in invalid format</exception>
        // ReSharper disable once RedundantNameQualifier
        public static Version Parse(System.ReadOnlySpan<char> charSpan)
        {
            if (charSpan.IsEmpty)
                throw new ArgumentException("Span is empty");
            var extra = "";
            var substringStart = 0;
            var index = 0;
            // major
            while (index < charSpan.Length && charSpan[index] != '.') index++;
            if (!int.TryParse(charSpan[..index], out var major))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(Version)} at major version step.");

            if (index == charSpan.Length)
                return new Version(1, 0, 0, string.Empty);
            substringStart = ++index;
            // minor
            while (index < charSpan.Length && charSpan[index] != '.') index++;
            if (!int.TryParse(charSpan.Slice(substringStart, index - substringStart), out var minor))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(Version)} at minor version step.");
            if (index == charSpan.Length)
                return new Version(major, minor, 0, string.Empty);
            substringStart = ++index;
            // patch
            while (index < charSpan.Length && charSpan[index] != '-') index++;
            if (!int.TryParse(charSpan.Slice(substringStart, index - substringStart), out var patch))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(Version)} at patch version");
            if (index == charSpan.Length)
                return new Version(major, minor, patch, string.Empty);
            // extra
            if (charSpan[index] == '-')
            {
                substringStart = ++index;
                extra = charSpan.Slice(substringStart, charSpan.Length - substringStart).ToString();
            }

            return new Version(major, minor, patch, extra);
        }

        /// <summary>
        ///     Overload returning well formatted version representation that is parsable.
        /// </summary>
        /// <returns>String in format {major}.{minor}.{patch}[-{extra}]</returns>
        public override string ToString()
        {
            var str = $"{Major}.{Minor}.{Patch}";
            if (!string.IsNullOrEmpty(Extra))
                str += $"-{Extra}";
            return str;
        }

        /// <summary>
        ///     Checks version equality.
        /// </summary>
        /// <param name="other">other component version</param>
        /// <returns>True if versions are equal.</returns>
        public bool Equals(Version other)
        {
            return this == other;
        }

        /// <summary>
        ///     Casts object to <see cref="Version" /> and then compares it.
        /// </summary>
        /// <param name="obj">Any object.</param>
        /// <returns>True if argument is and <see cref="Version" /> and it's values are equal to this.</returns>
        public override bool Equals(object obj)
        {
            return obj is Version other && Equals(other);
        }

        /// <summary>
        ///     Computes version hash value.
        /// </summary>
        /// <returns>Combined hash value of Major, Minor, Patch and Extra fields.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Extra);
        }

        /// <summary>
        ///     Lesser than operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first argument version is lesser than second.</returns>
        public static bool operator <(Version first, Version second)
        {
            if (first.Major < second.Major)
                return true;
            if (first.Major > second.Major)
                return false;
            // major are equal
            if (first.Minor < second.Minor)
                return true;
            if (first.Minor > second.Minor)
                return false;
            // minor are equal
            return first.Patch < second.Patch;
        }

        /// <summary>
        ///     Greater than operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first argument version is greater than second.</returns>
        public static bool operator >(Version first, Version second)
        {
            if (first.Major > second.Major)
                return true;
            if (first.Major < second.Major)
                return false;
            // major are equal
            if (first.Minor > second.Minor)
                return true;
            if (first.Minor < second.Minor)
                return false;
            // minor are equal
            return first.Patch > second.Patch;
        }

        /// <summary>
        ///     Equality operator.
        ///     Extra are not compared in this operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first and second argument are equal.</returns>
        public static bool operator ==(Version first, Version second)
        {
            return first.Major == second.Major && first.Minor == second.Minor && first.Patch == second.Patch;
        }

        /// <summary>
        ///     Equality operator.
        ///     Extra are not compared in this operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first and second argument are not equal.</returns>
        public static bool operator !=(Version first, Version second)
        {
            return !(first == second);
        }
    }
}