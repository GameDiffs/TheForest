using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("AnimateVariables"), HutongGames.PlayMaker.Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveRect : CurveFsmAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmRect rectVariable;

		[RequiredField]
		public FsmRect fromValue;

		[RequiredField]
		public FsmRect toValue;

		[RequiredField]
		public FsmAnimationCurve curveX;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public CurveFsmAction.Calculation calculationX;

		[RequiredField]
		public FsmAnimationCurve curveY;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public CurveFsmAction.Calculation calculationY;

		[RequiredField]
		public FsmAnimationCurve curveW;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
		public CurveFsmAction.Calculation calculationW;

		[RequiredField]
		public FsmAnimationCurve curveH;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
		public CurveFsmAction.Calculation calculationH;

		private Rect rct;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			this.rectVariable = new FsmRect
			{
				UseVariable = true
			};
			this.toValue = new FsmRect
			{
				UseVariable = true
			};
			this.fromValue = new FsmRect
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
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value.x : 0f);
			this.fromFloats[1] = ((!this.fromValue.IsNone) ? this.fromValue.Value.y : 0f);
			this.fromFloats[2] = ((!this.fromValue.IsNone) ? this.fromValue.Value.width : 0f);
			this.fromFloats[3] = ((!this.fromValue.IsNone) ? this.fromValue.Value.height : 0f);
			this.toFloats = new float[4];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value.x : 0f);
			this.toFloats[1] = ((!this.toValue.IsNone) ? this.toValue.Value.y : 0f);
			this.toFloats[2] = ((!this.toValue.IsNone) ? this.toValue.Value.width : 0f);
			this.toFloats[3] = ((!this.toValue.IsNone) ? this.toValue.Value.height : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveW.curve;
			this.curves[3] = this.curveH.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationW;
			this.calculations[2] = this.calculationH;
			base.Init();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.rectVariable.Value = this.rct;
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
				if (!this.rectVariable.IsNone)
				{
					this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.rectVariable.Value = this.rct;
				}
				this.finishInNextStep = true;
			}
		}
	}
}
