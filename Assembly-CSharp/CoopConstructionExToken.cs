using Bolt;
using System;
using UdpKit;
using UnityEngine;

public class CoopConstructionExToken : IProtocolToken
{
	public struct ArchitectData
	{
		public int PointsCount;

		public Vector3[] PointsPositions;

		public IProtocolToken CustomToken;

		public bool AboveGround;

		public BoltEntity Support;
	}

	public BoltEntity Parent;

	public CoopConstructionExToken.ArchitectData[] Architects;

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Parent);
		packet.WriteInt(this.Architects.Length);
		for (int i = 0; i < this.Architects.Length; i++)
		{
			packet.WriteInt(this.Architects[i].PointsCount);
			packet.WriteInt(this.Architects[i].PointsPositions.Length);
			packet.WriteToken(this.Architects[i].CustomToken);
			packet.WriteBool(this.Architects[i].AboveGround);
			packet.WriteBoltEntity(this.Architects[i].Support);
			for (int j = 0; j < this.Architects[i].PointsPositions.Length; j++)
			{
				packet.WriteVector3(this.Architects[i].PointsPositions[j]);
			}
		}
	}

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Parent = packet.ReadBoltEntity();
		this.Architects = new CoopConstructionExToken.ArchitectData[packet.ReadInt()];
		for (int i = 0; i < this.Architects.Length; i++)
		{
			this.Architects[i].PointsCount = packet.ReadInt();
			this.Architects[i].PointsPositions = new Vector3[packet.ReadInt()];
			this.Architects[i].CustomToken = packet.ReadToken();
			this.Architects[i].AboveGround = packet.ReadBool();
			this.Architects[i].Support = packet.ReadBoltEntity();
			for (int j = 0; j < this.Architects[i].PointsPositions.Length; j++)
			{
				this.Architects[i].PointsPositions[j] = packet.ReadVector3();
			}
		}
	}
}
