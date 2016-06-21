using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Sets a Float Variable to a random value between Min/Max.")]
	public class RandomFloat : FsmStateAction
	{
		[RequiredField]
		public FsmFloat min;

		[RequiredField]
		public FsmFloat max;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			this.min = 0f;
			this.max = 1f;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			this.storeResult.Value = UnityEngine.Random.Range(this.min.Value, this.max.Value);
			base.Finish();
		}
	}
}
