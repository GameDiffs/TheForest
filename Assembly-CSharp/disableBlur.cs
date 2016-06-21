using System;
using UnityEngine;

public class disableBlur : MonoBehaviour
{
	public bool cleanAmplify;

	private void Start()
	{
		if (this.cleanAmplify)
		{
			base.Invoke("doRemove", 0.05f);
		}
	}

	private void OnEnable()
	{
		if (this.cleanAmplify)
		{
			base.Invoke("doRemove", 0.05f);
		}
	}

	private void doRemove()
	{
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			AmplifyMotionObjectBase component = particleSystem.GetComponent<AmplifyMotionObjectBase>();
			if (component)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}
}
