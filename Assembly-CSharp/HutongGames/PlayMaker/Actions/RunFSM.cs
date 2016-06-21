using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Creates an FSM from a saved FSM Template.")]
	public class RunFSM : FsmStateAction
	{
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

		[UIHint(UIHint.Variable)]
		public FsmInt storeID;

		[HutongGames.PlayMaker.Tooltip("Event to send when the FSM has finished (usually because it ran a Finish FSM action).")]
		public FsmEvent finishEvent;

		private Fsm runFsm;

		public override void Reset()
		{
			this.fsmTemplateControl = new FsmTemplateControl();
			this.storeID = null;
			this.runFsm = null;
		}

		public override void Awake()
		{
			if (this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				this.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
			}
		}

		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.runFsm != null && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
			{
				this.runFsm.Event(fsmEvent);
			}
			return false;
		}

		public override void OnEnter()
		{
			if (this.runFsm == null)
			{
				base.Finish();
				return;
			}
			this.fsmTemplateControl.UpdateValues();
			this.fsmTemplateControl.ApplyOverrides(this.runFsm);
			this.runFsm.OnEnable();
			if (!this.runFsm.Started)
			{
				this.runFsm.Start();
			}
			this.storeID.Value = this.fsmTemplateControl.ID;
			this.CheckIfFinished();
		}

		public override void OnUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Update();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.FixedUpdate();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		public override void OnLateUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.LateUpdate();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		public override void DoTriggerEnter(Collider other)
		{
			if (this.runFsm.HandleTriggerEnter)
			{
				this.runFsm.OnTriggerEnter(other);
			}
		}

		public override void DoTriggerStay(Collider other)
		{
			if (this.runFsm.HandleTriggerStay)
			{
				this.runFsm.OnTriggerStay(other);
			}
		}

		public override void DoTriggerExit(Collider other)
		{
			if (this.runFsm.HandleTriggerExit)
			{
				this.runFsm.OnTriggerExit(other);
			}
		}

		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionEnter)
			{
				this.runFsm.OnCollisionEnter(collisionInfo);
			}
		}

		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionStay)
			{
				this.runFsm.OnCollisionStay(collisionInfo);
			}
		}

		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionExit)
			{
				this.runFsm.OnCollisionExit(collisionInfo);
			}
		}

		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			this.runFsm.OnControllerColliderHit(collisionInfo);
		}

		public override void OnGUI()
		{
			if (this.runFsm != null && this.runFsm.HandleOnGUI)
			{
				this.runFsm.OnGUI();
			}
		}

		public override void OnExit()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Stop();
			}
		}

		private void CheckIfFinished()
		{
			if (this.runFsm == null || this.runFsm.Finished)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}
	}
}
