using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Changes a GameObject's scale over time.")]
	public class iTweenScaleTo : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[HutongGames.PlayMaker.Tooltip("Scale To a transform scale.")]
		public FsmGameObject transformScale;

		[HutongGames.PlayMaker.Tooltip("A scale vector the GameObject will animate To.")]
		public FsmVector3 vectorScale;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformScale = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorScale = new FsmVector3
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
			Vector3 vector = (!this.vectorScale.IsNone) ? this.vectorScale.Value : Vector3.zero;
			if (!this.transformScale.IsNone && this.transformScale.Value)
			{
				vector = this.transformScale.Value.transform.localScale + vector;
			}
			this.itweenType = "scale";
			iTween.ScaleTo(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"scale",
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
				!this.realTime.IsNone && this.realTime.Value
			}));
		}
	}
}
