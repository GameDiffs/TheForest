using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRRenderModels
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRRenderModelError _LoadRenderModel_Async(string pchRenderModelName, ref IntPtr ppRenderModel);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FreeRenderModel(IntPtr pRenderModel);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRRenderModelError _LoadTexture_Async(int textureId, ref IntPtr ppTexture);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FreeTexture(IntPtr pTexture);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRRenderModelError _LoadTextureD3D11_Async(int textureId, IntPtr pD3D11Device, ref IntPtr ppD3D11Texture2D);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRRenderModelError _LoadIntoTextureD3D11_Async(int textureId, IntPtr pDstTexture);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FreeTextureD3D11(IntPtr pD3D11Texture2D);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetRenderModelName(uint unRenderModelIndex, StringBuilder pchRenderModelName, uint unRenderModelNameLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetRenderModelCount();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetComponentCount(string pchRenderModelName);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetComponentName(string pchRenderModelName, uint unComponentIndex, StringBuilder pchComponentName, uint unComponentNameLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetComponentButtonMask(string pchRenderModelName, string pchComponentName);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetComponentRenderModelName(string pchRenderModelName, string pchComponentName, StringBuilder pchComponentRenderModelName, uint unComponentRenderModelNameLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetComponentState(string pchRenderModelName, string pchComponentName, ref VRControllerState_t pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _RenderModelHasComponent(string pchRenderModelName, string pchComponentName);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._LoadRenderModel_Async LoadRenderModel_Async;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._FreeRenderModel FreeRenderModel;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._LoadTexture_Async LoadTexture_Async;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._FreeTexture FreeTexture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._LoadTextureD3D11_Async LoadTextureD3D11_Async;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._LoadIntoTextureD3D11_Async LoadIntoTextureD3D11_Async;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._FreeTextureD3D11 FreeTextureD3D11;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetRenderModelName GetRenderModelName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetRenderModelCount GetRenderModelCount;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetComponentCount GetComponentCount;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetComponentName GetComponentName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetComponentButtonMask GetComponentButtonMask;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetComponentRenderModelName GetComponentRenderModelName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._GetComponentState GetComponentState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRRenderModels._RenderModelHasComponent RenderModelHasComponent;
	}
}
