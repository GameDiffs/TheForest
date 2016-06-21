using System;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

public class activateClimbTop : MonoBehaviour
{
	public enum Types
	{
		ropeClimb,
		wallClimb
	}

	public GameObject Sheen;

	public GameObject MyPickUp;

	public GameObject exitTop;

	public activateClimbTop.Types climbType;

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
			this.Sheen.SetActive(true);
			this.MyPickUp.SetActive(false);
			base.enabled = false;
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
		if (TheForest.Utils.Input.GetButtonDown("Take") && !LocalPlayer.FpCharacter.Sitting)
		{
			if (this.climbType == activateClimbTop.Types.ropeClimb)
			{
				LocalPlayer.SpecialActions.SendMessage("enterClimbRopeTop", base.transform);
			}
			if (this.climbType == activateClimbTop.Types.wallClimb && LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId))
			{
				LocalPlayer.SpecialActions.SendMessage("enterClimbWallTop", base.transform);
			}
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
	}

	private void enableExitTop()
	{
		this.exitTop.SetActive(true);
	}
}
