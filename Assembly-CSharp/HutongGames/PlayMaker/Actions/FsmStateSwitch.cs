using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Sends Events based on the current State of an FSM.")]
	public class FsmStateSwitch : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of Fsm on GameObject. Useful if there is more than one FSM on the GameObject."), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[CompoundArray("State Switches", "Compare State", "Send Event")]
		public FsmString[] compareTo;

		public FsmEvent[] sendEvent;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;

		private GameObject previousGo;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoFsmStateSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFsmStateSwitch();
		}

		private void DoFsmStateSwitch()
		{
			GameObject value = this.gameObject.Value;
			if (value == null)
			{
				return;
			}
			if (value != this.previousGo)
			{
				this.fsm = ActionHelpers.GetGameObjectFsm(value, this.fsmName.Value);
				this.previousGo = value;
			}
			if (this.fsm == null)
			{
				return;
			}
			string activeStateName = this.fsm.ActiveStateName;
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (activeStateName == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}
	}
}
