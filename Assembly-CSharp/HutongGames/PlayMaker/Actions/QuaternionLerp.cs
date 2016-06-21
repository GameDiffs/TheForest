using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1092"), HutongGames.PlayMaker.Tooltip("Interpolates between from and to by t and normalizes the result afterwards.")]
	public class QuaternionLerp : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("From Quaternion.")]
		public FsmQuaternion fromQuaternion;

		[RequiredField, HutongGames.PlayMaker.Tooltip("To Quaternion.")]
		public FsmQuaternion toQuaternion;

		[HasFloatSlider(0f, 1f), RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between.")]
		public FsmFloat amount;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the result in this quaternion variable."), UIHint(UIHint.Variable)]
		public FsmQuaternion storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.fromQuaternion = new FsmQuaternion
			{
				UseVariable = true
			};
			this.toQuaternion = new FsmQuaternion
			{
				UseVariable = true
			};
			this.amount = 0.5f;
			this.storeResult = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoQuatLerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatLerp();
		}

		private void DoQuatLerp()
		{
			this.storeResult.Value = Quaternion.Lerp(this.fromQuaternion.Value, this.toQuaternion.Value, this.amount.Value);
		}
	}
}
