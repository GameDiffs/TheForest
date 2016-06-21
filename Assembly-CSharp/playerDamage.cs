using System;
using UnityEngine;

public class playerDamage : MonoBehaviour
{
	private playerHitReactions hitReactions;

	private void Awake()
	{
		this.hitReactions = base.transform.root.GetComponent<playerHitReactions>();
	}

	private void lookAtExplosion(Vector3 pos)
	{
		this.hitReactions.lookAtExplosion(pos);
	}
}
