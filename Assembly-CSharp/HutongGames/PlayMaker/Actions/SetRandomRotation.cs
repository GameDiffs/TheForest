using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets Random Rotation for a Game Object. Uncheck an axis to keep its current value.")]
	public class SetRandomRotation : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool x;

		[RequiredField]
		public FsmBool y;

		[RequiredField]
		public FsmBool z;

		public override void Reset()
		{
			this.gameObject = null;
			this.x = true;
			this.y = true;
			this.z = true;
		}

		public override void OnEnter()
		{
			this.DoRandomRotation();
			base.Finish();
		}

		private void DoRandomRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
			float num = localEulerAngles.x;
			float num2 = localEulerAngles.y;
			float num3 = localEulerAngles.z;
			if (this.x.Value)
			{
				num = (float)UnityEngine.Random.Range(0, 360);
			}
			if (this.y.Value)
			{
				num2 = (float)UnityEngine.Random.Range(0, 360);
			}
			if (this.z.Value)
			{
				num3 = (float)UnityEngine.Random.Range(0, 360);
			}
			ownerDefaultTarget.transform.localEulerAngles = new Vector3(num, num2, num3);
		}
	}
}
