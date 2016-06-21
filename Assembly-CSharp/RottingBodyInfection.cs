using System;
using TheForest.Utils;
using UnityEngine;

public class RottingBodyInfection : MonoBehaviour
{
	public float _rottingDelay = 300f;

	private void Awake()
	{
		base.Invoke("Rotting", this._rottingDelay);
	}

	private void Rotting()
	{
		ParticleSystem particleSystem = (ParticleSystem)UnityEngine.Object.Instantiate(Prefabs.Instance.FliesPSPrefab, base.transform.position, base.transform.rotation);
		particleSystem.transform.parent = base.transform;
		particleSystem.gameObject.SetActive(true);
		CoopSliceAndDiceMutant componentInParent = base.GetComponentInParent<CoopSliceAndDiceMutant>();
		if (componentInParent)
		{
			for (int i = 0; i < componentInParent.BodyParts.Length; i++)
			{
				if (componentInParent.BodyParts[i])
				{
					componentInParent.BodyParts[i].infected = true;
				}
			}
		}
	}
}
