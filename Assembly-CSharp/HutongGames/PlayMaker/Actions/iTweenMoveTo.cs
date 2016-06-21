using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Changes a GameObject's position over time to a supplied destination.")]
	public class iTweenMoveTo : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[HutongGames.PlayMaker.Tooltip("Move To a transform position.")]
		public FsmGameObject transformPosition;

		[HutongGames.PlayMaker.Tooltip("Position the GameObject will animate to. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
		public Space space;

		[HutongGames.PlayMaker.Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		[ActionSection("LookAt"), HutongGames.PlayMaker.Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[HutongGames.PlayMaker.Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the object will take to look at either the Look Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		[HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;

		[ActionSection("Path"), HutongGames.PlayMaker.Tooltip("Whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.")]
		public FsmBool moveToPath;

		[HutongGames.PlayMaker.Tooltip("How much of a percentage (from 0 to 1) to look ahead on a path to influence how strict Orient To Path is and how much the object will anticipate each curve.")]
		public FsmFloat lookAhead;

		[CompoundArray("Path Nodes", "Transform", "Vector"), HutongGames.PlayMaker.Tooltip("A list of objects to draw a Catmull-Rom spline through for a curved animation path.")]
		public FsmGameObject[] transforms;

		[HutongGames.PlayMaker.Tooltip("A list of positions to draw a Catmull-Rom through for a curved animation path. If Transform is defined, this value is added as a local offset.")]
		public FsmVector3[] vectors;

		[HutongGames.PlayMaker.Tooltip("Reverse the path so object moves from End to Start node.")]
		public FsmBool reverse;

		private Vector3[] tempVct3;

		public override void OnDrawGizmos()
		{
			if (this.transforms.Length >= 2)
			{
				this.tempVct3 = new Vector3[this.transforms.Length];
				for (int i = 0; i < this.transforms.Length; i++)
				{
					if (this.transforms[i].IsNone)
					{
						this.tempVct3[i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
					else if (this.transforms[i].Value == null)
					{
						this.tempVct3[i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
					else
					{
						this.tempVct3[i] = this.transforms[i].Value.transform.position + ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
				}
				iTween.DrawPathGizmos(this.tempVct3, Color.yellow);
			}
		}

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
			this.lookTime = new FsmFloat
			{
				UseVariable = true
			};
			this.moveToPath = true;
			this.lookAhead = new FsmFloat
			{
				UseVariable = true
			};
			this.transforms = new FsmGameObject[0];
			this.vectors = new FsmVector3[0];
			this.tempVct3 = new Vector3[0];
			this.axis = iTweenFsmAction.AxisRestriction.none;
			this.reverse = false;
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
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("islocal", this.space == Space.Self);
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
			if (this.transforms.Length >= 2)
			{
				this.tempVct3 = new Vector3[this.transforms.Length];
				if (!this.reverse.IsNone && this.reverse.Value)
				{
					for (int i = 0; i < this.transforms.Length; i++)
					{
						if (this.transforms[i].IsNone)
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
						else if (this.transforms[i].Value == null)
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
						else
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((this.space != Space.World) ? this.transforms[i].Value.transform.localPosition : this.transforms[i].Value.transform.position) + ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
					}
				}
				else
				{
					for (int j = 0; j < this.transforms.Length; j++)
					{
						if (this.transforms[j].IsNone)
						{
							this.tempVct3[j] = ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
						else if (this.transforms[j].Value == null)
						{
							this.tempVct3[j] = ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
						else
						{
							this.tempVct3[j] = ((this.space != Space.World) ? ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transforms[j].Value.transform.position) : this.transforms[j].Value.transform.position) + ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
					}
				}
				hashtable.Add("path", this.tempVct3);
				hashtable.Add("movetopath", this.moveToPath.IsNone || this.moveToPath.Value);
				hashtable.Add("lookahead", (!this.lookAhead.IsNone) ? this.lookAhead.Value : 1f);
			}
			this.itweenType = "move";
			iTween.MoveTo(ownerDefaultTarget, hashtable);
		}
	}
}
