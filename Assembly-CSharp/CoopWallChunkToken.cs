using Bolt;
using System;
using TheForest.Buildings.Creation;
using UdpKit;
using UnityEngine;

internal class CoopWallChunkToken : IProtocolToken
{
	public Vector3[] PointsPositions;

	public BoltEntity Support;

	public BoltEntity Parent;

	public Vector3 P1;

	public Vector3 P2;

	public WallChunkArchitect.Additions Additions;

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Parent);
		packet.WriteVector3(this.P1);
		packet.WriteVector3(this.P2);
		packet.WriteBoltEntity(this.Support);
		packet.WriteInt((int)this.Additions);
		packet.WriteInt((this.PointsPositions == null) ? 0 : this.PointsPositions.Length);
		if (this.PointsPositions != null)
		{
			for (int i = 0; i < this.PointsPositions.Length; i++)
			{
				packet.WriteVector3(this.PointsPositions[i]);
			}
		}
	}

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Parent = packet.ReadBoltEntity();
		this.P1 = packet.ReadVector3();
		this.P2 = packet.ReadVector3();
		this.Support = packet.ReadBoltEntity();
		this.Additions = (WallChunkArchitect.Additions)packet.ReadInt();
		int num = packet.ReadInt();
		if (num > 0)
		{
			this.PointsPositions = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				this.PointsPositions[i] = packet.ReadVector3();
			}
		}
	}
}
