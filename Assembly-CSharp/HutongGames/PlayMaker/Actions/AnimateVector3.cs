using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
	public class AnimateVector3 : AnimateFsmAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vectorVariable;

		[RequiredField]
		public FsmAnimationCurve curveX;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
		public AnimateFsmAction.Calculation calculationX;

		[RequiredField]
		public FsmAnimationCurve curveY;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
		public AnimateFsmAction.Calculation calculationY;

		[RequiredField]
		public FsmAnimationCurve curveZ;

		[HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
		public AnimateFsmAction.Calculation calculationZ;

		private bool finishInNextStep;

		private Vector3 vct;

		public override void Reset()
		{
			base.Reset();
			this.vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[3];
			this.fromFloats = new float[3];
			this.fromFloats[0] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.x : 0f);
			this.fromFloats[1] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.y : 0f);
			this.fromFloats[2] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.z : 0f);
			this.curves = new AnimationCurve[3];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveZ.curve;
			this.calculations = new AnimateFsmAction.Calculation[3];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationZ;
			base.Init();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vectorVariable.IsNone && this.isRunning)
			{
				this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				this.vectorVariable.Value = this.vct;
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
				if (!this.vectorVariable.IsNone)
				{
					this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
					this.vectorVariable.Value = this.vct;
				}
				this.finishInNextStep = true;
			}
		}
	}
}
