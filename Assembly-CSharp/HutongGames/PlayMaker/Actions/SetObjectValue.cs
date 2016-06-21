using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject), Tooltip("Sets the value of an Object Variable.")]
	public class SetObjectValue : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmObject objectVariable;

		[RequiredField]
		public FsmObject objectValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.objectVariable = null;
			this.objectValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.objectVariable.Value = this.objectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.objectVariable.Value = this.objectValue.Value;
		}
	}
}
