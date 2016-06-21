using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time), HutongGames.PlayMaker.Tooltip("Multiplies a Float by Time.deltaTime to use in frame-rate independent operations. E.g., 10 becomes 10 units per second.")]
	public class PerSecond : FsmStateAction
	{
		[RequiredField]
		public FsmFloat floatValue;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.floatValue = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoPerSecond();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoPerSecond();
		}

		private void DoPerSecond()
		{
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.floatValue.Value * Time.deltaTime;
		}
	}
}
