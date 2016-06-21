using System;
using UnityEngine;

public class TransformLerp : MonoBehaviour
{
	private bool slerpOn;

	public float fNetworkLerpTransformSpeed = 0.01f;

	private Vector3 vtNextPos;

	private Vector3 vtNextRot;

	public Vector3 VTNextPos
	{
		set
		{
			this.vtNextPos = value;
		}
	}

	public Vector3 VTNextkRot
	{
		set
		{
			this.vtNextRot = value;
		}
	}

	public void LerpProcessing()
	{
		if (this.slerpOn)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.vtNextPos, this.fNetworkLerpTransformSpeed);
			base.transform.rotation = Quaternion.Lerp(Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z), Quaternion.Euler(this.vtNextRot.x, this.vtNextRot.y, this.vtNextRot.z), this.fNetworkLerpTransformSpeed);
		}
		else
		{
			base.transform.position = this.vtNextPos;
			base.transform.rotation = Quaternion.Euler(this.vtNextRot);
		}
	}
}
