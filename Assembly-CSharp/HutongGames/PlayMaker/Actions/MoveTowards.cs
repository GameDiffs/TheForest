using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Moves a Game Object towards a Target. Optionally sends an event when successful. The Target can be specified as a Game Object or a world Position. If you specify both, then the Position is used as a local offset from the Object's Position.")]
	public class MoveTowards : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to Move")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("A target GameObject to move towards. Or use a world Target Position below.")]
		public FsmGameObject targetObject;

		[HutongGames.PlayMaker.Tooltip("A world position if no Target Object. Otherwise used as a local offset from the Target Object.")]
		public FsmVector3 targetPosition;

		[HutongGames.PlayMaker.Tooltip("Ignore any height difference in the target.")]
		public FsmBool ignoreVertical;

		[HasFloatSlider(0f, 20f), HutongGames.PlayMaker.Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
		public FsmFloat maxSpeed;

		[HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
		public FsmFloat finishDistance;

		[HutongGames.PlayMaker.Tooltip("Event to send when the Finish Distance is reached.")]
		public FsmEvent finishEvent;

		private GameObject go;

		private GameObject goTarget;

		private Vector3 targetPos;

		private Vector3 targetPosWithVertical;

		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.maxSpeed = 10f;
			this.finishDistance = 1f;
			this.finishEvent = null;
		}

		public override void OnUpdate()
		{
			this.DoMoveTowards();
		}

		private void DoMoveTowards()
		{
			if (!this.UpdateTargetPos())
			{
				return;
			}
			this.go.transform.position = Vector3.MoveTowards(this.go.transform.position, this.targetPos, this.maxSpeed.Value * Time.deltaTime);
			float magnitude = (this.go.transform.position - this.targetPos).magnitude;
			if (magnitude < this.finishDistance.Value)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}

		public bool UpdateTargetPos()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				return false;
			}
			this.goTarget = this.targetObject.Value;
			if (this.goTarget == null && this.targetPosition.IsNone)
			{
				return false;
			}
			if (this.goTarget != null)
			{
				this.targetPos = (this.targetPosition.IsNone ? this.goTarget.transform.position : this.goTarget.transform.TransformPoint(this.targetPosition.Value));
			}
			else
			{
				this.targetPos = this.targetPosition.Value;
			}
			this.targetPosWithVertical = this.targetPos;
			if (this.ignoreVertical.Value)
			{
				this.targetPos.y = this.go.transform.position.y;
			}
			return true;
		}

		public Vector3 GetTargetPos()
		{
			return this.targetPos;
		}

		public Vector3 GetTargetPosWithVertical()
		{
			return this.targetPosWithVertical;
		}
	}
}
