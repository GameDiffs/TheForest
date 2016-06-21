using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Action version of Unity's Smooth Follow script.")]
	public class SmoothFollowAction : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The game object to control. E.g. The camera.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The distance in the x-z plane to the target.")]
		public FsmFloat distance;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The height we want the camera to be above the target")]
		public FsmFloat height;

		[RequiredField, HutongGames.PlayMaker.Tooltip("How much to dampen height movement.")]
		public FsmFloat heightDamping;

		[RequiredField, HutongGames.PlayMaker.Tooltip("How much to dampen rotation changes.")]
		public FsmFloat rotationDamping;

		private GameObject cachedObect;

		private Transform myTransform;

		private Transform targetTransform;

		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.distance = 10f;
			this.height = 5f;
			this.heightDamping = 2f;
			this.rotationDamping = 3f;
		}

		public override void OnLateUpdate()
		{
			if (this.targetObject.Value == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.cachedObect != ownerDefaultTarget)
			{
				this.cachedObect = ownerDefaultTarget;
				this.myTransform = ownerDefaultTarget.transform;
				this.targetTransform = this.targetObject.Value.transform;
			}
			float y = this.targetTransform.eulerAngles.y;
			float to = this.targetTransform.position.y + this.height.Value;
			float num = this.myTransform.eulerAngles.y;
			float num2 = this.myTransform.position.y;
			num = Mathf.LerpAngle(num, y, this.rotationDamping.Value * Time.deltaTime);
			num2 = Mathf.Lerp(num2, to, this.heightDamping.Value * Time.deltaTime);
			Quaternion rotation = Quaternion.Euler(0f, num, 0f);
			this.myTransform.position = this.targetTransform.position;
			this.myTransform.position -= rotation * Vector3.forward * this.distance.Value;
			this.myTransform.position = new Vector3(this.myTransform.position.x, num2, this.myTransform.position.z);
			this.myTransform.LookAt(this.targetTransform);
		}
	}
}
