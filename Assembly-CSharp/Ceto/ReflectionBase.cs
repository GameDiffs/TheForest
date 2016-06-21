using System;
using UnityEngine;

namespace Ceto
{
	[DisallowMultipleComponent]
	public abstract class ReflectionBase : OceanComponent
	{
		public Func<GameObject, RenderTexture> RenderReflectionCustom;

		public abstract void RenderReflection(GameObject go);
	}
}
