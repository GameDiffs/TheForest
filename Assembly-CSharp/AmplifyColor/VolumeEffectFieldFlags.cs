using System;
using System.Reflection;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectFieldFlags
	{
		public string fieldName;

		public string fieldType;

		public bool blendFlag;

		public VolumeEffectFieldFlags(FieldInfo pi)
		{
			this.fieldName = pi.Name;
			this.fieldType = pi.FieldType.FullName;
		}

		public VolumeEffectFieldFlags(VolumeEffectField field)
		{
			this.fieldName = field.fieldName;
			this.fieldType = field.fieldType;
			this.blendFlag = true;
		}
	}
}
