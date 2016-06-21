using Ceto;
using System;
using TheForest.Utils;
using UnityEngine;

namespace CetoTF
{
	public class OceanQualitySettings : MonoBehaviour
	{
		[Serializable]
		public class OceanSetting
		{
			public FOURIER_SIZE fourierSize;

			public MESH_RESOLUTION meshResolution;

			public bool spectrumFoamOn;

			public Material topSideMaterial;

			public Material underSideMaterial;
		}

		public static OceanQualitySettings Instance;

		public bool preCache = true;

		public bool usingSharedDepthGrab;

		public bool disableCaustics = true;

		public bool disableSpectrumOnTrigger = true;

		public bool disableUnderWaterOnTrigger = true;

		public MESH_RESOLUTION meshResolutionOnTrigger;

		public float whiteCapAmount = 1.6f;

		public float whiteCapFadeRate = 1f;

		private CETO_QUALITY_SETTING currentQualitySetting = CETO_QUALITY_SETTING.HIGH;

		public OceanQualitySettings.OceanSetting lowSetting;

		public OceanQualitySettings.OceanSetting highSetting;

		private OceanQualityTrigger[] m_triggers;

		private bool m_playerInTrigger;

		private bool m_beenCached;

		private void Awake()
		{
			OceanQualitySettings.Instance = this;
		}

		private void Start()
		{
			this.m_triggers = base.GetComponentsInChildren<OceanQualityTrigger>();
			this.QualityChanged(this.currentQualitySetting);
			this.SetCommandBuffers();
		}

		private void OnDestroy()
		{
			OceanQualitySettings.Instance = null;
		}

		private OceanQualitySettings.OceanSetting CurrentSetting()
		{
			CETO_QUALITY_SETTING cETO_QUALITY_SETTING = this.currentQualitySetting;
			if (cETO_QUALITY_SETTING == CETO_QUALITY_SETTING.LOW)
			{
				return this.lowSetting;
			}
			if (cETO_QUALITY_SETTING != CETO_QUALITY_SETTING.HIGH)
			{
				return this.highSetting;
			}
			return this.highSetting;
		}

		private void Update()
		{
			this.SetCommandBuffers();
			this.PreCache();
			if (this.m_triggers == null)
			{
				return;
			}
			this.m_playerInTrigger = false;
			if (LocalPlayer.Transform)
			{
				int num = this.m_triggers.Length;
				for (int i = 0; i < num; i++)
				{
					if (this.m_triggers[i].Contains(LocalPlayer.Transform.position))
					{
						this.m_playerInTrigger = true;
					}
				}
			}
			this.TriggerChanged();
		}

		private void TriggerChanged()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			WaveSpectrum waveSpectrum = Ocean.Instance.Spectrum as WaveSpectrum;
			UnderWater underWater = Ocean.Instance.UnderWater as UnderWater;
			ProjectedGrid projectedGrid = Ocean.Instance.Grid as ProjectedGrid;
			OceanQualitySettings.OceanSetting oceanSetting = this.CurrentSetting();
			if (waveSpectrum != null)
			{
				if (this.m_playerInTrigger)
				{
					waveSpectrum.foamAmount -= Time.deltaTime * this.whiteCapFadeRate;
					waveSpectrum.foamAmount = Mathf.Max(0f, waveSpectrum.foamAmount);
					if (this.disableSpectrumOnTrigger)
					{
						waveSpectrum.disableDisplacements = true;
						waveSpectrum.disableSlopes = true;
						if (waveSpectrum.foamAmount <= 0f)
						{
							waveSpectrum.disableFoam = true;
						}
					}
				}
				else
				{
					waveSpectrum.foamAmount += Time.deltaTime * this.whiteCapFadeRate;
					waveSpectrum.foamAmount = Mathf.Min(this.whiteCapAmount, waveSpectrum.foamAmount);
					waveSpectrum.disableDisplacements = false;
					waveSpectrum.disableSlopes = false;
					waveSpectrum.disableFoam = !oceanSetting.spectrumFoamOn;
				}
			}
			if (underWater != null)
			{
				if (this.m_playerInTrigger)
				{
					if (this.disableUnderWaterOnTrigger)
					{
						underWater.enabled = false;
					}
				}
				else
				{
					underWater.enabled = true;
				}
			}
			if (projectedGrid != null)
			{
				if (this.m_playerInTrigger)
				{
					projectedGrid.resolution = this.meshResolutionOnTrigger;
				}
				else
				{
					projectedGrid.resolution = oceanSetting.meshResolution;
				}
			}
		}

		public void QualityChanged(CETO_QUALITY_SETTING newQuality)
		{
			this.currentQualitySetting = newQuality;
			if (Ocean.Instance == null)
			{
				return;
			}
			WaveSpectrum waveSpectrum = Ocean.Instance.Spectrum as WaveSpectrum;
			ProjectedGrid projectedGrid = Ocean.Instance.Grid as ProjectedGrid;
			OceanQualitySettings.OceanSetting oceanSetting = this.CurrentSetting();
			if (waveSpectrum != null)
			{
				waveSpectrum.fourierSize = oceanSetting.fourierSize;
				waveSpectrum.disableFoam = !oceanSetting.spectrumFoamOn;
			}
			if (projectedGrid != null)
			{
				projectedGrid.resolution = oceanSetting.meshResolution;
				if (oceanSetting.topSideMaterial != null)
				{
					projectedGrid.oceanTopSideMat = oceanSetting.topSideMaterial;
				}
				if (oceanSetting.underSideMaterial != null)
				{
					projectedGrid.oceanUnderSideMat = oceanSetting.underSideMaterial;
				}
			}
		}

		private void PreCache()
		{
			if (!this.preCache || this.m_beenCached)
			{
				return;
			}
			if (Ocean.Instance == null)
			{
				return;
			}
			WaveSpectrum waveSpectrum = Ocean.Instance.Spectrum as WaveSpectrum;
			if (waveSpectrum != null)
			{
				waveSpectrum.CreateAndCacheCondition(this.lowSetting.fourierSize);
				waveSpectrum.CreateAndCacheCondition(this.highSetting.fourierSize);
				this.m_beenCached = true;
			}
		}

		private void SetCommandBuffers()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			if (this.usingSharedDepthGrab)
			{
				Ocean.Instance.UnderWater.DisableCopyDepthCmd = true;
				DepthBufferGrabCommand.AddBinding(Camera.main, Ocean.DEPTH_GRAB_TEXTURE_NAME);
			}
			else
			{
				Ocean.Instance.UnderWater.DisableCopyDepthCmd = false;
				DepthBufferGrabCommand.RemoveBinding(Camera.main, Ocean.DEPTH_GRAB_TEXTURE_NAME);
			}
			if (this.disableCaustics)
			{
				Ocean.Instance.UnderWater.DisableNormalFadeCmd = true;
			}
			else
			{
				Ocean.Instance.UnderWater.DisableNormalFadeCmd = false;
			}
		}
	}
}
