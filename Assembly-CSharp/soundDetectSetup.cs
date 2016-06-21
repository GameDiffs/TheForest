using System;
using UnityEngine;

public class soundDetectSetup : MonoBehaviour
{
	public void setRange(float range)
	{
		base.transform.GetComponent<SphereCollider>().radius = range;
	}
}
