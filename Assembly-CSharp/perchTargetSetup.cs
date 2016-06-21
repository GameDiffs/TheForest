using System;
using UnityEngine;

public class perchTargetSetup : MonoBehaviour
{
	private void OnEnable()
	{
		this.resetPerchTarget();
	}

	private void OnDisable()
	{
		base.CancelInvoke("resetPerchTarget");
	}

	private void disableThisTarget()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.tag = "Untagged";
			base.gameObject.layer = 31;
			base.Invoke("resetPerchTarget", 60f);
		}
	}

	private void resetPerchTarget()
	{
		base.gameObject.tag = "lb_perchTarget";
		base.gameObject.layer = 13;
	}
}
