using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), Tooltip("Get Vector3 Length.")]
	public class GetVectorLength : FsmStateAction
	{
		public FsmVector3 vector3;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeLength;

		public override void Reset()
		{
			this.vector3 = null;
			this.storeLength = null;
		}

		public override void OnEnter()
		{
			this.DoVectorLength();
			base.Finish();
		}

		private void DoVectorLength()
		{
			if (this.vector3 == null)
			{
				return;
			}
			if (this.storeLength == null)
			{
				return;
			}
			this.storeLength.Value = this.vector3.Value.magnitude;
		}
	}
}
