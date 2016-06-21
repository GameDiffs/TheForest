using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Adds a Component to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
	public class AddComponent : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to add the Component to.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, Title("Component Type"), HutongGames.PlayMaker.Tooltip("The type of Component to add to the Game Object."), UIHint(UIHint.ScriptComponent)]
		public FsmString component;

		[ObjectType(typeof(Component)), HutongGames.PlayMaker.Tooltip("Store the component in an Object variable. E.g., to use with Set Property."), UIHint(UIHint.Variable)]
		public FsmObject storeComponent;

		[HutongGames.PlayMaker.Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		private Component addedComponent;

		public override void Reset()
		{
			this.gameObject = null;
			this.component = null;
			this.storeComponent = null;
		}

		public override void OnEnter()
		{
			this.DoAddComponent();
			base.Finish();
		}

		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				UnityEngine.Object.Destroy(this.addedComponent);
			}
		}

		private void DoAddComponent()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.addedComponent = ownerDefaultTarget.AddComponent(AddComponent.GetType(this.component.Value));
			this.storeComponent.Value = this.addedComponent;
			if (this.addedComponent == null)
			{
				this.LogError("Can't add component: " + this.component.Value);
			}
		}

		private static Type GetType(string name)
		{
			Type globalType = ReflectionUtils.GetGlobalType(name);
			if (globalType != null)
			{
				return globalType;
			}
			globalType = ReflectionUtils.GetGlobalType("UnityEngine." + name);
			if (globalType != null)
			{
				return globalType;
			}
			return ReflectionUtils.GetGlobalType("HutongGames.PlayMaker." + name);
		}
	}
}
