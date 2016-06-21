using System;
using UnityEngine;

public class showOnEnable : MonoBehaviour
{
	public bool invert;

	public GameObject go;

	private void Awake()
	{
		if (this.go)
		{
			this.go.SetActive(this.invert);
		}
	}

	private void OnEnable()
	{
		if (this.go)
		{
			this.go.SetActive(!this.invert);
		}
	}

	private void OnDisable()
	{
		if (this.go)
		{
			this.go.SetActive(this.invert);
		}
	}
}
