using Ceto.Common.Unity.Utility;
using System;
using UnityEngine;

namespace Ceto
{
	public class GUIDisplay : MonoBehaviour
	{
		private float m_textWidth = 150f;

		private Rect m_hideToggle = new Rect(20f, 20f, 95f, 30f);

		private Rect m_reflectionsToggle = new Rect(120f, 20f, 95f, 30f);

		private Rect m_refractionToggle = new Rect(220f, 20f, 95f, 30f);

		private Rect m_detailToggle = new Rect(320f, 20f, 95f, 30f);

		private Rect m_settings = new Rect(20f, 60f, 340f, 600f);

		private Ceto.Common.Unity.Utility.FPSCounter m_fps;

		private bool m_ultraDetailOn;

		private bool m_supportsDX11;

		public GameObject m_camera;

		public bool m_hide;

		private void Start()
		{
			this.m_fps = base.GetComponent<Ceto.Common.Unity.Utility.FPSCounter>();
			this.m_supportsDX11 = (SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders);
		}

		private void OnGUI()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			UnderWaterPostEffect component = this.m_camera.GetComponent<UnderWaterPostEffect>();
			WaveSpectrum component2 = Ocean.Instance.GetComponent<WaveSpectrum>();
			PlanarReflection component3 = Ocean.Instance.GetComponent<PlanarReflection>();
			UnderWater component4 = Ocean.Instance.GetComponent<UnderWater>();
			ProjectedGrid component5 = Ocean.Instance.GetComponent<ProjectedGrid>();
			GUILayout.BeginArea(this.m_hideToggle);
			GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
			this.m_hide = GUILayout.Toggle(this.m_hide, " Hide GUI", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			if (this.m_hide)
			{
				return;
			}
			if (component3 != null)
			{
				bool flag = component3.enabled;
				GUILayout.BeginArea(this.m_reflectionsToggle);
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				flag = GUILayout.Toggle(flag, " Reflection", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				component3.enabled = flag;
			}
			if (component4 != null)
			{
				bool flag2 = component4.enabled;
				GUILayout.BeginArea(this.m_refractionToggle);
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				flag2 = GUILayout.Toggle(flag2, " Refraction", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				component4.enabled = flag2;
			}
			if (component2 != null && component5 != null)
			{
				GUILayout.BeginArea(this.m_detailToggle);
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				this.m_ultraDetailOn = GUILayout.Toggle(this.m_ultraDetailOn, " Ultra Detail", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				if (this.m_ultraDetailOn)
				{
					component5.resolution = MESH_RESOLUTION.ULTRA;
					component2.fourierSize = FOURIER_SIZE.ULTRA_256_GPU;
					component2.disableReadBack = !this.m_supportsDX11;
				}
				else
				{
					component5.resolution = MESH_RESOLUTION.HIGH;
					component2.fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;
					component2.disableReadBack = true;
				}
			}
			GUILayout.BeginArea(this.m_settings);
			GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
			float num = Ocean.Instance.windDir;
			GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
			GUILayout.Label("Wind Direction", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(this.m_textWidth)
			});
			num = GUILayout.HorizontalSlider(num, 0f, 360f, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			Ocean.Instance.windDir = num;
			if (component2 != null)
			{
				float num2 = component2.windSpeed;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Wind Speed", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num2 = GUILayout.HorizontalSlider(num2, 0f, 30f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.windSpeed = num2;
			}
			if (component2 != null)
			{
				float num3 = component2.waveAge;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Wave Age", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num3 = GUILayout.HorizontalSlider(num3, 0.5f, 1f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.waveAge = num3;
			}
			if (component2 != null)
			{
				float num4 = component2.waveSpeed;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Wave Speed", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num4 = GUILayout.HorizontalSlider(num4, 0f, 10f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.waveSpeed = num4;
			}
			if (component2 != null)
			{
				float num5 = component2.choppyness;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Choppyness", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num5 = GUILayout.HorizontalSlider(num5, 0f, 1.2f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.choppyness = num5;
			}
			if (component2 != null)
			{
				float num6 = component2.foamAmount;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Foam Amount", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num6 = GUILayout.HorizontalSlider(num6, 0f, 6f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.foamAmount = num6;
			}
			if (component2 != null)
			{
				float num7 = component2.foamCoverage;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Foam Coverage", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num7 = GUILayout.HorizontalSlider(num7, 0f, 0.5f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.foamCoverage = num7;
			}
			if (component3 != null)
			{
				int num8 = component3.blurIterations;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Reflection blur", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num8 = (int)GUILayout.HorizontalSlider((float)num8, 0f, 4f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component3.blurIterations = num8;
			}
			if (component3 != null)
			{
				float num9 = component3.reflectionIntensity;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Reflection Intensity", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num9 = GUILayout.HorizontalSlider(num9, 0f, 2f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component3.reflectionIntensity = num9;
			}
			if (component4 != null)
			{
				float num10 = component4.refractionIntensity;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Refraction Intensity", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num10 = GUILayout.HorizontalSlider(num10, 0f, 2f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component4.refractionIntensity = num10;
			}
			if (component2 != null)
			{
				int num11 = component2.numberOfGrids;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Num Grids", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num11 = (int)GUILayout.HorizontalSlider((float)num11, 1f, 4f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.numberOfGrids = num11;
			}
			if (component2 != null)
			{
				float num12 = component2.gridScale;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Grid Scale", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num12 = GUILayout.HorizontalSlider(num12, 0.1f, 1f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component2.gridScale = num12;
			}
			if (component4 != null)
			{
				float num13 = component4.subSurfaceScatterModifier.intensity;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("SSS Intensity", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num13 = GUILayout.HorizontalSlider(num13, 0f, 10f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component4.subSurfaceScatterModifier.intensity = num13;
			}
			if (component != null)
			{
				int num14 = component.blurIterations;
				GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
				GUILayout.Label("Underwater Blur", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(this.m_textWidth)
				});
				num14 = (int)GUILayout.HorizontalSlider((float)num14, 0f, 4f, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				component.blurIterations = num14;
			}
			string text = "W to move ship forward.\r\nA/D to turn.\r\nLeft click and drag to rotate camera.\r\nF2 to toggle wireframe.\r\nKeypad +/- to move sun.";
			if (this.m_fps != null)
			{
				text = text + "\nCurrent FPS = " + this.m_fps.FrameRate.ToString("F2");
			}
			GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
			GUILayout.TextArea(text, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}
