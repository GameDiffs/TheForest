using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class InputFieldInfo : UIElementInfo
	{
		public int actionId
		{
			get;
			set;
		}

		public AxisRange axisRange
		{
			get;
			set;
		}

		public int actionElementMapId
		{
			get;
			set;
		}

		public ControllerType controllerType
		{
			get;
			set;
		}

		public int controllerId
		{
			get;
			set;
		}
	}
}
