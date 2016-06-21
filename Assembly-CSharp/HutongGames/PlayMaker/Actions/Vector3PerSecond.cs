using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Multiplies a Vector3 variable by Time.deltaTime. Useful for frame rate independent motion.")]
	public class Vector3PerSecond : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		public bool everyFrame;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
		}
	}
}
