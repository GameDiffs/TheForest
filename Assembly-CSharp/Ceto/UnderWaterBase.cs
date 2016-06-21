using System;
using UnityEngine;

namespace Ceto
{
	[DisallowMultipleComponent]
	public abstract class UnderWaterBase : OceanComponent
	{
		public abstract UNDERWATER_MODE Mode
		{
			get;
		}

		public abstract DEPTH_MODE DepthMode
		{
			get;
		}

		public bool DisableCopyDepthCmd
		{
			get;
			set;
		}

		public bool DisableNormalFadeCmd
		{
			get;
			set;
		}

		public abstract void RenderOceanMask(GameObject go);

		public abstract void RenderOceanDepth(GameObject go);
	}
}
