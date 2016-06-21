using System;
using UnityEngine;

public class ExtinguishableParticleSystem : MonoBehaviour
{
	public float multiplier = 1f;

	private ParticleSystem[] systems;

	private void Start()
	{
		this.systems = base.GetComponentsInChildren<ParticleSystem>();
	}

	public void Extinguish()
	{
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			particleSystem.enableEmission = false;
		}
	}
}
