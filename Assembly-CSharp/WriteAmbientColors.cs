using System;
using UnityEngine;

[ExecuteInEditMode]
public class WriteAmbientColors : MonoBehaviour
{
	private void Update()
	{
		Shader.SetGlobalColor("_AmbientSkyColor", RenderSettings.ambientSkyColor.linear * RenderSettings.ambientIntensity);
	}
}
