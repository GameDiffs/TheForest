using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Performs most possible operations on 2 Vector3: Dot product, Cross product, Distance, Angle, Project, Reflect, Add, Subtract, Multiply, Divide, Min, Max")]
	public class Vector3Operator : FsmStateAction
	{
		public enum Vector3Operation
		{
			DotProduct,
			CrossProduct,
			Distance,
			Angle,
			Project,
			Reflect,
			Add,
			Subtract,
			Multiply,
			Divide,
			Min,
			Max
		}

		[RequiredField]
		public FsmVector3 vector1;

		[RequiredField]
		public FsmVector3 vector2;

		public Vector3Operator.Vector3Operation operation = Vector3Operator.Vector3Operation.Add;

		[UIHint(UIHint.Variable)]
		public FsmVector3 storeVector3Result;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeFloatResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.vector1 = null;
			this.vector2 = null;
			this.operation = Vector3Operator.Vector3Operation.Add;
			this.storeVector3Result = null;
			this.storeFloatResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoVector3Operator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoVector3Operator();
		}

		private void DoVector3Operator()
		{
			Vector3 value = this.vector1.Value;
			Vector3 value2 = this.vector2.Value;
			switch (this.operation)
			{
			case Vector3Operator.Vector3Operation.DotProduct:
				this.storeFloatResult.Value = Vector3.Dot(value, value2);
				break;
			case Vector3Operator.Vector3Operation.CrossProduct:
				this.storeVector3Result.Value = Vector3.Cross(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Distance:
				this.storeFloatResult.Value = Vector3.Distance(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Angle:
				this.storeFloatResult.Value = Vector3.Angle(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Project:
				this.storeVector3Result.Value = Vector3.Project(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Reflect:
				this.storeVector3Result.Value = Vector3.Reflect(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Add:
				this.storeVector3Result.Value = value + value2;
				break;
			case Vector3Operator.Vector3Operation.Subtract:
				this.storeVector3Result.Value = value - value2;
				break;
			case Vector3Operator.Vector3Operation.Multiply:
			{
				Vector3 zero = Vector3.zero;
				zero.x = value.x * value2.x;
				zero.y = value.y * value2.y;
				zero.z = value.z * value2.z;
				this.storeVector3Result.Value = zero;
				break;
			}
			case Vector3Operator.Vector3Operation.Divide:
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = value.x / value2.x;
				zero2.y = value.y / value2.y;
				zero2.z = value.z / value2.z;
				this.storeVector3Result.Value = zero2;
				break;
			}
			case Vector3Operator.Vector3Operation.Min:
				this.storeVector3Result.Value = Vector3.Min(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Max:
				this.storeVector3Result.Value = Vector3.Max(value, value2);
				break;
			}
		}
	}
}
