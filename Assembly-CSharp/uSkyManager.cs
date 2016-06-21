using System;
using UnityEngine;
using uSky;

[AddComponentMenu("uSky/uSky Manager"), ExecuteInEditMode]
public class uSkyManager : MonoBehaviour
{
	[Tooltip("Update of the sky calculations in each frame.")]
	public bool SkyUpdate = true;

	[Range(0f, 24f), Tooltip("This value controls the light vertically. It represents sunrise/day and sunset/night time( Rotation X )")]
	public float Timeline = 17f;

	[Range(-180f, 180f), Tooltip("This value controls the light horizionally.( Rotation Y )")]
	public float Longitude;

	[Range(0f, 5f), Space(10f), Tooltip("This value sets the brightness of the sky.(for day time only)")]
	public float Exposure = 1f;

	[Range(0f, 5f), Tooltip("Rayleigh scattering is caused by particles in the atmosphere (up to 8 km). It produces typical earth-like sky colors (reddish/yellowish colors at sun set, and the like).")]
	public float RayleighScattering = 1f;

	[Range(0f, 5f), Tooltip("Mie scattering is caused by aerosols in the lower atmosphere (up to 1.2 km). It is for haze and halos around the sun on foggy days.")]
	public float MieScattering = 1f;

	[Range(0f, 0.9995f), Tooltip("The anisotropy factor controls the sun's appearance in the sky.The closer this value gets to 1.0, the sharper and smaller the sun spot will be. Higher values cause more fuzzy and bigger sun spots.")]
	public float SunAnisotropyFactor = 0.76f;

	[Range(0.001f, 10f), Tooltip("Size of the sun spot in the sky")]
	public float SunSize = 1f;

	[Tooltip("It is visible spectrum light waves. Tweaking these values will shift the colors of the resulting gradients and produce different kinds of atmospheres.")]
	public Vector3 Wavelengths = new Vector3(680f, 550f, 440f);

