using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class SOUND_INFO
	{
		public byte[] name_or_data;

		public MODE mode;

		public CREATESOUNDEXINFO exinfo;

		public int subsoundIndex;

		public string name
		{
			get
			{
				if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT || this.name_or_data == null)
				{
					return null;
				}
				int num = Array.IndexOf<byte>(this.name_or_data, 0);
				if (num > 0)
				{
					return Encoding.UTF8.GetString(this.name_or_data, 0, num);
				}
				return null;
			}
		}

		~SOUND_INFO()
		{
			if (this.exinfo.inclusionlist != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.exinfo.inclusionlist);
			}
		}
	}
}
