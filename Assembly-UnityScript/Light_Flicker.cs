using System;
using UnityEngine;

[Serializable]
public class Light_Flicker : MonoBehaviour
{
	public float time;

	public float min;

	public float max;

	public bool useSmooth;

	public float smoothTime;

	public Light _light;

	public Light_Flicker()
	{
		this.time = 0.2f;
		this.min = 0.5f;
		this.max = (float)5;
		this.smoothTime = (float)10;
	}

	public override void Start()
	{
		this._light = (Light)this.transform.GetComponent(typeof(Light));
		if (!this.useSmooth && this._light)
		{
			this.InvokeRepeating("OneLightChange", this.time, this.time);
		}
	}

	public override void OneLightChange()
	{
		this._light.intensity = UnityEngine.Random.Range(this.min, this.max);
	}

	public override void Update()
	{
		if (this.useSmooth && this._light)
		{
			this._light.intensity = Mathf.Lerp(this._light.intensity, UnityEngine.Random.Range(this.min, this.max), Time.deltaTime * this.smoothTime);
		}
		if (this._light == null)
		{
			MonoBehaviour.print("Please add a light component for light flicker");
		}
	}

	public override void Main()
	{
	}
}
