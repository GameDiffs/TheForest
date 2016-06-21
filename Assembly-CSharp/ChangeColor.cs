using System;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	[SerializeThis]
	private Color color;

	private void Start()
	{
		this.color.r = UnityEngine.Random.value;
		this.color.g = UnityEngine.Random.value;
		this.color.b = UnityEngine.Random.value;
		this.color.a = 1f;
	}

	private void Update()
	{
		this.color.r = this.color.r + UnityEngine.Random.value / 100f;
		this.color.g = this.color.g + UnityEngine.Random.value / 100f;
		this.color.b = this.color.b + UnityEngine.Random.value / 100f;
		base.GetComponent<Renderer>().material.color = this.color;
	}
}
