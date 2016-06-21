using Bolt;
using System;
using UdpKit;

internal class CoopSingleAnchorToken : IProtocolToken
{
	public BoltEntity Anchor;

	public int Index;

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Anchor);
		packet.WriteInt(this.Index);
	}

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Anchor = packet.ReadBoltEntity();
		this.Index = packet.ReadInt();
	}
}
