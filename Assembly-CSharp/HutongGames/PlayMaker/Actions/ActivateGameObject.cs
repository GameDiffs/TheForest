using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	public class ActivateGameObject : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		public FsmBool activate;

		[HutongGames.PlayMaker.Tooltip("Recursively activate/deactivate all children.")]
		public FsmBool recursive;

		[HutongGames.PlayMaker.Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		[HutongGames.PlayMaker.Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private GameObject activatedGameObject;

		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
			this.recursive = true;
			this.resetOnExit = false;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoActivateGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoActivateGameObject();
		}

		public override void OnExit()
		{
			if (this.activatedGameObject == null)
			{
				return;
			}
			if (this.resetOnExit)
			{
				if (this.recursive.Value)
				{
					this.SetActiveRecursively(this.activatedGameObject, !this.activate.Value);
				}
				else
				{
					this.activatedGameObject.SetActive(!this.activate.Value);
				}
			}
		}

		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.recursive.Value)
			{
				this.SetActiveRecursively(ownerDefaultTarget, this.activate.Value);
			}
			else
			{
				ownerDefaultTarget.SetActive(this.activate.Value);
			}
			this.activatedGameObject = ownerDefaultTarget;
		}

		public void SetActiveRecursively(GameObject go, bool state)
		{
			go.SetActive(state);
			foreach (Transform transform in go.transform)
			{
				this.SetActiveRecursively(transform.gameObject, state);
			}
		}
	}
}
