using System;
using UnityEngine;

public class AmbientLightFake : MonoBehaviour
{
	private void Start()
	{
		base.InvokeRepeating("Refresh", 0f, 2f);
	}

	private void Refresh()
	{
		base.GetComponent<Light>().color = TheForestAtmosphere.Ambient;
	}
}
