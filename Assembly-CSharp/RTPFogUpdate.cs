using System;
using UnityEngine;

[ExecuteInEditMode]
public class RTPFogUpdate : MonoBehaviour
{
	public bool UpdateOnEveryFrame = true;

	public bool LinearColorSpace;

	private bool prev_LinearColorSpace;

	private void Start()
	{
	}

	private void Update()
	{
		if (this.UpdateOnEveryFrame)
		{
			RTPFogUpdate.Refresh(this.LinearColorSpace);
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
	}

	private void RefreshAll()
	{
		ReliefTerrain reliefTerrain = (ReliefTerrain)UnityEngine.Object.FindObjectOfType(typeof(ReliefTerrain));
		if (reliefTerrain != null && reliefTerrain.globalSettingsHolder != null)
		{
			reliefTerrain.globalSettingsHolder.RefreshAll();
		}
	}

	public static void Refresh(bool _LinearColorSpace = false)
	{
		if (RenderSettings.fog)
		{
			Shader.SetGlobalFloat("_Fdensity", RenderSettings.fogDensity);
			if (_LinearColorSpace)
			{
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor.linear);
			}
			else
			{
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor);
			}
			Shader.SetGlobalFloat("_Fstart", RenderSettings.fogStartDistance);
			Shader.SetGlobalFloat("_Fend", RenderSettings.fogEndDistance);
		}
		else
		{
			Shader.SetGlobalFloat("_Fdensity", 0f);
			Shader.SetGlobalFloat("_Fstart", 1000000f);
			Shader.SetGlobalFloat("_Fend", 2000000f);
		}
		Shader.SetGlobalColor("RTP_ambLight", RenderSettings.ambientLight);
	}
}
