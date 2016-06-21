using Bolt;
using System;
using UdpKit;

internal class CoopBridgeToken : IProtocolToken
{
	public struct Anchor
	{
		public BoltEntity Entity;

		public int Index;
	}

	public BoltEntity Parent;

	public CoopBridgeToken.Anchor[] Anchors = new CoopBridgeToken.Anchor[2];

	public void Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Parent);
		for (int i = 0; i < this.Anchors.Length; i++)
		{
			packet.WriteBoltEntity(this.Anchors[i].Entity);
			packet.WriteInt(this.Anchors[i].Index);
		}
	}

	public void Read(UdpPacket packet)
	{
		this.Parent = packet.ReadBoltEntity();
		for (int i = 0; i < this.Anchors.Length; i++)
		{
			this.Anchors[i].Entity = packet.ReadBoltEntity();
			this.Anchors[i].Index = packet.ReadInt();
		}
	}
}
