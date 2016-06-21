using System;
using UnityEngine;

public class boarEvents : MonoBehaviour
{
	public SphereCollider weaponCollider;

	[Header("FMOD")]
	public string attackEvent;

	private void Start()
	{
		this.weaponCollider.enabled = false;
	}

	private void enableAttack()
	{
		this.weaponCollider.enabled = true;
		base.Invoke("disableAttack", 2f);
	}

	private void disableAttack()
	{
		this.weaponCollider.enabled = false;
	}

	private void playAttackSound()
	{
		FMODCommon.PlayOneshotNetworked(this.attackEvent, base.transform, FMODCommon.NetworkRole.Server);
	}
}
