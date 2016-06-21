using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Tests if a GameObject is a Child of another GameObject.")]
	public class GameObjectIsChildOf : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("GameObject to test.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Is it a child of this GameObject?")]
		public FsmGameObject isChildOf;

		[HutongGames.PlayMaker.Tooltip("Event to send if GameObject is a child.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if GameObject is NOT a child.")]
		public FsmEvent falseEvent;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store result in a bool variable"), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			this.gameObject = null;
			this.isChildOf = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			this.DoIsChildOf(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		private void DoIsChildOf(GameObject go)
		{
			if (go == null || this.isChildOf == null)
			{
				return;
			}
			bool flag = go.transform.IsChildOf(this.isChildOf.Value.transform);
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}
	}
}
