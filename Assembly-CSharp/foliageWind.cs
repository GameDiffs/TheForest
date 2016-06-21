using System;
using UnityEngine;

public class foliageWind : MonoBehaviour
{
	public Vector4 Wind;

	private float WaveSizeFoliageShader = 10f;

	private float WindRandomAmt;

	private float WindForce;

	private void Awake()
	{
		Shader.SetGlobalVector("_Wind", this.Wind);
		Shader.SetGlobalFloat("_AfsWaveSize", this.WaveSizeFoliageShader);
		base.InvokeRepeating("ChangeWindAmount", 0f, 15f);
	}

	private void Update()
	{
		if (this.WindRandomAmt > this.WindForce)
		{
			this.WindForce += 0.05f * Time.deltaTime;
		}
		else if (this.WindRandomAmt > this.WindForce)
		{
			this.WindForce -= 0.05f * Time.deltaTime;
		}
		this.Wind.w = this.WindForce;
		Shader.SetGlobalVector("_Wind", this.Wind);
	}

	private void ChangeWindAmount()
	{
		this.WindRandomAmt = UnityEngine.Random.Range(1f, 4f);
		Shader.SetGlobalVector("_Wind", this.Wind);
	}
}
