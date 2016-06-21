using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ropeSetGroundHeight : MonoBehaviour
{
	public LayerMask floorLayers;

	public GameObject triggerBottom;

	public bool disableGroundHeightCheck;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		ropeSetGroundHeight.<Start>c__IteratorF1 <Start>c__IteratorF = new ropeSetGroundHeight.<Start>c__IteratorF1();
		<Start>c__IteratorF.<>f__this = this;
		return <Start>c__IteratorF;
	}

	private void setGroundTriggerHeight()
	{
		if (this.floorLayers == 0)
		{
			Vector3 position = this.triggerBottom.transform.position;
			position.y = Terrain.activeTerrain.SampleHeight(this.triggerBottom.transform.position) + Terrain.activeTerrain.transform.position.y + 3.5f;
			this.triggerBottom.transform.position = position;
		}
		else
		{
			Vector3 position2 = this.triggerBottom.transform.position;
			position2.y = base.transform.position.y;
			RaycastHit raycastHit;
			if (Physics.Raycast(position2, Vector3.down, out raycastHit, 16.5f, this.floorLayers))
			{
				position2.y = raycastHit.point.y + 3.5f;
				this.triggerBottom.transform.position = position2;
			}
		}
	}
}
