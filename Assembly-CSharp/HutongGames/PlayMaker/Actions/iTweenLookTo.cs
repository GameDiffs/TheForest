using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
	public class iTweenLookTo : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[HutongGames.PlayMaker.Tooltip("Look at a transform position.")]
		public FsmGameObject transformTarget;

		[HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a local offset.")]
		public FsmVector3 vectorTarget;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("For the shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		[HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
		public iTweenFsmAction.AxisRestriction axis;

		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformTarget = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorTarget = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
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
			Vector3 vector = (!this.vectorTarget.IsNone) ? this.vectorTarget.Value : Vector3.zero;
			if (!this.transformTarget.IsNone && this.transformTarget.Value)
			{
				vector = this.transformTarget.Value.transform.position + vector;
			}
			this.itweenType = "rotate";
			iTween.LookTo(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"looktarget",
				vector,
				"name",
				(!this.id.IsNone) ? this.id.Value : string.Empty,
				(!this.speed.IsNone) ? "speed" : "time",
				(!this.speed.IsNone) ? this.speed.Value : ((!this.time.IsNone) ? this.time.Value : 1f),
				"delay",
				(!this.delay.IsNone) ? this.delay.Value : 0f,
				"easetype",
				this.easeType,
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
				"axis",
				(this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty
			}));
		}
	}
}
