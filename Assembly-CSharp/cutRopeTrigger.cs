using System;
using TheForest.Utils;
using UnityEngine;

public class cutRopeTrigger : MonoBehaviour
{
	public trapTrigger trap;

	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (!Scene.SceneTracker.allTrapTriggers.Contains(base.gameObject))
		{
			Scene.SceneTracker.allTrapTriggers.Add(base.gameObject);
		}
	}

	private void OnDisable()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (Scene.SceneTracker.allTrapTriggers.Contains(base.gameObject))
		{
			Scene.SceneTracker.allTrapTriggers.Remove(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Weapon"))
		{
			this.CutRope();
		}
	}

	private void CutRope()
	{
		Scene.SceneTracker.allTrapTriggers.Remove(base.gameObject);
		if (BoltNetwork.isRunning)
		{
			this.trap.releaseNooseTrapMP();
		}
		else
		{
			this.trap.releaseNooseTrap();
		}
		base.gameObject.SetActive(false);
	}
}
