using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRSettings
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetSettingsErrorNameFromEnum(EVRSettingsError eError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _Sync(bool bForce, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetBool(string pchSection, string pchSettingsKey, bool bDefaultValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetBool(string pchSection, string pchSettingsKey, bool bValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int _GetInt32(string pchSection, string pchSettingsKey, int nDefaultValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetInt32(string pchSection, string pchSettingsKey, int nValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetFloat(string pchSection, string pchSettingsKey, float flDefaultValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetFloat(string pchSection, string pchSettingsKey, float flValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetString(string pchSection, string pchSettingsKey, string pchValue, uint unValueLen, string pchDefaultValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetString(string pchSection, string pchSettingsKey, string pchValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RemoveSection(string pchSection, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RemoveKeyInSection(string pchSection, string pchSettingsKey, ref EVRSettingsError peError);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetSettingsErrorNameFromEnum GetSettingsErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._Sync Sync;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetBool GetBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetBool SetBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetInt32 GetInt32;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetInt32 SetInt32;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetFloat GetFloat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetFloat SetFloat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetString GetString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetString SetString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._RemoveSection RemoveSection;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._RemoveKeyInSection RemoveKeyInSection;
	}
}
