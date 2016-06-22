using System;
using UnityEngine;

[Serializable]
public class TreeControlPhysics : MonoBehaviour
{
	public override void OnBecameVisible()
	{
		this.GetComponent<Collider>().enabled = true;
	}

	public override void OnBecameInvisible()
	{
		this.GetComponent<Collider>().enabled = false;
	}

	public override void Main()
	{
	}
}
