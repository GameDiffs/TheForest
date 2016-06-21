using System;
using UnityEngine;

public class TurnOffEnemies : MonoBehaviour
{
	private void Awake()
	{
		if (Cheats.NoEnemies)
		{
			base.gameObject.SetActive(false);
		}
	}
}
