using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W968"), HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates from fromDirection to toDirection. Usually you use this to rotate a transform so that one of its axes eg. the y-axis - follows a target direction toDirection in world space.")]
	public class GetQuaternionFromRotation : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("the 'from' direction")]
		public FsmVector3 fromDirection;

		[RequiredField, HutongGames.PlayMaker.Tooltip("the 'to' direction")]
		public FsmVector3 toDirection;

		[RequiredField, HutongGames.PlayMaker.Tooltip("the resulting quaternion"), UIHint(UIHint.Variable)]
		public FsmQuaternion result;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.fromDirection = null;
			this.toDirection = null;
			this.result = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoQuatFromRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatFromRotation();
		}

		private void DoQuatFromRotation()
		{
			this.result.Value = Quaternion.FromToRotation(this.fromDirection.Value, this.toDirection.Value);
		}
	}
}
