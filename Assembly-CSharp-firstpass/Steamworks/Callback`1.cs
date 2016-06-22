using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public sealed class Callback<T>
	{
		public delegate void DispatchDelegate(T param);

		private CCallbackBaseVTable VTable;

		private IntPtr m_pVTable = IntPtr.Zero;

		private CCallbackBase m_CCallbackBase;

		private GCHandle m_pCCallbackBase;

		private bool m_bGameServer;

		private readonly int m_size = Marshal.SizeOf(typeof(T));

		private event Callback<T>.DispatchDelegate m_Func
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_Func = (Callback<T>.DispatchDelegate)Delegate.Combine(this.m_Func, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_Func = (Callback<T>.DispatchDelegate)Delegate.Remove(this.m_Func, value);
			}
		}

		public Callback(Callback<T>.DispatchDelegate func, bool bGameServer = false)
		{
			this.m_bGameServer = bGameServer;
			this.BuildCCallbackBase();
			this.Register(func);
		}

		public static Callback<T> Create(Callback<T>.DispatchDelegate func)
		{
			return new Callback<T>(func, false);
		}

		public static Callback<T> CreateGameServer(Callback<T>.DispatchDelegate func)
		{
			return new Callback<T>(func, true);
		}

		~Callback()
		{
			this.Unregister();
			if (this.m_pVTable != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pVTable);
			}
			if (this.m_pCCallbackBase.IsAllocated)
			{
				this.m_pCCallbackBase.Free();
			}
		}

		public void Register(Callback<T>.DispatchDelegate func)
		{
			if (func == null)
			{
				throw new Exception("Callback function must not be null.");
			}
			if ((this.m_CCallbackBase.m_nCallbackFlags & 1) == 1)
			{
				this.Unregister();
			}
			if (this.m_bGameServer)
			{
				this.SetGameserverFlag();
			}
			this.m_Func = func;
			NativeMethods.SteamAPI_RegisterCallback(this.m_pCCallbackBase.AddrOfPinnedObject(), CallbackIdentities.GetCallbackIdentity(typeof(T)));
		}

		public void Unregister()
		{
			NativeMethods.SteamAPI_UnregisterCallback(this.m_pCCallbackBase.AddrOfPinnedObject());
		}

		public void SetGameserverFlag()
		{
			CCallbackBase expr_06 = this.m_CCallbackBase;
			expr_06.m_nCallbackFlags |= 2;
		}

		private void OnRunCallback(IntPtr pvParam)
		{
			try
			{
				this.m_Func((T)((object)Marshal.PtrToStructure(pvParam, typeof(T))));
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		private void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall)
		{
			try
			{
				this.m_Func((T)((object)Marshal.PtrToStructure(pvParam, typeof(T))));
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		private int OnGetCallbackSizeBytes()
		{
			return this.m_size;
		}

		private void BuildCCallbackBase()
		{
			this.VTable = new CCallbackBaseVTable
			{
				m_RunCallResult = new CCallbackBaseVTable.RunCRDel(this.OnRunCallResult),
				m_RunCallback = new CCallbackBaseVTable.RunCBDel(this.OnRunCallback),
				m_GetCallbackSizeBytes = new CCallbackBaseVTable.GetCallbackSizeBytesDel(this.OnGetCallbackSizeBytes)
			};
			this.m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CCallbackBaseVTable)));
			Marshal.StructureToPtr(this.VTable, this.m_pVTable, false);
			this.m_CCallbackBase = new CCallbackBase
			{
				m_vfptr = this.m_pVTable,
				m_iCallback = CallbackIdentities.GetCallbackIdentity(typeof(T))
			};
			this.m_pCCallbackBase = GCHandle.Alloc(this.m_CCallbackBase, GCHandleType.Pinned);
		}
	}
}
