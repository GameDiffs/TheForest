using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public class CueInstance : HandleBase
	{
		public CueInstance(IntPtr raw) : base(raw)
		{
		}

		public RESULT trigger()
		{
			return CueInstance.FMOD_Studio_CueInstance_Trigger(this.rawPtr);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_CueInstance_IsValid(IntPtr cue);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CueInstance_Trigger(IntPtr cue);

		protected override bool isValidInternal()
		{
			return CueInstance.FMOD_Studio_CueInstance_IsValid(this.rawPtr);
		}
	}
}
