using Bolt;
using System;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

public class CoopPlayerUpgrades : EntityBehaviour<IPlayerState>
{
	[Serializable]
	public class Upgrade
	{
		[ItemIdPicker]
		public int upgradeId;

		public string PlayerNetUpgradeObjectName;
	}

	[Serializable]
	public class Item
	{
		[ItemIdPicker]
		public int itemId;

		public GameObject PlayerNetObject;
	}

	[Header("max 8 currently")]
	public CoopPlayerUpgrades.Item[] UpgradableItems;

	[Header("max 32 currently")]
	public CoopPlayerUpgrades.Upgrade[] Upgrades;

	public override void Attached()
	{
		base.state.AddCallback("WeaponUpgrades[]", new PropertyCallbackSimple(this.OnWeaponUpgradesChanged));
	}

	private void OnWeaponUpgradesChanged()
	{
		for (int i = 0; i < this.UpgradableItems.Length; i++)
		{
			GameObject playerNetObject = this.UpgradableItems[i].PlayerNetObject;
			if (playerNetObject)
			{
				for (int j = 0; j < this.Upgrades.Length; j++)
				{
					Transform transform = playerNetObject.transform.FindChildIncludingDeactivated(this.Upgrades[j].PlayerNetUpgradeObjectName);
					if (transform)
					{
						transform.gameObject.SetActive((base.state.WeaponUpgrades[i] & 1 << j) == 1 << j);
					}
				}
			}
		}
	}

	private void Start()
	{
		base.enabled = BoltNetwork.isRunning;
	}

	private void Update()
	{
		if (this.entity.IsAttached() && this.entity.isOwner)
		{
			for (int i = 0; i < this.UpgradableItems.Length; i++)
			{
				if (LocalPlayer.Inventory.Owns(this.UpgradableItems[i].itemId))
				{
					int num = 0;
					for (int j = 0; j < this.Upgrades.Length; j++)
					{
						if (LocalPlayer.Inventory.GetAmountOfUpgrades(this.UpgradableItems[i].itemId, this.Upgrades[j].upgradeId) > 0)
						{
							num |= 1 << j;
						}
					}
					if (base.state.WeaponUpgrades[i] != num)
					{
						base.state.WeaponUpgrades[i] = num;
					}
				}
			}
		}
	}
}
