using System;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour
{
	public float particleScale = 1f;

	public bool alsoScaleGameobject = true;

	public float startScale;

	public float prevScale;

	private ParticleSystem[] systems;

	public void Awake()
	{
		if (this.systems == null)
		{
			this.startScale = (this.prevScale = this.particleScale);
			this.systems = base.GetComponentsInChildren<ParticleSystem>();
		}
	}

	private void OnEnable()
	{
		if (this.prevScale <= 0f)
		{
			this.ResetParticleScale(this.startScale);
		}
	}

	public void Update()
	{
		if (this.prevScale != this.particleScale && this.particleScale > 0f && this.prevScale > 0f)
		{
			if (this.alsoScaleGameobject)
			{
				base.transform.localScale = new Vector3(this.particleScale, this.particleScale, this.particleScale);
			}
			float num = this.particleScale / this.prevScale;
			if (num != float.PositiveInfinity)
			{
				this.ScaleShurikenSystems(num);
			}
			this.prevScale = this.particleScale;
		}
	}

	private void ScaleShurikenSystems(float scaleFactor)
	{
		for (int i = 0; i < this.systems.Length; i++)
		{
			ParticleSystem particleSystem = this.systems[i];
			if (particleSystem)
			{
				particleSystem.startSpeed *= scaleFactor;
				particleSystem.startSize *= scaleFactor;
				particleSystem.gravityModifier *= scaleFactor;
			}
		}
	}

	public void ResetParticleScale(float newScale)
	{
		this.Update();
		this.prevScale = this.startScale;
		if (this.alsoScaleGameobject)
		{
			base.transform.localScale = new Vector3(this.startScale, this.startScale, this.startScale);
		}
		float scaleFactor = this.startScale / this.particleScale;
		this.ScaleShurikenSystems(scaleFactor);
		this.particleScale = newScale;
		this.Update();
	}
}
