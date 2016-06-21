using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Quaternion"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W970"), Tooltip("Get the vector3 from a quaternion multiplied by a vector.")]
	public class GetQuaternionMultipliedByVector : FsmStateAction
	{
		[RequiredField, Tooltip("The quaternion to multiply")]
		public FsmQuaternion quaternion;

		[RequiredField, Tooltip("The vector3 to multiply")]
		public FsmVector3 vector3;

		[RequiredField, Tooltip("The resulting vector3"), UIHint(UIHint.Variable)]
		public FsmVector3 result;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.quaternion = null;
			this.vector3 = null;
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
			this.result.Value = this.quaternion.Value * this.vector3.Value;
		}
	}
}
