using System;
using UnityEngine;

public class crocodileEvents : MonoBehaviour
{
	public SphereCollider weaponCollider;

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
}
