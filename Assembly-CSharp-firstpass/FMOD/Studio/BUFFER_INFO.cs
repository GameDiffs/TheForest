using System;

namespace FMOD.Studio
{
	public struct BUFFER_INFO
	{
		public int currentUsage;

		public int peakUsage;

		public int capacity;

		public int stallCount;

		public float stallTime;
	}
}
