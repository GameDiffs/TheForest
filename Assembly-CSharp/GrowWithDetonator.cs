using PathologicalGames;
using System;
using UnityEngine;

[RequireComponent(typeof(Detonator))]
public class GrowWithDetonator : MonoBehaviour
{
	public Detonator detonator;

	private Transform xform;

	private void Awake()
	{
		this.xform = base.transform;
	}

	private void Update()
	{
		Vector3 localScale = this.detonator.range * 2.1f;
		localScale.y *= 0.2f;
		this.xform.localScale = localScale;
		Color color = base.GetComponent<Renderer>().material.GetColor("_TintColor");
		color.a = Mathf.Lerp(0.7f, 0f, this.detonator.range.x / this.detonator.maxRange.x);
		base.GetComponent<Renderer>().material.SetColor("_TintColor", color);
	}
}
