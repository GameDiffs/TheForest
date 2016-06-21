using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets the visibility of a GameObject. Note: this action sets the GameObject Renderer's enabled state.")]
	public class SetVisibility : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Should the object visibility be toggled?\nHas priority over the 'visible' setting")]
		public FsmBool toggle;

		[HutongGames.PlayMaker.Tooltip("Should the object be set to visible or invisible?")]
		public FsmBool visible;

		[HutongGames.PlayMaker.Tooltip("Resets to the initial visibility when it leaves the state")]
		public bool resetOnExit;

		private bool initialVisibility;

		public override void Reset()
		{
			this.gameObject = null;
			this.toggle = false;
			this.visible = false;
			this.resetOnExit = true;
			this.initialVisibility = false;
		}

		public override void OnEnter()
		{
			this.DoSetVisibility(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		private void DoSetVisibility(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			this.initialVisibility = base.renderer.enabled;
			if (!this.toggle.Value)
			{
				base.renderer.enabled = this.visible.Value;
				return;
			}
			base.renderer.enabled = !base.renderer.enabled;
		}

		public override void OnExit()
		{
			if (this.resetOnExit)
			{
				this.ResetVisibility();
			}
		}

		private void ResetVisibility()
		{
			if (base.renderer != null)
			{
				base.renderer.enabled = this.initialVisibility;
			}
		}
	}
}
