using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class ThemedElement : MonoBehaviour
	{
		[Serializable]
		public class ElementInfo
		{
			[SerializeField]
			private string _themeClass;

			[SerializeField]
			private Component _component;

			public string themeClass
			{
				get
				{
					return this._themeClass;
				}
			}

			public Component component
			{
				get
				{
					return this._component;
				}
			}
		}

		[SerializeField]
		private ThemedElement.ElementInfo[] _elements;

		private void Start()
		{
			ControlMapper.ApplyTheme(this._elements);
		}
	}
}
