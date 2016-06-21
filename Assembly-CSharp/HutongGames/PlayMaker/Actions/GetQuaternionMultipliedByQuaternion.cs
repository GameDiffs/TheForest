using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W969"), Tooltip("Get the quaternion from a quaternion multiplied by a quaternion.")]
	public class GetQuaternionMultipliedByQuaternion : FsmStateAction
	{
		[RequiredField, Tooltip("The first quaternion to multiply")]
		public FsmQuaternion quaternionA;

		[RequiredField, Tooltip("The second quaternion to multiply")]
		public FsmQuaternion quaternionB;

		[RequiredField, Tooltip("The resulting quaternion"), UIHint(UIHint.Variable)]
		public FsmQuaternion result;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.quaternionA = null;
			this.quaternionB = null;
			this.result = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoQuatMult();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoQuatMult();
		}

		private void DoQuatMult()
		{
			this.result.Value = this.quaternionA.Value * this.quaternionB.Value;
		}
	}
}
