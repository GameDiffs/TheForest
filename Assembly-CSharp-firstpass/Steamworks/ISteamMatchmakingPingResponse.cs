using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class ISteamMatchmakingPingResponse
	{
		[StructLayout(LayoutKind.Sequential)]
		private class VTable
		{
			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public ISteamMatchmakingPingResponse.InternalServerResponded m_VTServerResponded;

			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public ISteamMatchmakingPingResponse.InternalServerFailedToRespond m_VTServerFailedToRespond;
		}

		public delegate void ServerResponded(gameserveritem_t server);

		public delegate void ServerFailedToRespond();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void InternalServerResponded(gameserveritem_t server);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void InternalServerFailedToRespond();

		private ISteamMatchmakingPingResponse.VTable m_VTable;

		private IntPtr m_pVTable;

		private GCHandle m_pGCHandle;

		private ISteamMatchmakingPingResponse.ServerResponded m_ServerResponded;

		private ISteamMatchmakingPingResponse.ServerFailedToRespond m_ServerFailedToRespond;

		public ISteamMatchmakingPingResponse(ISteamMatchmakingPingResponse.ServerResponded onServerResponded, ISteamMatchmakingPingResponse.ServerFailedToRespond onServerFailedToRespond)
		{
			if (onServerResponded == null || onServerFailedToRespond == null)
			{
				throw new ArgumentNullException();
			}
			this.m_ServerResponded = onServerResponded;
			this.m_ServerFailedToRespond = onServerFailedToRespond;
			this.m_VTable = new ISteamMatchmakingPingResponse.VTable
			{
				m_VTServerResponded = new ISteamMatchmakingPingResponse.InternalServerResponded(this.InternalOnServerResponded),
				m_VTServerFailedToRespond = new ISteamMatchmakingPingResponse.InternalServerFailedToRespond(this.InternalOnServerFailedToRespond)
			};
			this.m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ISteamMatchmakingPingResponse.VTable)));
			Marshal.StructureToPtr(this.m_VTable, this.m_pVTable, false);
			this.m_pGCHandle = GCHandle.Alloc(this.m_pVTable, GCHandleType.Pinned);
		}

		~ISteamMatchmakingPingResponse()
		{
			if (this.m_pVTable != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pVTable);
			}
			if (this.m_pGCHandle.IsAllocated)
			{
				this.m_pGCHandle.Free();
			}
		}

		private void InternalOnServerResponded(gameserveritem_t server)
		{
			this.m_ServerResponded(server);
		}

		private void InternalOnServerFailedToRespond()
		{
			this.m_ServerFailedToRespond();
		}

		public static explicit operator IntPtr(ISteamMatchmakingPingResponse that)
		{
			return that.m_pGCHandle.AddrOfPinnedObject();
		}
	}
}
