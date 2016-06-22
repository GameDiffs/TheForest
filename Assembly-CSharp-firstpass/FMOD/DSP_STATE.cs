using System;

namespace FMOD
{
	public struct DSP_STATE
	{
		public IntPtr instance;

		public IntPtr plugindata;

		public uint channelmask;

		public int source_speakermode;

		public IntPtr sidechaindata;

		public int sidechainchannels;

		public IntPtr callbacks;

		public int systemobject;
	}
}
