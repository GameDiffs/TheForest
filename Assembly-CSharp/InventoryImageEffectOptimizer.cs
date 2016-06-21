using System;
using UnityEngine;

[ExecuteInEditMode]
public class InventoryImageEffectOptimizer : MonoBehaviour
{
	private Camera MyCamera;

	private SESSAO ssao;

	private void OnEnable()
	{
		this.MyCamera = base.gameObject.GetComponent<Camera>();
		this.ssao = base.GetComponent<SESSAO>();
	}

	private void Update()
	{
		TheForestQualitySettings.SSAOTechnique sSAOTechnique = TheForestQualitySettings.UserSettings.SSAO;
		if (SystemInfo.systemMemorySize <= 4096 || PlayerPreferences.LowMemoryMode)
		{
			sSAOTechnique = TheForestQualitySettings.SSAOTechnique.Off;
		}
		if (this.ssao)
		{
			this.ssao.enabled = (sSAOTechnique != TheForestQualitySettings.SSAOTechnique.Off);
			TheForestQualitySettings.SSAOTechnique sSAOTechnique2 = sSAOTechnique;
			if (sSAOTechnique2 != TheForestQualitySettings.SSAOTechnique.Ultra)
			{
				if (sSAOTechnique2 == TheForestQualitySettings.SSAOTechnique.High)
				{
					this.ssao.halfSampling = true;
				}
			}
			else
			{
				this.ssao.halfSampling = false;
			}
		}
	}
}
