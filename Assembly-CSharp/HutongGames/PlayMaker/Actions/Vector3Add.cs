using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Adds a value to Vector3 Variable.")]
	public class Vector3Add : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		[RequiredField]
		public FsmVector3 addVector;

		public bool everyFrame;

		public bool perSecond;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.addVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.perSecond = false;
		}

		public override void OnEnter()
		{
			this.DoVector3Add();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoVector3Add();
		}

		private void DoVector3Add()
		{
			if (this.perSecond)
			{
				this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value * Time.deltaTime;
			}
			else
			{
				this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value;
			}
		}
	}
}
