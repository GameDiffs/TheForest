using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct SOUND_INFO_INTERNAL
	{
		private IntPtr name_or_data;

		private MODE mode;

		private CREATESOUNDEXINFO exinfo;

		private int subsoundIndex;

		public void assign(out SOUND_INFO publicInfo)
		{
			publicInfo = new SOUND_INFO();
			publicInfo.mode = this.mode;
			publicInfo.exinfo = this.exinfo;
			publicInfo.exinfo.inclusionlist = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
			Marshal.WriteInt32(publicInfo.exinfo.inclusionlist, this.subsoundIndex);
			publicInfo.exinfo.inclusionlistnum = 1;
			publicInfo.subsoundIndex = this.subsoundIndex;
			if (this.name_or_data != IntPtr.Zero)
			{
				int num;
				int num2;
				if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT)
				{
					publicInfo.mode = ((publicInfo.mode & ~MODE.OPENMEMORY_POINT) | MODE.OPENMEMORY);
					num = (int)this.exinfo.fileoffset;
					publicInfo.exinfo.fileoffset = 0u;
					num2 = (int)this.exinfo.length;
				}
				else
				{
					num = 0;
					num2 = MarshallingHelper.stringLengthUtf8(this.name_or_data) + 1;
				}
				publicInfo.name_or_data = new byte[num2];
				Marshal.Copy(new IntPtr(this.name_or_data.ToInt64() + (long)num), publicInfo.name_or_data, 0, num2);
			}
			else
			{
				publicInfo.name_or_data = null;
			}
		}
	}
}
