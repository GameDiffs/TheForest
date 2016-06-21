using System;
using UnityEngine;

public class CaveShadow : MonoBehaviour
{
	public GameObject CaveShadowGo;

	private void Update()
	{
		if (Clock.InCave)
		{
			this.CaveShadowGo.SetActive(true);
		}
		else
		{
			this.CaveShadowGo.SetActive(false);
		}
	}
}
