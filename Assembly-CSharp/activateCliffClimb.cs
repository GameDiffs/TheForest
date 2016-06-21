using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Player.Actions;
using UnityEngine;

public class activateCliffClimb : MonoBehaviour
{
	public GameObject MyPickUp;

	private PlayerClimbCliffAction cliffAction;

	public global::Types climbType;

	private RaycastHit hit;

	private int layerMask;

	private bool allowClimb;

	private bool doingClimb;

	private bool activateCoolDown;

	[ItemIdPicker]
	public int _itemId;

	private void Start()
	{
		this.layerMask = 67117056;
		this.MyPickUp.SetActive(false);
		base.StopAllCoroutines();
	}

	private void OnEnable()
	{
		base.StartCoroutine("scanForCliff");
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.MyPickUp.SetActive(false);
		this.allowClimb = false;
	}

	[DebuggerHidden]
	private IEnumerator scanForCliff()
	{
		activateCliffClimb.<scanForCliff>c__Iterator2F <scanForCliff>c__Iterator2F = new activateCliffClimb.<scanForCliff>c__Iterator2F();
		<scanForCliff>c__Iterator2F.<>f__this = this;
		return <scanForCliff>c__Iterator2F;
	}

	private void resetCoolDown()
	{
		this.activateCoolDown = false;
	}
}
