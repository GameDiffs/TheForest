using System;

public interface IControlSerializationEx : IControlSerialization
{
	bool ShouldSaveWholeObject();
}
