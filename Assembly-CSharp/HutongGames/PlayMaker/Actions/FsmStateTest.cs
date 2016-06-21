using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Tests if an FSM is in the specified State.")]
	public class FsmStateTest : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject."), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Check to see if the FSM is in this state.")]
		public FsmString stateName;

		[HutongGames.PlayMaker.Tooltip("Event to send if the FSM is in the specified state.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if the FSM is NOT in the specified state.")]
		public FsmEvent falseEvent;

		[HutongGames.PlayMaker.Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a particular state.")]
		public bool everyFrame;

		private GameObject previousGo;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.stateName = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoFsmStateTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFsmStateTest();
		}

		private void DoFsmStateTest()
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
			bool value2 = false;
			if (this.fsm.ActiveStateName == this.stateName.Value)
			{
				base.Fsm.Event(this.trueEvent);
				value2 = true;
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			this.storeResult.Value = value2;
		}
	}
}
