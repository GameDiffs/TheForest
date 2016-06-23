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
			DecayingPickUp.<DelayedAwake>c__Iterator17D <DelayedAwake>c__Iterator17D = new DecayingPickUp.<DelayedAwake>c__Iterator17D();
			<DelayedAwake>c__Iterator17D.<>f__this = this;
			return <DelayedAwake>c__Iterator17D;
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
