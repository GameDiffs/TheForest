using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using UnityEngine;

public class exitClimb : MonoBehaviour
{
	public enum Types
	{
		ropeClimb,
		wallClimb
	}

	private PlayerInventory Player;

	private BoxCollider collider;

	private bool triggerCoolDown;

	public exitClimb.Types climbType;

	private void Start()
	{
		this.collider = base.transform.GetComponent<BoxCollider>();
		this.collider.enabled = true;
		this.triggerCoolDown = false;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && !this.triggerCoolDown)
		{
			this.triggerCoolDown = true;
			this.Player = other.transform.root.GetComponent<PlayerInventory>();
			if (this.climbType == exitClimb.Types.ropeClimb)
			{
				this.Player.SpecialActions.SendMessage("exitClimbRopeTop", base.transform);
			}
			if (this.climbType == exitClimb.Types.wallClimb)
			{
				this.Player.SpecialActions.SendMessage("exitClimbWallTop", base.transform);
			}
			base.Invoke("resetCoolDown", 0.1f);
		}
	}

	private void resetCoolDown()
	{
		this.triggerCoolDown = false;
	}

	[DebuggerHidden]
	private IEnumerator pulseCollider()
	{
		exitClimb.<pulseCollider>c__Iterator5C <pulseCollider>c__Iterator5C = new exitClimb.<pulseCollider>c__Iterator5C();
		<pulseCollider>c__Iterator5C.<>f__this = this;
		return <pulseCollider>c__Iterator5C;
	}
}
