using System;
using UnityEngine;

public class deactivateOnStart : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("disableMe", 0.3f);
	}

	private void disableMe()
	{
		base.gameObject.SetActive(false);
	}
}
