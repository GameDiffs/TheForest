using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopDynamicPickUp : CoopBase<IDynamicPickup>
{
	[SerializeField]
	private MonoBehaviour[] disableOnProxies = new MonoBehaviour[0];

	public float destroyAfter = 600f;

	public bool disablePhysics = true;

	public override void Attached()
	{
		this.MultiplayerPriority = 1f;
		base.state.Transform.SetTransforms(base.transform);
		if (this.entity.isOwner)
		{
			if (this.destroyAfter > 0f)
			{
				base.StartCoroutine(this.DestroyIn(this.destroyAfter));
			}
		}
		else
		{
			if (this.disablePhysics)
			{
				Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Collider collider = componentsInChildren[i];
					if (!collider.isTrigger)
					{
						UnityEngine.Object.Destroy(collider);
					}
				}
				Rigidbody[] componentsInChildren2 = base.GetComponentsInChildren<Rigidbody>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					Rigidbody rigidbody = componentsInChildren2[j];
					if (!rigidbody.isKinematic)
					{
						UnityEngine.Object.Destroy(rigidbody);
					}
				}
			}
			for (int k = 0; k < this.disableOnProxies.Length; k++)
			{
				if (this.disableOnProxies[k])
				{
					this.disableOnProxies[k].enabled = false;
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator DestroyIn(float seconds)
	{
		CoopDynamicPickUp.<DestroyIn>c__Iterator19 <DestroyIn>c__Iterator = new CoopDynamicPickUp.<DestroyIn>c__Iterator19();
		<DestroyIn>c__Iterator.seconds = seconds;
		<DestroyIn>c__Iterator.<$>seconds = seconds;
		<DestroyIn>c__Iterator.<>f__this = this;
		return <DestroyIn>c__Iterator;
	}
}
