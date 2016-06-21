using System;
using System.Net.NetworkInformation;
using UdpKit;

public class DotNetInterface : UdpPlatformInterface
{
	private string name;

	private UdpLinkType linkType;

	private byte[] physicalAddress;

	private UdpIPv4Address[] gatewayAddresses;

	private UdpIPv4Address[] unicastAddresses;

	private UdpIPv4Address[] multicastAddresses;

	public override string Name
	{
		get
		{
			return this.name;
		}
	}

	public override UdpLinkType LinkType
	{
		get
		{
			return this.linkType;
		}
	}

	public override byte[] PhysicalAddress
	{
		get
		{
			return this.physicalAddress;
		}
	}

	public override UdpIPv4Address[] GatewayAddresses
	{
		get
		{
			return this.gatewayAddresses;
		}
	}

	public override UdpIPv4Address[] UnicastAddresses
	{
		get
		{
			return this.unicastAddresses;
		}
	}

	public override UdpIPv4Address[] MulticastAddresses
	{
		get
		{
			return this.multicastAddresses;
		}
	}

	public DotNetInterface(NetworkInterface n, UdpIPv4Address[] gw, UdpIPv4Address[] uni, UdpIPv4Address[] multi)
	{
		this.name = DotNetInterface.ParseName(n);
		this.linkType = DotNetInterface.ParseLinkType(n);
		this.physicalAddress = DotNetInterface.ParsePhysicalAddress(n);
		this.gatewayAddresses = gw;
		this.unicastAddresses = uni;
		this.multicastAddresses = multi;
	}

	private static string ParseName(NetworkInterface n)
	{
		string result;
		try
		{
			result = n.Description;
		}
		catch
		{
			result = "UNKNOWN";
		}
		return result;
	}

	private static byte[] ParsePhysicalAddress(NetworkInterface n)
	{
		byte[] result;
		try
		{
			result = n.GetPhysicalAddress().GetAddressBytes();
		}
		catch
		{
			result = new byte[0];
		}
		return result;
	}

	private static UdpLinkType ParseLinkType(NetworkInterface n)
	{
		NetworkInterfaceType networkInterfaceType = n.NetworkInterfaceType;
		switch (networkInterfaceType)
		{
		case NetworkInterfaceType.FastEthernetFx:
			return UdpLinkType.Ethernet;
		case (NetworkInterfaceType)70:
			IL_1C:
			if (networkInterfaceType != NetworkInterfaceType.Ethernet && networkInterfaceType != NetworkInterfaceType.Ethernet3Megabit && networkInterfaceType != NetworkInterfaceType.FastEthernetT && networkInterfaceType != NetworkInterfaceType.GigabitEthernet)
			{
				return UdpLinkType.Unknown;
			}
			return UdpLinkType.Ethernet;
		case NetworkInterfaceType.Wireless80211:
			return UdpLinkType.Wifi;
		}
		goto IL_1C;
	}
}
