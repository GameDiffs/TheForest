using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class JetParticleEffect : MonoBehaviour
{
	public Color minColour;

	private AeroplaneController jet;

	private ParticleSystem system;

	private float originalStartSize;

	private float originalLifetime;

	private Color originalStartColor;

	private void Start()
	{
		this.jet = this.FindAeroplaneParent();
		this.system = base.GetComponent<ParticleSystem>();
		this.originalLifetime = this.system.startLifetime;
		this.originalStartSize = this.system.startSize;
		this.originalStartColor = this.system.startColor;
	}

	private void Update()
	{
		this.system.startLifetime = Mathf.Lerp(0f, this.originalLifetime, this.jet.Throttle);
		this.system.startSize = Mathf.Lerp(this.originalStartSize * 0.3f, this.originalStartSize, this.jet.Throttle);
		this.system.startColor = Color.Lerp(this.minColour, this.originalStartColor, this.jet.Throttle);
	}

	private AeroplaneController FindAeroplaneParent()
	{
		Transform transform = base.transform;
		while (transform != null)
		{
			AeroplaneController component = transform.GetComponent<AeroplaneController>();
			if (!(component == null))
			{
				return component;
			}
			transform = transform.parent;
		}
		throw new Exception(" AeroplaneContoller not found in object hierarchy");
	}
}
