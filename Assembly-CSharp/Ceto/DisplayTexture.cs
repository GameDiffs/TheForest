using System;
using UnityEngine;

namespace Ceto
{
	[RequireComponent(typeof(Camera))]
	public class DisplayTexture : MonoBehaviour
	{
		public enum DISPLAY
		{
			NONE,
			OVERLAY_HEIGHT,
			OVERLAY_NORMAL,
			OVERLAY_FOAM,
			OVERLAY_CLIP,
			REFLECTION,
			OCEAN_MASK,
			OCEAN_DEPTH,
			WAVE_SLOPEMAP0,
			WAVE_SLOPEMAP1,
			WAVE_DISPLACEMENTMAP0,
			WAVE_DISPLACEMENTMAP1,
			WAVE_DISPLACEMENTMAP2,
			WAVE_DISPLACEMENTMAP3,
			WAVE_FOAM0,
			WAVE_FOAM1
		}

		public bool enlarge;

		public DisplayTexture.DISPLAY display;

		private void Start()
		{
		}

		private void OnGUI()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			Camera component = base.GetComponent<Camera>();
			CameraData cameraData = Ocean.Instance.FindCameraData(component);
			if (cameraData == null)
			{
				return;
			}
			Texture texture = this.FindTexture(cameraData, component);
			if (texture == null)
			{
				return;
			}
			int num;
			int num2;
			if ((texture.width == Screen.width && texture.height == Screen.height) || (texture.width == Screen.width / 2 && texture.height == Screen.height / 2))
			{
				num = Screen.width / ((!this.enlarge) ? 3 : 2);
				num2 = Screen.height / ((!this.enlarge) ? 3 : 2);
			}
			else
			{
				num = 256 * ((!this.enlarge) ? 1 : 2);
				num2 = 256 * ((!this.enlarge) ? 1 : 2);
			}
			GUI.DrawTexture(new Rect((float)(Screen.width - num - 5), 5f, (float)num, (float)num2), texture, ScaleMode.StretchToFill, false);
		}

		private Texture FindTexture(CameraData data, Camera cam)
		{
			if (Ocean.Instance == null)
			{
				return null;
			}
			WaveSpectrum component = Ocean.Instance.GetComponent<WaveSpectrum>();
			switch (this.display)
			{
			case DisplayTexture.DISPLAY.OVERLAY_HEIGHT:
				return (data.overlay != null) ? data.overlay.height : null;
			case DisplayTexture.DISPLAY.OVERLAY_NORMAL:
				return (data.overlay != null) ? data.overlay.normal : null;
			case DisplayTexture.DISPLAY.OVERLAY_FOAM:
				return (data.overlay != null) ? data.overlay.foam : null;
			case DisplayTexture.DISPLAY.OVERLAY_CLIP:
				return (data.overlay != null) ? data.overlay.clip : null;
			case DisplayTexture.DISPLAY.REFLECTION:
				return (data.reflection != null) ? data.reflection.tex : null;
			case DisplayTexture.DISPLAY.OCEAN_MASK:
				return (data.mask != null) ? data.mask.cam.targetTexture : null;
			case DisplayTexture.DISPLAY.OCEAN_DEPTH:
				return (data.depth != null && !(data.depth.cam == null)) ? data.depth.cam.targetTexture : null;
			case DisplayTexture.DISPLAY.WAVE_SLOPEMAP0:
				return (!(component == null)) ? component.SlopeMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_SLOPEMAP1:
				return (!(component == null)) ? component.SlopeMaps[1] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP0:
				return (!(component == null)) ? component.DisplacementMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP1:
				return (!(component == null)) ? component.DisplacementMaps[1] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP2:
				return (!(component == null)) ? component.DisplacementMaps[2] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP3:
				return (!(component == null)) ? component.DisplacementMaps[3] : null;
			case DisplayTexture.DISPLAY.WAVE_FOAM0:
				return (!(component == null)) ? component.FoamMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_FOAM1:
				return (!(component == null)) ? component.FoamMaps[1] : null;
			default:
				return null;
			}
		}
	}
}
