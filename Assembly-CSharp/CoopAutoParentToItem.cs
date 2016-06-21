using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class CoopAutoParentToItem : EntityBehaviour<IWeaponFire>
{
	public Item.EquipmentSlot _slot;

	public override void Attached()
	{
		if (this.entity.isOwner)
		{
			HeldItemIdentifier componentInParent = base.GetComponentInParent<HeldItemIdentifier>();
			base.state.Position = componentInParent.transform.position - base.transform.position;
			base.state.Rotation = Quaternion.Inverse(componentInParent.transform.rotation) * base.transform.rotation;
			base.state.Entity = LocalPlayer.Entity;
		}
		else
		{
			base.state.AddCallback("Entity", new PropertyCallbackSimple(this.SetParent));
			base.state.AddCallback("Position", new PropertyCallbackSimple(this.SetPosition));
			base.state.AddCallback("Rotation", new PropertyCallbackSimple(this.SetRotation));
			this.SetParent();
		}
	}

	private void SetParent()
	{
		if (base.state.Entity)
		{
			CoopPlayerRemoteSetup component = base.state.Entity.GetComponent<CoopPlayerRemoteSetup>();
			Item.EquipmentSlot slot = this._slot;
			Transform transform;
			if (slot != Item.EquipmentSlot.RightHand)
			{
				if (slot != Item.EquipmentSlot.LeftHand)
				{
					transform = null;
				}
				else
				{
					transform = component.leftHand.ActiveItem;
				}
			}
			else
			{
				transform = component.rightHand.ActiveItem;
			}
			if (transform)
			{
				base.transform.parent = transform;
				base.transform.localPosition = base.state.Position;
				base.transform.localRotation = base.state.Rotation;
			}
		}
		else
		{
			base.transform.parent = null;
		}
	}

	private void SetPosition()
	{
		if (base.transform.parent)
		{
			base.transform.localPosition = base.state.Position;
		}
	}

	private void SetRotation()
	{
		if (base.transform.parent)
		{
			base.transform.localRotation = base.state.Rotation;
		}
	}
}
