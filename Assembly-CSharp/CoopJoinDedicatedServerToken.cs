using Bolt;
using System;
using UdpKit;

internal class CoopJoinDedicatedServerToken : IProtocolToken
{
	public string AdminPassword;

	public string ServerPassword;

	public void Write(UdpPacket packet)
	{
		packet.WriteString(this.AdminPassword);
		packet.WriteString(this.ServerPassword);
	}

	public void Read(UdpPacket packet)
	{
		this.AdminPassword = packet.ReadString();
		this.ServerPassword = packet.ReadString();
	}
}
