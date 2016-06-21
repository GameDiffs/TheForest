using Bolt;
using System;
using UdpKit;

public class CoopConnectToken : IProtocolToken
{
	public string PlayerName;

	void IProtocolToken.Read(UdpPacket packet)
	{
		this.PlayerName = packet.ReadString();
	}

	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteString(this.PlayerName);
	}
}
