using Bolt;
using System;
using TheForest.Buildings.Creation;
using UdpKit;

internal class CoopFloorToken : IProtocolToken
{
	public BoltEntity Parent;

	public Hole[] Holes;

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Parent);
		if (packet.WriteBool(this.Holes != null))
		{
			packet.WriteInt(this.Holes.Length);
			for (int i = 0; i < this.Holes.Length; i++)
			{
				packet.WriteVector3(this.Holes[i]._position);
				packet.WriteVector2(this.Holes[i]._size);
			}
		}
	}

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Parent = packet.ReadBoltEntity();
		if (packet.ReadBool())
		{
			this.Holes = new Hole[packet.ReadInt()];
			for (int i = 0; i < this.Holes.Length; i++)
			{
				this.Holes[i]._position = packet.ReadVector3();
				this.Holes[i]._size = packet.ReadVector2();
			}
		}
	}
}
