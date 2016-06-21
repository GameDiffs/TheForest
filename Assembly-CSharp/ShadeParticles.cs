using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(ParticleSystem))]
public class ShadeParticles : MonoBehaviour
{
	private Renderer rend;

	private Material Particle_Mat;

	[Header("Raycasted Shadows from Sun"), Space(5f)]
	public bool raycastShadows;

	public GameObject lightRef;

	private int DirLightStrength_PropID;

	private bool SunIsSetUp;

	public Vector3 SampleOffset = Vector3.zero;

	[Range(0f, 1f)]
	public float MinLight;

	[Range(0f, 1f)]
	public float MaxLight = 1f;

	[Space(10f)]
	public bool UseUniqueMaterial;

	[Header("2nd additional Light: Point Light"), Space(5f)]
	public GameObject importantPointLightRef;

	private GameObject currentImportantPointLightRef;

	private bool importantPointLightIsSetUp;

	private Light m_light01;

	private bool usesCookie01;

	public Texture customLightCookie01;

	private int CustomLightMatrix01_PropID;

	private int CustomLightPositionFalloff01_PropID;

	private int CustomLightColor01_PropID;

	private int CustomLightCookie01_PropID;

	private void Start()
	{
		this.Init();
		this.SetupPixelights();
	}

	private void Update()
	{
		this.CheckLights();
		this.Shade();
	}

	private void Init()
	{
		if (this.lightRef != null)
		{
			this.SunIsSetUp = true;
		}
		else
		{
			this.SunIsSetUp = false;
		}
		if (this.importantPointLightRef != null)
		{
			this.importantPointLightIsSetUp = true;
		}
		else
		{
			this.importantPointLightIsSetUp = false;
		}
		if (this.rend == null)
		{
			this.rend = base.GetComponent<Renderer>();
		}
		if (this.rend)
		{
			if (this.UseUniqueMaterial)
			{
				this.Particle_Mat = this.rend.material;
			}
			else
			{
				this.Particle_Mat = this.rend.sharedMaterial;
			}
			this.DirLightStrength_PropID = Shader.PropertyToID("_DirLightStrength");
			this.CustomLightMatrix01_PropID = Shader.PropertyToID("_CustomLightMatrix01");
			this.CustomLightPositionFalloff01_PropID = Shader.PropertyToID("_CustomLightPositionFalloff01");
			this.CustomLightColor01_PropID = Shader.PropertyToID("_CustomLightColor01");
			this.CustomLightCookie01_PropID = Shader.PropertyToID("_CustomLightCookie01");
		}
	}

	private void CheckLights()
	{
		if (this.currentImportantPointLightRef != this.importantPointLightRef)
		{
			this.m_light01 = this.importantPointLightRef.GetComponent<Light>();
			if (this.m_light01.type == LightType.Point)
			{
				this.usesCookie01 = false;
			}
			else
			{
				this.usesCookie01 = true;
			}
		}
		this.currentImportantPointLightRef = this.importantPointLightRef;
	}

	public void SetupPixelights()
	{
		if (this.importantPointLightRef != null)
		{
			this.m_light01 = this.importantPointLightRef.GetComponent<Light>();
		}
	}

	public void Shade()
	{
		if (this.rend.isVisible)
		{
			if (this.raycastShadows && this.SunIsSetUp)
			{
				Vector3 direction = this.lightRef.transform.rotation * new Vector3(0f, 0f, -1f);
				if (Physics.Raycast(base.transform.position + this.SampleOffset, direction, 100f))
				{
					this.Particle_Mat.SetFloat(this.DirLightStrength_PropID, this.MinLight);
				}
				else
				{
					this.Particle_Mat.SetFloat(this.DirLightStrength_PropID, this.MaxLight);
				}
			}
			if (this.importantPointLightIsSetUp)
			{
				ShadeParticles.UpdatePixelLight(this.Particle_Mat, this.importantPointLightRef.transform, this.m_light01, this.usesCookie01, this.customLightCookie01, this.CustomLightMatrix01_PropID, this.CustomLightPositionFalloff01_PropID, this.CustomLightColor01_PropID, this.CustomLightCookie01_PropID);
			}
		}
	}

	public static void UpdatePixelLight(Material particle_mat, Transform m_lightreftransform, Light m_light, bool usecookie, Texture cookietex, int matrixpropid, int lightpospropid, int lightcolpropid, int cookiepropid)
	{
		Matrix4x4 matrix4x = m_lightreftransform.localToWorldMatrix.inverse;
		float range = m_light.range;
		matrix4x = Matrix4x4.Perspective(m_light.spotAngle * 2f, 1f, 0f, range) * matrix4x;
		particle_mat.SetMatrix(matrixpropid, matrix4x);
		particle_mat.SetVector(lightpospropid, new Vector4(m_lightreftransform.position.x, m_lightreftransform.position.y, m_lightreftransform.position.z, range));
		float num = Mathf.GammaToLinearSpace(m_light.intensity) * 3.14159274f;
		Color linear = m_light.color.linear;
		particle_mat.SetVector(lightcolpropid, new Vector4
		{
			x = linear.r * num,
			y = linear.g * num,
			z = linear.b * num,
			w = num
		});
		if (usecookie)
		{
			particle_mat.SetTexture(cookiepropid, cookietex);
		}
	}
}
