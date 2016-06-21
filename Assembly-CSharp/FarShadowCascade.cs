using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class FarShadowCascade : MonoBehaviour
{
	public enum Dimension
	{
		_256 = 256,
		_512 = 512,
		_1024 = 1024,
		_2048 = 2048,
		_4096 = 4096
	}

	public bool enableFarShadows = true;

	public FarShadowCascade.Dimension shadowMapSize = FarShadowCascade.Dimension._1024;

	public float FarCascadeDistance = 200f;

	private float FarCascadeDepthBias = 0.01f;

	private float biasLerp;

	public float MinFarCascadeDepthBias = 0.01f;

	public float MaxFarCascadeDepthBias = 0.03f;

	public int refresh = 10;

	private int c_refresh;

	public LayerMask CullingMask;

	private Camera eyeCamera;

	private RenderTexture m_shadowTexture;

	private Camera lightCamera;

	private Matrix4x4 m_shadowSpaceMatrix;

	private Matrix4x4 m_shadowMatrix;

	private Shader m_depthShader;

	private GameObject shadowCameraGO;

	private float last_boundingRadius;

	private Vector2 last_lightWorldOrigin = new Vector2(0f, 0f);

	public Texture TopShadowTexture;

	private float bb_quad;

	private float bb_billboard = 1f;

	private void OnPreCull()
	{
		this.SetShadowCamera();
	}

	private void Init()
	{
		this.m_shadowMatrix = Matrix4x4.identity;
		this.m_depthShader = Shader.Find("Hidden/Far Shadow Depth Only");
		if (this.shadowCameraGO == null)
		{
			this.shadowCameraGO = new GameObject("__Far_Shadow Camera - " + base.name);
			this.shadowCameraGO.hideFlags = HideFlags.DontSave;
		}
		this.lightCamera = this.shadowCameraGO.AddComponent<Camera>();
		this.lightCamera.renderingPath = RenderingPath.Forward;
		this.lightCamera.clearFlags = CameraClearFlags.Depth;
		this.lightCamera.depthTextureMode = DepthTextureMode.None;
		this.lightCamera.useOcclusionCulling = false;
		this.lightCamera.cullingMask = this.CullingMask;
		this.lightCamera.orthographic = true;
		this.lightCamera.depth = -10f;
		this.lightCamera.aspect = 1f;
		this.lightCamera.SetReplacementShader(this.m_depthShader, "RenderType");
		this.lightCamera.enabled = false;
		this.eyeCamera = base.GetComponent<Camera>();
	}

	private void Awake()
	{
		if (!this.lightCamera)
		{
			this.Init();
		}
		if (this.TopShadowTexture)
		{
			Shader.SetGlobalTexture("_TopShadowTexture", this.TopShadowTexture);
		}
	}

	private void OnEnable()
	{
		this.AllocateTarget();
	}

	private void OnDisable()
	{
		this.ReleaseTarget();
	}

	private void OnDestroy()
	{
		if (this.lightCamera)
		{
			UnityEngine.Object.DestroyImmediate(this.lightCamera.gameObject);
		}
	}

	private void OnValidate()
	{
		if (!Application.isPlaying || !this.lightCamera)
		{
			return;
		}
		this.ReleaseTarget();
		this.AllocateTarget();
	}

	private void AllocateTarget()
	{
		if (this.lightCamera)
		{
			this.m_shadowTexture = new RenderTexture((int)this.shadowMapSize, (int)this.shadowMapSize, 16, RenderTextureFormat.Shadowmap, RenderTextureReadWrite.Linear);
			this.m_shadowTexture.filterMode = FilterMode.Bilinear;
			this.m_shadowTexture.useMipMap = false;
			this.m_shadowTexture.generateMips = false;
			this.lightCamera.targetTexture = this.m_shadowTexture;
		}
	}

	private void ReleaseTarget()
	{
		if (this.lightCamera)
		{
			this.lightCamera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(this.m_shadowTexture);
			this.m_shadowTexture = null;
		}
	}

	private void SetFarShadowMatrix(float focusRadius, float fardistance)
	{
		this.lightCamera.projectionMatrix = GL.GetGPUProjectionMatrix(Matrix4x4.Ortho(-focusRadius, focusRadius, -focusRadius, focusRadius, 0f, focusRadius * 2f), false);
		bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9;
		bool flag2 = flag || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11;
		float num = (!flag) ? 0f : (0.5f / (float)this.shadowMapSize);
		float z = (!flag2) ? 0.5f : 1f;
		float num2 = (!flag2) ? 0.5f : 0f;
		float num3 = -this.FarCascadeDepthBias;
		this.m_shadowSpaceMatrix.SetRow(0, new Vector4(0.5f, 0f, 0f, 0.5f + num));
		this.m_shadowSpaceMatrix.SetRow(1, new Vector4(0f, 0.5f, 0f, 0.5f + num));
		this.m_shadowSpaceMatrix.SetRow(2, new Vector4(0f, 0f, z, num2 + num3));
		this.m_shadowSpaceMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		Matrix4x4 worldToCameraMatrix = this.lightCamera.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this.lightCamera.projectionMatrix, false);
		this.m_shadowMatrix = this.m_shadowSpaceMatrix * gPUProjectionMatrix * worldToCameraMatrix;
	}

	private void SetShadowCamera()
	{
		if (this.eyeCamera.renderingPath == RenderingPath.DeferredShading && this.enableFarShadows)
		{
			Shader.EnableKeyword("_USING_DEFERREDSHADING");
			this.shadowCameraGO.transform.rotation = Quaternion.LookRotation(Sunshine.Instance.SunLight.transform.forward);
			Transform transform = this.shadowCameraGO.transform;
			SunshineMath.BoundingSphere boundingSphere = default(SunshineMath.BoundingSphere);
			boundingSphere = SunshineMath.FrustumBoundingSphereBinarySearch(this.eyeCamera, this.eyeCamera.nearClipPlane, this.FarCascadeDistance, true, 0f, 0.01f, 20);
			float num = SunshineMath.QuantizeValueWithoutFlicker(boundingSphere.radius, 100, this.last_boundingRadius);
			this.last_boundingRadius = num;
			float num2 = num * 2f;
			transform.position = boundingSphere.origin;
			transform.position = this.eyeCamera.transform.position - Sunshine.Instance.SunLight.transform.forward * this.FarCascadeDistance * 0.5f;
			Vector3 vector = transform.InverseTransformPoint(Vector3.zero);
			float step = num2 / (float)this.shadowMapSize;
			vector.x = SunshineMath.QuantizeValueWithoutFlicker(vector.x, step, this.last_lightWorldOrigin.x);
			vector.y = SunshineMath.QuantizeValueWithoutFlicker(vector.y, step, this.last_lightWorldOrigin.y);
			this.last_lightWorldOrigin = vector;
			transform.position -= transform.TransformPoint(vector);
			Vector3 vector2 = transform.InverseTransformPoint(boundingSphere.origin);
			transform.position += transform.forward * (vector2.z - (boundingSphere.radius + this.lightCamera.nearClipPlane));
			this.lightCamera.orthographicSize = num2 * 0.5f;
			this.lightCamera.nearClipPlane = this.eyeCamera.nearClipPlane;
			this.lightCamera.farClipPlane = (boundingSphere.radius + this.lightCamera.nearClipPlane) * 2f;
			this.lightCamera.cullingMask = this.CullingMask;
			this.bb_quad = Vector3.Dot(-Sunshine.Instance.SunLight.transform.forward, Vector3.up);
			this.biasLerp = Mathf.Abs(this.bb_quad);
			this.bb_quad = Mathf.Clamp01(Mathf.Clamp01(this.bb_quad + 0.0001f) - 0.75f) * 4f;
			this.bb_billboard = 1f - this.bb_quad;
			Shader.SetGlobalVector("_BillboardShadeFadeFactors", new Vector4(this.bb_billboard, this.bb_quad, 0f, 0f));
			this.biasLerp = Mathf.Clamp01(Mathf.Abs(0.7071f - this.biasLerp) * 4f);
			this.FarCascadeDepthBias = Mathf.Lerp(this.MaxFarCascadeDepthBias, this.MinFarCascadeDepthBias, this.biasLerp);
			this.SetFarShadowMatrix(boundingSphere.radius, this.FarCascadeDistance);
			float shadowDistance = QualitySettings.shadowDistance;
			QualitySettings.shadowDistance = 0f;
			this.lightCamera.Render();
			QualitySettings.shadowDistance = shadowDistance;
			Shader.SetGlobalTexture("_FarCascade", this.m_shadowTexture);
			Shader.SetGlobalMatrix("_FarCascadeMatrix", this.m_shadowMatrix);
		}
		else
		{
			Shader.DisableKeyword("_USING_DEFERREDSHADING");
		}
		float num3 = QualitySettings.shadowDistance * 0.125f;
		Shader.SetGlobalVector("_FarCascadeBlendValues", new Vector4(QualitySettings.shadowDistance - num3 * 2f, num3, this.FarCascadeDistance - this.FarCascadeDistance * 0.275f));
	}
}
