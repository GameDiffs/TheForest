using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Tests if a Game Object is visible.")]
	public class GameObjectIsVisible : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is visible.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is NOT visible.")]
		public FsmEvent falseEvent;

		[HutongGames.PlayMaker.Tooltip("Store the result in a bool variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoIsVisible();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoIsVisible();
		}

		private void DoIsVisible()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isVisible = base.renderer.isVisible;
				this.storeResult.Value = isVisible;
				base.Fsm.Event((!isVisible) ? this.falseEvent : this.trueEvent);
			}
		}
	}
}
