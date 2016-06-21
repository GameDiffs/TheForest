using System;
using UnityEngine;

public class setLayer : MonoBehaviour
{
	public int setToLayer;

	public float delay;

	private void Start()
	{
		base.Invoke("doSetLayer", this.delay);
	}

	private void OnEnable()
	{
		base.Invoke("doSetLayer", this.delay);
	}

	private void doSetLayer()
	{
		base.gameObject.layer = this.setToLayer;
	}
}
