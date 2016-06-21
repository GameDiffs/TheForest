using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1095"), HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates angle degrees around axis.")]
	public class QuaternionAngleAxis : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The angle.")]
		public FsmFloat angle;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The rotation axis.")]
		public FsmVector3 axis;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the rotation of this quaternion variable."), UIHint(UIHint.Variable)]
		public FsmQuaternion result;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.angle = null;
			this.axis = null;
			this.result = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoQuatAngleAxis();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatAngleAxis();
		}

		private void DoQuatAngleAxis()
		{
			this.result.Value = Quaternion.AngleAxis(this.angle.Value, this.axis.Value);
		}
	}
}
