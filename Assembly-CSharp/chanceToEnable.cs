using System;
using UnityEngine;

public class chanceToEnable : MonoBehaviour
{
	public GameObject go;

	public float chance = 0.1f;

	private void Start()
	{
		if (UnityEngine.Random.value < this.chance && this.go)
		{
			this.go.SetActive(true);
		}
	}
}
