using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace uSky
{
	[AddComponentMenu("uSky/uSky Light"), ExecuteInEditMode, RequireComponent(typeof(uSkyManager))]
	public class uSkyLight : MonoBehaviour
	{
		[Range(0f, 4f), Tooltip("Brightness of the Sun (directional light)")]
		public float SunIntensity = 1f;

		[Tooltip("The color of the both Sun and Moon light emitted")]
		public Gradient LightColor = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color32(55, 66, 77, 255), 0.23f),
				new GradientColorKey(new Color32(245, 173, 84, 255), 0.26f),
				new GradientColorKey(new Color32(249, 208, 144, 255), 0.32f),
				new GradientColorKey(new Color32(252, 222, 186, 255), 0.5f),
				new GradientColorKey(new Color32(249, 208, 144, 255), 0.68f),
				new GradientColorKey(new Color32(245, 173, 84, 255), 0.74f),
				new GradientColorKey(new Color32(55, 66, 77, 255), 0.77f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[Tooltip("Toggle the Moon lighting during night time")]
		public bool EnableMoonLighting = true;

		[Range(0f, 2f), Tooltip("Brightness of the Moon (directional light)")]
		public float MoonIntensity = 0.4f;

		[Tooltip("Ambient light that shines into the scene.")]
		public uSkyAmbient Ambient;

		private uSkyManager _uSM;

		private Light _sun_Light;

		private Light _moon_Light;

		private float currentTime
		{
			get
			{
				return (!(this.uSM != null)) ? 1f : this.uSM.Timeline01;
			}
		}

		private float dayTime
		{
			get
			{
				return (!(this.uSM != null)) ? 1f : this.uSM.DayTime;
			}
		}

		private float nightTime
		{
			get
			{
				return (!(this.uSM != null)) ? 0f : this.uSM.NightTime;
			}
		}

		private float sunsetTime
		{
			get
			{
				return (!(this.uSM != null)) ? 1f : this.uSM.SunsetTime;
			}
		}

		private uSkyManager uSM
		{
			get
			{
				if (this._uSM == null)
				{
					this._uSM = base.gameObject.GetComponent<uSkyManager>();
				}
				return this._uSM;
			}
		}

		private GameObject sunLightObj
		{
			get
			{
				if (this.uSM != null)
				{
					return (!(this.uSM.m_sunLight != null)) ? null : this.uSM.m_sunLight;
				}
				return null;
			}
		}

		private GameObject moonLightObj
		{
			get
			{
				if (this.uSM != null)
				{
					return (!(this.uSM.m_moonLight != null)) ? null : this.uSM.m_moonLight;
				}
				return null;
			}
		}

		private Light sun_Light
		{
			get
			{
				if (this.sunLightObj)
				{
					this._sun_Light = this.sunLightObj.GetComponent<Light>();
				}
				if (this._sun_Light)
				{
					return this._sun_Light;
				}
				return null;
			}
		}

		private Light moon_Light
		{
			get
			{
				if (this.moonLightObj)
				{
					this._moon_Light = this.moonLightObj.GetComponent<Light>();
				}
				if (this._moon_Light)
				{
					return this._moon_Light;
				}
				return null;
			}
		}

		private float exposure
		{
			get
			{
				return (!(this.uSM != null)) ? 1f : this.uSM.Exposure;
			}
		}

		private Color groundColorTint
		{
			get
			{
				return (!(this.uSM != null)) ? new Color(0.369f, 0.349f, 0.341f, 1f) : this.uSM.m_GroundColor;
			}
		}

		private float rayleighSlider
		{
			get
			{
				return (!(this.uSM != null)) ? 1f : this.uSM.RayleighScattering;
			}
		}

		public Color CurrentLightColor
		{
			get
			{
				return this.LightColor.Evaluate(this.currentTime);
			}
		}

		public Color CurrentSkyColor
		{
			get
			{
				return this.colorOffset(this.Ambient.SkyColor.Evaluate(this.currentTime), 0.15f, 0.7f, false);
			}
		}

		public Color CurrentEquatorColor
		{
			get
			{
				return this.colorOffset(this.Ambient.EquatorColor.Evaluate(this.currentTime), 0.15f, 0.9f, false);
			}
		}

		public Color CurrentGroundColor
		{
			get
			{
				return this.colorOffset(this.Ambient.GroundColor.Evaluate(this.currentTime), 0.25f, 0.85f, true);
			}
		}

		private void Start()
		{
			if (this.uSM != null)
			{
				this.InitUpdate();
			}
		}

		private void Update()
		{
			if (this.uSM != null && this.uSM.SkyUpdate)
			{
				this.InitUpdate();
			}
		}

		private void InitUpdate()
		{
			this.SunAndMoonLightUpdate();
			if (RenderSettings.ambientMode == AmbientMode.Trilight)
			{
				this.AmbientGradientUpdate();
			}
			else
			{
				RenderSettings.ambientLight = this.CurrentSkyColor;
			}
		}

		private void SunAndMoonLightUpdate()
		{
			if (this.sunLightObj != null && this.sun_Light != null)
			{
				this.sun_Light.intensity = this.uSM.Exposure * this.SunIntensity * this.dayTime;
				this.sun_Light.color = this.CurrentLightColor * this.dayTime;
				this.sun_Light.enabled = (this.currentTime >= 0.24f && this.currentTime <= 0.76f);
			}
			if (this.moonLightObj != null)
			{
				if (this.moon_Light != null)
				{
					this.moon_Light.intensity = this.uSM.Exposure * this.MoonIntensity * this.nightTime;
					this.moon_Light.color = this.CurrentLightColor * this.nightTime;
					this.moon_Light.enabled = ((this.currentTime <= 0.26f || this.currentTime >= 0.74f) && this.EnableMoonLighting && this.EnableMoonLighting);
				}
			}
			else if (this.sun_Light)
			{
				this.sun_Light.enabled = true;
			}
		}

		private void AmbientGradientUpdate()
		{
			RenderSettings.ambientSkyColor = this.CurrentSkyColor;
			RenderSettings.ambientEquatorColor = this.CurrentEquatorColor;
			RenderSettings.ambientGroundColor = this.CurrentGroundColor;
		}

		private Color colorOffset(Color currentColor, float offsetRange, float rayleighOffsetRange, bool IsGround)
		{
			Vector3 vector = (!(this.uSM != null)) ? new Vector3(5.81f, 13.57f, 33.13f) : (this.uSM.BetaR * 1000f);
			Vector3 to = new Vector3(0.5f, 0.5f, 0.5f);
			if (IsGround)
			{
				to = new Vector3(this.groundColorTint.r / 0.369f * 0.5f, this.groundColorTint.g / 0.349f * 0.5f, this.groundColorTint.b / 0.341f * 0.5f);
			}
			else
			{
				to = new Vector3(vector.x / 5.81f * 0.5f, vector.y / 13.57f * 0.5f, vector.z / 33.13f * 0.5f);
			}
			to = Vector3.Lerp(new Vector3(Mathf.Abs(1f - to.x), Mathf.Abs(1f - to.y), Mathf.Abs(1f - to.z)), to, this.sunsetTime);
			to = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), to, this.dayTime);
			Vector3 vector2 = new Vector3(Mathf.Lerp(currentColor.r - offsetRange, currentColor.r + offsetRange, to.x), Mathf.Lerp(currentColor.g - offsetRange, currentColor.g + offsetRange, to.y), Mathf.Lerp(currentColor.b - offsetRange, currentColor.b + offsetRange, to.z));
			Vector3 to2 = new Vector3(vector2.x / vector.x, vector2.y / vector.y, vector2.z / vector.z) * 4f;
			vector2 = ((this.rayleighSlider >= 1f) ? Vector3.Lerp(vector2, to2, Mathf.Max(0f, this.rayleighSlider - 1f) / 4f * rayleighOffsetRange) : Vector3.Lerp(Vector3.zero, vector2, this.rayleighSlider));
			return new Color(vector2.x, vector2.y, vector2.z, 1f) * this.exposure;
		}
	}
}
