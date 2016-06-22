using System;
using UnityEngine;

[Serializable]
public class TreeNudge : MonoBehaviour
{
	private bool ShouldNudge;

	public TreeNudge()
	{
		this.ShouldNudge = true;
	}

	public override void fixedUpdate()
	{
		if (this.ShouldNudge)
		{
			this.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(5, 20), (float)UnityEngine.Random.Range(5, 20), (float)UnityEngine.Random.Range(5, 20));
		}
	}

	public override void Awake()
	{
		this.Invoke("StopPush", 0.2f);
	}

	public override void StopPush()
	{
		this.ShouldNudge = false;
	}

	public override void Main()
	{
	}
}
