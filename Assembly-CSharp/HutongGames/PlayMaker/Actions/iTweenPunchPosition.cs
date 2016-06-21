using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.")]
	public class iTweenPunchPosition : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[RequiredField, HutongGames.PlayMaker.Tooltip("A vector punch range.")]
		public FsmVector3 vector;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		public Space space;

		[HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;

		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.vector = new FsmVector3
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			this.itweenType = "punch";
			iTween.PunchPosition(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"amount",
				vector,
				"name",
				(!this.id.IsNone) ? this.id.Value : string.Empty,
				"time",
				(!this.time.IsNone) ? this.time.Value : 1f,
				"delay",
				(!this.delay.IsNone) ? this.delay.Value : 0f,
				"looptype",
				this.loopType,
				"oncomplete",
				"iTweenOnComplete",
				"oncompleteparams",
				this.itweenID,
				"onstart",
				"iTweenOnStart",
				"onstartparams",
				this.itweenID,
				"ignoretimescale",
				!this.realTime.IsNone && this.realTime.Value,
				"space",
				this.space,
				"axis",
				(this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty
			}));
		}
	}
}
