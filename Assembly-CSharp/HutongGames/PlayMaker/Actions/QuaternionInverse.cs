using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HutongGames.PlayMaker.Tooltip("Inverse a quaternion")]
	public class QuaternionInverse : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("the rotation")]
		public FsmQuaternion rotation;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the inverse of the rotation variable."), UIHint(UIHint.Variable)]
		public FsmQuaternion result;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.rotation = null;
			this.result = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoQuatInverse();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatInverse();
		}

		private void DoQuatInverse()
		{
			this.result.Value = Quaternion.Inverse(this.rotation.Value);
		}
	}
}
