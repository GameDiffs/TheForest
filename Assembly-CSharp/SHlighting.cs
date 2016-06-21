using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SHlighting : MonoBehaviour
{
	public Renderer DummyRenderer;

	public SphericalHarmonicsL2 sampledProbe;

	public bool injectLight;

	private void LateUpdate()
	{
		if (LightmapSettings.lightProbes.count > 0)
		{
			LightProbes.GetInterpolatedProbe(this.DummyRenderer.transform.position, this.DummyRenderer, out this.sampledProbe);
			if (this.injectLight)
			{
				this.sampledProbe.AddDirectionalLight(new Vector3(1f, 0f, 0f), Color.red, 0.25f);
			}
			RenderSettings.ambientProbe = this.sampledProbe;
		}
	}
}
