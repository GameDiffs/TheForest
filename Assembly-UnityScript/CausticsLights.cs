using System;
using UnityEngine;

[Serializable]
public class CausticsLights : MonoBehaviour
{
	public Texture2D[] frames;

	public float framesPerSecond;

	public CausticsLights()
	{
		this.framesPerSecond = 10f;
	}

	public override void Update()
	{
		int num = (int)(Time.time * this.framesPerSecond);
		num %= this.frames.Length;
		this.GetComponent<Light>().cookie = this.frames[num];
	}

	public override void Main()
	{
	}
}
