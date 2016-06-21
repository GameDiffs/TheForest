using Ceto;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Graphics
{
	[AddComponentMenu("The Forest/Graphics/WaterEngine"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class WaterEngine : MonoBehaviour
	{
		public delegate void RenderCallback();

		public delegate void RenderImageCallback(RenderTexture source, RenderTexture destination);

		public bool usingSharedDepthGrab;

		public Shader depthShader;

		private Material depthMaterial;

		private List<Camera> depthCameras = new List<Camera>();

		private List<RenderTexture> depthTextures = new List<RenderTexture>();

		public Utility.TextureResolution reflectionResolution = Utility.TextureResolution._512;

		public LayerMask reflectionLayers = -1;

		public bool reflectionSettingsFoldout;

		public float reflectionClipPlaneOffset = 0.07f;

		public bool reflectionEnablePixelLights;

		private Camera reflectionCamera;

		private List<Camera> reflectionCameras = new List<Camera>();

		private List<RenderTexture> reflectionTextures = new List<RenderTexture>();

		private int reflectionCachedIndex = -1;

		private List<Vector3> reflectionCachedPosition = new List<Vector3>();

		private List<Vector3> reflectionCachedEuler = new List<Vector3>();

		public Shader underWaterShader;

		public float fogDensityMultiplier = 1f;

		public float fogPowerMultiplier = 1f;

		private Material underWaterMaterial;

		private Material waterMaterial;

		private Light sunLight;

		private Light moonLight;

		public static WaterEngine.RenderCallback RenderCameras = null;

		public static WaterEngine.RenderImageCallback RenderImage = null;

		[SerializeField]
		private bool debug;

		private static Ocean ocean = null;

		private static List<Lake> lakes = new List<Lake>();

		private static Camera currentCamera = null;

		private static Transform currentTransform = null;

		private static Water currentWater = null;

		private static bool currentUnderWater = false;

		public static Camera Camera
		{
			get
			{
				if (Application.isPlaying)
				{
					return WaterEngine.currentCamera;
				}
				return Camera.current;
			}
		}

		public static Transform CameraTransform
		{
			get
			{
				if (Application.isPlaying)
				{
					return WaterEngine.currentTransform;
				}
				if (WaterEngine.Camera == null)
				{
					return null;
				}
				return WaterEngine.Camera.transform;
			}
		}

		public static Ocean Ocean
		{
			get
			{
				return WaterEngine.ocean;
			}
			set
			{
				WaterEngine.ocean = value;
			}
		}

		public static List<Lake> Lakes
		{
			get
			{
				return WaterEngine.lakes;
			}
		}

		public static Water Water
		{
			get
			{
				return WaterEngine.currentWater;
			}
		}

		public static bool IsUnderWater
		{
			get
			{
				return WaterEngine.currentUnderWater;
			}
		}

		private void EnableDepth()
		{
			if (this.depthShader == null)
			{
				this.depthShader = Shader.Find("The Forest/Water-Depth");
			}
			if (this.depthShader)
			{
				this.depthMaterial = new Material(this.depthShader);
				this.depthMaterial.hideFlags = HideFlags.DontSave;
			}
		}

		private void DisableDepth()
		{
			if (this.depthMaterial)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.depthMaterial);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.depthMaterial);
				}
				this.depthMaterial = null;
			}
			this.depthCameras.Clear();
			for (int i = 0; i < this.depthTextures.Count; i++)
			{
				if (this.depthTextures[i])
				{
					this.depthTextures[i].Release();
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(this.depthTextures[i]);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(this.depthTextures[i]);
					}
					this.depthTextures[i] = null;
				}
			}
			this.depthTextures.Clear();
		}

		private void RenderDepth(Camera camera)
		{
			if (camera.name != "SceneCamera" && DepthBufferGrabCommand.HasCamera(camera) && this.usingSharedDepthGrab)
			{
				DepthBufferGrabCommand.AddBinding(camera, "CameraDepthTexture");
				this.ReleaseDepthTexture(camera);
			}
			else if (this.depthMaterial)
			{
				DepthBufferGrabCommand.RemoveBinding(camera, "CameraDepthTexture");
				RenderTexture depthTexture = this.GetDepthTexture(camera);
				Graphics.Blit(null, depthTexture, this.depthMaterial);
				Shader.SetGlobalTexture("CameraDepthTexture", depthTexture);
			}
		}

		private void ReleaseDepthTexture(Camera camera)
		{
			int num = this.depthCameras.IndexOf(camera);
			if (num == -1)
			{
				return;
			}
			if (this.depthTextures[num] == null)
			{
				return;
			}
			this.depthTextures[num].Release();
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.depthTextures[num]);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.depthTextures[num]);
			}
			this.depthTextures[num] = null;
		}

		private RenderTexture GetDepthTexture(Camera camera)
		{
			int num = this.depthCameras.IndexOf(camera);
			if (num == -1)
			{
				this.depthCameras.Add(camera);
				this.depthTextures.Add(null);
				num = this.depthCameras.Count - 1;
			}
			if (this.depthTextures[num] && (!this.depthTextures[num].IsCreated() || this.depthTextures[num].width != camera.pixelWidth || this.depthTextures[num].height != camera.pixelHeight))
			{
				this.depthTextures[num].Release();
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.depthTextures[num]);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.depthTextures[num]);
				}
				this.depthTextures[num] = null;
			}
			if (this.depthTextures[num] == null)
			{
				this.depthTextures[num] = Utility.CreateRenderTexture(camera.pixelWidth, camera.pixelHeight, 24, RenderTextureFormat.ARGBFloat, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, false);
				this.depthTextures[num].name = "Water Engine Depth Texture: " + camera.name;
				this.depthTextures[num].isPowerOfTwo = true;
				this.depthTextures[num].hideFlags = HideFlags.DontSave;
			}
			return this.depthTextures[num];
		}

		private void EnableReflection()
		{
			this.reflectionCamera = this.CreateReflectionCamera();
		}

		private void DisableReflection()
		{
			if (this.reflectionCamera)
			{
				this.reflectionCamera.targetTexture = null;
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.reflectionCamera.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.reflectionCamera.gameObject);
				}
				this.reflectionCamera = null;
			}
			this.reflectionCameras.Clear();
			for (int i = 0; i < this.reflectionTextures.Count; i++)
			{
				if (this.reflectionTextures[i])
				{
					this.reflectionTextures[i].Release();
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(this.reflectionTextures[i]);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(this.reflectionTextures[i]);
					}
					this.reflectionTextures[i] = null;
				}
			}
			this.reflectionTextures.Clear();
			this.reflectionCachedIndex = -1;
			this.reflectionCachedPosition.Clear();
			this.reflectionCachedEuler.Clear();
		}

		private void RenderReflection(Water water, Camera camera, Transform cameraTransform)
		{
			this.RenderReflection(water.transform, camera, cameraTransform);
		}

		private RenderTexture RenderReflection(Transform waterTransform, Camera camera, Transform cameraTransform)
		{
			RenderTexture reflectionTexture = this.GetReflectionTexture(camera);
			if (!Application.isPlaying && this.reflectionCachedPosition[this.reflectionCachedIndex] == cameraTransform.position && this.reflectionCachedEuler[this.reflectionCachedIndex] == cameraTransform.eulerAngles)
			{
				return null;
			}
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (!this.reflectionEnablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			Vector3 position = waterTransform.position;
			Vector3 vector = waterTransform.up;
			if (WaterEngine.currentUnderWater)
			{
				vector = -waterTransform.up;
			}
			float w = -Vector3.Dot(vector, position) - this.reflectionClipPlaneOffset;
			Vector4 plane = new Vector4(vector.x, vector.y, vector.z, w);
			Matrix4x4 zero = Matrix4x4.zero;
			this.CalculateReflectionMatrix(ref zero, plane);
			Vector4 clipPlane = this.CameraSpacePlane(camera.worldToCameraMatrix * zero, position, vector, 1f);
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			this.CalculateObliqueMatrix(ref projectionMatrix, clipPlane);
			GL.invertCulling = true;
			if (this.reflectionCamera)
			{
				this.RenderReflectionCamera(this.reflectionCamera, camera, reflectionTexture, camera.worldToCameraMatrix * zero, projectionMatrix, this.reflectionLayers, zero.MultiplyPoint(cameraTransform.position), new Vector3(0f, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z));
			}
			GL.invertCulling = false;
			if (!this.reflectionEnablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
			if (!Application.isPlaying)
			{
				this.reflectionCachedPosition[this.reflectionCachedIndex] = cameraTransform.position;
				this.reflectionCachedEuler[this.reflectionCachedIndex] = cameraTransform.eulerAngles;
			}
			Shader.SetGlobalTexture("CameraReflectionTexture", reflectionTexture);
			return reflectionTexture;
		}

		private Camera CreateReflectionCamera()
		{
			GameObject gameObject = new GameObject("Reflection Camera");
			gameObject.transform.Reset();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			Camera camera = gameObject.AddComponent<Camera>();
			camera.enabled = false;
			return camera;
		}

		private void RenderReflectionCamera(Camera camera, Camera copy, RenderTexture renderTexture, Matrix4x4 reflection, Matrix4x4 projection, LayerMask layerMask, Vector3 position, Vector3 eulerAngles)
		{
			camera.clearFlags = copy.clearFlags;
			camera.backgroundColor = copy.backgroundColor;
			camera.aspect = copy.aspect;
			camera.fieldOfView = copy.fieldOfView;
			camera.nearClipPlane = copy.nearClipPlane;
			camera.farClipPlane = copy.farClipPlane;
			camera.orthographic = copy.orthographic;
			camera.orthographicSize = copy.orthographicSize;
			camera.depth = copy.depth;
			camera.renderingPath = copy.renderingPath;
			camera.useOcclusionCulling = copy.useOcclusionCulling;
			camera.hdr = copy.hdr;
			camera.stereoConvergence = copy.stereoConvergence;
			camera.stereoSeparation = copy.stereoSeparation;
			camera.targetTexture = renderTexture;
			camera.cullingMask = (~(1 << LayerMask.NameToLayer("Water")) & layerMask);
			camera.worldToCameraMatrix = reflection;
			camera.projectionMatrix = projection;
			camera.renderingPath = RenderingPath.Forward;
			camera.useOcclusionCulling = false;
			camera.transform.position = position;
			camera.transform.eulerAngles = eulerAngles;
			camera.Render();
		}

		private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 v = pos + normal * this.reflectionClipPlaneOffset;
			Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
			Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
		}

		private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
		}

		private void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
		{
			Vector4 b = projection.inverse * new Vector4(Mathf.Sign(clipPlane.x), Mathf.Sign(clipPlane.y), 1f, 1f);
			Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
			projection[2] = vector.x - projection[3];
			projection[6] = vector.y - projection[7];
			projection[10] = vector.z - projection[11];
			projection[14] = vector.w - projection[15];
		}

		private RenderTexture GetReflectionTexture(Camera camera)
		{
			this.reflectionCachedIndex = this.reflectionCameras.IndexOf(camera);
			if (this.reflectionCachedIndex == -1)
			{
				this.reflectionCameras.Add(camera);
				this.reflectionTextures.Add(null);
				this.reflectionCachedPosition.Add(Vector3.zero);
				this.reflectionCachedEuler.Add(Vector3.zero);
				this.reflectionCachedIndex = this.reflectionCameras.Count - 1;
			}
			if (this.reflectionTextures[this.reflectionCachedIndex] && !this.reflectionTextures[this.reflectionCachedIndex].IsCreated())
			{
				this.reflectionTextures[this.reflectionCachedIndex].Release();
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.reflectionTextures[this.reflectionCachedIndex]);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.reflectionTextures[this.reflectionCachedIndex]);
				}
				this.reflectionTextures[this.reflectionCachedIndex] = null;
			}
			if (this.reflectionTextures[this.reflectionCachedIndex] == null)
			{
				this.reflectionTextures[this.reflectionCachedIndex] = Utility.CreateRenderTexture((int)this.reflectionResolution, (int)this.reflectionResolution, 24, RenderTextureFormat.ARGB32, FilterMode.Bilinear, TextureWrapMode.Repeat, 1, false);
				this.reflectionTextures[this.reflectionCachedIndex].isPowerOfTwo = true;
				this.reflectionTextures[this.reflectionCachedIndex].hideFlags = HideFlags.DontSave;
				this.reflectionCachedPosition[this.reflectionCachedIndex] = Vector3.zero;
				this.reflectionCachedEuler[this.reflectionCachedIndex] = Vector3.zero;
			}
			return this.reflectionTextures[this.reflectionCachedIndex];
		}

		private void EnableUnderWater()
		{
			if (this.underWaterShader == null)
			{
				this.underWaterShader = Shader.Find("The Forest/UnderWater");
			}
			if (this.underWaterShader)
			{
				this.underWaterMaterial = new Material(this.underWaterShader);
				this.underWaterMaterial.hideFlags = HideFlags.DontSave;
			}
			else
			{
				Debug.LogError("[WATERENGINE] Script does not have shader for UnderWaterEffect, shutting down.");
				base.enabled = false;
			}
			GameObject gameObject = GameObject.FindWithTag("Sun");
			if (gameObject)
			{
				this.sunLight = gameObject.GetComponent<Light>();
			}
			GameObject gameObject2 = GameObject.FindWithTag("Moon");
			if (gameObject2)
			{
				this.moonLight = gameObject2.GetComponent<Light>();
			}
		}

		private void DisableUnderWater()
		{
			if (this.underWaterMaterial)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.underWaterMaterial);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.underWaterMaterial);
				}
				this.underWaterMaterial = null;
			}
			this.sunLight = null;
			this.moonLight = null;
		}

		private void RenderUnderWater(Water water, Camera camera, Transform cameraTransform, RenderTexture source, RenderTexture destination)
		{
			if (this.underWaterMaterial)
			{
				float num = 1f / Mathf.Tan(camera.fieldOfView * 0.5f * 0.0174532924f);
				float aspect = camera.aspect;
				Shader.SetGlobalMatrix("UnderWaterCameraMatrix", Matrix4x4.TRS(Vector3.zero, cameraTransform.rotation, new Vector3(aspect / num, 1f / num, 1f) * camera.nearClipPlane));
				Shader.SetGlobalFloat("UnderWaterWaterLevel", water.transform.position.y);
				Shader.SetGlobalTexture("UnderWaterScreenTexture", source);
				if (this.sunLight && this.sunLight.enabled)
				{
					Vector4 vec = -this.sunLight.transform.forward;
					vec.w = this.sunLight.intensity;
					Shader.SetGlobalVector("UnderWaterSunParams", vec);
					Shader.SetGlobalColor("UnderWaterSunColor", this.sunLight.color);
				}
				else
				{
					Shader.SetGlobalVector("UnderWaterSunParams", new Vector4(0f, 0f, 0f, 0f));
					Shader.SetGlobalColor("UnderWaterSunColor", Color.white);
				}
				if (this.moonLight && this.moonLight.enabled)
				{
					Vector4 vec2 = -this.moonLight.transform.forward;
					vec2.w = this.moonLight.intensity;
					Shader.SetGlobalVector("UnderWaterMoonParams", vec2);
					Shader.SetGlobalColor("UnderWaterMoonColor", this.moonLight.color);
				}
				else
				{
					Shader.SetGlobalVector("UnderWaterMoonParams", new Vector4(0f, 0f, 0f, 0f));
					Shader.SetGlobalColor("UnderWaterMoonColor", Color.white);
				}
				Shader.SetGlobalFloat("UnderWaterFogDensity", this.fogDensityMultiplier);
				Shader.SetGlobalFloat("UnderWaterFogPower", this.fogPowerMultiplier);
				if (water.SharedMaterial && this.waterMaterial != water.SharedMaterial)
				{
					this.waterMaterial = water.SharedMaterial;
					this.underWaterMaterial.CopyPropertiesFromMaterial(this.waterMaterial);
				}
				Graphics.Blit(source, destination, this.underWaterMaterial);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		[ContextMenu("DEBUG ON")]
		private void DebugOn()
		{
			this.debug = true;
			this.DebugFlags();
		}

		[ContextMenu("DEBUG OFF")]
		private void DebugOff()
		{
			this.debug = false;
			this.DebugFlags();
		}

		private void DebugFlags()
		{
			if (this.reflectionCamera)
			{
				this.reflectionCamera.gameObject.hideFlags = ((!this.debug) ? HideFlags.HideAndDontSave : HideFlags.DontSave);
			}
		}

		public static Water WaterAt(Vector3 position)
		{
			Water result = WaterEngine.ocean;
			for (int i = 0; i < WaterEngine.lakes.Count; i++)
			{
				if (WaterEngine.lakes[i].IsInBounds(position))
				{
					result = WaterEngine.lakes[i];
				}
			}
			return result;
		}

		private void Start()
		{
			if (Ceto.Ocean.Instance != null && Ceto.Ocean.Instance.Reflection != null)
			{
				Ceto.Ocean.Instance.Reflection.RenderReflectionCustom = new Func<GameObject, RenderTexture>(this.Ceto_OnRenderCameras);
			}
		}

		private void OnEnable()
		{
			WaterEngine.RenderCameras = new WaterEngine.RenderCallback(this.OnRenderCameras);
			WaterEngine.RenderImage = new WaterEngine.RenderImageCallback(this.OnRenderImage);
			WaterEngine.currentCamera = base.GetComponent<Camera>();
			WaterEngine.currentTransform = base.transform;
			this.EnableDepth();
			this.EnableReflection();
			this.EnableUnderWater();
			this.DebugFlags();
		}

		private void OnDisable()
		{
			WaterEngine.RenderCameras = null;
			WaterEngine.RenderImage = null;
			WaterEngine.currentCamera = null;
			WaterEngine.currentTransform = null;
			this.DisableDepth();
			this.DisableReflection();
			this.DisableUnderWater();
		}

		private void Update()
		{
			if (Ceto.Ocean.Instance != null && Ceto.Ocean.Instance.Reflection != null)
			{
				Ceto.Ocean.Instance.Reflection.RenderReflectionCustom = new Func<GameObject, RenderTexture>(this.Ceto_OnRenderCameras);
			}
			if (WaterEngine.Camera)
			{
				this.UpdateCurrentWater(WaterEngine.CameraTransform.position);
			}
		}

		private void OnRenderCameras()
		{
			Camera camera = WaterEngine.Camera;
			Transform cameraTransform = WaterEngine.CameraTransform;
			if (camera)
			{
				this.UpdateCurrentWater(cameraTransform.position);
			}
			if (WaterEngine.currentWater && camera)
			{
				this.RenderReflection(WaterEngine.currentWater, camera, cameraTransform);
			}
		}

		private RenderTexture Ceto_OnRenderCameras(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			Camera camera = WaterEngine.Camera;
			Transform cameraTransform = WaterEngine.CameraTransform;
			RenderTexture result = null;
			if (camera)
			{
				this.UpdateCurrentWater(cameraTransform.position);
				result = this.RenderReflection(go.transform, camera, cameraTransform);
			}
			return result;
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Camera camera = WaterEngine.Camera;
			Transform cameraTransform = WaterEngine.CameraTransform;
			if (camera)
			{
				this.UpdateCurrentWater(cameraTransform.position);
				this.RenderDepth(camera);
			}
			if (WaterEngine.currentWater && camera && LocalPlayer.WaterViz && LocalPlayer.WaterViz.InWater && Scene.Atmosphere && !Scene.Atmosphere.InACave)
			{
				this.RenderUnderWater(WaterEngine.currentWater, camera, cameraTransform, source, destination);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		private void UpdateCurrentWater(Vector3 position)
		{
			WaterEngine.currentWater = WaterEngine.ocean;
			for (int i = 0; i < WaterEngine.lakes.Count; i++)
			{
				if (WaterEngine.lakes[i].IsInBounds(position))
				{
					WaterEngine.currentWater = WaterEngine.lakes[i];
				}
			}
			if (WaterEngine.currentWater)
			{
				WaterEngine.currentUnderWater = WaterEngine.currentWater.IsUnderWater(position);
			}
			else
			{
				WaterEngine.currentUnderWater = false;
			}
		}
	}
}
