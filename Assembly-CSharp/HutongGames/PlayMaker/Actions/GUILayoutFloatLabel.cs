using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Label for a Float Variable.")]
	public class GUILayoutFloatLabel : GUILayoutAction
	{
		[HutongGames.PlayMaker.Tooltip("Text to put before the float variable.")]
		public FsmString prefix;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Float variable to display."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			this.prefix = string.Empty;
			this.style = string.Empty;
			this.floatVariable = null;
		}

		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value), this.style.Value, base.LayoutOptions);
			}
		}
	}
}
