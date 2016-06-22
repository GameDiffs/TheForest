using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using UnityEngine;

public class CoopRack : EntityBehaviour<IWeaponRackState>
{
	[SerializeField]
	private WeaponRackSlot[] Slots;

	public override void Attached()
	{
		base.state.AddCallback("Slots[]", new PropertyCallback(this.SlotChanged));
		if (this.entity.isOwner)
		{
			base.StartCoroutine(this.UpdateSlots());
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateSlots()
	{
		CoopRack.<UpdateSlots>c__Iterator20 <UpdateSlots>c__Iterator = new CoopRack.<UpdateSlots>c__Iterator20();
		<UpdateSlots>c__Iterator.<>f__this = this;
		return <UpdateSlots>c__Iterator;
	}

	public int GetSlotIndex(WeaponRackSlot weaponRackSlot)
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			if (object.ReferenceEquals(this.Slots[i], weaponRackSlot))
			{
				return i;
			}
		}
		return -1;
	}

	private void SlotChanged(IState _, string path, ArrayIndices indices)
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			this.Slots[i].ItemIdChanged_Network(base.state.Slots[i]);
		}
	}
}
