using System;
using System.Collections.Generic;
using System.Diagnostics;
using UdpKit;

public class NullPlatform : UdpPlatform
{
	private class PrecisionTimer
	{
		private static readonly long start = Stopwatch.GetTimestamp();

		private static readonly double freq = 1.0 / (double)Stopwatch.Frequency;

		internal static uint GetCurrentTime()
		{
			long num = Stopwatch.GetTimestamp() - NullPlatform.PrecisionTimer.start;
			double num2 = (double)num * NullPlatform.PrecisionTimer.freq;
			return (uint)(num2 * 1000.0);
		}
	}

	public override bool SupportsBroadcast
	{
		get
		{
			return false;
		}
	}

	public override bool SupportsMasterServer
	{
		get
		{
			return false;
		}
	}

	public override bool IsNull
	{
		get
		{
			return true;
		}
	}

	public NullPlatform()
	{
		this.GetPrecisionTime();
	}

	public override UdpPlatformSocket CreateSocket()
	{
		return new NullSocket(this);
	}

	public override UdpIPv4Address GetBroadcastAddress()
	{
		return UdpIPv4Address.Broadcast;
	}

	public override List<UdpPlatformInterface> GetNetworkInterfaces()
	{
		return new List<UdpPlatformInterface>();
	}

	public override uint GetPrecisionTime()
	{
		return NullPlatform.PrecisionTimer.GetCurrentTime();
	}

	public override UdpIPv4Address[] ResolveHostAddresses(string host)
	{
		return new UdpIPv4Address[0];
	}
}
