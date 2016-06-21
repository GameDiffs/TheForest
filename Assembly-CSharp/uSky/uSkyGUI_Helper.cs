using System;
using UnityEngine;
using UnityEngine.UI;

namespace uSky
{
	[AddComponentMenu("uSky/uSkyGUI Helper")]
	public class uSkyGUI_Helper : MonoBehaviour
	{
		public uSkyManager m_uSkyManager;

		public Text TimeDisplay;

		public Slider[] slider;

		public void SetTimeline(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Timeline = value;
			}
		}

		public void SetLongitude(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Longitude = value;
			}
		}

		public void SetExposure(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Exposure = value;
			}
		}

		public void SetRayleigh(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.RayleighScattering = value;
			}
		}

		public void SetMie(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.MieScattering = value;
			}
		}

		public void SetSunAnisotropyFactor(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.SunAnisotropyFactor = value;
			}
		}

		public void SetSunSize(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.SunSize = value;
			}
		}

		public void SetWavelength_X(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Wavelengths.x = value;
			}
		}

		public void SetWavelength_Y(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Wavelengths.y = value;
			}
		}

		public void SetWavelength_Z(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.Wavelengths.z = value;
			}
		}

		public void SetStarIntensity(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.StarIntensity = value;
			}
		}

		public void SetOuterSpaceIntensity(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.OuterSpaceIntensity = value;
			}
		}

		public void SetMoonSize(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.MoonSize = value;
			}
		}

		public void SetMoonInnerCoronaScale(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.MoonInnerCorona.a = value;
			}
		}

		public void SetMoonOuterCoronaScale(float value)
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.MoonOuterCorona.a = value;
			}
		}

		private void Start()
		{
			if (this.m_uSkyManager)
			{
				this.m_uSkyManager.SkyUpdate = true;
			}
		}

		private void Update()
		{
			if (this.TimeDisplay && this.m_uSkyManager)
			{
				TimeSpan timeSpan = TimeSpan.FromHours((double)this.m_uSkyManager.Timeline);
				this.TimeDisplay.text = string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes);
			}
		}

		public void Reset_uSky()
		{
			this.slider[0].value = 1f;
			this.slider[1].value = 1f;
			this.slider[2].value = 1f;
			this.slider[3].value = 0.76f;
			this.slider[4].value = 1f;
			this.slider[5].value = 680f;
			this.slider[6].value = 550f;
			this.slider[7].value = 440f;
			this.slider[8].value = 1f;
			this.slider[9].value = 0.25f;
			this.slider[10].value = 0.15f;
			this.slider[11].value = 0.5f;
			this.slider[12].value = 0.5f;
		}
	}
}
