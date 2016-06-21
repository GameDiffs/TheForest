using System;
using TheForest.Utils;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
	public class GetButton : FsmStateAction
	{
		[RequiredField, Tooltip("The name of the button. Set in the Unity Input Manager.")]
		public FsmString buttonName;

		[RequiredField, Tooltip("Store the result in a bool variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.storeResult = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoGetButton();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetButton();
		}

		private void DoGetButton()
		{
			this.storeResult.Value = Input.GetButton(this.buttonName.Value);
		}
	}
}
