using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Easing Animation - Color")]
	public class EaseColor : EaseFsmAction
	{
		[RequiredField]
		public FsmColor fromValue;

		[RequiredField]
		public FsmColor toValue;

		[UIHint(UIHint.Variable)]
		public FsmColor colorVariable;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			this.colorVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.r;
			this.fromFloats[1] = this.fromValue.Value.g;
			this.fromFloats[2] = this.fromValue.Value.b;
			this.fromFloats[3] = this.fromValue.Value.a;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.r;
			this.toFloats[1] = this.toValue.Value.g;
			this.toFloats[2] = this.toValue.Value.b;
			this.toFloats[3] = this.toValue.Value.a;
			this.resultFloats = new float[4];
			this.finishInNextStep = false;
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.colorVariable.Value = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
				if (!this.colorVariable.IsNone)
				{
					this.colorVariable.Value = new Color((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.r : this.fromValue.Value.r) : this.toValue.Value.r, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.g : this.fromValue.Value.g) : this.toValue.Value.g, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.b : this.fromValue.Value.b) : this.toValue.Value.b, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.a : this.fromValue.Value.a) : this.toValue.Value.a);
				}
				this.finishInNextStep = true;
			}
		}
	}
}
