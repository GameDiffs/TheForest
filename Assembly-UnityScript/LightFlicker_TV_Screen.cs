using System;
using UnityEngine;

[Serializable]
public class LightFlicker_TV_Screen : MonoBehaviour
{
	public float FlickerSpeed;

	public int LightMaxRange;

	public int LightMinRange;

	private Color MyColor;

	public LightFlicker_TV_Screen()
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
		this.MyColor = new Color(UnityEngine.Random.Range((float)0, 1f), UnityEngine.Random.Range((float)0, 1f), UnityEngine.Random.Range((float)0, 1f));
		this.GetComponent<Light>().color = this.MyColor;
	}

	public override void Main()
	{
	}
}
