using System;
using UnityEngine;

public class SyncCustomLitParticles : MonoBehaviour
{
	public GameObject GlobalPrimaryParticleLight;

	private GameObject current_GlobalPrimaryParticleLight;

	private Light m_light;

	private Texture m_cookie;

	public Texture StandardSpotCookie;

	private void Start()
	{
	}

	private void Update()
	{
		this.CheckForLightChanges();
		SyncCustomLitParticles.UpdateGlobalParticleLight(this.GlobalPrimaryParticleLight.transform, this.m_light, true, this.m_cookie);
	}

	private void GetPrimaryLight()
	{
		GameObject gameObject = GameObject.Find("PlayerFlashLight");
		GameObject globalPrimaryParticleLight = GameObject.Find("PlayerTorchLight");
		Light component = gameObject.GetComponent<Light>();
		if (component.enabled)
		{
			this.GlobalPrimaryParticleLight = gameObject;
		}
		else
		{
			this.GlobalPrimaryParticleLight = globalPrimaryParticleLight;
		}
	}

	public void CheckForLightChanges()
	{
		if (this.GlobalPrimaryParticleLight != this.current_GlobalPrimaryParticleLight)
		{
			this.m_light = this.GlobalPrimaryParticleLight.GetComponent<Light>();
			if (this.m_light.cookie == null && this.m_light.type == LightType.Spot)
			{
				this.m_cookie = this.StandardSpotCookie;
			}
			else
			{
				this.m_cookie = this.m_light.cookie;
			}
		}
		this.current_GlobalPrimaryParticleLight = this.GlobalPrimaryParticleLight;
	}

	public static void UpdateGlobalParticleLight(Transform m_lightreftransform, Light m_light, bool usecookie, Texture cookietex)
	{
		Matrix4x4 matrix4x = m_lightreftransform.localToWorldMatrix.inverse;
		float range = m_light.range;
		matrix4x = Matrix4x4.Perspective(m_light.spotAngle * 2f, 1f, 0f, range) * matrix4x;
		Shader.SetGlobalMatrix("_G_CustomLightMatrix", matrix4x);
		Shader.SetGlobalVector("_G_CustomLightPositionFalloff", new Vector4(m_lightreftransform.position.x, m_lightreftransform.position.y, m_lightreftransform.position.z, range));
		float num = Mathf.GammaToLinearSpace(m_light.intensity) * 3.14159274f;
		Color linear = m_light.color.linear;
		Shader.SetGlobalVector("_G_CustomLightColor", new Vector4
		{
			x = linear.r * num,
			y = linear.g * num,
			z = linear.b * num,
			w = num
		});
		if (usecookie)
		{
			Shader.SetGlobalTexture("_G_CustomLightCookie", cookietex);
		}
	}
}
