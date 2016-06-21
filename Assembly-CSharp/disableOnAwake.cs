using System;
using UnityEngine;

public class disableOnAwake : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.Invoke("enableGo", 0.5f);
	}

	private void enableGo()
	{
		base.gameObject.SetActive(true);
	}
}
