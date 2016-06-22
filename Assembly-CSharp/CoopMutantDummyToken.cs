using Bolt;
using System;
using UdpKit;
using UnityEngine;

internal class CoopMutantDummyToken : IProtocolToken
{
	public bool Skinny;

	public int Props;

	public Vector3 HipPosition;

	public Vector3 Scale;

	public Quaternion HipRotation;

	public int MaterialIndex = -1;

	public BoltEntity OriginalMutant;

	public float skinDamage1;

	public float skinDamage2;

	public float skinDamage3;

	public float skinDamage4;

	public int storedRagDollName;

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Scale = packet.ReadVector3();
		this.HipPosition = packet.ReadVector3();
		this.HipRotation = packet.ReadQuaternion();
		this.MaterialIndex = packet.ReadInt();
		this.skinDamage1 = packet.ReadFloat();
		this.skinDamage2 = packet.ReadFloat();
		this.skinDamage3 = packet.ReadFloat();
		this.skinDamage4 = packet.ReadFloat();
		this.storedRagDollName = packet.ReadInt();
		if (packet.ReadBool())
		{
			this.OriginalMutant = packet.ReadBoltEntity();
		}
		this.Skinny = packet.ReadBool();
		this.Props = packet.ReadInt();
	}

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteVector3(this.Scale);
		packet.WriteVector3(this.HipPosition);
		packet.WriteQuaternion(this.HipRotation);
		packet.WriteInt(this.MaterialIndex);
		packet.WriteFloat(this.skinDamage1);
		packet.WriteFloat(this.skinDamage2);
		packet.WriteFloat(this.skinDamage3);
		packet.WriteFloat(this.skinDamage4);
		packet.WriteInt(this.storedRagDollName);
		if (packet.WriteBool(this.OriginalMutant && this.OriginalMutant.IsAttached()))
		{
			packet.WriteBoltEntity(this.OriginalMutant);
		}
		packet.WriteBool(this.Skinny);
		packet.WriteInt(this.Props);
	}
}
