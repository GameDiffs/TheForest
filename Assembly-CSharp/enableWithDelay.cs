using System;
using UnityEngine;

public class enableWithDelay : MonoBehaviour
{
	public GameObject go;

	public float delay;

	private void Start()
	{
		base.Invoke("enableThisGo", this.delay);
	}

	private void enableThisGo()
	{
		if (this.go)
		{
			this.go.SetActive(true);
		}
	}
}