	[Tooltip("It is wavelength dependent. Tweaking these values will shift the colors of sky color.")]
	public Color SkyTint = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Tooltip("It is the bottom half color of the skybox")]
	public Color m_GroundColor = new Color(0.369f, 0.349f, 0.341f, 1f);

	[Tooltip("It is a Directional Light from the scene, it represents Sun Ligthing")]
	public GameObject m_sunLight;

	[Space(10f), Tooltip("Toggle the Night Sky On and Off")]
	public bool EnableNightSky = true;

	[Tooltip("The zenith color of the night sky gradient. (Top of the night sky)")]
	public Gradient NightZenithColor = new Gradient
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color32(50, 71, 99, 255), 0.225f),
			new GradientColorKey(new Color32(74, 107, 148, 255), 0.25f),
			new GradientColorKey(new Color32(74, 107, 148, 255), 0.75f),
			new GradientColorKey(new Color32(50, 71, 99, 255), 0.775f)
		},
		alphaKeys = new GradientAlphaKey[]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	[Tooltip("The horizon color of the night sky gradient.")]
	public Color NightHorizonColor = new Color(0.43f, 0.47f, 0.5f, 1f);

	[Range(0f, 5f), Tooltip("This controls the intensity of the Stars field in night sky.")]
	public float StarIntensity = 1f;

	[Range(0f, 2f), Tooltip("This controls the intensity of the Outer Space Cubemap in night sky.")]
	public float OuterSpaceIntensity = 0.25f;

	[Tooltip("The color of the moon's inner corona. This Alpha value controls the size and blurriness corona.")]
	public Color MoonInnerCorona = new Color(1f, 1f, 1f, 0.5f);

	[Tooltip("The color of the moon's outer corona. This Alpha value controls the size and blurriness corona.")]
	public Color MoonOuterCorona = new Color(0.25f, 0.39f, 0.5f, 0.5f);

	[Range(0f, 1f), Tooltip("This controls the moon texture size in the night sky.")]
	public float MoonSize = 0.15f;

	[Tooltip("It is additional Directional Light from the scene, it represents Moon Ligthing.")]
	public GameObject m_moonLight;

	[Tooltip("It is the uSkybox Material of the uSky.")]
	public Material SkyboxMaterial;

	[SerializeField, Tooltip("It will automatically assign the current skybox material to Render Settings.")]
	private bool _AutoApplySkybox = true;

	[HideInInspector, SerializeField]
	public bool LinearSpace;

	[Tooltip("Toggle it if the Main Camera is using HDR mode and Tonemapping image effect.")]
	public bool Tonemapping;

	private Vector3 euler;

	private Matrix4x4 moon_wtl;

	private StarField Stars;

	private Mesh starsMesh;

	private Material m_starMaterial;

	public bool AutoApplySkybox
	{
		get
		{
			return this._AutoApplySkybox;
		}
		set
		{
			if (value && this.SkyboxMaterial && RenderSettings.skybox != this.SkyboxMaterial)
			{
				RenderSettings.skybox = this.SkyboxMaterial;
			}
			this._AutoApplySkybox = value;
		}
	}

	protected Material starMaterial
	{
		get
		{
			if (this.m_starMaterial == null)
			{
				this.m_starMaterial = new Material(Shader.Find("Hidden/uSky/Stars"));
				this.m_starMaterial.hideFlags = HideFlags.DontSave;
			}
			return this.m_starMaterial;
		}
	}

	public float Timeline01
	{
		get
		{
			return this.Timeline / 24f;
		}
	}

	public Vector3 SunDir
	{
		get
		{
			return (!(this.m_sunLight != null)) ? new Vector3(0.321f, 0.766f, -0.557f) : (this.m_sunLight.transform.forward * -1f);
		}
	}

	private Matrix4x4 getMoonMatrix
	{
		get
		{
			if (this.m_moonLight == null)
			{
				this.moon_wtl = Matrix4x4.TRS(Vector3.zero, new Quaternion(-0.9238795f, 8.817204E-08f, 8.817204E-08f, 0.3826835f), Vector3.one);
			}
			else if (this.m_moonLight != null)
			{
				this.moon_wtl = this.m_moonLight.transform.worldToLocalMatrix;
				this.moon_wtl.SetColumn(2, Vector4.Scale(new Vector4(1f, 1f, 1f, -1f), this.moon_wtl.GetColumn(2)));
			}
			return this.moon_wtl;
		}
	}

	private Vector3 variableRangeWavelengths
	{
		get
		{
			return new Vector3(Mathf.Lerp(this.Wavelengths.x + 150f, this.Wavelengths.x - 150f, this.SkyTint.r), Mathf.Lerp(this.Wavelengths.y + 150f, this.Wavelengths.y - 150f, this.SkyTint.g), Mathf.Lerp(this.Wavelengths.z + 150f, this.Wavelengths.z - 150f, this.SkyTint.b));
		}
	}

	public Vector3 BetaR
	{
		get
		{
			Vector3 vector = this.variableRangeWavelengths * 1E-09f;
			Vector3 a = new Vector3(Mathf.Pow(vector.x, 4f), Mathf.Pow(vector.y, 4f), Mathf.Pow(vector.z, 4f));
			Vector3 vector2 = 7.635E+25f * a * 5.755f;
			float num = 8f * Mathf.Pow(3.14159274f, 3f) * Mathf.Pow(0.0006002188f, 2f) * 6.105f;
			return 1000f * new Vector3(num / vector2.x, num / vector2.y, num / vector2.z);
		}
	}

	private Vector3 betaR_RayleighOffset
	{
		get
		{
			return this.BetaR * Mathf.Max(0.001f, this.RayleighScattering);
		}
	}

	public Vector3 BetaM
	{
		get
		{
			return new Vector3(Mathf.Pow(this.Wavelengths.x, -0.84f), Mathf.Pow(this.Wavelengths.y, -0.84f), Mathf.Pow(this.Wavelengths.z, -0.84f));
		}
	}

	public float uMuS
	{
		get
		{
			return Mathf.Atan(Mathf.Max(this.SunDir.y, -0.1975f) * 5.35f) / 1.1f + 0.74f;
		}
	}

	public float DayTime
	{
		get
		{
			return Mathf.Min(1f, this.uMuS);
		}
	}

	public float SunsetTime
	{
		get
		{
			return Mathf.Clamp01((this.uMuS - 1f) * (2f / Mathf.Pow(this.RayleighScattering, 4f)));
		}
	}

	public float NightTime
	{
		get
		{
			return 1f - this.DayTime;
		}
	}

	public Vector3 miePhase_g
	{
		get
		{
			float num = this.SunAnisotropyFactor * this.SunAnisotropyFactor;
			float num2 = (!this.LinearSpace || !this.Tonemapping) ? 1f : 2f;
			return new Vector3(num2 * ((1f - num) / (2f + num)), 1f + num, 2f * this.SunAnisotropyFactor);
		}
	}

	public Vector3 mieConst
	{
		get
		{
			return new Vector3(1f, this.BetaR.x / this.BetaR.y, this.BetaR.x / this.BetaR.z) * 0.004f * this.MieScattering;
		}
	}

	public Vector3 skyMultiplier
	{
		get
		{
			return new Vector3(this.SunsetTime, this.Exposure * 4f * this.DayTime * Mathf.Sqrt(this.RayleighScattering), this.NightTime);
		}
	}

	private Vector3 bottomTint
	{
		get
		{
			float num = (!this.LinearSpace) ? 0.02f : 0.01f;
			return new Vector3(this.betaR_RayleighOffset.x / (this.m_GroundColor.r * num), this.betaR_RayleighOffset.y / (this.m_GroundColor.g * num), this.betaR_RayleighOffset.z / (this.m_GroundColor.b * num));
		}
	}

	public Vector2 ColorCorrection
	{
		get
		{
			return (!this.LinearSpace || !this.Tonemapping) ? ((!this.LinearSpace) ? Vector2.one : new Vector2(1f, 2f)) : new Vector2(0.38317f, 1.413f);
		}
	}

	public Color getNightHorizonColor
	{
		get
		{
			return this.NightHorizonColor * this.NightTime;
		}
	}

	public Color getNightZenithColor
	{
		get
		{
			return this.NightZenithColor.Evaluate(this.Timeline01) * 0.01f;
		}
	}

	private Vector4 getMoonInnerCorona
	{
		get
		{
			return new Vector4(this.MoonInnerCorona.r * this.NightTime, this.MoonInnerCorona.g * this.NightTime, this.MoonInnerCorona.b * this.NightTime, 400f / this.MoonInnerCorona.a);
		}
	}

	private Vector4 getMoonOuterCorona
	{
		get
		{
			float num = (!this.LinearSpace) ? 8f : ((!this.Tonemapping) ? 12f : 16f);
			return new Vector4(this.MoonOuterCorona.r * 0.25f * this.NightTime, this.MoonOuterCorona.g * 0.25f * this.NightTime, this.MoonOuterCorona.b * 0.25f * this.NightTime, num / this.MoonOuterCorona.a);
		}
	}

	private float starBrightness
	{
		get
		{
			float num = (!this.LinearSpace) ? 1.5f : 0.5f;
			return this.StarIntensity * this.NightTime * num;
		}
	}

	protected void InitStarsMesh()
	{
		if (this.Stars == null)
		{
			this.Stars = new StarField();
		}
		this.starsMesh = this.Stars.InitializeStarfield();
		this.starsMesh.hideFlags = HideFlags.DontSave;
	}

	private void OnEnable()
	{
		if (this.m_sunLight == null)
		{
			this.m_sunLight = GameObject.Find("Directional Light");
		}
		if (this.EnableNightSky && this.starsMesh == null)
		{
			Debug.Log("InitMaterial Starfield");
			this.InitStarsMesh();
		}
		this.InitMaterial(this.SkyboxMaterial);
	}

	private void OnDisable()
	{
		if (this.starsMesh)
		{
			UnityEngine.Object.DestroyImmediate(this.starsMesh);
		}
		if (this.m_starMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.m_starMaterial);
		}
	}

	private void detectColorSpace()
	{
		if (this.SkyboxMaterial != null)
		{
			this.InitMaterial(this.SkyboxMaterial);
		}
	}

	private void Start()
	{
		this.InitSun();
		if (this.SkyboxMaterial != null)
		{
			this.InitMaterial(this.SkyboxMaterial);
		}
		this.AutoApplySkybox = this._AutoApplySkybox;
		if (this.EnableNightSky && this.starsMesh == null)
		{
			this.InitStarsMesh();
		}
	}

	private void Update()
	{
		if (this.SkyUpdate)
		{
			if (this.Timeline >= 24f)
			{
				this.Timeline = 0f;
			}
			if (this.SkyboxMaterial != null)
			{
				this.InitSun();
				this.InitMaterial(this.SkyboxMaterial);
			}
		}
		if (this.EnableNightSky && this.starsMesh != null && this.starMaterial != null && this.SunDir.y < 0.2f && Camera.main != null)
		{
			Vector3 position = Camera.main.transform.position;
			Graphics.DrawMesh(this.starsMesh, position, Quaternion.identity, this.starMaterial, 0);
		}
	}

	private void InitSun()
	{
	}

	private void InitMaterial(Material mat)
	{
		mat.SetVector("_SunDir", this.SunDir);
		mat.SetMatrix("_Moon_wtl", this.getMoonMatrix);
		mat.SetVector("_betaR", this.betaR_RayleighOffset);
		mat.SetVector("_betaM", this.BetaM);
		mat.SetVector("_SkyMultiplier", this.skyMultiplier);
		mat.SetFloat("_SunSize", 32f / this.SunSize);
		mat.SetVector("_mieConst", this.mieConst);
		mat.SetVector("_miePhase_g", this.miePhase_g);
		mat.SetVector("_GroundColor", this.bottomTint);
		mat.SetVector("_NightHorizonColor", this.getNightHorizonColor);
		mat.SetVector("_NightZenithColor", this.getNightZenithColor);
		mat.SetVector("_MoonInnerCorona", this.getMoonInnerCorona);
		mat.SetVector("_MoonOuterCorona", this.getMoonOuterCorona);
		mat.SetFloat("_MoonSize", this.MoonSize);
		mat.SetVector("_colorCorrection", this.ColorCorrection);
		if (this.Tonemapping)
		{
		}
		mat.SetFloat("_OuterSpaceIntensity", this.OuterSpaceIntensity);
		if (this.starMaterial != null)
		{
			this.starMaterial.SetFloat("StarIntensity", this.starBrightness);
		}
	}
}
