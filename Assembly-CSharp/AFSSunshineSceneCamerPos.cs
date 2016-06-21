using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class AFSSunshineSceneCamerPos : MonoBehaviour
{
	public Color SunShine_Sun_Col;

	private void OnPreCull()
	{
		Shader.SetGlobalVector("_AFSSceneCameraPos", base.transform.position);
		if (Sunshine.Instance)
		{
			Shader.SetGlobalVector("_AFSSunshineBillboardLightDirection", Sunshine.Instance.SunLight.transform.forward);
			Shader.SetGlobalVector("_SunShineScatterColor", Sunshine.Instance.ScatterColor.linear);
			float scatterExaggeration = Sunshine.Instance.ScatterExaggeration;
			float value = 1f / (Mathf.Clamp01(scatterExaggeration) * Sunshine.Instance.LightDistance / base.GetComponent<Camera>().farClipPlane);
			Shader.SetGlobalFloat("_SunShineDustVolumeScale", value);
			Shader.SetGlobalFloat("_SunShineScatterIntensity", Sunshine.Instance.ScatterIntensity);
			this.SunShine_Sun_Col = Sunshine.Instance.SunLight.color.linear * Sunshine.Instance.SunLight.intensity;
			Shader.SetGlobalVector("_SunShineSunLight", this.SunShine_Sun_Col);
			Shader.SetGlobalVector("_SunShineSunDir", Sunshine.Instance.SunLight.transform.forward * -1f);
		}
	}
}
