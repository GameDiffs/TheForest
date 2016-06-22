using Boo.Lang.Runtime;
using System;
using UnityEngine;

[Serializable]
public class ScriptPlaneLights : MonoBehaviour
{
	public GameObject lights1;

	public GameObject lights2;

	public GameObject lights3;

	public Light LightningLight;

	public Renderer TVscreen;

	private object Pulse;

	public Renderer SeatBeltLights;

	public override void Start()
	{
		this.Invoke("LightsChange", (float)2);
		this.Invoke("Lightning", (float)10);
		this.InvokeRepeating("PulseTv", (float)2, (float)2);
	}

	public override void LightsChange()
	{
		this.Invoke("Lights1Off", (float)1);
		this.Invoke("Lights2Off", (float)2);
		this.Invoke("Lights3Off", (float)3);
		this.Invoke("SeatBeltLightsOn", (float)4);
		this.Invoke("Lights1On", (float)20);
		this.Invoke("Lights2On", (float)21);
		this.Invoke("Lights3On", (float)22);
	}

	public override void PulseTv()
	{
		if (!RuntimeServices.ToBool(this.Pulse))
		{
			this.Pulse = true;
		}
		else
		{
			this.Pulse = false;
		}
	}

	public override void Update()
	{
		if (Input.GetKeyDown("1"))
		{
			this.LightsChange();
		}
		if (Input.GetKeyDown("2"))
		{
			this.Lightning();
		}
		if (RuntimeServices.ToBool(this.Pulse))
		{
			this.TVscreen.material.SetFloat("_EmissionIntensity", UnityEngine.Random.Range(-1f, 2f));
		}
	}

	public override void Lightning()
	{
	}

	public override void SeatBeltLightsOn()
	{
		this.SeatBeltLights.material.SetFloat("_EmissionIntensity", (float)10);
	}

	public override void Lights1Off()
	{
		this.lights1.SetActive(false);
	}

	public override void Lights2Off()
	{
		this.lights2.SetActive(false);
	}

	public override void Lights3Off()
	{
		this.lights3.SetActive(false);
	}

	public override void Lights1On()
	{
		this.lights1.SetActive(true);
	}

	public override void Lights2On()
	{
		this.lights2.SetActive(true);
	}

	public override void Lights3On()
	{
		this.lights3.SetActive(true);
	}

	public override void Main()
	{
	}
}
