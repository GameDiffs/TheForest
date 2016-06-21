using System;
using UnityEngine;

public class BlackSkinArms : MonoBehaviour
{
	public PlayerStats Stats;

	private void Start()
	{
		this.Stats.IsBlackMan();
	}
}
