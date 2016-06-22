using System;
using UnityEngine;

[Serializable]
public class TimedExplosive : MonoBehaviour
{
	public GameObject MyExplosion;

	public int WaitTime;

	public TimedExplosive()
	{
		this.WaitTime = 5;
	}

	public override void Start()
	{
		this.Invoke("Explode", (float)this.WaitTime);
	}

	public override void Explode()
	{
		this.MyExplosion.transform.parent = null;
	}

	public override void Main()
	{
	}
}
