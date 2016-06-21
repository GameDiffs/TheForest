using System;
using UnityEngine;

public class CutUpEnemy : MonoBehaviour
{
	public GameObject RagDollExploded;

	public GameObject Top;

	private void Start()
	{
	}

	private void Hit()
	{
		UnityEngine.Object.Instantiate(this.RagDollExploded, base.transform.position, base.transform.rotation);
		UnityEngine.Object.Destroy(this.Top);
	}
}
