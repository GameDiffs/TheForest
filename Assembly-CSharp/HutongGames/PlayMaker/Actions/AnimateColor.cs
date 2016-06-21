using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
	public class AnimateColor : AnimateFsmAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmColor colorVariable;

		[RequiredField]
		public FsmAnimationCurve curveR;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.r.")]
		public AnimateFsmAction.Calculation calculationR;

		[RequiredField]
		public FsmAnimationCurve curveG;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.g.")]
		public AnimateFsmAction.Calculation calculationG;

		[RequiredField]
		public FsmAnimationCurve curveB;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.b.")]
		public AnimateFsmAction.Calculation calculationB;

		[RequiredField]
		public FsmAnimationCurve curveA;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.a.")]
		public AnimateFsmAction.Calculation calculationA;

		private bool finishInNextStep;

		private Color clr;

		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.r : 0f);
			this.fromFloats[1] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.g : 0f);
			this.fromFloats[2] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.b : 0f);
			this.fromFloats[3] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.a : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new AnimateFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[3] = this.calculationA;
			base.Init();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.colorVariable.Value = this.clr;
			}
			if (this.finishInNextStep && !this.looping)
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
					this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.colorVariable.Value = this.clr;
				}
				this.finishInNextStep = true;
			}
		}
	}
}
