using System;
using TheForest.Items.Inventory;
using TheForest.UI.Multiplayer;
using UnityEngine;

namespace TheForest.UI
{
	public class PlayerListRow : MonoBehaviour
	{
		public PlayerOverlay _overlay;

		public UIButton _kickButton;

		public UIButton _banButton;

		public BoltEntity _entity;

		private void Update()
		{
			if (this._entity && this._entity.isAttached)
			{
				PlayerInventory.PlayerViews currentView = (PlayerInventory.PlayerViews)this._entity.GetState<IPlayerState>().CurrentView;
				this._overlay._bookIcon.enabled = (currentView == PlayerInventory.PlayerViews.Book);
				this._overlay._inventoryIcon.enabled = (currentView == PlayerInventory.PlayerViews.Inventory);
				this._overlay._menuIcon.enabled = (currentView == PlayerInventory.PlayerViews.Pause);
				this._overlay._deathIcon.enabled = (currentView == PlayerInventory.PlayerViews.Death);
				this._overlay._sleepIcon.enabled = (currentView == PlayerInventory.PlayerViews.Sleep);
			}
		}
	}
}
