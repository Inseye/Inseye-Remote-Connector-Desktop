// Module name: Tests
// File name: OscDatagramBuilderTest.cs
// Last edit: 2024-10-02 14:41 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using VRChatConnector;
using VRChatConnector.DataStructures;

namespace Tests.OscTests;

public class OscDatagramBuilderTest
{
	[Test]
	public void TestGazeDataMessage()
	{
		var array = new byte[265];
		var vector4 = new VrChatVector4(1f, 2f, 3f, 4f);
		const string address = "/tracking/eye/LeftRightPitchYaw";
		const string format = ",ffff";
		var buffer = new OscDatagramBuilder(array).Create(address, vector4);
		Assert.That(buffer.Length, Is.EqualTo(56));
		var index = 0;
		foreach (var character in address)
			Assert.That(array[index++], Is.EqualTo((byte)character));
		Assert.That(array[index++], Is.EqualTo(0));
		foreach (var character in format)
			Assert.That(array[index++], Is.EqualTo((byte)character));
		Assert.That(array[index++], Is.EqualTo(0));
		Assert.That(array[index++], Is.EqualTo(0));
		Assert.That(array[index++], Is.EqualTo(0));

	}
}