using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class FloatOperator : FsmStateAction
	{
		public enum Operation
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			Min,
			Max
		}

		[RequiredField, HutongGames.PlayMaker.Tooltip("The first float.")]
		public FsmFloat float1;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The second float.")]
		public FsmFloat float2;

		[HutongGames.PlayMaker.Tooltip("The math operation to perform on the floats.")]
		public FloatOperator.Operation operation;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the result of the operation in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.float1 = null;
			this.float2 = null;
			this.operation = FloatOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoFloatOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFloatOperator();
		}

		private void DoFloatOperator()
		{
			float value = this.float1.Value;
			float value2 = this.float2.Value;
			switch (this.operation)
			{
			case FloatOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				break;
			case FloatOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				break;
			case FloatOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				break;
			case FloatOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				break;
			case FloatOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				break;
			case FloatOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				break;
			}
		}
	}
}
