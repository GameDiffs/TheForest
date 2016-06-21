using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ComponentSerializerFor : Attribute
{
	public Type SerializesType;

	public ComponentSerializerFor(Type serializesType)
	{
		this.SerializesType = serializesType;
	}
}
