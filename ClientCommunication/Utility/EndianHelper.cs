// Module name: ClientCommunication
// File name: EndianHelper.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ClientCommunication.Utility;

internal static class EndianHelper
{
    #region Int32

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 ReadBigEndian(this in Int32 bigEndian)
    {
        return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(bigEndian) : bigEndian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBigEndian(this ref Int32 bigEndian, Int32 value)
    {
        bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 ReadLittleEndian(this in Int32 littleEndian)
    {
        return BitConverter.IsLittleEndian ? littleEndian : BinaryPrimitives.ReverseEndianness(littleEndian);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this ref Int32 littleEndian, Int32 value)
    {
        littleEndian = BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    #endregion

    #region UInt32

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 ReadBigEndian(this in UInt32 bigEndian)
    {
        return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(bigEndian) : bigEndian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBigEndian(this ref UInt32 bigEndian, UInt32 value)
    {
        bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 ReadLittleEndian(this in UInt32 littleEndian)
    {
        return BitConverter.IsLittleEndian ? littleEndian : BinaryPrimitives.ReverseEndianness(littleEndian);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this ref UInt32 littleEndian, UInt32 value)
    {
        littleEndian = BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    #endregion

    #region Int64

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 ReadBigEndian(this in Int64 bigEndian)
    {
        return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(bigEndian) : bigEndian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBigEndian(this ref Int64 bigEndian, Int64 value)
    {
        bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 ReadLittleEndian(this in Int64 littleEndian)
    {
        return BitConverter.IsLittleEndian ? littleEndian : BinaryPrimitives.ReverseEndianness(littleEndian);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this ref Int64 littleEndian, Int64 value)
    {
        littleEndian = BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    #endregion

    #region UInt64

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 ReadBigEndian(this in UInt64 bigEndian)
    {
        return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(bigEndian) : bigEndian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBigEndian(this ref UInt64 bigEndian, UInt64 value)
    {
        bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 ReadLittleEndian(this in UInt64 littleEndian)
    {
        return BitConverter.IsLittleEndian ? littleEndian : BinaryPrimitives.ReverseEndianness(littleEndian);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this ref UInt64 littleEndian, UInt64 value)
    {
        littleEndian = BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    #endregion

    #region Single

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single ReadBigEndian(this in Single bigEndian)
    {
        if (!BitConverter.IsLittleEndian) return bigEndian;
        var span = MemoryMarshal.Cast<float, byte>(MemoryMarshal.CreateReadOnlySpan(in bigEndian, 1));
        return BinaryPrimitives.ReadSingleBigEndian(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBigEndian(this ref Single bigEndian, Single value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var span = MemoryMarshal.Cast<float, byte>(MemoryMarshal.CreateSpan(ref bigEndian, 1));
            BinaryPrimitives.WriteSingleBigEndian(span, bigEndian);
            return;
        }

        bigEndian = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single ReadLittleEndian(this in Single littleEndian)
    {
        if (BitConverter.IsLittleEndian)
            return littleEndian;
        var span = MemoryMarshal.Cast<float, byte>(MemoryMarshal.CreateReadOnlySpan(in littleEndian, 1));
        return BinaryPrimitives.ReadSingleLittleEndian(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this ref Single littleEndian, Single value)
    {
        if (BitConverter.IsLittleEndian)
        {
            littleEndian = value;
            return;
        }

        var span = MemoryMarshal.Cast<float, byte>(MemoryMarshal.CreateSpan(ref littleEndian, 1));
        BinaryPrimitives.WriteSingleLittleEndian(span, littleEndian);
    }

    #endregion
}