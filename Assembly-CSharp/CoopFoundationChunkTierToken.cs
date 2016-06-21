using Bolt;
using System;
using UdpKit;

internal class CoopFoundationChunkTierToken : IProtocolToken
{
	public bool Applied;

	public int ChunkIndex;

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteInt(this.ChunkIndex);
	}

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.ChunkIndex = packet.ReadInt();
	}
}
