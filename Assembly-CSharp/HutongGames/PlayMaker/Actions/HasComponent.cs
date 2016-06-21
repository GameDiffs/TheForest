using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Checks if an Object has a Component. Optionally remove the Component on exiting the state.")]
	public class HasComponent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField, UIHint(UIHint.ScriptComponent)]
		public FsmString component;

		public FsmBool removeOnExit;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool store;

		public bool everyFrame;

		private Component aComponent;

		public override void Reset()
		{
			this.aComponent = null;
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.component = null;
			this.store = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoHasComponent((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoHasComponent((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
		}

		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.aComponent != null)
			{
				UnityEngine.Object.Destroy(this.aComponent);
			}
		}

		private void DoHasComponent(GameObject go)
		{
			this.aComponent = go.GetComponent(this.component.Value);
			base.Fsm.Event((!(this.aComponent != null)) ? this.falseEvent : this.trueEvent);
		}
	}
}
