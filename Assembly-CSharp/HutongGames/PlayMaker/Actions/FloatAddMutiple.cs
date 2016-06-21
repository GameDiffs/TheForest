using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), Tooltip("Adds multipe float variables to float variable.")]
	public class FloatAddMutiple : FsmStateAction
	{
		[Tooltip("The float variables to add."), UIHint(UIHint.Variable)]
		public FsmFloat[] floatVariables;

		[RequiredField, Tooltip("Add to this variable."), UIHint(UIHint.Variable)]
		public FsmFloat addTo;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariables = null;
			this.addTo = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFloatAdd();
		}

		private void DoFloatAdd()
		{
			for (int i = 0; i < this.floatVariables.Length; i++)
			{
				this.addTo.Value += this.floatVariables[i].Value;
			}
		}
	}
}
