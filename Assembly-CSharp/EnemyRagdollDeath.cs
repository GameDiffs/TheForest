using System;
using UnityEngine;

public class EnemyRagdollDeath : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("TurnMeOff", 100f);
	}

	private void TurnMeOff()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
