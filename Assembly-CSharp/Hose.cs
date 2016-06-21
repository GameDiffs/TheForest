using System;
using UnityEngine;

public class Hose : MonoBehaviour
{
	public float maxPower = 20f;

	public float minPower = 5f;

	public float changeSpeed = 5f;

	private float power;

	public ParticleSystem[] hoseWaterSystems;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			this.power = Mathf.Lerp(this.power, this.maxPower, Time.deltaTime * this.changeSpeed);
		}
		else
		{
			this.power = Mathf.Lerp(this.power, this.minPower, Time.deltaTime * this.changeSpeed);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ParticleSystem component = base.transform.Find("Callback Particles").GetComponent<ParticleSystem>();
			component.GetComponent<Renderer>().enabled = !component.GetComponent<Renderer>().enabled;
		}
		ParticleSystem[] array = this.hoseWaterSystems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			particleSystem.startSpeed = this.power;
			particleSystem.enableEmission = (this.power > this.minPower * 1.1f);
		}
	}
}
