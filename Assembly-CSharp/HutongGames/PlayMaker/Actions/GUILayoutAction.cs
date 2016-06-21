using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HutongGames.PlayMaker.Tooltip("GUILayout base action - don't use!")]
	public abstract class GUILayoutAction : FsmStateAction
	{
		public LayoutOption[] layoutOptions;

		private GUILayoutOption[] options;

		public GUILayoutOption[] LayoutOptions
		{
			get
			{
				if (this.options == null)
				{
					this.options = new GUILayoutOption[this.layoutOptions.Length];
					for (int i = 0; i < this.layoutOptions.Length; i++)
					{
						this.options[i] = this.layoutOptions[i].GetGUILayoutOption();
					}
				}
				return this.options;
			}
		}

		public override void Reset()
		{
			this.layoutOptions = new LayoutOption[0];
		}
	}
}
