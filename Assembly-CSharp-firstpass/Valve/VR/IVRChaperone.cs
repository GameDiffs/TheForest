using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRChaperone
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ChaperoneCalibrationState _GetCalibrationState();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetPlayAreaSize(ref float pSizeX, ref float pSizeZ);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetPlayAreaRect(ref HmdQuad_t rect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ReloadInfo();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetSceneColor(HmdColor_t color);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetBoundsColor(ref HmdColor_t pOutputColorArray, int nNumOutputColors, float flCollisionBoundsFadeDistance, ref HmdColor_t pOutputCameraColor);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _AreBoundsVisible();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ForceBoundsVisible(bool bForce);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._GetCalibrationState GetCalibrationState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._GetPlayAreaSize GetPlayAreaSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._GetPlayAreaRect GetPlayAreaRect;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._ReloadInfo ReloadInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._SetSceneColor SetSceneColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._GetBoundsColor GetBoundsColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._AreBoundsVisible AreBoundsVisible;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperone._ForceBoundsVisible ForceBoundsVisible;
	}
}
