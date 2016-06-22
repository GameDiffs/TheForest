using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRTrackedCamera
	{
		private IVRTrackedCamera FnTable;

		internal CVRTrackedCamera(IntPtr pInterface)
		{
			this.FnTable = (IVRTrackedCamera)Marshal.PtrToStructure(pInterface, typeof(IVRTrackedCamera));
		}

		public bool HasCamera(uint nDeviceIndex)
		{
			return this.FnTable.HasCamera(nDeviceIndex);
		}

		public bool GetCameraFirmwareDescription(uint nDeviceIndex, string pBuffer, uint nBufferLen)
		{
			return this.FnTable.GetCameraFirmwareDescription(nDeviceIndex, pBuffer, nBufferLen);
		}

		public bool GetCameraFrameDimensions(uint nDeviceIndex, ECameraVideoStreamFormat nVideoStreamFormat, ref uint pWidth, ref uint pHeight)
		{
			pWidth = 0u;
			pHeight = 0u;
			return this.FnTable.GetCameraFrameDimensions(nDeviceIndex, nVideoStreamFormat, ref pWidth, ref pHeight);
		}

		public bool SetCameraVideoStreamFormat(uint nDeviceIndex, ECameraVideoStreamFormat nVideoStreamFormat)
		{
			return this.FnTable.SetCameraVideoStreamFormat(nDeviceIndex, nVideoStreamFormat);
		}

		public ECameraVideoStreamFormat GetCameraVideoStreamFormat(uint nDeviceIndex)
		{
			return this.FnTable.GetCameraVideoStreamFormat(nDeviceIndex);
		}

		public bool EnableCameraForStreaming(uint nDeviceIndex, bool bEnable)
		{
			return this.FnTable.EnableCameraForStreaming(nDeviceIndex, bEnable);
		}

		public bool StartVideoStream(uint nDeviceIndex)
		{
			return this.FnTable.StartVideoStream(nDeviceIndex);
		}

		public bool StopVideoStream(uint nDeviceIndex)
		{
			return this.FnTable.StopVideoStream(nDeviceIndex);
		}

		public bool IsVideoStreamActive(uint nDeviceIndex, ref bool pbPaused, ref float pflElapsedTime)
		{
			pbPaused = false;
			pflElapsedTime = 0f;
			return this.FnTable.IsVideoStreamActive(nDeviceIndex, ref pbPaused, ref pflElapsedTime);
		}

		public CameraVideoStreamFrame_t GetVideoStreamFrame(uint nDeviceIndex)
		{
			IntPtr ptr = this.FnTable.GetVideoStreamFrame(nDeviceIndex);
			return (CameraVideoStreamFrame_t)Marshal.PtrToStructure(ptr, typeof(CameraVideoStreamFrame_t));
		}

		public bool ReleaseVideoStreamFrame(uint nDeviceIndex, ref CameraVideoStreamFrame_t pFrameImage)
		{
			return this.FnTable.ReleaseVideoStreamFrame(nDeviceIndex, ref pFrameImage);
		}

		public bool SetAutoExposure(uint nDeviceIndex, bool bEnable)
		{
			return this.FnTable.SetAutoExposure(nDeviceIndex, bEnable);
		}

		public bool PauseVideoStream(uint nDeviceIndex)
		{
			return this.FnTable.PauseVideoStream(nDeviceIndex);
		}

		public bool ResumeVideoStream(uint nDeviceIndex)
		{
			return this.FnTable.ResumeVideoStream(nDeviceIndex);
		}

		public bool GetCameraDistortion(uint nDeviceIndex, float flInputU, float flInputV, ref float pflOutputU, ref float pflOutputV)
		{
			pflOutputU = 0f;
			pflOutputV = 0f;
			return this.FnTable.GetCameraDistortion(nDeviceIndex, flInputU, flInputV, ref pflOutputU, ref pflOutputV);
		}

		public bool GetCameraProjection(uint nDeviceIndex, float flWidthPixels, float flHeightPixels, float flZNear, float flZFar, ref HmdMatrix44_t pProjection)
		{
			return this.FnTable.GetCameraProjection(nDeviceIndex, flWidthPixels, flHeightPixels, flZNear, flZFar, ref pProjection);
		}

		public bool GetRecommendedCameraUndistortion(uint nDeviceIndex, ref uint pUndistortionWidthPixels, ref uint pUndistortionHeightPixels)
		{
			pUndistortionWidthPixels = 0u;
			pUndistortionHeightPixels = 0u;
			return this.FnTable.GetRecommendedCameraUndistortion(nDeviceIndex, ref pUndistortionWidthPixels, ref pUndistortionHeightPixels);
		}

		public bool SetCameraUndistortion(uint nDeviceIndex, uint nUndistortionWidthPixels, uint nUndistortionHeightPixels)
		{
			return this.FnTable.SetCameraUndistortion(nDeviceIndex, nUndistortionWidthPixels, nUndistortionHeightPixels);
		}

		public void RequestVideoServicesForTool()
		{
			this.FnTable.RequestVideoServicesForTool();
		}

		public void ReleaseVideoServicesForTool()
		{
			this.FnTable.ReleaseVideoServicesForTool();
		}

		public bool GetVideoStreamFrameSharedTextureGL(bool bUndistorted, ref uint pglTextureId, IntPtr pglSharedTextureHandle)
		{
			pglTextureId = 0u;
			return this.FnTable.GetVideoStreamFrameSharedTextureGL(bUndistorted, ref pglTextureId, pglSharedTextureHandle);
		}

		public bool ReleaseVideoStreamFrameSharedTextureGL(uint glTextureId, IntPtr glSharedTextureHandle)
		{
			return this.FnTable.ReleaseVideoStreamFrameSharedTextureGL(glTextureId, glSharedTextureHandle);
		}

		public void LockSharedTextureGL(IntPtr glSharedTextureHandle, ref CameraVideoStreamFrame_t pFrameImage)
		{
			this.FnTable.LockSharedTextureGL(glSharedTextureHandle, ref pFrameImage);
		}

		public void UnlockSharedTextureGL(IntPtr glSharedTextureHandle)
		{
			this.FnTable.UnlockSharedTextureGL(glSharedTextureHandle);
		}
	}
}
