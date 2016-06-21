using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
	public class SetTagsOnChildren : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("GameObject Parent")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Set Tag To..."), UIHint(UIHint.Tag)]
		public FsmString tag;

		[HutongGames.PlayMaker.Tooltip("Only set the Tag on children with this component."), UIHint(UIHint.ScriptComponent)]
		public FsmString filterByComponent;

		private Type componentFilter;

		public override void Reset()
		{
			this.gameObject = null;
			this.tag = null;
			this.filterByComponent = null;
		}

		public override void OnEnter()
		{
			this.SetTag(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		private void SetTag(GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.filterByComponent.Value))
			{
				foreach (Transform transform in parent.transform)
				{
					transform.gameObject.tag = this.tag.Value;
				}
			}
			else
			{
				this.UpdateComponentFilter();
				if (this.componentFilter != null)
				{
					Component[] componentsInChildren = parent.GetComponentsInChildren(this.componentFilter);
					Component[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						Component component = array[i];
						component.gameObject.tag = this.tag.Value;
					}
				}
			}
			base.Finish();
		}

		private void UpdateComponentFilter()
		{
			this.componentFilter = ReflectionUtils.GetGlobalType(this.filterByComponent.Value);
			if (this.componentFilter == null)
			{
				this.componentFilter = ReflectionUtils.GetGlobalType("UnityEngine." + this.filterByComponent.Value);
			}
			if (this.componentFilter == null)
			{
				Debug.LogWarning("Couldn't get type: " + this.filterByComponent.Value);
			}
		}
	}
}
