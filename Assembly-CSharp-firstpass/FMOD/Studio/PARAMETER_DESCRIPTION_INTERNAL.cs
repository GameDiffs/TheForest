using System;

namespace FMOD.Studio
{
	internal struct PARAMETER_DESCRIPTION_INTERNAL
	{
		public IntPtr name;

		public float minimum;

		public float maximum;

		public PARAMETER_TYPE type;

		public void assign(out PARAMETER_DESCRIPTION publicDesc)
		{
			publicDesc.name = MarshallingHelper.stringFromNativeUtf8(this.name);
			publicDesc.minimum = this.minimum;
			publicDesc.maximum = this.maximum;
			publicDesc.type = this.type;
		}
	}
}
