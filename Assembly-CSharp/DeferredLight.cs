using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Light))]
public class DeferredLight : MonoBehaviour
{
	private Transform t;

	private Light l;

	[Range(0f, 1f)]
	public float fakeReflectedLight = 0.2f;

	public Texture2D spotlightProjectTexture;

	public float fakerefl
	{
		get
		{
			return this.fakeReflectedLight;
		}
	}

	public Vector3 position
	{
		get
		{
			return this.t.position;
		}
	}

	public float intensity
	{
		get
		{
			return Mathf.Exp(this.l.intensity / 8f * 4f) - 1f;
		}
	}

	public Color color
	{
		get
		{
			return this.l.color;
		}
	}

	public float range
	{
		get
		{
			return this.l.range;
		}
	}

	public LightType type
	{
		get
		{
			return this.l.type;
		}
	}

	public float spotAngle
	{
		get
		{
			return this.l.spotAngle / 180f;
		}
	}

	public Vector3 spotDirection
	{
		get
		{
			return Vector3.Normalize(this.t.forward);
		}
	}

	private void Start()
	{
		this.t = base.transform;
		this.l = base.GetComponent<Light>();
	}

	private void OnEnable()
	{
		DeferredShadingCamera.AddLight(this);
	}

	private void OnDisable()
	{
		DeferredShadingCamera.RemoveLight(this);
	}
}
