using System;
using UnityEngine;

[ExecuteInEditMode]
public class EnvironmentGetIBL : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (SSSSS.Instance)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetFloat("Intensity", SSSSS.Instance.IBL_Debug);
		}
	}
}
