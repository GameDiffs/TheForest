using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class setupDummyFire : MonoBehaviour
{
	public GameObject[] fire;

	public float rate = 0.2f;

	private void Start()
	{
	}

	private void OnDisable()
	{
		this.disableFire();
	}

	private void startDummyFire()
	{
		GameObject[] array = this.fire;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(true);
				base.StartCoroutine("burnDown", gameObject.transform);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator burnDown(Transform tr)
	{
		setupDummyFire.<burnDown>c__IteratorF7 <burnDown>c__IteratorF = new setupDummyFire.<burnDown>c__IteratorF7();
		<burnDown>c__IteratorF.tr = tr;
		<burnDown>c__IteratorF.<$>tr = tr;
		<burnDown>c__IteratorF.<>f__this = this;
		return <burnDown>c__IteratorF;
	}

	private void disableFire()
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
}
