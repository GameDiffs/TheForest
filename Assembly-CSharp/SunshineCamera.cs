using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(SunshinePostprocess))]
public class SunshineCamera : MonoBehaviour
{
	public SunshineCamera StereoscopicMasterCamera;

	private Camera attachedCamera;

	private bool refreshRequested;

	private Vector3 lastBoundsOrigin = Vector3.zero;

	private SunshinePostprocess sunshinePostprocess;

	private LightShadows _lightShadows;

	private LightRenderMode _lightRenderMode;

	private List<Terrain> tempTerrainList = new List<Terrain>();

	private List<float> tempTerrainPixelError = new List<float>();

	private SunshineMath.ShadowCameraTemporalData[] cascadeTemporalData = new SunshineMath.ShadowCameraTemporalData[4];

	private float scatterNoiseSeed;

	private Camera AttachedCamera
	{
		get
		{
			if (!this.attachedCamera)
			{
				this.attachedCamera = base.GetComponent<Camera>();
			}
			return this.attachedCamera;
		}
	}

	public bool GoodToGo
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			if (!Sunshine.Instance || !Sunshine.Instance.enabled)
			{
				return false;
			}
			if (this.ShadowsActive)
			{
				if (Sunshine.Instance.IsMobile && SystemInfo.graphicsShaderLevel < 20)
				{
					return false;
				}
				if (!Sunshine.Instance.IsMobile && SystemInfo.graphicsShaderLevel < 30)
				{
					return false;
				}
			}
			else if (!Sunshine.Instance.RequiresPostprocessing)
			{
				return false;
			}
			return Sunshine.Instance.LightDistance > 0f && Sunshine.Instance.Lightmap && Sunshine.Instance.SunLight && Sunshine.Instance.SunLight.enabled && Sunshine.Instance.Ready;
		}
	}

	public bool ForwardShadersOK
	{
		get
		{
			return SunshineProjectPreferences.Instance && SunshineProjectPreferences.Instance.UseCustomShadows && (SunshineProjectPreferences.Instance.ForwardShadersInstalled || SunshineProjectPreferences.Instance.ManualShaderInstallation);
		}
	}

	public bool ShadowsActive
	{
		get
		{
			return Sunshine.Instance && SunshineProjectPreferences.Instance.UseCustomShadows && Sunshine.Instance.SunLight && (this.AttachedCamera.actualRenderingPath == RenderingPath.DeferredLighting || this.AttachedCamera.actualRenderingPath == RenderingPath.DeferredShading || (this.ForwardShadersOK && this.AttachedCamera.actualRenderingPath == RenderingPath.Forward));
		}
	}

	public float BoundsPadding
	{
		get
		{
			SunshineUpdateInterval updateInterval = Sunshine.Instance.UpdateInterval;
			if (updateInterval == SunshineUpdateInterval.AfterXFrames)
			{
				return Sunshine.Instance.UpdateIntervalPadding;
			}
			if (updateInterval != SunshineUpdateInterval.AfterXMovement)
			{
				return 0f;
			}
			return Sunshine.Instance.UpdateIntervalMovement;
		}
	}

	public void RequestRefresh()
	{
		this.refreshRequested = true;
	}

	public bool NeedsRefresh(Vector3 boundsOrigin)
	{
		if (!Application.isPlaying)
		{
			return true;
		}
		if (this.StereoscopicMasterCamera != null)
		{
			return false;
		}
		bool flag = this.refreshRequested;
		switch (Sunshine.Instance.UpdateInterval)
		{
		case SunshineUpdateInterval.EveryFrame:
			flag = true;
			break;
		case SunshineUpdateInterval.AfterXFrames:
			flag = (flag || Time.frameCount <= 3 || Time.frameCount % Sunshine.Instance.UpdateIntervalFrames == 0);
			break;
		case SunshineUpdateInterval.AfterXMovement:
			if (Time.frameCount <= 3)
			{
				flag = true;
			}
			else
			{
				Vector3 vector = boundsOrigin - this.lastBoundsOrigin;
				flag = (flag || vector.sqrMagnitude >= Sunshine.Instance.UpdateIntervalMovement * Sunshine.Instance.UpdateIntervalMovement);
			}
			break;
		}
		if (flag)
		{
			this.lastBoundsOrigin = boundsOrigin;
		}
		return flag;
	}

	private void OnEnable()
	{
		this.sunshinePostprocess = base.GetComponent<SunshinePostprocess>();
		if (this.sunshinePostprocess == null)
		{
			this.sunshinePostprocess = base.gameObject.AddComponent<SunshinePostprocess>();
		}
	}

	private void OnDisable()
	{
		SunshineKeywords.DisableShadows();
	}

	private void OnDestroy()
	{
		SunshineKeywords.DisableShadows();
	}

	private void ConfigureTheForestShaders()
	{
		Shader.SetGlobalVector("sunshine_CameraPosition", base.GetComponent<Camera>().transform.position);
	}

	private void RenderCascades()
	{
		SunshineMath.BoundingSphere boundingSphere = default(SunshineMath.BoundingSphere);
		if (Sunshine.Instance.UsingCustomBounds)
		{
			boundingSphere = Sunshine.Instance.CustomBounds;
		}
		else
		{
			boundingSphere = SunshineMath.FrustumBoundingSphereBinarySearch(this.AttachedCamera, this.AttachedCamera.nearClipPlane, Sunshine.Instance.LightDistance, true, this.BoundsPadding, 0.01f, 20);
		}
		if (!this.NeedsRefresh(boundingSphere.origin))
		{
			return;
		}
		bool flag = Sunshine.Instance.TerrainLODTweak > 0f;
		if (flag)
		{
			this.tempTerrainList.Clear();
			this.tempTerrainPixelError.Clear();
			Terrain[] activeTerrains = Terrain.activeTerrains;
			for (int i = 0; i < activeTerrains.Length; i++)
			{
				Terrain terrain = activeTerrains[i];
				if (terrain)
				{
					this.tempTerrainList.Add(terrain);
					this.tempTerrainPixelError.Add(terrain.heightmapPixelError);
					terrain.heightmapPixelError *= 1f - Sunshine.Instance.TerrainLODTweak;
				}
			}
		}
		for (int j = 0; j < Sunshine.Instance.CascadeCount; j++)
		{
			Camera camera = Sunshine.Instance.SunLightCameras[j];
			camera.cullingMask = Sunshine.Instance.GetCascadeOccluders(j);
			SunshineMath.SetupShadowCamera(Sunshine.Instance.SunLight, camera, this.AttachedCamera, Sunshine.Instance.CascadeNearClip(j), Sunshine.Instance.CascadeFarClip(j), Sunshine.Instance.LightPaddingZ, this.BoundsPadding, Sunshine.Instance.CascadeMapResolution, ref boundingSphere, ref this.cascadeTemporalData[j]);
			Shader.SetGlobalVector("sunshine_DepthBiases", new Vector2(Sunshine.Instance.ShadowBias, Sunshine.Instance.ShadowSlopeBias(j)));
			camera.rect = Sunshine.Instance.CascadeRects[j];
			camera.targetTexture = Sunshine.Instance.Lightmap;
			camera.useOcclusionCulling = Sunshine.Instance.UseOcclusionCulling;
			camera.RenderWithShader(Sunshine.Instance.OccluderShader, "RenderType");
		}
		if (flag)
		{
			for (int k = this.tempTerrainList.Count - 1; k >= 0; k--)
			{
				Terrain terrain2 = this.tempTerrainList[k];
				terrain2.heightmapPixelError = this.tempTerrainPixelError[k];
			}
			this.tempTerrainList.Clear();
			this.tempTerrainPixelError.Clear();
		}
		this.refreshRequested = false;
	}

	private void ConfigureCascadeClips(float cameraFarClip)
	{
		float num = Sunshine.Instance.LightDistance / cameraFarClip;
		if (this.AttachedCamera.orthographic)
		{
			num = 99999f;
		}
		Vector4 vector = new Vector4(Sunshine.Instance.CascadeNearClipScale(0) * num, Sunshine.Instance.CascadeNearClipScale(1) * num, Sunshine.Instance.CascadeNearClipScale(2) * num, Sunshine.Instance.CascadeNearClipScale(3) * num);
		Shader.SetGlobalVector("sunshine_CascadeNearRatiosSq", new Vector4(vector.x * vector.x, vector.y * vector.y, vector.z * vector.z, vector.w * vector.w));
		Vector4 vector2 = new Vector4(Sunshine.Instance.CascadeFarClipScale(0) * num, Sunshine.Instance.CascadeFarClipScale(1) * num, Sunshine.Instance.CascadeFarClipScale(2) * num, Sunshine.Instance.CascadeFarClipScale(3) * num);
		Shader.SetGlobalVector("sunshine_CascadeFarRatiosSq", new Vector4(vector2.x * vector2.x, vector2.y * vector2.y, vector2.z * vector2.z, vector2.w * vector2.w));
		float num2 = (!this.AttachedCamera.orthographic) ? Sunshine.Instance.LightDistance : 1E+09f;
		float value = (!this.AttachedCamera.orthographic) ? Sunshine.Instance.LightFadeRatio : 1E-05f;
		float f = Mathf.Clamp(value, 1E-06f, 1f);
		float num3 = 1f / Mathf.Sqrt(f);
		float num4 = cameraFarClip / num2 * num3;
		num3 *= num3;
		num4 *= num4;
		Shader.SetGlobalVector("sunshine_ShadowFadeParams", new Vector3(num3, num4, num));
	}

	private void ConfigureOvercast(bool overcastEnabled, Texture2D overcastTexture, float overcastScale, Vector2 overcastMovement, float overcastPlaneHeight = 0f)
	{
		if (!overcastTexture)
		{
			overcastEnabled = false;
		}
		Shader.SetGlobalTexture("sunshine_OvercastMap", (!overcastEnabled) ? Sunshine.Instance.BlankOvercastTexture : overcastTexture);
		if (overcastEnabled)
		{
			Camera sunLightCamera = Sunshine.Instance.SunLightCamera;
			Ray ray = sunLightCamera.ViewportPointToRay(new Vector3(0f, 0f, 0f));
			float num = sunLightCamera.farClipPlane - sunLightCamera.nearClipPlane;
			float num2 = ray.direction.y * num;
			if (Mathf.Abs(num2) < 0.001f)
			{
				num2 = 0.001f;
			}
			float num3 = (overcastPlaneHeight - ray.origin.y) / num2;
			Vector3 a = ray.GetPoint(num3 * num);
			Ray ray2 = sunLightCamera.ViewportPointToRay(new Vector3(1f, 0f, 0f));
			float num4 = (overcastPlaneHeight - ray2.origin.y) / num2;
			Vector3 a2 = ray2.GetPoint(num4 * num);
			Ray ray3 = sunLightCamera.ViewportPointToRay(new Vector3(0f, 1f, 0f));
			float num5 = (overcastPlaneHeight - ray3.origin.y) / num2;
			Vector3 a3 = ray3.GetPoint(num5 * num);
			Vector3 b = new Vector3(overcastMovement.x, 0f, overcastMovement.y) * Time.timeSinceLevelLoad;
			a += b;
			a2 += b;
			a3 += b;
			Vector2 b2 = new Vector2(a.x, a.z);
			Vector2 vector = new Vector2(a2.x, a2.z) - b2;
			Vector2 vector2 = new Vector2(a3.x, a3.z) - b2;
			Rect rect = Sunshine.Instance.CascadeRect(0);
			Vector2 a4 = new Vector2(b2.x, b2.y);
			Vector4 a5 = new Vector4(vector.x / rect.width, vector.y / rect.width, vector2.x / rect.height, vector2.y / rect.height);
			Shader.SetGlobalVector("sunshine_OvercastCoord", a4 * (1f / overcastScale));
			Shader.SetGlobalVector("sunshine_OvercastVectorsUV", a5 * (1f / overcastScale));
		}
	}

	private void ConfigureShaders()
	{
		this.ConfigureTheForestShaders();
		if (this.ShadowsActive)
		{
			SunshineKeywords.SetFilterStyle(Sunshine.Instance.ShadowFilter);
		}
		else
		{
			SunshineKeywords.DisableShadows();
		}
		Matrix4x4 lhs = Matrix4x4.identity;
		lhs = SunshineMath.ToRectSpaceProjection(Sunshine.Instance.CascadeRect(0)) * Sunshine.Instance.SunLightCamera.projectionMatrix;
		SunshineMath.SetLinearDepthProjection(ref lhs, Sunshine.Instance.SunLightCamera.farClipPlane);
		Matrix4x4 matrix4x = lhs * Sunshine.Instance.SunLightCamera.worldToCameraMatrix;
		Matrix4x4 mat = matrix4x * this.AttachedCamera.cameraToWorldMatrix;
		Shader.SetGlobalMatrix("sunshine_CameraVToSunVP", mat);
		Shader.SetGlobalMatrix("sunshine_WorldToSunVP", matrix4x);
		float num = (float)Sunshine.Instance.Lightmap.width;
		Shader.SetGlobalVector("sunshine_ShadowParamsAndHalfTexel", new Vector4(Sunshine.Instance.SunLight.shadowStrength, Sunshine.Instance.CascadeFade, 0.5f / num, 0.5f / num));
		this.ConfigureCascadeClips(this.AttachedCamera.farClipPlane);
		Vector3 position = (!this.AttachedCamera.orthographic) ? base.transform.position : this.AttachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3 position2 = this.AttachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, this.AttachedCamera.farClipPlane));
		Vector3 position3 = this.AttachedCamera.ViewportToWorldPoint(new Vector3(1f, 0f, this.AttachedCamera.farClipPlane));
		Vector3 position4 = this.AttachedCamera.ViewportToWorldPoint(new Vector3(0f, 1f, this.AttachedCamera.farClipPlane));
		Transform transform = Sunshine.Instance.SunLightCamera.transform;
		Vector3 vector = transform.InverseTransformPoint(position);
		Vector3 vector2 = transform.InverseTransformPoint(position2);
		Vector3 vector3 = transform.InverseTransformPoint(position3);
		Vector3 vector4 = transform.InverseTransformPoint(position4);
		Vector2 vector5 = SunshineMath.xy(Sunshine.Instance.SunLightCamera.WorldToViewportPoint(position));
		Vector2 vector6 = SunshineMath.xy(Sunshine.Instance.SunLightCamera.WorldToViewportPoint(position2));
		Vector2 vector7 = SunshineMath.xy(Sunshine.Instance.SunLightCamera.WorldToViewportPoint(position3));
		Vector2 vector8 = SunshineMath.xy(Sunshine.Instance.SunLightCamera.WorldToViewportPoint(position4));
		Vector4 vector9 = new Vector4(vector6.x, vector6.y, vector2.z / Sunshine.Instance.SunLightCamera.farClipPlane, vector.y);
		Vector4 vector10 = new Vector4(vector5.x, vector5.y, vector.z / Sunshine.Instance.SunLightCamera.farClipPlane, vector.y);
		Vector4 vec = vector9 - vector10;
		Vector4 vec2 = new Vector4(vector7.x, vector7.y, vector3.z / Sunshine.Instance.SunLightCamera.farClipPlane, vector3.y) - vector9;
		Vector4 vec3 = new Vector4(vector8.x, vector8.y, vector4.z / Sunshine.Instance.SunLightCamera.farClipPlane, vector4.y) - vector9;
		Rect rect = Sunshine.Instance.CascadeRect(0);
		SunshineMath.ShadowCoordDataInRect(ref vector10, ref rect);
		SunshineMath.ShadowCoordDataRayInRect(ref vec, ref rect);
		SunshineMath.ShadowCoordDataRayInRect(ref vec2, ref rect);
		SunshineMath.ShadowCoordDataRayInRect(ref vec3, ref rect);
		Shader.SetGlobalFloat("sunshine_IsOrthographic", (!this.AttachedCamera.orthographic) ? 0f : 1f);
		Shader.SetGlobalVector("sunshine_ShadowCoordDepthStart", vector10);
		Shader.SetGlobalVector("sunshine_ShadowCoordDepthRayZ", vec);
		Shader.SetGlobalVector("sunshine_ShadowCoordDepthRayU", vec2);
		Shader.SetGlobalVector("sunshine_ShadowCoordDepthRayV", vec3);
		float num2 = Sunshine.Instance.SunLightCamera.orthographicSize * 2f;
		Vector2 vector11 = new Vector2(num2, num2);
		vector11.x /= rect.width;
		vector11.y /= rect.height;
		Vector3 v = new Vector3(vector11.x, vector11.y, Sunshine.Instance.SunLightCamera.farClipPlane) / this.AttachedCamera.farClipPlane;
		Shader.SetGlobalVector("sunshine_ShadowToWorldScale", v);
		Matrix4x4 zero = Matrix4x4.zero;
		Vector3 position5 = Sunshine.Instance.SunLightCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3 position6 = Sunshine.Instance.SunLightCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
		for (int i = 0; i < Sunshine.Instance.CascadeCount; i++)
		{
			Vector4 v2 = new Vector4(0f, 0f, 1f, 1f);
			if (i > 0)
			{
				Camera camera = Sunshine.Instance.SunLightCameras[i];
				Vector3 vector12 = camera.WorldToViewportPoint(position5);
				Vector3 vector13 = camera.WorldToViewportPoint(position6);
				v2 = new Vector4(vector12.x, vector12.y, vector13.x, vector13.y);
			}
			Rect rect2 = Sunshine.Instance.CascadeRect(i);
			v2.x = rect2.xMin + rect2.width * v2.x;
			v2.y = rect2.yMin + rect2.height * v2.y;
			v2.z = rect2.xMin + rect2.width * v2.z;
			v2.w = rect2.yMin + rect2.height * v2.w;
			v2.z -= v2.x;
			v2.w -= v2.y;
			zero.SetRow(i, v2);
		}
		Vector4 row = zero.GetRow(0);
		for (int j = 0; j < Sunshine.Instance.CascadeCount; j++)
		{
			Vector4 row2 = zero.GetRow(j);
			row2.z /= row.z;
			row2.w /= row.w;
			zero.SetRow(j, row2);
		}
		bool flag = Sunshine.Instance.OvercastTexture;
		this.ConfigureOvercast(flag, (!flag) ? Sunshine.Instance.BlankOvercastTexture : Sunshine.Instance.OvercastTexture, Sunshine.Instance.OvercastScale, Sunshine.Instance.OvercastMovement, Sunshine.Instance.OvercastPlaneHeight);
		Shader.SetGlobalMatrix("sunshine_CascadeRanges", zero);
	}

	private void Update()
	{
		if (!Sunshine.Instance)
		{
			return;
		}
		bool requiresPostprocessing = Sunshine.Instance.RequiresPostprocessing;
		if (this.sunshinePostprocess && this.sunshinePostprocess.enabled != requiresPostprocessing)
		{
			this.sunshinePostprocess.enabled = requiresPostprocessing;
		}
		if (this.StereoscopicMasterCamera != null)
		{
			if (this.StereoscopicMasterCamera.StereoscopicMasterCamera == this)
			{
				this.StereoscopicMasterCamera = null;
			}
			else
			{
				this.AttachedCamera.depth = this.StereoscopicMasterCamera.AttachedCamera.depth + 1f;
			}
		}
	}

	private void OnPreCull()
	{
		if (!this.GoodToGo)
		{
			SunshineKeywords.DisableShadows();
			return;
		}
		this.RenderCascades();
		this.ConfigureShaders();
		if (this.ShadowsActive)
		{
			this._lightShadows = Sunshine.Instance.SunLight.shadows;
			this._lightRenderMode = Sunshine.Instance.SunLight.renderMode;
			Sunshine.Instance.SunLight.shadows = LightShadows.None;
			Sunshine.Instance.SunLight.renderMode = LightRenderMode.ForcePixel;
		}
		if (Sunshine.Instance.RequiresPostprocessing && (this.AttachedCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.AttachedCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	private void OnPostRender()
	{
		if (this.ShadowsActive)
		{
			Sunshine.Instance.SunLight.shadows = this._lightShadows;
			Sunshine.Instance.SunLight.renderMode = this._lightRenderMode;
		}
		SunshineKeywords.DisableShadows();
	}

	public void OnPostProcess(RenderTexture source, RenderTexture destination)
	{
		if (!this.GoodToGo)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (Sunshine.Instance.DebugView == SunshineDebugViews.Cascades)
		{
			SunshinePostprocess.Blit(source, destination, Sunshine.Instance.PostDebugMaterial, SunshinePostDebugPass.DebugCascades);
		}
		else
		{
			bool scatterActive = Sunshine.Instance.ScatterActive;
			if (scatterActive)
			{
				bool flag = Sunshine.Instance.ScatterBlur;
				if (!Sunshine.Instance.PostBlurSupported)
				{
					flag = false;
				}
				bool flag2 = Sunshine.Instance.OvercastAffectsScatter && (Sunshine.Instance.OvercastTexture || Sunshine.Instance.ScatterOvercastTexture);
				bool customScatterOvercast = Sunshine.Instance.CustomScatterOvercast;
				Texture2D overcastTexture = (!flag2) ? null : ((!customScatterOvercast) ? Sunshine.Instance.OvercastTexture : Sunshine.Instance.ScatterOvercastTexture);
				float overcastScale = (!customScatterOvercast) ? Sunshine.Instance.OvercastScale : Sunshine.Instance.ScatterOvercastScale;
				Vector2 overcastMovement = (!customScatterOvercast) ? Sunshine.Instance.OvercastMovement : Sunshine.Instance.ScatterOvercastMovement;
				float overcastPlaneHeight = (!customScatterOvercast) ? Sunshine.Instance.OvercastPlaneHeight : Sunshine.Instance.ScatterOvercastPlaneHeight;
				this.ConfigureOvercast(flag2, overcastTexture, overcastScale, overcastMovement, overcastPlaneHeight);
				Vector3 direction = this.AttachedCamera.ViewportPointToRay(new Vector3(0f, 0f, 0f)).direction;
				Vector3 v = this.AttachedCamera.ViewportPointToRay(new Vector3(1f, 0f, 0f)).direction - direction;
				Vector3 v2 = this.AttachedCamera.ViewportPointToRay(new Vector3(0f, 1f, 0f)).direction - direction;
				Sunshine.Instance.PostScatterMaterial.SetVector("worldLightRay", Sunshine.Instance.SunLight.transform.forward);
				Sunshine.Instance.PostScatterMaterial.SetVector("worldRay", direction);
				Sunshine.Instance.PostScatterMaterial.SetVector("worldRayU", v);
				Sunshine.Instance.PostScatterMaterial.SetVector("worldRayV", v2);
				Sunshine.Instance.PostScatterMaterial.SetTexture("_ScatterRamp", Sunshine.Instance.ScatterRamp);
				SunshineKeywords.SetScatterQuality(Sunshine.Instance.ScatterSamplingQuality);
				Sunshine.Instance.PostScatterMaterial.SetVector("ScatterColor", Sunshine.Instance.ScatterColor);
				if (Sunshine.Instance.ScatterAnimateNoise)
				{
					this.scatterNoiseSeed += Time.deltaTime * Sunshine.Instance.ScatterAnimateNoiseSpeed;
					this.scatterNoiseSeed -= Mathf.Floor(this.scatterNoiseSeed);
				}
				Sunshine.Instance.PostScatterMaterial.SetTexture("ScatterDitherMap", Sunshine.Instance.ScatterDitherTexture);
				float value = 1f - Sunshine.Instance.ScatterExaggeration;
				float y = 1f / (Mathf.Clamp01(value) * Sunshine.Instance.LightDistance / this.AttachedCamera.farClipPlane);
				float num = Sunshine.Instance.ScatterSky * Sunshine.Instance.ScatterIntensity;
				Sunshine.Instance.PostScatterMaterial.SetVector("ScatterIntensityVolumeSky", new Vector4(Sunshine.Instance.ScatterIntensity, y, num * 0.333f, num * 0.667f));
				bool flag3 = Sunshine.Instance.ScatterResolution == SunshineRelativeResolutions.Full && !flag && Sunshine.Instance.DebugView != SunshineDebugViews.Scatter;
				SunshineKeywords.ToggleScatterBlendNow(flag3);
				if (!flag3)
				{
					int b = SunshineMath.RelativeResolutionDivisor(Sunshine.Instance.ScatterResolution);
					int2 @int = new int2(source.width, source.height) / b;
					@int.x = Mathf.Max(@int.x, 1);
					@int.y = Mathf.Max(@int.y, 1);
					Sunshine.Instance.PostScatterMaterial.SetVector("ScatterDitherData", new Vector3((float)@int.x / (float)Sunshine.Instance.ScatterDitherTexture.width, (float)@int.y / (float)Sunshine.Instance.ScatterDitherTexture.height, (!Sunshine.Instance.ScatterAnimateNoise) ? 0f : this.scatterNoiseSeed));
					RenderTexture temporary = RenderTexture.GetTemporary(@int.x, @int.y, 0, source.format, RenderTextureReadWrite.Default);
					if (temporary)
					{
						temporary.filterMode = FilterMode.Point;
						temporary.wrapMode = TextureWrapMode.Clamp;
						SunshinePostprocess.Blit(source, temporary, Sunshine.Instance.PostScatterMaterial, SunshinePostScatterPass.DrawScatter);
						if (flag)
						{
							Sunshine.Instance.PostBlurMaterial.SetFloat("BlurDepthTollerance", Sunshine.Instance.ScatterBlurDepthTollerance);
							RenderTexture temporary2 = RenderTexture.GetTemporary(temporary.width, temporary.height, 0, temporary.format, RenderTextureReadWrite.Default);
							if (temporary2)
							{
								temporary2.filterMode = temporary.filterMode;
								temporary2.wrapMode = temporary.wrapMode;
								Sunshine.Instance.PostBlurMaterial.SetVector("BlurXY", new Vector2(1f, 0f));
								SunshinePostprocess.Blit(temporary, temporary2, Sunshine.Instance.PostBlurMaterial, 0);
								temporary.DiscardContents();
								Sunshine.Instance.PostBlurMaterial.SetVector("BlurXY", new Vector2(0f, 1f));
								SunshinePostprocess.Blit(temporary2, temporary, Sunshine.Instance.PostBlurMaterial, 0);
								RenderTexture.ReleaseTemporary(temporary2);
							}
						}
						temporary.filterMode = FilterMode.Bilinear;
						if (Sunshine.Instance.DebugView == SunshineDebugViews.Scatter)
						{
							SunshinePostprocess.Blit(temporary, destination, Sunshine.Instance.PostDebugMaterial, SunshinePostDebugPass.DebugAlpha);
						}
						else
						{
							Sunshine.Instance.PostScatterMaterial.SetTexture("_ScatterTexture", temporary);
							SunshinePostprocess.Blit(source, destination, Sunshine.Instance.PostScatterMaterial, SunshinePostScatterPass.ApplyScatter);
						}
						RenderTexture.ReleaseTemporary(temporary);
					}
					else
					{
						flag3 = true;
					}
				}
				if (flag3)
				{
					Sunshine.Instance.PostScatterMaterial.SetVector("ScatterDitherData", new Vector3((float)this.AttachedCamera.pixelWidth / (float)Sunshine.Instance.ScatterDitherTexture.width, (float)this.AttachedCamera.pixelHeight / (float)Sunshine.Instance.ScatterDitherTexture.height, (!Sunshine.Instance.ScatterAnimateNoise) ? 0f : this.scatterNoiseSeed));
					SunshinePostprocess.Blit(source, destination, Sunshine.Instance.PostScatterMaterial, SunshinePostScatterPass.DrawScatter);
				}
			}
		}
	}
}
