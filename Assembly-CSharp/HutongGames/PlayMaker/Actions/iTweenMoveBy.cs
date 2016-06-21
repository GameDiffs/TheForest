using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Adds the supplied vector to a GameObject's position.")]
	public class iTweenMoveBy : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The vector to add to the GameObject's position.")]
		public FsmVector3 vector;

		[HutongGames.PlayMaker.Tooltip("For the time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("For the time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("For the shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[HutongGames.PlayMaker.Tooltip("For the type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		public Space space;

		[ActionSection("LookAt"), HutongGames.PlayMaker.Tooltip("For whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[HutongGames.PlayMaker.Tooltip("For a target the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[HutongGames.PlayMaker.Tooltip("For a target the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[HutongGames.PlayMaker.Tooltip("For the time in seconds the object will take to look at either the 'looktarget' or 'orienttopath'. 0 by default")]
		public FsmFloat lookTime;

		[HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
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
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.orientToPath = false;
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
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", (!this.vector.IsNone) ? this.vector.Value : Vector3.zero);
			hashtable.Add((!this.speed.IsNone) ? "speed" : "time", (!this.speed.IsNone) ? this.speed.Value : ((!this.time.IsNone) ? this.time.Value : 1f));
			hashtable.Add("delay", (!this.delay.IsNone) ? this.delay.Value : 0f);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", this.itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", this.itweenID);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			hashtable.Add("space", this.space);
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
			iTween.MoveBy(ownerDefaultTarget, hashtable);
		}
	}
}
