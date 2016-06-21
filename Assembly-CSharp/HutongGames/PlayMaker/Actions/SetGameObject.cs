using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObject : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmGameObject variable;

		public FsmGameObject gameObject;

		public bool everyFrame;

		public override void Reset()
		{
			this.variable = null;
			this.gameObject = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.variable.Value = this.gameObject.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.variable.Value = this.gameObject.Value;
		}
	}
}
