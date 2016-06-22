using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Camera/Camera Motion Blur"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class CameraMotionBlur : PostEffectsBase
	{
		public enum MotionBlurFilter
		{
			CameraMotion,
			LocalBlur,
			Reconstruction,
			ReconstructionDX11,
			ReconstructionDisc
		}

		private static float MAX_RADIUS = 10f;

		public CameraMotionBlur.MotionBlurFilter filterType = CameraMotionBlur.MotionBlurFilter.Reconstruction;

		public bool preview;

		public Vector3 previewScale = Vector3.one;

		public float movementScale;

		public float rotationScale = 1f;

		public float maxVelocity = 8f;

		public float minVelocity = 0.1f;

		public float velocityScale = 0.375f;

		public float softZDistance = 0.005f;

		public int velocityDownsample = 1;

		public LayerMask excludeLayers = 0;

		private GameObject tmpCam;

		public Shader shader;

		public Shader dx11MotionBlurShader;

		public Shader replacementClear;

		private Material motionBlurMaterial;

		private Material dx11MotionBlurMaterial;

		public Texture2D noiseTexture;

		public float jitter = 0.05f;

		public bool showVelocity;

		public float showVelocityScale = 1f;

		private Matrix4x4 currentViewProjMat;

		private Matrix4x4 prevViewProjMat;

		private int prevFrameCount;

		private bool wasActive;

		private Vector3 prevFrameForward = Vector3.forward;

		private Vector3 prevFrameUp = Vector3.up;

		private Vector3 prevFramePos = Vector3.zero;

		private Camera _camera;

		private void CalculateViewProjection()
		{
			Matrix4x4 worldToCameraMatrix = this._camera.worldToCameraMatrix;
			Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this._camera.projectionMatrix, true);
			this.currentViewProjMat = gPUProjectionMatrix * worldToCameraMatrix;
		}

		private new void Start()
		{
			this.CheckResources();
			if (this._camera == null)
			{
				this._camera = base.GetComponent<Camera>();
			}
			this.wasActive = base.gameObject.activeInHierarchy;
			this.CalculateViewProjection();
			this.Remember();
			this.wasActive = false;
		}

		private void OnEnable()
		{
			if (this._camera == null)
			{
				this._camera = base.GetComponent<Camera>();
			}
			this._camera.depthTextureMode |= DepthTextureMode.Depth;
		}

		private void OnDisable()
		{
			if (null != this.motionBlurMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.motionBlurMaterial);
				this.motionBlurMaterial = null;
			}
			if (null != this.dx11MotionBlurMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.dx11MotionBlurMaterial);
				this.dx11MotionBlurMaterial = null;
			}
			if (null != this.tmpCam)
			{
				UnityEngine.Object.DestroyImmediate(this.tmpCam);
				this.tmpCam = null;
			}
		}

		public override bool CheckResources()
		{
			base.CheckSupport(true, true);
			this.motionBlurMaterial = base.CheckShaderAndCreateMaterial(this.shader, this.motionBlurMaterial);
			if (this.supportDX11 && this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11)
			{
				this.dx11MotionBlurMaterial = base.CheckShaderAndCreateMaterial(this.dx11MotionBlurShader, this.dx11MotionBlurMaterial);
			}
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				this.StartFrame();
			}
			RenderTextureFormat format = (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.RGHalf;
			RenderTexture temporary = RenderTexture.GetTemporary(CameraMotionBlur.divRoundUp(source.width, this.velocityDownsample), CameraMotionBlur.divRoundUp(source.height, this.velocityDownsample), 0, format);
			this.maxVelocity = Mathf.Max(2f, this.maxVelocity);
			float value = this.maxVelocity;
			bool flag = this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11 && this.dx11MotionBlurMaterial == null;
			int num;
			int height;
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.Reconstruction || flag || this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDisc)
			{
				this.maxVelocity = Mathf.Min(this.maxVelocity, CameraMotionBlur.MAX_RADIUS);
				num = CameraMotionBlur.divRoundUp(temporary.width, (int)this.maxVelocity);
				height = CameraMotionBlur.divRoundUp(temporary.height, (int)this.maxVelocity);
				value = (float)(temporary.width / num);
			}
			else
			{
				num = CameraMotionBlur.divRoundUp(temporary.width, (int)this.maxVelocity);
				height = CameraMotionBlur.divRoundUp(temporary.height, (int)this.maxVelocity);
				value = (float)(temporary.width / num);
			}
			RenderTexture temporary2 = RenderTexture.GetTemporary(num, height, 0, format);
			RenderTexture temporary3 = RenderTexture.GetTemporary(num, height, 0, format);
			temporary.filterMode = FilterMode.Point;
			temporary2.filterMode = FilterMode.Point;
			temporary3.filterMode = FilterMode.Point;
			if (this.noiseTexture)
			{
				this.noiseTexture.filterMode = FilterMode.Point;
			}
			source.wrapMode = TextureWrapMode.Clamp;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary3.wrapMode = TextureWrapMode.Clamp;
			temporary2.wrapMode = TextureWrapMode.Clamp;
			this.CalculateViewProjection();
			if (base.gameObject.activeInHierarchy && !this.wasActive)
			{
				this.Remember();
			}
			this.wasActive = base.gameObject.activeInHierarchy;
			Matrix4x4 matrix4x = Matrix4x4.Inverse(this.currentViewProjMat);
			this.motionBlurMaterial.SetMatrix("_InvViewProj", matrix4x);
			this.motionBlurMaterial.SetMatrix("_PrevViewProj", this.prevViewProjMat);
			this.motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", this.prevViewProjMat * matrix4x);
			this.motionBlurMaterial.SetFloat("_MaxVelocity", value);
			this.motionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", value);
			this.motionBlurMaterial.SetFloat("_MinVelocity", this.minVelocity);
			this.motionBlurMaterial.SetFloat("_VelocityScale", this.velocityScale);
			this.motionBlurMaterial.SetFloat("_Jitter", this.jitter);
			this.motionBlurMaterial.SetTexture("_NoiseTex", this.noiseTexture);
			this.motionBlurMaterial.SetTexture("_VelTex", temporary);
			this.motionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
			this.motionBlurMaterial.SetTexture("_TileTexDebug", temporary2);
			if (this.preview)
			{
				Matrix4x4 worldToCameraMatrix = this._camera.worldToCameraMatrix;
				Matrix4x4 identity = Matrix4x4.identity;
				identity.SetTRS(this.previewScale * 0.3333f, Quaternion.identity, Vector3.one);
				Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this._camera.projectionMatrix, true);
				this.prevViewProjMat = gPUProjectionMatrix * identity * worldToCameraMatrix;
				this.motionBlurMaterial.SetMatrix("_PrevViewProj", this.prevViewProjMat);
				this.motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", this.prevViewProjMat * matrix4x);
			}
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				Vector4 zero = Vector4.zero;
				float num2 = Vector3.Dot(base.transform.up, Vector3.up);
				Vector3 rhs = this.prevFramePos - base.transform.position;
				float magnitude = rhs.magnitude;
				float num3 = Vector3.Angle(base.transform.up, this.prevFrameUp) / this._camera.fieldOfView * ((float)source.width * 0.75f);
				zero.x = this.rotationScale * num3;
				num3 = Vector3.Angle(base.transform.forward, this.prevFrameForward) / this._camera.fieldOfView * ((float)source.width * 0.75f);
				zero.y = this.rotationScale * num2 * num3;
				num3 = Vector3.Angle(base.transform.forward, this.prevFrameForward) / this._camera.fieldOfView * ((float)source.width * 0.75f);
				zero.z = this.rotationScale * (1f - num2) * num3;
				if (magnitude > Mathf.Epsilon && this.movementScale > Mathf.Epsilon)
				{
					zero.w = this.movementScale * Vector3.Dot(base.transform.forward, rhs) * ((float)source.width * 0.5f);
					zero.x += this.movementScale * Vector3.Dot(base.transform.up, rhs) * ((float)source.width * 0.5f);
					zero.y += this.movementScale * Vector3.Dot(base.transform.right, rhs) * ((float)source.width * 0.5f);
				}
				if (this.preview)
				{
					this.motionBlurMaterial.SetVector("_BlurDirectionPacked", new Vector4(this.previewScale.y, this.previewScale.x, 0f, this.previewScale.z) * 0.5f * this._camera.fieldOfView);
				}
				else
				{
					this.motionBlurMaterial.SetVector("_BlurDirectionPacked", zero);
				}
			}
			else
			{
				Graphics.Blit(source, temporary, this.motionBlurMaterial, 0);
				Camera camera = null;
				if (this.excludeLayers.value != 0)
				{
					camera = this.GetTmpCam();
				}
				if (camera && this.excludeLayers.value != 0 && this.replacementClear && this.replacementClear.isSupported)
				{
					camera.targetTexture = temporary;
					camera.cullingMask = this.excludeLayers;
					camera.RenderWithShader(this.replacementClear, string.Empty);
				}
			}
			if (!this.preview && Time.frameCount != this.prevFrameCount)
			{
				this.prevFrameCount = Time.frameCount;
				this.Remember();
			}
			source.filterMode = FilterMode.Bilinear;
			if (this.showVelocity)
			{
				this.motionBlurMaterial.SetFloat("_DisplayVelocityScale", this.showVelocityScale);
				Graphics.Blit(temporary, destination, this.motionBlurMaterial, 1);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11 && !flag)
			{
				this.dx11MotionBlurMaterial.SetFloat("_MinVelocity", this.minVelocity);
				this.dx11MotionBlurMaterial.SetFloat("_VelocityScale", this.velocityScale);
				this.dx11MotionBlurMaterial.SetFloat("_Jitter", this.jitter);
				this.dx11MotionBlurMaterial.SetTexture("_NoiseTex", this.noiseTexture);
				this.dx11MotionBlurMaterial.SetTexture("_VelTex", temporary);
				this.dx11MotionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
				this.dx11MotionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				this.dx11MotionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", value);
				Graphics.Blit(temporary, temporary2, this.dx11MotionBlurMaterial, 0);
				Graphics.Blit(temporary2, temporary3, this.dx11MotionBlurMaterial, 1);
				Graphics.Blit(source, destination, this.dx11MotionBlurMaterial, 2);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.Reconstruction || flag)
			{
				this.motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				Graphics.Blit(temporary, temporary2, this.motionBlurMaterial, 2);
				Graphics.Blit(temporary2, temporary3, this.motionBlurMaterial, 3);
				Graphics.Blit(source, destination, this.motionBlurMaterial, 4);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				Graphics.Blit(source, destination, this.motionBlurMaterial, 6);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDisc)
			{
				this.motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				Graphics.Blit(temporary, temporary2, this.motionBlurMaterial, 2);
				Graphics.Blit(temporary2, temporary3, this.motionBlurMaterial, 3);
				Graphics.Blit(source, destination, this.motionBlurMaterial, 7);
			}
			else
			{
				Graphics.Blit(source, destination, this.motionBlurMaterial, 5);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
		}

		private void Remember()
		{
			this.prevViewProjMat = this.currentViewProjMat;
			this.prevFrameForward = base.transform.forward;
			this.prevFrameUp = base.transform.up;
			this.prevFramePos = base.transform.position;
		}

		private Camera GetTmpCam()
		{
			if (this.tmpCam == null)
			{
				string name = "_" + this._camera.name + "_MotionBlurTmpCam";
				GameObject y = GameObject.Find(name);
				if (null == y)
				{
					this.tmpCam = new GameObject(name, new Type[]
					{
						typeof(Camera)
					});
				}
				else
				{
					this.tmpCam = y;
				}
			}
			this.tmpCam.hideFlags = HideFlags.DontSave;
			this.tmpCam.transform.position = this._camera.transform.position;
			this.tmpCam.transform.rotation = this._camera.transform.rotation;
			this.tmpCam.transform.localScale = this._camera.transform.localScale;
			this.tmpCam.GetComponent<Camera>().CopyFrom(this._camera);
			this.tmpCam.GetComponent<Camera>().enabled = false;
			this.tmpCam.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
			this.tmpCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
			return this.tmpCam.GetComponent<Camera>();
		}

		private void StartFrame()
		{
			this.prevFramePos = Vector3.Slerp(this.prevFramePos, base.transform.position, 0.75f);
		}

		private static int divRoundUp(int x, int d)
		{
			return (x + d - 1) / d;
		}
	}
}
