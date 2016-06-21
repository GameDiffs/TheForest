using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UdpKit;
using UniLinq;

public class DotNetPlatform : UdpPlatform
{
	private class PrecisionTimer
	{
		private static readonly long start = Stopwatch.GetTimestamp();

		private static readonly double freq = 1.0 / (double)Stopwatch.Frequency;

		internal static uint GetCurrentTime()
		{
			long num = Stopwatch.GetTimestamp() - DotNetPlatform.PrecisionTimer.start;
			double num2 = (double)num * DotNetPlatform.PrecisionTimer.freq;
			return (uint)(num2 * 1000.0);
		}
	}

	public override bool SupportsBroadcast
	{
		get
		{
			return true;
		}
	}

	public override bool SupportsMasterServer
	{
		get
		{
			return true;
		}
	}

	public DotNetPlatform()
	{
		DotNetPlatform.PrecisionTimer.GetCurrentTime();
	}

	public override uint GetPrecisionTime()
	{
		return DotNetPlatform.PrecisionTimer.GetCurrentTime();
	}

	public override UdpIPv4Address GetBroadcastAddress()
	{
		return new UdpIPv4Address(DotNetPlatform.FindBroadcastAddress(true).Address);
	}

	public override UdpPlatformSocket CreateSocket()
	{
		return new DotNetSocket(this);
	}

	public override List<UdpPlatformInterface> GetNetworkInterfaces()
	{
		return this.FindInterfaces();
	}

	public override UdpIPv4Address[] ResolveHostAddresses(string host)
	{
		if (host == null)
		{
			throw new ArgumentNullException("host", "argument was null");
		}
		if (host.Length == 0)
		{
			throw new ArgumentException("host name was empty", "host");
		}
		return (from x in Dns.GetHostAddresses(host)
		select DotNetPlatform.ConvertAddress(x)).ToArray<UdpIPv4Address>();
	}

