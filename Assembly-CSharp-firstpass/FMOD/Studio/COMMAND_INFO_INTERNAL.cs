using System;

namespace FMOD.Studio
{
	internal struct COMMAND_INFO_INTERNAL
	{
		public IntPtr commandName;

		public int parentCommandIndex;

		public int frameNumber;

		public float frameTime;

		public INSTANCETYPE instanceType;

		public INSTANCETYPE outputType;

		public uint instanceHandle;

		public uint outputHandle;

		public COMMAND_INFO createPublic()
		{
			return new COMMAND_INFO
			{
				commandName = MarshallingHelper.stringFromNativeUtf8(this.commandName),
				parentCommandIndex = this.parentCommandIndex,
				frameNumber = this.frameNumber,
				frameTime = this.frameTime,
				instanceType = this.instanceType,
				outputType = this.outputType,
				instanceHandle = this.instanceHandle,
				outputHandle = this.outputHandle
			};
		}
	}
}
