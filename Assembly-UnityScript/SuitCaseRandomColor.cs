using System;
using UnityEngine;

[Serializable]
public class SuitCaseRandomColor : MonoBehaviour
{
	public Renderer Lid;

	public Renderer Base;

	public Color Color;

	public override void Awake()
	{
		this.Color = new Color(UnityEngine.Random.Range((float)0, 1f), UnityEngine.Random.Range((float)0, 1f), UnityEngine.Random.Range((float)0, 1f));
		this.Lid.material.color = this.Color;
		this.Base.material.color = this.Color;
	}

	public override void Main()
	{
	}
}
