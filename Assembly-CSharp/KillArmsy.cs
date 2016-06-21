using System;
using UnityEngine;

public class KillArmsy : MonoBehaviour
{
	public GameObject RagDoll;

	private void Explosion(float dist)
	{
		UnityEngine.Object.Instantiate(this.RagDoll, base.transform.parent.position, base.transform.parent.rotation);
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}
}
