using System;

namespace FMOD.Studio
{
	public struct COMMAND_INFO
	{
		public string commandName;

		public int parentCommandIndex;

		public int frameNumber;

		public float frameTime;

		public INSTANCETYPE instanceType;

		public INSTANCETYPE outputType;

		public uint instanceHandle;

		public uint outputHandle;
	}
}
