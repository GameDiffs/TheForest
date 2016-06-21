using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs"), HutongGames.PlayMaker.Tooltip("Returns true if key exists in the preferences.")]
	public class PlayerPrefsHasKey : FsmStateAction
	{
		[RequiredField]
		public FsmString key;

		[Title("Store Result"), UIHint(UIHint.Variable)]
		public FsmBool variable;

		[HutongGames.PlayMaker.Tooltip("Event to send if key exists.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if key does not exist.")]
		public FsmEvent falseEvent;

		public override void Reset()
		{
			this.key = string.Empty;
		}

		public override void OnEnter()
		{
			base.Finish();
			if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
			{
				this.variable.Value = PlayerPrefs.HasKey(this.key.Value);
			}
			base.Fsm.Event((!this.variable.Value) ? this.falseEvent : this.trueEvent);
		}
	}
}
