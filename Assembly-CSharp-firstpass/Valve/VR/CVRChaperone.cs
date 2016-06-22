using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRChaperone
	{
		private IVRChaperone FnTable;

		internal CVRChaperone(IntPtr pInterface)
		{
			this.FnTable = (IVRChaperone)Marshal.PtrToStructure(pInterface, typeof(IVRChaperone));
		}

		public ChaperoneCalibrationState GetCalibrationState()
		{
			return this.FnTable.GetCalibrationState();
		}

		public bool GetPlayAreaSize(ref float pSizeX, ref float pSizeZ)
		{
			pSizeX = 0f;
			pSizeZ = 0f;
			return this.FnTable.GetPlayAreaSize(ref pSizeX, ref pSizeZ);
		}

		public bool GetPlayAreaRect(ref HmdQuad_t rect)
		{
			return this.FnTable.GetPlayAreaRect(ref rect);
		}

		public void ReloadInfo()
		{
			this.FnTable.ReloadInfo();
		}

		public void SetSceneColor(HmdColor_t color)
		{
			this.FnTable.SetSceneColor(color);
		}

		public void GetBoundsColor(ref HmdColor_t pOutputColorArray, int nNumOutputColors, float flCollisionBoundsFadeDistance, ref HmdColor_t pOutputCameraColor)
		{
			this.FnTable.GetBoundsColor(ref pOutputColorArray, nNumOutputColors, flCollisionBoundsFadeDistance, ref pOutputCameraColor);
		}

		public bool AreBoundsVisible()
		{
			return this.FnTable.AreBoundsVisible();
		}

		public void ForceBoundsVisible(bool bForce)
		{
			this.FnTable.ForceBoundsVisible(bForce);
		}
	}
}
