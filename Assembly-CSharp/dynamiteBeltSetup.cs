using System;
using UnityEngine;

public class dynamiteBeltSetup : MonoBehaviour
{
	public GameObject[] enableGo;

	private void Start()
	{
	}

	public void enableBeltExplosion()
	{
		GameObject[] array = this.enableGo;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(true);
		}
	}
}
