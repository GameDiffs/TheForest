using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Easing Animation - Vector3")]
	public class EaseVector3 : EaseFsmAction
	{
		[RequiredField]
		public FsmVector3 fromValue;

		[RequiredField]
		public FsmVector3 toValue;

		[UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			this.vector3Variable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vector3Variable.IsNone && this.isRunning)
			{
				this.vector3Variable.Value = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
			}
			if (this.finishInNextStep)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				if (!this.vector3Variable.IsNone)
				{
					this.vector3Variable.Value = new Vector3((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.x : this.fromValue.Value.x) : this.toValue.Value.x, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.y : this.fromValue.Value.y) : this.toValue.Value.y, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.z : this.fromValue.Value.z) : this.toValue.Value.z);
				}
				this.finishInNextStep = true;
			}
		}
	}
}
