using Bolt;
using System;
using UdpKit;

internal class CoopJoinDedicatedServerFailed : IProtocolToken
{
	public string Error;

	public void Write(UdpPacket packet)
	{
		packet.WriteString(this.Error);
	}

	public void Read(UdpPacket packet)
	{
		this.Error = packet.ReadString();
	}
}
