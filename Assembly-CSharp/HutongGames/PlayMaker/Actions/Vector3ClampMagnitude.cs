using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Clamps the Magnitude of Vector3 Variable.")]
	public class Vector3ClampMagnitude : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		[RequiredField]
		public FsmFloat maxLength;

		public bool everyFrame;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.maxLength = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoVector3ClampMagnitude();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoVector3ClampMagnitude();
		}

		private void DoVector3ClampMagnitude()
		{
			this.vector3Variable.Value = Vector3.ClampMagnitude(this.vector3Variable.Value, this.maxLength.Value);
		}
	}
}
