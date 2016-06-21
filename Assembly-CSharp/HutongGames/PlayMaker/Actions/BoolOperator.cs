using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), Tooltip("Performs boolean operations on 2 Bool Variables.")]
	public class BoolOperator : FsmStateAction
	{
		public enum Operation
		{
			AND,
			NAND,
			OR,
			XOR
		}

		[RequiredField, Tooltip("The first Bool variable.")]
		public FsmBool bool1;

		[RequiredField, Tooltip("The second Bool variable.")]
		public FsmBool bool2;

		[Tooltip("Boolean Operation.")]
		public BoolOperator.Operation operation;

		[RequiredField, Tooltip("Store the result in a Bool Variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.bool1 = false;
			this.bool2 = false;
			this.operation = BoolOperator.Operation.AND;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoBoolOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoBoolOperator();
		}

		private void DoBoolOperator()
		{
			bool value = this.bool1.Value;
			bool value2 = this.bool2.Value;
			switch (this.operation)
			{
			case BoolOperator.Operation.AND:
				this.storeResult.Value = (value && value2);
				break;
			case BoolOperator.Operation.NAND:
				this.storeResult.Value = (!value || !value2);
				break;
			case BoolOperator.Operation.OR:
				this.storeResult.Value = (value || value2);
				break;
			case BoolOperator.Operation.XOR:
				this.storeResult.Value = (value ^ value2);
				break;
			}
		}
	}
}
