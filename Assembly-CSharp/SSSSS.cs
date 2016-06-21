using System;
using UnityEngine;

[AddComponentMenu("Image Effects/SSSSS"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class SSSSS : MonoBehaviour
{
	private enum EffectPass
	{
		Convolution = 2,
		Composite = 1
	}

	public static SSSSS Instance;

	public float Radius = 2f;

	public float IBL_Debug = 0.2f;

	public int Iterations = 4;

	public int Downsampling = 1;

	public float ShadowDistance = 50f;

	private GameObject secondaryCamera;

	private Shader Shader_SSSSS_Camera;

	private Shader Shader_SSSSS_Skin;

	private Material Material_SSSSS_Camera;

	private RenderTextureFormat RTT_Format;

	public RenderTexture RTT_FinalBlur;

	public RenderTexture ReplaceShader_RTT;

	public bool ShowRTT;

	public bool Gamma2Linear = true;

	public bool ShowScreenControls;

	public bool UseDepthTest = true;

	public bool UseComplexSpecular = true;

	private float InitialFOV;

	private float FOV_Radius;

	private int ScreenX;

	private int ScreenY;

	private void OnDisable()
	{
		UnityEngine.Object.DestroyImmediate(this.Material_SSSSS_Camera);
	}

	private static void ToggleKeyword(bool toggle, string keywordON, string keywordOFF)
	{
		Shader.DisableKeyword((!toggle) ? keywordON : keywordOFF);
		Shader.EnableKeyword((!toggle) ? keywordOFF : keywordON);
	}

	private void CameraSetup()
	{
		this.secondaryCamera = GameObject.Find("SSSSS_Camera");
		if (!this.secondaryCamera)
		{
			this.secondaryCamera = new GameObject("SSSSS_Camera");
			this.secondaryCamera.hideFlags = HideFlags.DontSave;
			this.secondaryCamera.AddComponent<Camera>();
			this.secondaryCamera.GetComponent<Camera>().backgroundColor = new Color(0f, 0f, 0f, 1f);
		}
		this.secondaryCamera.GetComponent<Camera>().renderingPath = RenderingPath.Forward;
		this.secondaryCamera.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
		this.secondaryCamera.GetComponent<Camera>().hdr = false;
		this.secondaryCamera.GetComponent<Camera>().enabled = false;
		this.secondaryCamera.GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Character");
	}

	private void Start()
	{
		this.CameraSetup();
		SSSSS.ToggleKeyword(this.UseDepthTest, "DEPTH_TEST_ON", "DEPTH_TEST_OFF");
		SSSSS.ToggleKeyword(this.Gamma2Linear, "GAMMA2LINEAR_ON", "GAMMA2LINEAR_OFF");
		SSSSS.ToggleKeyword(this.UseComplexSpecular, "KELEMEN_KALOS_ON", "KELEMEN_KALOS_OFF");
	}

	private void Update()
	{
		this.FOV_Radius = this.InitialFOV / Camera.main.fieldOfView * this.Radius;
	}

	private void OnEnable()
	{
		SSSSS.Instance = this;
		this.InitialFOV = Camera.main.fieldOfView;
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			this.RTT_Format = RenderTextureFormat.ARGBHalf;
		}
		else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
		{
			this.RTT_Format = RenderTextureFormat.ARGBFloat;
		}
		else
		{
			this.RTT_Format = RenderTextureFormat.ARGB32;
		}
		Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
		this.Shader_SSSSS_Camera = Shader.Find("Hidden/SSSSS_Camera");
		if (this.Shader_SSSSS_Camera == null)
		{
			Debug.Log("#ERROR# Hidden/SSSSS_Camera Shader not found");
		}
		this.Material_SSSSS_Camera = new Material(this.Shader_SSSSS_Camera);
		this.Shader_SSSSS_Skin = Shader.Find("Hidden/Skin Composite");
		if (this.Shader_SSSSS_Skin == null)
		{
			Debug.Log("#ERROR# The Forest/SSSSS_Skin Shader not found");
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.ScreenX = source.width;
		this.ScreenY = source.height;
		Camera main = Camera.main;
		this.secondaryCamera.GetComponent<Camera>().transform.rotation = main.transform.rotation;
		this.secondaryCamera.GetComponent<Camera>().transform.position = main.transform.position;
		this.secondaryCamera.GetComponent<Camera>().fieldOfView = main.fieldOfView;
		if (this.RTT_FinalBlur != null)
		{
			RenderTexture.ReleaseTemporary(this.RTT_FinalBlur);
		}
		this.RTT_FinalBlur = RenderTexture.GetTemporary(this.ScreenX / (this.Downsampling + 1), this.ScreenY / (this.Downsampling + 1), 0, this.RTT_Format);
		if (this.Iterations > 0)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this.RTT_FinalBlur.width, this.RTT_FinalBlur.height, 0, this.RTT_Format);
			for (int i = 0; i < this.Iterations; i++)
			{
				this.Material_SSSSS_Camera.SetVector("_TexelOffsetScale", new Vector4(this.FOV_Radius / (float)this.ScreenX, 0f, 0f, 0f));
				Graphics.Blit((i != 0) ? this.RTT_FinalBlur : source, temporary, this.Material_SSSSS_Camera, 2);
				this.Material_SSSSS_Camera.SetVector("_TexelOffsetScale", new Vector4(0f, this.FOV_Radius / (float)this.ScreenY, 0f, 0f));
				Graphics.Blit(temporary, this.RTT_FinalBlur, this.Material_SSSSS_Camera, 2);
			}
			RenderTexture.ReleaseTemporary(temporary);
		}
		else
		{
			Graphics.Blit(source, this.RTT_FinalBlur);
		}
		Shader.SetGlobalTexture("_RTT", this.RTT_FinalBlur);
		this.ReplaceShader_RTT = RenderTexture.GetTemporary(this.ScreenX, this.ScreenY, 16, this.RTT_Format);
		this.secondaryCamera.GetComponent<Camera>().targetTexture = this.ReplaceShader_RTT;
		int shadowCascades = QualitySettings.shadowCascades;
		QualitySettings.shadowCascades = 1;
		float shadowDistance = QualitySettings.shadowDistance;
		QualitySettings.shadowDistance = this.ShadowDistance;
		this.secondaryCamera.GetComponent<Camera>().RenderWithShader(this.Shader_SSSSS_Skin, "OnlySkin");
		QualitySettings.shadowCascades = shadowCascades;
		QualitySettings.shadowDistance = shadowDistance;
		this.Material_SSSSS_Camera.SetTexture("Replaced", this.ReplaceShader_RTT);
		Graphics.Blit(source, destination, this.Material_SSSSS_Camera, 1);
		RenderTexture.ReleaseTemporary(this.ReplaceShader_RTT);
	}

	private void OnDestroy()
	{
		if (this.RTT_FinalBlur != null)
		{
			RenderTexture.ReleaseTemporary(this.RTT_FinalBlur);
		}
	}
}
