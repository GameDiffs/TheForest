using System;
using UnityEngine;

[ExecuteInEditMode]
public class SkylightingAndGICamera : MonoBehaviour
{
	private enum RenderMode
	{
		Skylight,
		GlobalIllumination
	}

	private Camera shadowCam;

	private GameObject shadowCamGameObject;

	private int skylightShadowMapResolution = 128;

	private int giBuffersResolution = 128;

	private RenderTexture skylightDepthTexture;

	private RenderTexture giDepthTexture;

	private RenderTexture giFluxTexture;

	private RenderTexture giNormalsTexture;

	private RenderBuffer giFluxBuffer;

	private RenderBuffer giNormalsBuffer;

	private RenderBuffer giDepthBuffer;

	public LayerMask skylightCullingMask;

	public LayerMask GICullingMask;

	private Material testMaterial;

	private bool initialized;

	public static Vector3 sunlightVector;

	private Vector3 snapPositionSkylight;

	private Vector3 snapPositionGI;

	public Texture2D uniformNoise;

	[Range(0f, 5f)]
	public float globalIlluminationGain;

	[Range(0f, 1f)]
	public float skylightOcclusionAmount;

	public bool GIEnabled;

	public bool OcclusionEnabled;

	private SkylightingAndGICamera.RenderMode currentRenderMode = SkylightingAndGICamera.RenderMode.GlobalIllumination;

	private void Start()
	{
	}

	private void Update()
	{
		Shader.SetGlobalInt("_IsRenderingSkylight", 0);
	}

	private void OnEnable()
	{
		if (!this.initialized)
		{
			this.Init();
		}
	}

	private void Init()
	{
		this.shadowCamGameObject = new GameObject();
		this.shadowCamGameObject.AddComponent(typeof(Camera));
		this.shadowCam = this.shadowCamGameObject.GetComponent<Camera>();
		this.shadowCamGameObject.name = "SKYLIGHT_GI_SHADOWCAM";
		this.shadowCamGameObject.hideFlags = HideFlags.HideAndDontSave;
		this.shadowCam.enabled = false;
		this.shadowCam.orthographic = true;
		this.shadowCam.orthographicSize = 100f;
		this.shadowCam.clearFlags = CameraClearFlags.Color;
		this.shadowCam.backgroundColor = new Color(0f, 0f, 0f, 1f);
		this.shadowCam.farClipPlane = 500f;
		this.shadowCam.cullingMask = this.skylightCullingMask;
		this.shadowCam.useOcclusionCulling = false;
		this.skylightDepthTexture = new RenderTexture(this.skylightShadowMapResolution, this.skylightShadowMapResolution, 16, RenderTextureFormat.Depth);
		this.skylightDepthTexture.wrapMode = TextureWrapMode.Clamp;
		this.skylightDepthTexture.filterMode = FilterMode.Bilinear;
		this.skylightDepthTexture.Create();
		this.giDepthTexture = new RenderTexture(this.giBuffersResolution, this.giBuffersResolution, 16, RenderTextureFormat.Depth);
		this.giDepthTexture.wrapMode = TextureWrapMode.Clamp;
		this.giDepthTexture.filterMode = FilterMode.Point;
		this.giDepthTexture.Create();
		this.giDepthBuffer = this.giDepthTexture.depthBuffer;
		this.giFluxTexture = new RenderTexture(this.giBuffersResolution, this.giBuffersResolution, 0, RenderTextureFormat.ARGB32);
		this.giFluxTexture.wrapMode = TextureWrapMode.Clamp;
		this.giFluxTexture.filterMode = FilterMode.Bilinear;
		this.giFluxTexture.useMipMap = true;
		this.giFluxTexture.Create();
		this.giFluxBuffer = this.giFluxTexture.colorBuffer;
		this.initialized = true;
	}

