using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetChild : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The name of the child to search for.")]
		public FsmString childName;

		[HutongGames.PlayMaker.Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match."), UIHint(UIHint.Tag)]
		public FsmString withTag;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable."), UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			this.gameObject = null;
			this.childName = string.Empty;
			this.withTag = "Untagged";
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			this.storeResult.Value = GetChild.DoGetChildByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value);
			base.Finish();
		}

		private static GameObject DoGetChildByName(GameObject root, string name, string tag)
		{
			if (root == null)
			{
				return null;
			}
			foreach (Transform transform in root.transform)
			{
				if (!string.IsNullOrEmpty(name))
				{
					if (transform.name == name)
					{
						if (string.IsNullOrEmpty(tag))
						{
							GameObject result = transform.gameObject;
							return result;
						}
						if (transform.tag.Equals(tag))
						{
							GameObject result = transform.gameObject;
							return result;
						}
					}
				}
				else if (!string.IsNullOrEmpty(tag) && transform.CompareTag(tag))
				{
					GameObject result = transform.gameObject;
					return result;
				}
				GameObject gameObject = GetChild.DoGetChildByName(transform.gameObject, name, tag);
				if (gameObject != null)
				{
					GameObject result = gameObject;
					return result;
				}
			}
			return null;
		}

		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.childName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Child Name, Tag, or both.";
			}
			return null;
		}
	}
}
