using System;
using UnityEngine;

public class FireLight : MonoBehaviour
{
	private float rnd;

	private bool burning = true;

	private void Start()
	{
		this.rnd = UnityEngine.Random.value * 100f;
	}

	private void Update()
	{
		if (this.burning)
		{
			base.GetComponent<Light>().intensity = 2f * Mathf.PerlinNoise(this.rnd + Time.time, this.rnd + 1f + Time.time * 1f);
			float x = Mathf.PerlinNoise(this.rnd + Time.time * 2f, this.rnd + 1f + Time.time * 2f) - 0.5f;
			float y = Mathf.PerlinNoise(this.rnd + 2f + Time.time * 2f, this.rnd + 3f + Time.time * 2f) - 0.5f;
			float z = Mathf.PerlinNoise(this.rnd + 4f + Time.time * 2f, this.rnd + 5f + Time.time * 2f) - 0.5f;
			base.transform.localPosition = Vector3.up + new Vector3(x, y, z) * 1f;
		}
	}

	public void Extinguish()
	{
		this.burning = false;
		base.GetComponent<Light>().enabled = false;
	}
}
