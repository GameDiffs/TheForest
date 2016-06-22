using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class TalkyWalkyControler : SpecialItemControlerBase
	{
		public float _batteryUpsurgeDelay = 10f;

		public float _batteryUpsurgeValue = 1.5f;

		private float _nextBatteryUpsurge;

		public override bool ToggleSpecial(bool enable)
		{
			if (enable)
			{
				if (LocalPlayer.Stats.BatteryCharge <= 0f)
				{
					enable = false;
				}
			}
			if (Scene.HudGui.TalkyWalkyInfo.activeSelf != enable)
			{
				Scene.HudGui.TalkyWalkyInfo.SetActive(enable);
			}
			return enable;
		}

		protected override void OnActivating()
		{
			if (!LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				LocalPlayer.Inventory.Equip(this._itemId, false);
			}
		}

		protected override void OnDeactivating()
		{
			base.StartCoroutine(this.DelayedStop(false));
		}

		[DebuggerHidden]
		private IEnumerator DelayedStop(bool equipPrevious)
		{
			TalkyWalkyControler.<DelayedStop>c__Iterator171 <DelayedStop>c__Iterator = new TalkyWalkyControler.<DelayedStop>c__Iterator171();
			<DelayedStop>c__Iterator.equipPrevious = equipPrevious;
			<DelayedStop>c__Iterator.<$>equipPrevious = equipPrevious;
			<DelayedStop>c__Iterator.<>f__this = this;
			return <DelayedStop>c__Iterator;
		}
	}
}
