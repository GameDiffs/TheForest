using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct CameraVideoStreamFrame_t
	{
		public ECameraVideoStreamFormat m_nStreamFormat;

		public uint m_nWidth;

		public uint m_nHeight;

		public uint m_nImageDataSize;

		public uint m_nFrameSequence;

		public uint m_nBufferIndex;

		public uint m_nBufferCount;

		public uint m_nExposureTime;

		public uint m_nISPFrameTimeStamp;

		public uint m_nISPReferenceTimeStamp;

		public uint m_nSyncCounter;

		public uint m_nCamSyncEvents;

		public double m_flReferenceCamSyncTime;

		public double m_flFrameElapsedTime;

		public double m_flFrameDeliveryRate;

		public double m_flFrameCaptureTime_DriverAbsolute;

		public double m_flFrameCaptureTime_ServerRelative;

		public ulong m_nFrameCaptureTicks_ServerAbsolute;

		public double m_flFrameCaptureTime_ClientRelative;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bPoseIsValid;

		public HmdMatrix34_t m_matDeviceToAbsoluteTracking;

		public float m_Pad0;

		public float m_Pad1;

		public float m_Pad2;

		public float m_Pad3;

		public IntPtr m_pImageData;
	}
}
