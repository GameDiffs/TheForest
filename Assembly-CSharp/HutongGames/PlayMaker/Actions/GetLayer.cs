using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
	public class GetLayer : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject gameObject;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetLayer();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetLayer();
		}

		private void DoGetLayer()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.layer;
		}
	}
}
