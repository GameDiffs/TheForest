using System;
using TheForest.Items.Inventory;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class BookControler : SpecialItemControlerBase
	{
		public SurvivalBook _book;

		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book;
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable)
			{
				if (LocalPlayer.FpCharacter.Grounded && !LocalPlayer.FpCharacter.PushingSled && !LocalPlayer.WaterViz.InWater && !LocalPlayer.AnimControl.WaterBlock)
				{
					LocalPlayer.Create.OpenBook();
				}
			}
			else
			{
				LocalPlayer.Create.CloseTheBook();
			}
			return true;
		}

		protected override bool CurrentViewTest()
		{
			return LocalPlayer.Inventory.CurrentView > PlayerInventory.PlayerViews.Loading && LocalPlayer.Inventory.CurrentView < PlayerInventory.PlayerViews.Pause;
		}

		protected override void OnActivating()
		{
			if (!LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				this.ToggleSpecial(true);
			}
		}

		protected override void OnDeactivating()
		{
			this.ToggleSpecial(false);
		}
	}
}
