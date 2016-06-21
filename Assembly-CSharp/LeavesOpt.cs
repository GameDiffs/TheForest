using System;
using UnityEngine;

public class LeavesOpt : MonoBehaviour
{
	public GameObject MyFake;

	private bool isVisible;

	private void OnBecameVisible()
	{
		this.isVisible = true;
	}

	private void OnBecameInvisible()
	{
		this.isVisible = false;
	}

	private void Update()
	{
		if (this.isVisible)
		{
			this.MyFake.SetActive(false);
			base.gameObject.layer = 11;
		}
		else
		{
			this.MyFake.SetActive(true);
			base.gameObject.layer = 21;
		}
	}
}
