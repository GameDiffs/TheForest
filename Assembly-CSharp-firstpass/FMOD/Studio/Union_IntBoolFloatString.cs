using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct Union_IntBoolFloatString
	{
		[FieldOffset(0)]
		public int intValue;

		[FieldOffset(0)]
		public bool boolValue;

		[FieldOffset(0)]
		public float floatValue;

		[FieldOffset(0)]
		public IntPtr stringValue;
	}
}
