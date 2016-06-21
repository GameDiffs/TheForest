using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("AnimateVariables"), HutongGames.PlayMaker.Tooltip("Easing Animation - Rect.")]
	public class EaseRect : EaseFsmAction
	{
		[RequiredField]
		public FsmRect fromValue;

		[RequiredField]
		public FsmRect toValue;

		[UIHint(UIHint.Variable)]
		public FsmRect rectVariable;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			this.rectVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.width;
			this.fromFloats[3] = this.fromValue.Value.height;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.width;
			this.toFloats[3] = this.toValue.Value.height;
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
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rectVariable.Value = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
				if (!this.rectVariable.IsNone)
				{
					this.rectVariable.Value = new Rect((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.x : this.fromValue.Value.x) : this.toValue.Value.x, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.y : this.fromValue.Value.y) : this.toValue.Value.y, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.width : this.fromValue.Value.width) : this.toValue.Value.width, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.height : this.fromValue.Value.height) : this.toValue.Value.height);
				}
				this.finishInNextStep = true;
			}
		}
	}
}
