// Module name: VRChatConnector
// File name: OscDatagramBuilder.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;
using VRChatConnector.DataStructures;

namespace VRChatConnector;

/// <summary>
///     Builds datagrams sent through VRChat
/// </summary>
public readonly struct OscDatagramBuilder
{
    private readonly byte[] _buffer;

    public OscDatagramBuilder(int size)
    {
        _buffer = new byte[size];
    }

    public OscDatagramBuilder(byte[] buffer)
    {
        _buffer = buffer;
    }

    public ReadOnlySpan<byte> Create(string address, float value)
    {
        var length = WriteAddress(address);
        length = WriteTags("f", length);
        length = WriteFloat(value, length);
        return _buffer.AsSpan(0, length);
    }

    public ReadOnlySpan<byte> Create(string address, VrChatVector2 value)
    {
        var length = WriteAddress(address);
        length = WriteTags("ff", length);
        length = WriteFloat(value.X, length);
        length = WriteFloat(value.Y, length);
        return _buffer.AsSpan(0, length);
    }

    public ReadOnlySpan<byte> Create(string address, VrChatVector3 value)
    {
        var length = WriteAddress(address);
        length = WriteTags("fff", length);
        length = WriteFloat(value.X, length);
        length = WriteFloat(value.Y, length);
        length = WriteFloat(value.Z, length);
        return _buffer.AsSpan(0, length);
    }

    public ReadOnlySpan<byte> Create(string address, VrChatVector4 value)
    {
        var length = WriteAddress(address);
        length = WriteTags("ffff", length);
        length = WriteFloat(value.X, length);
        length = WriteFloat(value.Y, length);
        length = WriteFloat(value.Z, length);
        length = WriteFloat(value.W, length);
        return _buffer.AsSpan(0, length);
    }

    private int WriteAddress(string address)
    {
        return WriteString(address, 0);
    }

    private int WriteTags(string tags, int writePosition)
    {
        if (writePosition < _buffer.Length)
            _buffer[writePosition] = (byte) ',';
        return WriteString(tags, writePosition + 1);
    }

    private int WriteString(string stringToWrite, int writePosition)
    {
        var length = writePosition + stringToWrite.Length + 1; // osc string are null terminated
        var alignedLength = CalculateNewAlignedLength(length);
        CheckNewLength(alignedLength);
        foreach (var character in stringToWrite)
            _buffer[writePosition++] = (byte) character;
        _buffer[writePosition++] = (byte) '\0';
        while (writePosition < alignedLength)
            _buffer[writePosition++] = 0;
        return writePosition;
    }

    private int WriteFloat(float data, int writePosition)
    {
        CheckNewLength(writePosition + sizeof(float));
        Span<float> swapBuffer = stackalloc float[1];
        swapBuffer[0] = data;
        var bytesSpan = MemoryMarshal.AsBytes(swapBuffer);
        _buffer[writePosition++] = bytesSpan[3];
        _buffer[writePosition++] = bytesSpan[2];
        _buffer[writePosition++] = bytesSpan[1];
        _buffer[writePosition++] = bytesSpan[0];
        return writePosition;
    }

    private void CheckNewLength(int newLength)
    {
        if (newLength > _buffer.Length) throw new InvalidOperationException("Buffer is to short for current operation");
    }

    /// <summary>
    ///     Aligns length to size of 4 bytes.
    /// </summary>
    /// <param name="unalignedLength"></param>
    /// <returns></returns>
    private int CalculateNewAlignedLength(int unalignedLength)
    {
        var rest = unalignedLength % 4;
        if (rest != 0) 
            unalignedLength += 4 - rest;
        return unalignedLength;
    }
}