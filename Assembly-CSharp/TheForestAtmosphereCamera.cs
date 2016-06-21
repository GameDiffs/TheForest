using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class TheForestAtmosphereCamera : MonoBehaviour
{
	private void OnPreCull()
	{
		if (TheForestAtmosphere.Instance != null)
		{
			TheForestAtmosphere.Instance.UpdateShaderParameters(base.GetComponent<Camera>());
		}
	}
}