	private List<UdpPlatformInterface> FindInterfaces()
	{
		List<UdpPlatformInterface> list = new List<UdpPlatformInterface>();
		try
		{
			if (NetworkInterface.GetIsNetworkAvailable())
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				for (int i = 0; i < allNetworkInterfaces.Length; i++)
				{
					NetworkInterface networkInterface = allNetworkInterfaces[i];
					try
					{
						if (networkInterface.OperationalStatus == OperationalStatus.Up || networkInterface.OperationalStatus == OperationalStatus.Unknown)
						{
							if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
							{
								DotNetInterface dotNetInterface = this.ParseInterface(networkInterface);
								if (dotNetInterface != null)
								{
									list.Add(dotNetInterface);
								}
							}
						}
					}
					catch (Exception ex)
					{
						UdpLog.Error(ex.Message, new object[0]);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			UdpLog.Error(ex2.Message, new object[0]);
		}
		return list;
	}

	private DotNetInterface ParseInterface(NetworkInterface n)
	{
		HashSet<UdpIPv4Address> hashSet = new HashSet<UdpIPv4Address>(UdpIPv4Address.Comparer.Instance);
		HashSet<UdpIPv4Address> hashSet2 = new HashSet<UdpIPv4Address>(UdpIPv4Address.Comparer.Instance);
		HashSet<UdpIPv4Address> hashSet3 = new HashSet<UdpIPv4Address>(UdpIPv4Address.Comparer.Instance);
		IPInterfaceProperties iPInterfaceProperties = null;
		try
		{
			iPInterfaceProperties = n.GetIPProperties();
		}
		catch
		{
			return null;
		}
		if (iPInterfaceProperties != null)
		{
			try
			{
				foreach (GatewayIPAddressInformation current in iPInterfaceProperties.GatewayAddresses)
				{
					try
					{
						if (current.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							hashSet.Add(DotNetPlatform.ConvertAddress(current.Address));
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (IPAddress current2 in iPInterfaceProperties.DnsAddresses)
				{
					try
					{
						if (current2.AddressFamily == AddressFamily.InterNetwork)
						{
							hashSet.Add(DotNetPlatform.ConvertAddress(current2));
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (UnicastIPAddressInformation current3 in iPInterfaceProperties.UnicastAddresses)
				{
					try
					{
						if (current3.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							UdpIPv4Address item = DotNetPlatform.ConvertAddress(current3.Address);
							hashSet2.Add(item);
							hashSet.Add(new UdpIPv4Address(item.Byte3, item.Byte2, item.Byte1, 1));
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (MulticastIPAddressInformation current4 in iPInterfaceProperties.MulticastAddresses)
				{
					try
					{
						if (current4.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							hashSet3.Add(DotNetPlatform.ConvertAddress(current4.Address));
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			if (hashSet2.Count == 0 || hashSet.Count == 0)
			{
				return null;
			}
		}
		return new DotNetInterface(n, hashSet.ToArray<UdpIPv4Address>(), hashSet2.ToArray<UdpIPv4Address>(), hashSet3.ToArray<UdpIPv4Address>());
	}

	public static UdpEndPoint ConvertEndPoint(EndPoint endpoint)
	{
		return DotNetPlatform.ConvertEndPoint((IPEndPoint)endpoint);
	}

	public static UdpEndPoint ConvertEndPoint(IPEndPoint endpoint)
	{
		return new UdpEndPoint(new UdpIPv4Address(endpoint.Address.Address), (ushort)endpoint.Port);
	}

	public static UdpIPv4Address ConvertAddress(IPAddress address)
	{
		return new UdpIPv4Address(address.Address);
	}

	public static IPEndPoint ConvertEndPoint(UdpEndPoint endpoint)
	{
		return new IPEndPoint(new IPAddress(new byte[]
		{
			endpoint.Address.Byte3,
			endpoint.Address.Byte2,
			endpoint.Address.Byte1,
			endpoint.Address.Byte0
		}), (int)endpoint.Port);
	}

	private static bool IsValidInterface(NetworkInterface nic, IPInterfaceProperties p)
	{
		foreach (GatewayIPAddressInformation current in p.GatewayAddresses)
		{
			byte[] addressBytes = current.Address.GetAddressBytes();
			if (addressBytes.Length == 4 && addressBytes[0] != 0 && addressBytes[1] != 0 && addressBytes[2] != 0 && addressBytes[3] != 0)
			{
				return true;
			}
		}
		return false;
	}

	private static IPAddress FindBroadcastAddress(bool strict)
	{
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		NetworkInterface[] array = allNetworkInterfaces;
		int i = 0;
		while (i < array.Length)
		{
			NetworkInterface networkInterface = array[i];
			NetworkInterfaceType networkInterfaceType = networkInterface.NetworkInterfaceType;
			switch (networkInterfaceType)
			{
			case NetworkInterfaceType.FastEthernetFx:
			case NetworkInterfaceType.Wireless80211:
				goto IL_59;
			case (NetworkInterfaceType)70:
				IL_31:
				if (networkInterfaceType != NetworkInterfaceType.Ethernet && networkInterfaceType != NetworkInterfaceType.Ethernet3Megabit && networkInterfaceType != NetworkInterfaceType.FastEthernetT && networkInterfaceType != NetworkInterfaceType.GigabitEthernet)
				{
					goto IL_182;
				}
				goto IL_59;
			}
			goto IL_31;
			IL_182:
			i++;
			continue;
			IL_59:
			if (networkInterface.OperationalStatus == OperationalStatus.Up || networkInterface.OperationalStatus == OperationalStatus.Unknown)
			{
				IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
				if (!strict || DotNetPlatform.IsValidInterface(networkInterface, iPProperties))
				{
					foreach (UnicastIPAddressInformation current in iPProperties.UnicastAddresses)
					{
						if (current.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							IPAddress result;
							if (iPProperties.DhcpServerAddresses.Count == 0 && !strict)
							{
								byte[] addressBytes = current.Address.GetAddressBytes();
								addressBytes[3] = 255;
								result = new IPAddress(addressBytes);
								return result;
							}
							byte[] addressBytes2 = iPProperties.DhcpServerAddresses[0].GetAddressBytes();
							byte[] addressBytes3 = current.IPv4Mask.GetAddressBytes();
							byte[] array2 = new byte[4];
							for (int j = 0; j < addressBytes2.Length; j++)
							{
								array2[j] = ((addressBytes2[j] & addressBytes3[j]) | ~addressBytes3[j]);
							}
							result = new IPAddress(array2);
							return result;
						}
					}
				}
			}
			goto IL_182;
		}
		if (strict)
		{
			return DotNetPlatform.FindBroadcastAddress(false);
		}
		return IPAddress.Any;
	}
}
