using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Decaying PickUp")]
	public class DecayingPickUp : PickUp
	{
		public DecayingInventoryItemView.DecayStates _state;

		protected override void Awake()
		{
			base.Awake();
			base.StartCoroutine(this.DelayedAwake());
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			DecayingPickUp.<DelayedAwake>c__Iterator174 <DelayedAwake>c__Iterator = new DecayingPickUp.<DelayedAwake>c__Iterator174();
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		protected override bool MainEffect()
		{
			if (base.MainEffect())
			{
				if (DecayingInventoryItemView.LastUsed != null)
				{
					DecayingInventoryItemView.LastUsed.SetDecayState(this._state);
					DecayingInventoryItemView.LastUsed = null;
				}
				return true;
			}
			return false;
		}
	}
}
