using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRChaperoneSetup
	{
		private IVRChaperoneSetup FnTable;

		internal CVRChaperoneSetup(IntPtr pInterface)
		{
			this.FnTable = (IVRChaperoneSetup)Marshal.PtrToStructure(pInterface, typeof(IVRChaperoneSetup));
		}

		public bool CommitWorkingCopy(EChaperoneConfigFile configFile)
		{
			return this.FnTable.CommitWorkingCopy(configFile);
		}

		public void RevertWorkingCopy()
		{
			this.FnTable.RevertWorkingCopy();
		}

		public bool GetWorkingPlayAreaSize(ref float pSizeX, ref float pSizeZ)
		{
			pSizeX = 0f;
			pSizeZ = 0f;
			return this.FnTable.GetWorkingPlayAreaSize(ref pSizeX, ref pSizeZ);
		}

		public bool GetWorkingPlayAreaRect(ref HmdQuad_t rect)
		{
			return this.FnTable.GetWorkingPlayAreaRect(ref rect);
		}

		public bool GetWorkingCollisionBoundsInfo(out HmdQuad_t[] pQuadsBuffer)
		{
			uint num = 0u;
			bool flag = this.FnTable.GetWorkingCollisionBoundsInfo(null, ref num);
			pQuadsBuffer = new HmdQuad_t[num];
			return this.FnTable.GetWorkingCollisionBoundsInfo(pQuadsBuffer, ref num);
		}

		public bool GetLiveCollisionBoundsInfo(out HmdQuad_t[] pQuadsBuffer)
		{
			uint num = 0u;
			bool flag = this.FnTable.GetLiveCollisionBoundsInfo(null, ref num);
			pQuadsBuffer = new HmdQuad_t[num];
			return this.FnTable.GetLiveCollisionBoundsInfo(pQuadsBuffer, ref num);
		}

		public bool GetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetWorkingSeatedZeroPoseToRawTrackingPose(ref pmatSeatedZeroPoseToRawTrackingPose);
		}

		public bool GetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatStandingZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetWorkingStandingZeroPoseToRawTrackingPose(ref pmatStandingZeroPoseToRawTrackingPose);
		}

		public void SetWorkingPlayAreaSize(float sizeX, float sizeZ)
		{
			this.FnTable.SetWorkingPlayAreaSize(sizeX, sizeZ);
		}

		public void SetWorkingCollisionBoundsInfo(HmdQuad_t[] pQuadsBuffer)
		{
			this.FnTable.SetWorkingCollisionBoundsInfo(pQuadsBuffer, (uint)pQuadsBuffer.Length);
		}

		public void SetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatSeatedZeroPoseToRawTrackingPose)
		{
			this.FnTable.SetWorkingSeatedZeroPoseToRawTrackingPose(ref pMatSeatedZeroPoseToRawTrackingPose);
		}

		public void SetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatStandingZeroPoseToRawTrackingPose)
		{
			this.FnTable.SetWorkingStandingZeroPoseToRawTrackingPose(ref pMatStandingZeroPoseToRawTrackingPose);
		}

		public void ReloadFromDisk(EChaperoneConfigFile configFile)
		{
			this.FnTable.ReloadFromDisk(configFile);
		}

		public bool GetLiveSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetLiveSeatedZeroPoseToRawTrackingPose(ref pmatSeatedZeroPoseToRawTrackingPose);
		}

		public void SetWorkingCollisionBoundsTagsInfo(byte[] pTagsBuffer)
		{
			this.FnTable.SetWorkingCollisionBoundsTagsInfo(pTagsBuffer, (uint)pTagsBuffer.Length);
		}

		public bool GetLiveCollisionBoundsTagsInfo(out byte[] pTagsBuffer)
		{
			uint num = 0u;
			bool flag = this.FnTable.GetLiveCollisionBoundsTagsInfo(null, ref num);
			pTagsBuffer = new byte[num];
			return this.FnTable.GetLiveCollisionBoundsTagsInfo(pTagsBuffer, ref num);
		}

		public bool SetWorkingPhysicalBoundsInfo(HmdQuad_t[] pQuadsBuffer)
		{
			return this.FnTable.SetWorkingPhysicalBoundsInfo(pQuadsBuffer, (uint)pQuadsBuffer.Length);
		}

		public bool GetLivePhysicalBoundsInfo(out HmdQuad_t[] pQuadsBuffer)
		{
			uint num = 0u;
			bool flag = this.FnTable.GetLivePhysicalBoundsInfo(null, ref num);
			pQuadsBuffer = new HmdQuad_t[num];
			return this.FnTable.GetLivePhysicalBoundsInfo(pQuadsBuffer, ref num);
		}

		public bool ExportLiveToBuffer(StringBuilder pBuffer, ref uint pnBufferLength)
		{
			pnBufferLength = 0u;
			return this.FnTable.ExportLiveToBuffer(pBuffer, ref pnBufferLength);
		}

		public bool ImportFromBufferToWorking(string pBuffer, uint nImportFlags)
		{
			return this.FnTable.ImportFromBufferToWorking(pBuffer, nImportFlags);
		}
	}
}
