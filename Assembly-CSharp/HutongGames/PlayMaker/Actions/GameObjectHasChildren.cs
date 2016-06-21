using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Tests if a GameObject has children.")]
	public class GameObjectHasChildren : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Event to send if the GameObject has children.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if the GameObject does not have children.")]
		public FsmEvent falseEvent;

		[HutongGames.PlayMaker.Tooltip("Store the result in a bool variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
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
			this.DoHasChildren();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoHasChildren();
		}

		private void DoHasChildren()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			bool flag = ownerDefaultTarget.transform.childCount > 0;
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}
	}
}
