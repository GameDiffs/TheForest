using System;
using UnityEngine;

public class CoopTreeFall : CoopBase<ITreeFallState>
{
	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Rigidbody rigidbody = componentsInChildren[i];
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
			}
			Collider[] componentsInChildren2 = base.GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Collider collider = componentsInChildren2[j];
				collider.enabled = false;
			}
		}
		base.state.Transform.SetTransforms(base.transform);
		base.state.AddCallback("CutTree", delegate
		{
			if (base.state.CutTree)
			{
				Transform transform = base.state.CutTree.transform;
				Transform transform2 = null;
				for (int k = 0; k < transform.childCount; k++)
				{
					Transform child = transform.GetChild(k);
					if (child.name == "Lower")
					{
						transform2 = child;
					}
					else
					{
						child.gameObject.SetActive(false);
					}
				}
				transform2.parent = null;
			}
		});
	}
}
