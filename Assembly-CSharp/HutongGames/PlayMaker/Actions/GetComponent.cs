using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject), HutongGames.PlayMaker.Tooltip("Gets a Component attached to a GameObject and stores it in an Object variable. NOTE: Set the Object variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
	public class GetComponent : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The GameObject that owns the component.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera."), UIHint(UIHint.Variable)]
		public FsmObject storeComponent;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeComponent = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetComponent();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetComponent();
		}

		private void DoGetComponent()
		{
			if (this.storeComponent == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeComponent.Value = ownerDefaultTarget.GetComponent(this.storeComponent.ObjectType);
		}
	}
}
