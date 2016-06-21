using System;
using System.Net;
using System.Net.Sockets;
using UdpKit;

internal class DotNetSocket : UdpPlatformSocket
{
	private string error;

	private Socket socket;

	private DotNetPlatform platform;

	private EndPoint recvEndPoint;

	private UdpEndPoint endpoint;

	public override UdpPlatform Platform
	{
		get
		{
			return this.platform;
		}
	}

	public override string Error
	{
		get
		{
			return this.error;
		}
	}

	public override bool IsBound
	{
		get
		{
			return this.socket != null && this.socket.IsBound;
		}
	}

	public override UdpEndPoint EndPoint
	{
		get
		{
			this.VerifyIsBound();
			return this.endpoint;
		}
	}

	public override bool Broadcast
	{
		get
		{
			this.VerifyIsBound();
			bool result;
			try
			{
				this.error = null;
				result = this.socket.EnableBroadcast;
			}
			catch (SocketException exn)
			{
				this.HandleSocketException(exn);
				result = false;
			}
			return result;
		}
		set
		{
			this.VerifyIsBound();
			try
			{
				this.socket.EnableBroadcast = value;
			}
			catch (SocketException exn)
			{
				this.error = null;
				this.HandleSocketException(exn);
			}
		}
	}

	public DotNetSocket(DotNetPlatform platform)
	{
		this.platform = platform;
		try
		{
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this.socket.Blocking = false;
			DotNetSocket.SetConnReset(this.socket);
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
		}
		this.recvEndPoint = new IPEndPoint(IPAddress.Any, 0);
	}

	public override void Close()
	{
		this.VerifyIsBound();
		try
		{
			this.error = null;
			this.socket.Close();
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
		}
	}

	public override void Bind(UdpEndPoint ep)
	{
		try
		{
			this.error = null;
			this.socket.Bind(DotNetPlatform.ConvertEndPoint(ep));
			this.endpoint = DotNetPlatform.ConvertEndPoint(this.socket.LocalEndPoint);
			UdpLog.Info("Socket bound to {0}", new object[]
			{
				this.endpoint
			});
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
		}
	}

	public override bool RecvPoll()
	{
		return this.RecvPoll(0);
	}

	public override bool RecvPoll(int timeoutInMs)
	{
		bool result;
		try
		{
			result = this.socket.Poll(timeoutInMs * 1000, SelectMode.SelectRead);
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
			result = false;
		}
		return result;
	}

	public override int RecvFrom(byte[] buffer, int bufferSize, ref UdpEndPoint endpoint)
	{
		int result;
		try
		{
			int num = this.socket.ReceiveFrom(buffer, 0, bufferSize, SocketFlags.None, ref this.recvEndPoint);
			if (num > 0)
			{
				endpoint = DotNetPlatform.ConvertEndPoint(this.recvEndPoint);
				result = num;
			}
			else
			{
				result = -1;
			}
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
			result = -1;
		}
		return result;
	}

	public override int SendTo(byte[] buffer, int bytesToSend, UdpEndPoint endpoint)
	{
		int result;
		try
		{
			result = this.socket.SendTo(buffer, 0, bytesToSend, SocketFlags.None, DotNetPlatform.ConvertEndPoint(endpoint));
		}
		catch (SocketException exn)
		{
			this.HandleSocketException(exn);
			result = -1;
		}
		return result;
	}

	private void HandleSocketException(SocketException exn)
	{
		this.error = exn.ErrorCode + ": " + exn.SocketErrorCode.ToString();
	}

	private void VerifyIsBound()
	{
		if (!this.IsBound)
		{
			throw new InvalidOperationException();
		}
	}

	private static void SetConnReset(Socket s)
	{
		try
		{
			uint ioctl_code = 2550136844u;
			s.IOControl((int)ioctl_code, new byte[]
			{
				Convert.ToByte(false)
			}, null);
		}
		catch
		{
		}
	}
}
