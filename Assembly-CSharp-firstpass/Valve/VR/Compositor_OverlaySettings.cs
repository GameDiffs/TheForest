using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct Compositor_OverlaySettings
	{
		public uint size;

		[MarshalAs(UnmanagedType.I1)]
		public bool curved;

		[MarshalAs(UnmanagedType.I1)]
		public bool antialias;

		public float scale;

		public float distance;

		public float alpha;

		public float uOffset;

		public float vOffset;

		public float uScale;

		public float vScale;

		public float gridDivs;

		public float gridWidth;

		public float gridScale;

		public HmdMatrix44_t transform;
	}
}
