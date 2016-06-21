using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("A Horizontal Slider linked to a Float Variable.")]
	public class GUILayoutHorizontalSlider : GUILayoutAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat leftValue;

		[RequiredField]
		public FsmFloat rightValue;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.changedEvent = null;
		}

		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUILayout.HorizontalSlider(this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, base.LayoutOptions);
			}
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = changed;
			}
		}
	}
}
