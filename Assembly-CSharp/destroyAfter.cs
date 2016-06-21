using System;
using TheForest.Utils;
using UnityEngine;

public class destroyAfter : MonoBehaviour
{
	public float destroyTime;

	public float destroyDistance;

	private void Start()
	{
		if (this.destroyDistance > 0f)
		{
			base.InvokeRepeating("destroyMe", this.destroyTime, 10f);
		}
		else
		{
			base.Invoke("destroyMe", this.destroyTime);
		}
	}

	private void destroyMe()
	{
		if (BoltNetwork.isRunning)
		{
			BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
			if (componentInParent && componentInParent.isAttached && !componentInParent.IsOwner())
			{
				return;
			}
		}
		if (this.destroyDistance > 0f)
		{
			if (Vector3.Distance(LocalPlayer.Transform.position, base.transform.position) > this.destroyDistance && base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		base.CancelInvoke("destroyMe");
	}

	private void OnDisable()
	{
		base.CancelInvoke("destroyMe");
	}
}
