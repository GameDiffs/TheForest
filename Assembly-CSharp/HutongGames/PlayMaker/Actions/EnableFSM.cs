using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Enables/Disables an FSM component on a GameObject.")]
	public class EnableFSM : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM component.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject."), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		[HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		private PlayMakerFSM fsmComponent;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.enable = true;
			this.resetOnExit = true;
		}

		public override void OnEnter()
		{
			this.DoEnableFSM();
			base.Finish();
		}

		private void DoEnableFSM()
		{
			GameObject gameObject = (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner;
			if (gameObject == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.fsmName.Value))
			{
				PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
				PlayMakerFSM[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					PlayMakerFSM playMakerFSM = array[i];
					if (playMakerFSM.FsmName == this.fsmName.Value)
					{
						this.fsmComponent = playMakerFSM;
						break;
					}
				}
			}
			else
			{
				this.fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
			}
			if (this.fsmComponent == null)
			{
				this.LogError("Missing FsmComponent!");
				return;
			}
			this.fsmComponent.enabled = this.enable.Value;
		}

		public override void OnExit()
		{
			if (this.fsmComponent == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.fsmComponent.enabled = !this.enable.Value;
			}
		}
	}
}
