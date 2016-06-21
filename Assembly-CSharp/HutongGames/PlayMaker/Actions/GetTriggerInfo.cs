using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last Trigger event and store in variables.")]
	public class GetTriggerInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObjectHit;

		[Tooltip("Useful for triggering different effects. Audio, particles..."), UIHint(UIHint.Variable)]
		public FsmString physicsMaterialName;

		public override void Reset()
		{
			this.gameObjectHit = null;
			this.physicsMaterialName = null;
		}

		private void StoreTriggerInfo()
		{
			if (base.Fsm.TriggerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.TriggerCollider.gameObject;
			this.physicsMaterialName.Value = base.Fsm.TriggerCollider.material.name;
		}

		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}
	}
}
