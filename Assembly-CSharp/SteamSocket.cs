using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading;
using UdpKit;

public class SteamSocket : UdpPlatformSocket
{
	private struct Packet
	{
		public UdpSteamID Id;

		public byte[] Data;

		public int Length;
	}

	private class PacketQueue
	{
		private Queue<SteamSocket.Packet> queue = new Queue<SteamSocket.Packet>();

		public int Count
		{
			get
			{
				Queue<SteamSocket.Packet> obj = this.queue;
				int count;
				lock (obj)
				{
					count = this.queue.Count;
				}
				return count;
			}
		}

		public bool TryDequeue(out SteamSocket.Packet packet)
		{
			Queue<SteamSocket.Packet> obj = this.queue;
			lock (obj)
			{
				if (this.queue.Count > 0)
				{
					packet = this.queue.Dequeue();
					return true;
				}
			}
			packet = default(SteamSocket.Packet);
			return false;
		}

		public void Enqueue(SteamSocket.Packet packet)
		{
			Queue<SteamSocket.Packet> obj = this.queue;
			lock (obj)
			{
				this.queue.Enqueue(packet);
			}
		}
	}

	private class BufferPool
	{
		private Dictionary<int, Queue<byte[]>> bufferQueues = new Dictionary<int, Queue<byte[]>>();

		private Queue<byte[]> GetQueue(int size)
		{
			Queue<byte[]> result;
			if (!this.bufferQueues.TryGetValue(size, out result))
			{
				result = (this.bufferQueues[size] = new Queue<byte[]>());
			}
			return result;
		}

		public byte[] Allocate(int size)
		{
			byte[] result = null;
			Dictionary<int, Queue<byte[]>> obj = this.bufferQueues;
			lock (obj)
			{
				Queue<byte[]> queue = this.GetQueue(size);
				if (queue.Count > 0)
				{
					result = queue.Dequeue();
				}
				else
				{
					result = new byte[size];
				}
			}
			return result;
		}

		public byte[] Duplicate(byte[] data)
		{
			byte[] array = this.Allocate(data.Length);
			Buffer.BlockCopy(data, 0, array, 0, data.Length);
			return array;
		}

		public void Free(byte[] buffer)
		{
			Array.Clear(buffer, 0, buffer.Length);
			Dictionary<int, Queue<byte[]>> obj = this.bufferQueues;
			lock (obj)
			{
				this.GetQueue(buffer.Length).Enqueue(buffer);
			}
		}
	}

	private bool bound;

	private UdpSteamID steamId;

	private SteamPlatform platform;

	private SteamSocket.BufferPool bufferPool = new SteamSocket.BufferPool();

	private SteamSocket.PacketQueue recvQueue = new SteamSocket.PacketQueue();

	private SteamSocket.PacketQueue sendQueue = new SteamSocket.PacketQueue();

	public override bool Broadcast
	{
		get
		{
			return false;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public override UdpEndPoint EndPoint
	{
		get
		{
			return new UdpEndPoint(this.steamId);
		}
	}

	public override string Error
	{
		get
		{
			return null;
		}
	}

	public override bool IsBound
	{
		get
		{
			return this.bound;
		}
	}

	public override UdpPlatform Platform
	{
		get
		{
			return this.platform;
		}
	}

	public SteamSocket(SteamPlatform steamPlatform)
	{
		this.platform = steamPlatform;
	}

	public void Send(bool isServer)
	{
		SteamSocket.Packet packet;
		while (this.sendQueue.TryDequeue(out packet))
		{
			if (!this.SendP2PPacket(isServer, new CSteamID(packet.Id.Id), packet.Data, packet.Length, EP2PSend.k_EP2PSendUnreliable))
			{
			}
			this.bufferPool.Free(packet.Data);
		}
	}

	public void Recv(bool isServer)
	{
		uint v;
		while (this.IsP2PPacketAvailable(isServer, out v))
		{
			byte[] data = this.bufferPool.Allocate((int)UdpMath.NextPow2(v));
			uint length;
			CSteamID cSteamID;
			if (this.ReadP2PPacket(isServer, data, out length, out cSteamID))
			{
				this.recvQueue.Enqueue(new SteamSocket.Packet
				{
					Id = new UdpSteamID(cSteamID.m_SteamID),
					Length = (int)length,
					Data = data
				});
			}
		}
	}

	private bool IsP2PPacketAvailable(bool isServer, out uint size)
	{
		if (isServer)
		{
			return SteamGameServerNetworking.IsP2PPacketAvailable(out size, 0);
		}
		return SteamNetworking.IsP2PPacketAvailable(out size, 0);
	}

	private bool SendP2PPacket(bool isServer, CSteamID id, byte[] data, int length, EP2PSend mode)
	{
		if (isServer)
		{
			return SteamGameServerNetworking.SendP2PPacket(id, data, (uint)length, mode, 0);
		}
		return SteamNetworking.SendP2PPacket(id, data, (uint)length, mode, 0);
	}

	private bool ReadP2PPacket(bool isServer, byte[] data, out uint msgSize, out CSteamID id)
	{
		if (isServer)
		{
			return SteamGameServerNetworking.ReadP2PPacket(data, (uint)data.Length, out msgSize, out id, 0);
		}
		return SteamNetworking.ReadP2PPacket(data, (uint)data.Length, out msgSize, out id, 0);
	}

	public override void Bind(UdpEndPoint ep)
	{
		this.bound = true;
		this.steamId = ep.SteamId;
	}

	public override void Close()
	{
		this.bound = false;
		this.steamId = default(UdpSteamID);
	}

	public override int RecvFrom(byte[] data, int dataSize, ref UdpEndPoint remoteEndpoint)
	{
		SteamSocket.Packet packet;
		if (this.recvQueue.TryDequeue(out packet))
		{
			Buffer.BlockCopy(packet.Data, 0, data, 0, packet.Data.Length);
			remoteEndpoint = new UdpEndPoint(packet.Id);
			this.bufferPool.Free(packet.Data);
			return packet.Length;
		}
		remoteEndpoint = default(UdpEndPoint);
		return 0;
	}

	public override bool RecvPoll(int timeout)
	{
		if (this.recvQueue.Count > 0)
		{
			return true;
		}
		Thread.Sleep(1);
		return false;
	}

	public override bool RecvPoll()
	{
		return this.RecvPoll(0);
	}

	public override int SendTo(byte[] buffer, int bytesToSend, UdpEndPoint endpoint)
	{
		this.sendQueue.Enqueue(new SteamSocket.Packet
		{
			Data = this.bufferPool.Duplicate(buffer),
			Length = bytesToSend,
			Id = endpoint.SteamId
		});
		return bytesToSend;
	}
}
