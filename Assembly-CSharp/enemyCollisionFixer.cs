using System;
using TheForest.Utils;
using UnityEngine;

public class enemyCollisionFixer : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if (other.collider.gameObject.CompareTag("enemyRoot"))
		{
			LocalPlayer.FpCharacter.setEnemyContact(true);
			LocalPlayer.FpCharacter.StartCoroutine("doClampVelocity");
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.collider.gameObject.CompareTag("enemyRoot"))
		{
			LocalPlayer.FpCharacter.setEnemyContact(true);
			LocalPlayer.FpCharacter.StartCoroutine("doClampVelocity");
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.collider.gameObject.CompareTag("enemyRoot"))
		{
			LocalPlayer.FpCharacter.setEnemyContact(false);
		}
	}
}
