using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Atmosphere"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class Atmosphere : MonoBehaviour
{
	public Shader fogShader;

	public Material fogMaterial;

	public float ViewDistance = 2000f;

	public float TransitionDistance = 1000f;

	public float StarsIntensity;

	public float heightScale = 0.031f;

	public float height = 5.16f;

	public float mieDirectionalG = 0.75f;

	public float InscatteringIntensity = 1f;

	public float DirectionalInscatteringStartDistance = 1000f;

	public Light Sun;

	public Light Moon;

	private Vector3 L = Vector3.up;

	public Color FogColor = Color.grey;

	public Color NoonFogColor = Color.grey;

	public Color SunsetFogColor = Color.grey;

	public Color InscatteringColor = Color.white;

	public Color SunsetInscatteringColor = Color.white;

	public Color NoonInscatteringColor = Color.white;

	public bool IsolateFog;

	public float Exposure = 0.004f;

	public float contrast = 6f;

	public float reileighCoefficient = 0.053f;

	public float mieCoefficient = 1f;

	public float WorldHeight = -1000f;

	public float Molecules = 2.55f;

	public float turbidity = 1f;

	public float ZenithScale = 1f;

	public float SunIntensity = 1000f;

	public float SunExponent = 2000f;

	private float CAMERA_NEAR = 0.5f;

	private float CAMERA_FAR = 50f;

	private float CAMERA_FOV = 60f;

	private float CAMERA_ASPECT_RATIO = 1.333333f;

	public Color DayColor = Color.white;

	public Color SunsetColor = Color.black;

	public Color NightColor = Color.blue;

	public float Lx;

	public float Ly;

	public float Lz;

	private float LdotUp;

	private float LdotDown;

	public float SunLightIntensity = 1f;

	public Color NoonLightColor = Color.white;

	public Color SunsetLightColor = Color.white;

	public GameObject OuterSpaceMesh;

	public float OceanLevel;

	public float Deepness = 120f;

	public float UnderwaterViewDistance = 50f;

	public Color DeepColor = Color.blue;

	public Color ShoreColor = Color.cyan;

	public GameObject OceanMesh;

	public GameObject AtmosphereMesh;

	public bool UseAtmosphereMesh;

	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		this.fogShader = Shader.Find("Hidden/Atmosphere");
		if (this.fogShader == null)
		{
			Debug.Log("No encuentra el xaderl Atmosphere");
		}
		this.fogMaterial = new Material(this.fogShader);
		if (this.Sun != null)
		{
			this.Lx = this.Sun.transform.localRotation.eulerAngles.x;
			this.Ly = this.Sun.transform.localRotation.eulerAngles.y;
			this.Lz = this.Sun.transform.localRotation.eulerAngles.z;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CAMERA_NEAR = base.GetComponent<Camera>().nearClipPlane;
		this.CAMERA_FAR = base.GetComponent<Camera>().farClipPlane;
		this.CAMERA_FOV = base.GetComponent<Camera>().fieldOfView;
		this.CAMERA_ASPECT_RATIO = base.GetComponent<Camera>().aspect;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = this.CAMERA_FOV * 0.5f;
		Vector3 b = base.GetComponent<Camera>().transform.right * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f) * this.CAMERA_ASPECT_RATIO;
		Vector3 b2 = base.GetComponent<Camera>().transform.up * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f);
		Vector3 vector = base.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR - b + b2;
		float d = vector.magnitude * this.CAMERA_FAR / this.CAMERA_NEAR;
		vector.Normalize();
		vector *= d;
		Vector3 vector2 = base.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR + b + b2;
		vector2.Normalize();
		vector2 *= d;
		Vector3 vector3 = base.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR + b - b2;
		vector3.Normalize();
		vector3 *= d;
		Vector3 vector4 = base.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR - b - b2;
		vector4.Normalize();
		vector4 *= d;
		identity.SetRow(0, vector);
		identity.SetRow(1, vector2);
		identity.SetRow(2, vector3);
		identity.SetRow(3, vector4);
		this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		this.fogMaterial.SetVector("_CameraWS", base.GetComponent<Camera>().transform.position);
		this.fogMaterial.SetVector("_Y", new Vector4(this.height, this.heightScale));
		this.fogMaterial.SetFloat("_GlobalDensity", this.ViewDistance);
		this.mieDirectionalG = Mathf.Clamp(this.mieDirectionalG, 0f, 0.999f);
		this.InscatteringIntensity = Mathf.Clamp(this.InscatteringIntensity, 0f, 30f);
		this.fogMaterial.SetFloat("mieDirectionalG", this.mieDirectionalG);
		this.fogMaterial.SetFloat("InscatteringIntensity", this.InscatteringIntensity);
		this.fogMaterial.SetColor("_FogColor", this.FogColor);
		this.fogMaterial.SetColor("InscatteringColor", this.InscatteringColor);
		this.fogMaterial.SetFloat("DirectionalInscatteringStartDistance", this.DirectionalInscatteringStartDistance);
		this.fogMaterial.SetFloat("reileighCoefficient", this.reileighCoefficient);
		this.fogMaterial.SetFloat("mieCoefficient", this.mieCoefficient);
		this.fogMaterial.SetFloat("Molecules", this.Molecules);
		this.fogMaterial.SetFloat("turbidity", this.turbidity);
		this.fogMaterial.SetFloat("Exposure", this.Exposure);
		this.fogMaterial.SetFloat("contrast", this.contrast);
		this.fogMaterial.SetFloat("TransitionDistance", this.TransitionDistance);
		this.fogMaterial.SetFloat("SunIntensity", (!this.isDaytime()) ? 0f : this.SunIntensity);
		this.fogMaterial.SetFloat("SunExponent", this.SunExponent);
		this.fogMaterial.SetFloat("WorldHeight", this.WorldHeight);
		this.fogMaterial.SetFloat("SunEditorIntensity", this.SunLightIntensity);
		this.fogMaterial.SetFloat("OceanLevel", this.OceanLevel);
		this.fogMaterial.SetFloat("Deepness", this.Deepness);
		this.fogMaterial.SetColor("DeepColor", this.DeepColor);
		this.fogMaterial.SetColor("ShoreColor", this.ShoreColor);
		this.fogMaterial.SetFloat("UnderwaterViewDistance", this.UnderwaterViewDistance);
		this.fogMaterial.SetFloat("isNighttime", (float)((!this.isDaytime()) ? 1 : 0));
		if (this.Sun == null)
		{
			Debug.Log("Set the main light");
		}
		else
		{
			this.L = -this.Sun.transform.forward;
			this.LdotUp = Mathf.Clamp01(Vector3.Dot(this.L, Vector3.up));
			this.LdotDown = Mathf.Clamp01(Vector3.Dot(this.L, Vector3.down));
		}
		this.fogMaterial.SetVector("L", this.L);
		this.fogMaterial.SetTexture("frameBuffer", source);
		if (this.IsolateFog)
		{
			this.fogMaterial.SetTexture("frameBuffer", null);
		}
		Atmosphere.CustomGraphicsBlit(source, destination, this.fogMaterial, 0);
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (!(this.Sun == null))
		{
			this.Sun.transform.rotation = Quaternion.Euler(new Vector3(this.Lx, this.Ly, this.Lz));
			this.Sun.color = Color.Lerp(this.SunsetLightColor, this.NoonLightColor, this.LdotUp);
			this.Sun.intensity = Mathf.Lerp(0f, this.SunLightIntensity, this.LdotUp);
			this.InscatteringColor = Color.Lerp(Color.Lerp(this.SunsetInscatteringColor, this.NoonInscatteringColor, this.LdotUp), Color.black, this.LdotDown) * this.LdotUp;
			this.FogColor = Color.Lerp(Color.Lerp(this.SunsetFogColor, this.NoonFogColor, this.LdotUp), Color.black, Mathf.Clamp(this.LdotDown * 50f, 0f, 1f));
		}
		if (this.OuterSpaceMesh != null)
		{
			this.OuterSpaceMesh.transform.rotation = Quaternion.Euler(new Vector3(this.Lx, this.Ly, this.Lz));
		}
		if (this.Moon == null)
		{
		}
		if (this.OuterSpaceMesh != null)
		{
			this.OuterSpaceMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("Intensity", this.StarsIntensity * Mathf.Clamp(this.LdotDown * 50f, 0f, 1f));
			if (this.isDaytime())
			{
				this.Sun.enabled = true;
				this.Moon.enabled = false;
				this.OuterSpaceMesh.gameObject.SetActive(false);
				RenderSettings.ambientLight = Color.Lerp(this.SunsetColor, this.DayColor, this.LdotUp);
			}
			if (!this.isDaytime())
			{
				this.Sun.enabled = false;
				this.Moon.enabled = true;
				RenderSettings.ambientLight = Color.Lerp(this.SunsetColor, this.NightColor, Mathf.Clamp(this.LdotDown * 50f, 0f, 1f));
				this.OuterSpaceMesh.gameObject.SetActive(true);
			}
		}
		else
		{
			Debug.Log("Set the Stars mesh");
		}
		if (this.OceanMesh == null)
		{
			Debug.Log("Set the ocean plane");
		}
		else
		{
			this.OceanMesh.transform.position = new Vector3(this.OceanMesh.transform.localPosition.x, this.OceanLevel, this.OceanMesh.transform.localPosition.z);
		}
		if (this.UseAtmosphereMesh)
		{
			if (this.AtmosphereMesh)
			{
				this.AtmosphereMesh.gameObject.SetActive(true);
				this.fogMaterial.SetVector("_Y", new Vector4(this.height, this.heightScale));
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("_GlobalDensity", this.ViewDistance);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("mieDirectionalG", this.mieDirectionalG);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("InscatteringIntensity", this.InscatteringIntensity);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetColor("_FogColor", this.FogColor);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetColor("InscatteringColor", this.InscatteringColor);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("DirectionalInscatteringStartDistance", this.DirectionalInscatteringStartDistance);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("reileighCoefficient", this.reileighCoefficient);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("mieCoefficient", this.mieCoefficient);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("Molecules", this.Molecules);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("turbidity", this.turbidity);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("Exposure", this.Exposure);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("contrast", this.contrast);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("TransitionDistance", this.TransitionDistance);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("SunIntensity", (!this.isDaytime()) ? 0f : this.SunIntensity);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("SunExponent", this.SunExponent);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("WorldHeight", this.WorldHeight);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("SunEditorIntensity", this.SunLightIntensity);
				this.AtmosphereMesh.GetComponent<Renderer>().sharedMaterial.SetVector("L", this.L);
			}
			else
			{
				Debug.Log("Set the atmosphere mesh");
			}
		}
		else
		{
			this.AtmosphereMesh.gameObject.SetActive(false);
		}
		if (!this.isDaytime())
		{
			this.AtmosphereMesh.gameObject.SetActive(false);
		}
	}

	private bool isDaytime()
	{
		bool result = false;
		if (this.LdotUp > 0f)
		{
			result = true;
		}
		if (this.LdotDown > 0f)
		{
			result = false;
		}
		return result;
	}

	private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0f, 0f);
		GL.Vertex3(0f, 0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0f);
		GL.Vertex3(1f, 0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}
}
