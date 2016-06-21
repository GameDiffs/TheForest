using Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializer(typeof(AnimationState))]
public class SerializeAnimationState : SerializerExtensionBase<AnimationState>
{
	public override IEnumerable<object> Save(AnimationState target)
	{
		return new object[]
		{
			target.name
		};
	}

	public override object Load(object[] data, object instance)
	{
		object uo = UnitySerializer.DeserializingObject;
		return new UnitySerializer.DeferredSetter(delegate(Dictionary<string, object> d)
		{
			MethodInfo getMethod = uo.GetType().GetProperty("animation").GetGetMethod();
			if (getMethod != null)
			{
				Animation animation = (Animation)getMethod.Invoke(uo, null);
				if (animation != null)
				{
					return animation[(string)data[0]];
				}
			}
			return null;
		});
	}
}
