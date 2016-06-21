using System;
using UnityEngine;

[AddComponentMenu("uSky/uSky Demo GUI (Legacy)"), ExecuteInEditMode]
public class uSky_GUI : MonoBehaviour
{
	private const int labelWidth = 115;

	public uSkyManager m_uSky;

	public Texture GuiTex;

	private Rect rect = new Rect(2f, 2f, 240f, 490f);

	private void resetAll()
	{
		this.m_uSky.Exposure = 1f;
		this.m_uSky.RayleighScattering = 1f;
		this.m_uSky.MieScattering = 1f;
		this.m_uSky.SunAnisotropyFactor = 0.76f;
		this.m_uSky.SunSize = 1f;
		this.m_uSky.Wavelengths.x = 680f;
		this.m_uSky.Wavelengths.y = 550f;
		this.m_uSky.Wavelengths.z = 440f;
		this.m_uSky.StarIntensity = 1f;
		this.m_uSky.OuterSpaceIntensity = 0.25f;
		this.m_uSky.MoonSize = 0.15f;
		this.m_uSky.MoonInnerCorona.a = 0.5f;
		this.m_uSky.MoonOuterCorona.a = 0.5f;
	}

	private void OnGUI()
	{
		if (this.m_uSky == null)
		{
			Debug.Log("Please assign the <b>uSky</b> gameobject to GUI Script");
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			GUIUtility.ScaleAroundPivot(Vector2.one * 2f, Vector2.zero);
		}
		GUILayout.BeginArea(this.rect, string.Empty, "Box");
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (this.GuiTex != null)
		{
			GUI.DrawTexture(new Rect(72f, 3f, 96f, 48f), this.GuiTex);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("<b>Timeline</b>", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		GUILayout.Label(this.m_uSky.Timeline.ToString("0.0"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		this.m_uSky.Timeline = GUILayout.HorizontalSlider(this.m_uSky.Timeline, 0f, 24f, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("<b>Longitude</b>", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.Longitude = GUILayout.HorizontalSlider(this.m_uSky.Longitude, 0f, 360f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.Longitude.ToString("##0."), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Exposure", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.Exposure = GUILayout.HorizontalSlider(this.m_uSky.Exposure, 0f, 5f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.Exposure.ToString("0.0"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Rayleigh Scattering", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.RayleighScattering = GUILayout.HorizontalSlider(this.m_uSky.RayleighScattering, 0f, 5f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.RayleighScattering.ToString("0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Mie Scattering", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.MieScattering = GUILayout.HorizontalSlider(this.m_uSky.MieScattering, 0f, 5f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.MieScattering.ToString("0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Sun Anisotropy Factor", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.SunAnisotropyFactor = GUILayout.HorizontalSlider(this.m_uSky.SunAnisotropyFactor, 0f, 1f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.SunAnisotropyFactor.ToString("0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Sun Size", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.SunSize = GUILayout.HorizontalSlider(this.m_uSky.SunSize, 0.001f, 10f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.SunSize.ToString("0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Wavelength R", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.Wavelengths.x = GUILayout.HorizontalSlider(this.m_uSky.Wavelengths.x, 380f, 780f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.Wavelengths.x.ToString("###"), new GUILayoutOption[]
		{
			GUILayout.Width(24f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Wavelength G", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.Wavelengths.y = GUILayout.HorizontalSlider(this.m_uSky.Wavelengths.y, 380f, 780f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.Wavelengths.y.ToString("###"), new GUILayoutOption[]
		{
			GUILayout.Width(24f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Wavelength B", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.Wavelengths.z = GUILayout.HorizontalSlider(this.m_uSky.Wavelengths.z, 380f, 780f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.Wavelengths.z.ToString("###"), new GUILayoutOption[]
		{
			GUILayout.Width(24f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Star Intensity", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.StarIntensity = GUILayout.HorizontalSlider(this.m_uSky.StarIntensity, 0f, 5f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.StarIntensity.ToString("#0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Outer Space Intensity", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.OuterSpaceIntensity = GUILayout.HorizontalSlider(this.m_uSky.OuterSpaceIntensity, 0f, 2f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.OuterSpaceIntensity.ToString("#0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Moon Size", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.MoonSize = GUILayout.HorizontalSlider(this.m_uSky.MoonSize, 0f, 1f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.MoonSize.ToString("#0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Moon Inner Corona", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.MoonInnerCorona.a = GUILayout.HorizontalSlider(this.m_uSky.MoonInnerCorona.a, 0f, 1f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.MoonInnerCorona.a.ToString("#0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Moon Outer Corona", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		});
		this.m_uSky.MoonOuterCorona.a = GUILayout.HorizontalSlider(this.m_uSky.MoonOuterCorona.a, 0f, 5f, new GUILayoutOption[0]);
		GUILayout.Label(this.m_uSky.MoonOuterCorona.a.ToString("#0.0#"), new GUILayoutOption[]
		{
			GUILayout.Width(28f)
		});
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Reset All", new GUILayoutOption[]
		{
			GUILayout.Width(115f)
		}))
		{
			this.resetAll();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
