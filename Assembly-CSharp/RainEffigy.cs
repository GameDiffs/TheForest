using System;
using UnityEngine;

public class RainEffigy : MonoBehaviour
{
	public static int RainAdd;

	private void Start()
	{
		base.Invoke("WearOff", 400f);
		RainEffigy.RainAdd++;
	}

	private void WearOff()
	{
		RainEffigy.RainAdd--;
	}
}
