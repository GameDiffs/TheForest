using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	public class iTweenRotateUpdate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Rotate to a transform rotation.")]
		public FsmGameObject transformRotation;

		[HutongGames.PlayMaker.Tooltip("A rotation the GameObject will animate from.")]
		public FsmVector3 vectorRotation;

		[HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete. If transformRotation is set, this is used as an offset.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
		public Space space;

		private Hashtable hash;

		private GameObject go;

		public override void Reset()
		{
			this.transformRotation = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorRotation = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.space = Space.World;
		}

		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", (!this.vectorRotation.IsNone) ? this.vectorRotation.Value : Vector3.zero);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == Space.World)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.hash.Add("time", (!this.time.IsNone) ? this.time.Value : 1f);
			this.hash.Add("islocal", this.space == Space.Self);
			this.DoiTween();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			this.hash.Remove("rotation");
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", (!this.vectorRotation.IsNone) ? this.vectorRotation.Value : Vector3.zero);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == Space.World)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.DoiTween();
		}

		private void DoiTween()
		{
			iTween.RotateUpdate(this.go, this.hash);
		}
	}
}
