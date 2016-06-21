using System;
using UnityEngine;

public class skullLampSetup : MonoBehaviour
{
	public Transform joint;

	public Transform newParent;

	private void Start()
	{
		this.joint.parent = this.newParent;
	}
}
