using System;

namespace FMOD.Studio
{
	internal struct USER_PROPERTY_INTERNAL
	{
		private IntPtr name;

		private USER_PROPERTY_TYPE type;

		private Union_IntBoolFloatString value;

		public USER_PROPERTY createPublic()
		{
			USER_PROPERTY result = default(USER_PROPERTY);
			result.name = MarshallingHelper.stringFromNativeUtf8(this.name);
			result.type = this.type;
			switch (this.type)
			{
			case USER_PROPERTY_TYPE.INTEGER:
				result.intValue = this.value.intValue;
				break;
			case USER_PROPERTY_TYPE.BOOLEAN:
				result.boolValue = this.value.boolValue;
				break;
			case USER_PROPERTY_TYPE.FLOAT:
				result.floatValue = this.value.floatValue;
				break;
			case USER_PROPERTY_TYPE.STRING:
				result.stringValue = MarshallingHelper.stringFromNativeUtf8(this.value.stringValue);
				break;
			}
			return result;
		}
	}
}
