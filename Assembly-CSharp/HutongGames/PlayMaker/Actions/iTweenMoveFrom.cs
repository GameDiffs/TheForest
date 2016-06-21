using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time.")]
	public class iTweenMoveFrom : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[HutongGames.PlayMaker.Tooltip("Move From a transform rotation.")]
		public FsmGameObject transformPosition;

		[HutongGames.PlayMaker.Tooltip("The position the GameObject will animate from. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

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

		[HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
		public Space space;

		[ActionSection("LookAt"), HutongGames.PlayMaker.Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[HutongGames.PlayMaker.Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		[HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;

		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformPosition = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorPosition = new FsmVector3
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
			this.space = Space.World;
			this.orientToPath = new FsmBool
			{
				Value = true
			};
			this.lookAtObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.lookAtVector = new FsmVector3
			{
				UseVariable = true
			};
			this.lookTime = 0f;
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
			Vector3 vector = (!this.vectorPosition.IsNone) ? this.vectorPosition.Value : Vector3.zero;
			if (!this.transformPosition.IsNone && this.transformPosition.Value)
			{
				vector = ((this.space != Space.World && !(ownerDefaultTarget.transform.parent == null)) ? (ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transformPosition.Value.transform.position) + vector) : (this.transformPosition.Value.transform.position + vector));
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("position", vector);
			hashtable.Add((!this.speed.IsNone) ? "speed" : "time", (!this.speed.IsNone) ? this.speed.Value : ((!this.time.IsNone) ? this.time.Value : 1f));
			hashtable.Add("delay", (!this.delay.IsNone) ? this.delay.Value : 0f);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", this.itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", this.itweenID);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			hashtable.Add("islocal", this.space == Space.Self);
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("axis", (this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty);
			if (!this.orientToPath.IsNone)
			{
				hashtable.Add("orienttopath", this.orientToPath.Value);
			}
			if (!this.lookAtObject.IsNone)
			{
				hashtable.Add("looktarget", (!this.lookAtVector.IsNone) ? (this.lookAtObject.Value.transform.position + this.lookAtVector.Value) : this.lookAtObject.Value.transform.position);
			}
			else if (!this.lookAtVector.IsNone)
			{
				hashtable.Add("looktarget", this.lookAtVector.Value);
			}
			if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
			{
				hashtable.Add("looktime", (!this.lookTime.IsNone) ? this.lookTime.Value : 0f);
			}
			this.itweenType = "move";
			iTween.MoveFrom(ownerDefaultTarget, hashtable);
		}
	}
}
