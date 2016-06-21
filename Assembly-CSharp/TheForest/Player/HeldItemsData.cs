using System;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.World;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class HeldItemsData : MonoBehaviour
	{
		public EquipmentPainting[] _weaponPaint;

		public Bloodify[] _weaponBlood;

		public BurnableCloth[] _weaponFire;

		[SerializeThis]
		private EquipmentPainting.Colors[] _weaponPaintColors;

		[SerializeThis]
		private bool[] _weaponBloodStatus;

		[SerializeThis]
		private WeaponStatUpgrade.Types[] _weaponFireStates;

		private void OnSerializing()
		{
			if (this._weaponPaintColors == null || this._weaponPaintColors.Length != this._weaponPaint.Length)
			{
				this._weaponPaintColors = new EquipmentPainting.Colors[this._weaponPaint.Length];
			}
			for (int i = 0; i < this._weaponPaint.Length; i++)
			{
				this._weaponPaintColors[i] = this._weaponPaint[i].Color;
			}
			if (this._weaponBloodStatus == null || this._weaponBloodStatus.Length != this._weaponBlood.Length)
			{
				this._weaponBloodStatus = new bool[this._weaponBlood.Length];
			}
			for (int j = 0; j < this._weaponBloodStatus.Length; j++)
			{
				if (this._weaponBlood[j])
				{
					this._weaponBloodStatus[j] = this._weaponBlood[j].IsBloody;
				}
			}
			if (this._weaponFireStates == null || this._weaponFireStates.Length != this._weaponFire.Length)
			{
				this._weaponFireStates = new WeaponStatUpgrade.Types[this._weaponFire.Length];
			}
			for (int k = 0; k < this._weaponFireStates.Length; k++)
			{
				if (this._weaponFire[k])
				{
					this._weaponFireStates[k] = this._weaponFire[k]._inventoryMirror.transform.parent.GetComponent<InventoryItemView>().ActiveBonus;
				}
			}
		}

		private void OnDeserialized()
		{
			if (this._weaponPaintColors != null)
			{
				int num = Mathf.Min(this._weaponPaintColors.Length, this._weaponPaint.Length);
				for (int i = 0; i < num; i++)
				{
					this._weaponPaint[i].Color = this._weaponPaintColors[i];
					this._weaponPaint[i].OnDeserialized();
				}
			}
			if (this._weaponBloodStatus != null)
			{
				int num2 = Mathf.Min(this._weaponBloodStatus.Length, this._weaponBlood.Length);
				for (int j = 0; j < num2; j++)
				{
					if (this._weaponBloodStatus[j] && this._weaponBlood[j])
					{
						this._weaponBlood[j].GotBloody();
					}
				}
			}
			if (this._weaponFireStates != null)
			{
				int num3 = Mathf.Min(this._weaponFireStates.Length, this._weaponFire.Length);
				for (int k = 0; k < num3; k++)
				{
					if (this._weaponFire[k])
					{
						this._weaponFire[k]._inventoryMirror.transform.parent.GetComponent<InventoryItemView>().ActiveBonus = this._weaponFireStates[k];
						this._weaponFire[k].OnDeserialized();
					}
				}
			}
		}
	}
}
