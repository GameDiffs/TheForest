using System;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

public class activateClimb : MonoBehaviour
{
	public enum Types
	{
		ropeClimb,
		wallClimb,
		cliffClimb
	}

	public GameObject Sheen;

	public GameObject MyPickUp;

	public activateClimb.Types climbType;

	[ItemIdPicker]
	public int _itemId;

	private void Start()
	{
		base.enabled = false;
	}

	private void GrabEnter(GameObject grabber)
	{
		if (!LocalPlayer.FpCharacter.Sitting)
		{
			base.enabled = true;
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(true);
		}
	}

	private void GrabExit(GameObject grabber)
	{
		if (base.enabled)
		{
			base.enabled = false;
			this.Sheen.SetActive(true);
			this.MyPickUp.SetActive(false);
		}
	}

	private void Update()
	{
		if (LocalPlayer.FpCharacter.Sitting)
		{
			this.Sheen.SetActive(true);
			this.MyPickUp.SetActive(false);
			base.enabled = false;
			return;
		}
		if (LocalPlayer.FpCharacter.PushingSled)
		{
			return;
		}
		if (TheForest.Utils.Input.GetButtonDown("Take") && !LocalPlayer.FpCharacter.Sitting)
		{
			if (this.climbType == activateClimb.Types.ropeClimb)
			{
				LocalPlayer.SpecialActions.SendMessage("enterClimbRope", base.transform);
			}
			else if (this.climbType == activateClimb.Types.wallClimb && LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId))
			{
				LocalPlayer.SpecialActions.SendMessage("enterClimbWall", base.transform);
			}
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
	}
}
