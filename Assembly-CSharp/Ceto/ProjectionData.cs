using System;
using UnityEngine;

namespace Ceto
{
	public class ProjectionData
	{
		public bool checkedForFlipping;

		public bool updated;

		public Matrix4x4 projectorVP;

		public Matrix4x4 interpolation;
	}
}
