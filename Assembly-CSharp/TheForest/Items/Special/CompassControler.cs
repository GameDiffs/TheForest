using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class CompassControler : SpecialItemControlerBase
	{
		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastUtility._itemId);
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable && LocalPlayer.Inventory.LastUtility != this)
			{
				LocalPlayer.Inventory.StashLeftHand();
				LocalPlayer.Inventory.LastUtility = this;
			}
			return true;
		}

		protected override void OnActivating()
		{
			if (LocalPlayer.Inventory.LastUtility == this && !LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				LocalPlayer.Inventory.TurnOnLastUtility();
			}
		}

		protected override void OnDeactivating()
		{
			if (LocalPlayer.Inventory.LastUtility == this)
			{
				base.StartCoroutine(this.DelayedStop());
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedStop()
		{
			CompassControler.<DelayedStop>c__Iterator161 <DelayedStop>c__Iterator = new CompassControler.<DelayedStop>c__Iterator161();
			<DelayedStop>c__Iterator.<>f__this = this;
			return <DelayedStop>c__Iterator;
		}
	}
}
