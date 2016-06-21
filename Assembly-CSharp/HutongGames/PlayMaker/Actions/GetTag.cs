using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), Tooltip("Gets a Game Object's Tag and stores it in a String Variable.")]
	public class GetTag : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject gameObject;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetTag();
		}

		private void DoGetTag()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.tag;
		}
	}
}
