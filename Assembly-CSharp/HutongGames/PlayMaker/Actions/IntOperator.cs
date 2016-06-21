using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class IntOperator : FsmStateAction
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

		[RequiredField]
		public FsmInt integer1;

		[RequiredField]
		public FsmInt integer2;

		public IntOperator.Operation operation;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.integer1 = null;
			this.integer2 = null;
			this.operation = IntOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoIntOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoIntOperator();
		}

		private void DoIntOperator()
		{
			int value = this.integer1.Value;
			int value2 = this.integer2.Value;
			switch (this.operation)
			{
			case IntOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				break;
			case IntOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				break;
			case IntOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				break;
			case IntOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				break;
			case IntOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				break;
			case IntOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				break;
			}
		}
	}
}
