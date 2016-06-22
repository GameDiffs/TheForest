using System;
using UnityEngine;

[Serializable]
public class FishDead : MonoBehaviour
{
	public Material Raw;

	public Material Cooked;

	public override void SwitchToCooked()
	{
		this.GetComponent<Renderer>().material = this.Cooked;
	}

	public override void Main()
	{
	}
}
