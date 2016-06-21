using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class PedometerControler : SpecialItemControlerBase
	{
		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastUtility._itemId);
			}
		}

		private void Awake()
		{
			LocalPlayer.Inventory.LastUtility = this;
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable && LocalPlayer.Inventory.LastUtility != this)
			{
				LocalPlayer.Inventory.StashLeftHand();
				LocalPlayer.Inventory.LastUtility = this;
			}
			if (Scene.HudGui.PedCam.activeSelf != enable)
			{
				Scene.HudGui.PedCam.SetActive(enable);
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
			PedometerControler.<DelayedStop>c__Iterator168 <DelayedStop>c__Iterator = new PedometerControler.<DelayedStop>c__Iterator168();
			<DelayedStop>c__Iterator.<>f__this = this;
			return <DelayedStop>c__Iterator;
		}
	}
}
