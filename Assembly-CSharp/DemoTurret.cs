using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DemoTurret : MonoBehaviour
{
	public SmoothLookAtConstraint turretY;

	public SmoothLookAtConstraint turretX;

	private void Awake()
	{
		FireController component = base.GetComponent<FireController>();
		component.AddOnTargetUpdateDelegate(new FireController.OnTargetUpdateDelegate(this.OnTargetUpdateDel));
		component.AddOnIdleUpdateDelegate(new FireController.OnIdleUpdateDelegate(this.OnTargetIdleUpdateDel));
	}

	private void OnTargetUpdateDel(List<Target> targets)
	{
		this.turretX.target = targets[0].transform;
		this.turretY.target = targets[0].transform;
	}

	private void OnTargetIdleUpdateDel()
	{
		this.turretX.target = null;
		this.turretY.target = null;
	}
}
