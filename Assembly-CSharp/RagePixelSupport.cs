using Serialization;
using System;

[SerializerPlugIn]
public class RagePixelSupport
{
	static RagePixelSupport()
	{
		new SerializePrivateFieldOfType("RagePixelSprite", "animationPingPongDirection");
		new SerializePrivateFieldOfType("RagePixelSprite", "myTime");
	}
}
