using System;
using UnityEngine;

[Serializable]
public class FlickerRedLight : MonoBehaviour
{
	public float FlickerSpeed;

	public int LightMaxRange;

	public int LightMinRange;

	public FlickerRedLight()
	{
		this.FlickerSpeed = 0.1f;
		this.LightMaxRange = 10;
	}

	public override void Start()
	{
		this.InvokeRepeating("Flicker", (float)0, this.FlickerSpeed);
	}

	public override void Flicker()
	{
		this.GetComponent<Light>().range = (float)UnityEngine.Random.Range(this.LightMinRange, this.LightMaxRange);
	}

	public override void Main()
	{
	}
}
