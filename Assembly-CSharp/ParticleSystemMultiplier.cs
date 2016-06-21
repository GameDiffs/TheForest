using System;
using UnityEngine;

public class ParticleSystemMultiplier : MonoBehaviour
{
	public float multiplier = 1f;

	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			particleSystem.startSize *= this.multiplier;
			particleSystem.startSpeed *= this.multiplier;
			particleSystem.startLifetime *= Mathf.Lerp(this.multiplier, 1f, 0.5f);
			particleSystem.Clear();
			particleSystem.Play();
		}
	}
}
