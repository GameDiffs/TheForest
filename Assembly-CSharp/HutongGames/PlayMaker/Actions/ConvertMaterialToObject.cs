using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts a Material variable to an Object variable. Useful if you want to use Set Property (which only works on Object variables).")]
	public class ConvertMaterialToObject : FsmStateAction
	{
		[RequiredField, Tooltip("The Material variable to convert to an Object."), UIHint(UIHint.Variable)]
		public FsmMaterial materialVariable;

		[RequiredField, Tooltip("Store the result in an Object variable."), UIHint(UIHint.Variable)]
		public FsmObject objectVariable;

		[Tooltip("Repeat every frame. Useful if the Material variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.materialVariable = null;
			this.objectVariable = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertMaterialToObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertMaterialToObject();
		}

		private void DoConvertMaterialToObject()
		{
			this.objectVariable.Value = this.materialVariable.Value;
		}
	}
}
