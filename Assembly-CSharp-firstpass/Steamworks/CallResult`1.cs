using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public sealed class CallResult<T>
	{
		public delegate void APIDispatchDelegate(T param, bool bIOFailure);

		private CCallbackBaseVTable VTable;

		private IntPtr m_pVTable = IntPtr.Zero;

		private CCallbackBase m_CCallbackBase;

		private GCHandle m_pCCallbackBase;

		private SteamAPICall_t m_hAPICall = SteamAPICall_t.Invalid;

		private readonly int m_size = Marshal.SizeOf(typeof(T));

		private event CallResult<T>.APIDispatchDelegate m_Func
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_Func = (CallResult<T>.APIDispatchDelegate)Delegate.Combine(this.m_Func, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_Func = (CallResult<T>.APIDispatchDelegate)Delegate.Remove(this.m_Func, value);
			}
		}

		public SteamAPICall_t Handle
		{
			get
			{
				return this.m_hAPICall;
			}
		}

		public CallResult(CallResult<T>.APIDispatchDelegate func = null)
		{
			this.m_Func = func;
			this.BuildCCallbackBase();
		}

		public static CallResult<T> Create(CallResult<T>.APIDispatchDelegate func = null)
		{
			return new CallResult<T>(func);
		}

		~CallResult()
		{
			this.Cancel();
			if (this.m_pVTable != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pVTable);
			}
			if (this.m_pCCallbackBase.IsAllocated)
			{
				this.m_pCCallbackBase.Free();
			}
		}

		public void Set(SteamAPICall_t hAPICall, CallResult<T>.APIDispatchDelegate func = null)
		{
			if (func != null)
			{
				this.m_Func = func;
			}
			if (this.m_Func == null)
			{
				throw new Exception("CallResult function was null, you must either set it in the CallResult Constructor or in Set()");
			}
			if (this.m_hAPICall != SteamAPICall_t.Invalid)
			{
				NativeMethods.SteamAPI_UnregisterCallResult(this.m_pCCallbackBase.AddrOfPinnedObject(), (ulong)this.m_hAPICall);
			}
			this.m_hAPICall = hAPICall;
			if (hAPICall != SteamAPICall_t.Invalid)
			{
				NativeMethods.SteamAPI_RegisterCallResult(this.m_pCCallbackBase.AddrOfPinnedObject(), (ulong)hAPICall);
			}
		}

		public bool IsActive()
		{
			return this.m_hAPICall != SteamAPICall_t.Invalid;
		}

		public void Cancel()
		{
			if (this.m_hAPICall != SteamAPICall_t.Invalid)
			{
				NativeMethods.SteamAPI_UnregisterCallResult(this.m_pCCallbackBase.AddrOfPinnedObject(), (ulong)this.m_hAPICall);
				this.m_hAPICall = SteamAPICall_t.Invalid;
			}
		}

		public void SetGameserverFlag()
		{
			CCallbackBase expr_06 = this.m_CCallbackBase;
			expr_06.m_nCallbackFlags |= 2;
		}

		private void OnRunCallback(IntPtr pvParam)
		{
			this.m_hAPICall = SteamAPICall_t.Invalid;
			try
			{
				this.m_Func((T)((object)Marshal.PtrToStructure(pvParam, typeof(T))), false);
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		private void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall)
		{
			SteamAPICall_t x = (SteamAPICall_t)hSteamAPICall;
			if (x == this.m_hAPICall)
			{
				try
				{
					this.m_Func((T)((object)Marshal.PtrToStructure(pvParam, typeof(T))), bFailed);
				}
				catch (Exception e)
				{
					CallbackDispatcher.ExceptionHandler(e);
				}
				if (x == this.m_hAPICall)
				{
					this.m_hAPICall = SteamAPICall_t.Invalid;
				}
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
				m_RunCallback = new CCallbackBaseVTable.RunCBDel(this.OnRunCallback),
				m_RunCallResult = new CCallbackBaseVTable.RunCRDel(this.OnRunCallResult),
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
