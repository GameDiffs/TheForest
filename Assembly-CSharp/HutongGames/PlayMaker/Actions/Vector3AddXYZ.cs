using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Adds a XYZ values to Vector3 Variable.")]
	public class Vector3AddXYZ : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		public FsmFloat addX;

		public FsmFloat addY;

		public FsmFloat addZ;

		public bool everyFrame;

		public bool perSecond;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.addX = 0f;
			this.addY = 0f;
			this.addZ = 0f;
			this.everyFrame = false;
			this.perSecond = false;
		}

		public override void OnEnter()
		{
			this.DoVector3AddXYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoVector3AddXYZ();
		}

		private void DoVector3AddXYZ()
		{
			Vector3 vector = new Vector3(this.addX.Value, this.addY.Value, this.addZ.Value);
			if (this.perSecond)
			{
				this.vector3Variable.Value += vector * Time.deltaTime;
			}
			else
			{
				this.vector3Variable.Value += vector;
			}
		}
	}
}
