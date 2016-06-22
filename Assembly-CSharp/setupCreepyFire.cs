using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class setupCreepyFire : MonoBehaviour
{
	public GameObject[] fire;

	public Material burntMat;

	public SkinnedMeshRenderer skin;

	private void Start()
	{
	}

	private void OnDisable()
	{
		this.disableAllFire();
	}

	private void enableBurntSkin()
	{
		if (this.burntMat && this.skin)
		{
			this.skin.material = this.burntMat;
		}
	}

	private void enableFire()
	{
		GameObject[] array = this.fire;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(true);
				base.StartCoroutine("disableFire", gameObject);
			}
		}
	}

	private void disableAllFire()
	{
		GameObject[] array = this.fire;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator disableFire(GameObject go)
	{
		setupCreepyFire.<disableFire>c__IteratorF6 <disableFire>c__IteratorF = new setupCreepyFire.<disableFire>c__IteratorF6();
		<disableFire>c__IteratorF.go = go;
		<disableFire>c__IteratorF.<$>go = go;
		return <disableFire>c__IteratorF;
	}
}
