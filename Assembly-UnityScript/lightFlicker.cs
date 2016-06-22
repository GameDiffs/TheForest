using System;
using UnityEngine;

[Serializable]
public class lightFlicker : MonoBehaviour
{
	public float Min;

	public float Max;

	public float MyAmount;

	public lightFlicker()
	{
		this.Max = 3f;
	}

	public override void Update()
	{
		this.MyAmount = UnityEngine.Random.Range(this.Min, this.Max);
		this.GetComponent<Light>().intensity = this.MyAmount;
	}

	public override void Main()
	{
	}
}
