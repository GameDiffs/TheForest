using System;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class MapControler : SpecialItemControlerBase
	{
		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId);
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable)
			{
				enable = (!LocalPlayer.FpCharacter.PushingSled && !LocalPlayer.WaterViz.InWater && !LocalPlayer.AnimControl.WaterBlock && !LocalPlayer.AnimControl.onRope);
				Scene.HudGui.MapIcon.SetActive(enable);
			}
			else
			{
				Scene.HudGui.MapIcon.SetActive(false);
			}
			return enable;
		}

		protected override void OnActivating()
		{
			if (!LocalPlayer.Animator.GetBool("drawBowBool") && !LocalPlayer.Create.CreateMode)
			{
				LocalPlayer.Inventory.Equip(this._itemId, false);
			}
		}

		protected override void OnDeactivating()
		{
			LocalPlayer.Inventory.StashEquipedWeapon(true);
		}
	}
}
