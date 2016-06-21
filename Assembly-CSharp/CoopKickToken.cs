using Bolt;
using System;
using UdpKit;

internal class CoopKickToken : IProtocolToken
{
	public bool Banned;

	public string KickMessage;

	public void Write(UdpPacket packet)
	{
		packet.WriteString(this.KickMessage);
	}

	public void Read(UdpPacket packet)
	{
		this.KickMessage = packet.ReadString();
	}
}
