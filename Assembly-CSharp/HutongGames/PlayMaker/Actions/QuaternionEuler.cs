using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1094"), HutongGames.PlayMaker.Tooltip("Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).")]
	public class QuaternionEuler : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The Euler angles.")]
		public FsmVector3 eulerAngles;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the euler angles of this quaternion variable."), UIHint(UIHint.Variable)]
		public FsmQuaternion result;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.eulerAngles = null;
			this.result = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoQuatEuler();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatEuler();
		}

		private void DoQuatEuler()
		{
			this.result.Value = Quaternion.Euler(this.eulerAngles.Value);
		}
	}
}