	private void OnPreRender()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		Shader.SetGlobalTexture("_UniformNoise", this.uniformNoise);
		RenderTexture active = RenderTexture.active;
		if (this.currentRenderMode == SkylightingAndGICamera.RenderMode.Skylight)
		{
			if (this.OcclusionEnabled)
			{
				Shader.SetGlobalInt("_IsRenderingSkylight", 1);
				Shader.DisableKeyword("SKY_OCCLUSION_OFF");
				this.shadowCam.cullingMask = this.skylightCullingMask;
				Vector3 vector = base.transform.position + Vector3.up * 100f;
				float num = this.shadowCam.orthographicSize * 2f;
				float num2 = num / (float)this.skylightShadowMapResolution;
				vector.x -= vector.x % num2;
				vector.y -= vector.x % num2;
				vector.z -= vector.x % num2;
				if (vector != this.snapPositionSkylight)
				{
					this.shadowCamGameObject.transform.position = vector;
					this.shadowCamGameObject.transform.LookAt(base.transform.position, Vector3.forward);
				}
				this.snapPositionSkylight = vector;
				this.shadowCam.depthTextureMode = DepthTextureMode.Depth;
				this.shadowCam.targetTexture = this.skylightDepthTexture;
				this.shadowCam.Render();
				Shader.SetGlobalMatrix("_SkyshadowMatrix", this.shadowCam.worldToCameraMatrix);
				Shader.SetGlobalMatrix("_SkyshadowInverseMatrix", this.shadowCam.worldToCameraMatrix.inverse);
				Shader.SetGlobalMatrix("_SkyshadowProjectionMatrix", this.shadowCam.projectionMatrix);
				Shader.SetGlobalMatrix("_SkyshadowProjectionMatrixInverse", this.shadowCam.projectionMatrix.inverse);
				Shader.SetGlobalTexture("_SkyshadowDepthTexture", this.skylightDepthTexture);
				Shader.SetGlobalFloat("_SkylightOcclusionAmount", this.skylightOcclusionAmount);
				Shader.SetGlobalInt("_IsRenderingSkylight", 0);
			}
			else
			{
				Shader.EnableKeyword("SKY_OCCLUSION_OFF");
			}
		}
		else if (this.currentRenderMode == SkylightingAndGICamera.RenderMode.GlobalIllumination)
		{
			if (this.GIEnabled)
			{
				Shader.DisableKeyword("GI_OFF");
				Shader.SetGlobalInt("_IsRenderingGIBuffers", 1);
				this.shadowCam.cullingMask = this.GICullingMask;
				Vector3 position = base.transform.position + Vector3.Normalize(SkylightingAndGICamera.sunlightVector) * 100f;
				this.shadowCamGameObject.transform.position = position;
				this.shadowCamGameObject.transform.LookAt(base.transform.position, Vector3.up);
				this.shadowCam.renderingPath = RenderingPath.Forward;
				this.shadowCam.depthTextureMode = DepthTextureMode.Depth;
				Graphics.SetRenderTarget(this.giFluxBuffer, this.giDepthBuffer);
				this.shadowCam.SetTargetBuffers(this.giFluxBuffer, this.giDepthBuffer);
				this.shadowCam.Render();
				Shader.SetGlobalMatrix("_GIMatrix", this.shadowCam.worldToCameraMatrix);
				Shader.SetGlobalMatrix("_GIInverseMatrix", this.shadowCam.worldToCameraMatrix.inverse);
				Shader.SetGlobalMatrix("_GIProjectionMatrix", this.shadowCam.projectionMatrix);
				Shader.SetGlobalMatrix("_GIProjectionMatrixInverse", this.shadowCam.projectionMatrix.inverse);
				Shader.SetGlobalTexture("_GIFluxTexture", this.giFluxTexture);
				Shader.SetGlobalTexture("_GIDepthTexture", this.giDepthTexture);
				Shader.SetGlobalFloat("_GIGain", this.globalIlluminationGain);
				Shader.SetGlobalInt("_IsRenderingGIBuffers", 0);
			}
			else
			{
				Shader.EnableKeyword("GI_OFF");
			}
		}
		RenderTexture.active = active;
		if (this.currentRenderMode == SkylightingAndGICamera.RenderMode.Skylight)
		{
			if (this.GIEnabled)
			{
				this.currentRenderMode = SkylightingAndGICamera.RenderMode.GlobalIllumination;
			}
		}
		else if (this.currentRenderMode == SkylightingAndGICamera.RenderMode.GlobalIllumination)
		{
			this.currentRenderMode = SkylightingAndGICamera.RenderMode.Skylight;
		}
	}
}
