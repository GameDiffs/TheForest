using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveColor : CurveFsmAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmColor colorVariable;

		[RequiredField]
		public FsmColor fromValue;

		[RequiredField]
		public FsmColor toValue;

		[RequiredField]
		public FsmAnimationCurve curveR;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
		public CurveFsmAction.Calculation calculationR;

		[RequiredField]
		public FsmAnimationCurve curveG;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
		public CurveFsmAction.Calculation calculationG;

		[RequiredField]
		public FsmAnimationCurve curveB;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
		public CurveFsmAction.Calculation calculationB;

		[RequiredField]
		public FsmAnimationCurve curveA;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
		public CurveFsmAction.Calculation calculationA;

		private Color clr;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
			this.toValue = new FsmColor
			{
				UseVariable = true
			};
			this.fromValue = new FsmColor
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
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value.r : 0f);
			this.fromFloats[1] = ((!this.fromValue.IsNone) ? this.fromValue.Value.g : 0f);
			this.fromFloats[2] = ((!this.fromValue.IsNone) ? this.fromValue.Value.b : 0f);
			this.fromFloats[3] = ((!this.fromValue.IsNone) ? this.fromValue.Value.a : 0f);
			this.toFloats = new float[4];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value.r : 0f);
			this.toFloats[1] = ((!this.toValue.IsNone) ? this.toValue.Value.g : 0f);
			this.toFloats[2] = ((!this.toValue.IsNone) ? this.toValue.Value.b : 0f);
			this.toFloats[3] = ((!this.toValue.IsNone) ? this.toValue.Value.a : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[2] = this.calculationA;
			base.Init();
		}

		public override void OnExit()
		{
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
