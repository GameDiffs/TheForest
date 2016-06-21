using System;
using UnityEngine;

namespace Smaa
{
	public sealed class MinAttribute : PropertyAttribute
	{
		public readonly float min;

		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}
