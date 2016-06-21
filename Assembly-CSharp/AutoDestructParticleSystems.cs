using System;
using UnityEngine;

public class AutoDestructParticleSystems : MonoBehaviour
{
	public bool ChildParticleSystems;

	private ParticleSystem[] allParticleSystems;

	private void Awake()
	{
		if (this.ChildParticleSystems)
		{
			this.allParticleSystems = base.GetComponentsInChildren<ParticleSystem>();
		}
	}

	private void LateUpdate()
	{
		if (this.ChildParticleSystems)
		{
			ParticleSystem[] array = this.allParticleSystems;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem particleSystem = array[i];
				if (particleSystem.IsAlive())
				{
					return;
				}
			}
		}
		else if (base.GetComponent<ParticleSystem>().IsAlive())
		{
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
