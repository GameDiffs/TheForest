using System;
using UnityEngine;

namespace Ceto
{
	[DisallowMultipleComponent]
	public abstract class OceanGridBase : OceanComponent
	{
		public bool ForceRecreate
		{
			get;
			set;
		}
	}
}
