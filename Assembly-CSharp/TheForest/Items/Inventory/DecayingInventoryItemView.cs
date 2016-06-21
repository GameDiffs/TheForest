using System;
using TheForest.Items.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Decaying Item Inventory View")]
	public class DecayingInventoryItemView : InventoryItemView
	{
		public enum DecayStates
		{
			None,
			Fresh,
			Edible,
			Spoilt
		}

		[SerializeThis]
		public DecayingInventoryItemView.DecayStates _state;

		public DecayingInventoryItemView.DecayStates _prevState;

		[NameFromEnumIndex(typeof(DecayingInventoryItemView.DecayStates))]
		public Material[] _decayStatesMats = new Material[4];

		public Renderer[] _renderers;

		public float _decayDelay = 10f;

		[SerializeThis]
		private float _decayDoneTime;

		private float _decayStartTime;

		private bool _initDone;

		public static DecayingInventoryItemView LastUsed
		{
			get;
			set;
		}

		private void Awake()
		{
			this.Init();
		}

		private void OnDestroy()
		{
			if (DecayingInventoryItemView.LastUsed == this)
			{
				DecayingInventoryItemView.LastUsed = null;
			}
		}

		public override void OnDeserialized()
		{
			base.OnDeserialized();
			this._canDropFromInventory = (this._state == DecayingInventoryItemView.DecayStates.Spoilt);
			if (this._state > DecayingInventoryItemView.DecayStates.None)
			{
				this._state--;
			}
			this.Decay();
		}

		public override void OnSerializing()
		{
			this._decayDoneTime = Time.time - this._decayStartTime;
		}

		public override void OnItemAdded()
		{
			DecayingInventoryItemView.LastUsed = this;
			if (this._state == DecayingInventoryItemView.DecayStates.None)
			{
				this.Decay();
			}
			else
			{
				this._canDropFromInventory = (this._state == DecayingInventoryItemView.DecayStates.Spoilt && base.ItemCache.MatchType(Item.Types.WorldObject));
			}
		}

		public override void OnItemRemoved()
		{
			DecayingInventoryItemView.LastUsed = this;
			this.ToggleState(DecayingInventoryItemView.DecayStates.None);
		}

		public override void OnItemDropped(GameObject worldGo)
		{
			DecayingItem component = worldGo.GetComponent<DecayingItem>();
			if (component)
			{
				DecayingInventoryItemView.LastUsed = this;
				component._renderer.sharedMaterial = this._decayStatesMats[(int)this._state];
			}
		}

		public override void OnItemEquipped()
		{
			this._state = (DecayingInventoryItemView.DecayStates)Mathf.Max(this._prevState - DecayingInventoryItemView.DecayStates.Fresh, 0);
			this.Decay();
			Renderer renderer = this._held.GetComponent<Renderer>();
			if (!renderer)
			{
				renderer = this._held.GetComponentInChildren<Renderer>();
			}
			if (renderer)
			{
				renderer.sharedMaterial = this._decayStatesMats[(int)this._state];
			}
		}

		public void Decay()
		{
			switch (this._state)
			{
			case DecayingInventoryItemView.DecayStates.None:
				this.ToggleState(DecayingInventoryItemView.DecayStates.Fresh);
				this._decayStartTime = Time.time;
				LocalPlayer.ItemDecayMachine.NewDecayCommand(this, this._decayDelay - this._decayDoneTime);
				this._decayDoneTime = 0f;
				this._canDropFromInventory = false;
				break;
			case DecayingInventoryItemView.DecayStates.Fresh:
				this.ToggleState(DecayingInventoryItemView.DecayStates.Edible);
				this._decayStartTime = Time.time;
				LocalPlayer.ItemDecayMachine.NewDecayCommand(this, this._decayDelay - this._decayDoneTime);
				this._decayDoneTime = 0f;
				this._canDropFromInventory = false;
				break;
			case DecayingInventoryItemView.DecayStates.Edible:
			case DecayingInventoryItemView.DecayStates.Spoilt:
				this.ToggleState(DecayingInventoryItemView.DecayStates.Spoilt);
				this._canDropFromInventory = base.ItemCache.MatchType(Item.Types.WorldObject);
				break;
			}
		}

		public void SetDecayState(DecayingInventoryItemView.DecayStates state)
		{
			LocalPlayer.ItemDecayMachine.CancelCommandFor(this);
			this.ToggleState((state <= DecayingInventoryItemView.DecayStates.None) ? DecayingInventoryItemView.DecayStates.Fresh : state);
			this._decayStartTime = Time.time;
			if (state < DecayingInventoryItemView.DecayStates.Spoilt)
			{
				LocalPlayer.ItemDecayMachine.NewDecayCommand(this, this._decayDelay);
			}
		}

		private void ToggleState(DecayingInventoryItemView.DecayStates state)
		{
			this._prevState = this._state;
			this._state = state;
			base.NormalMaterial = this._decayStatesMats[(int)this._state];
			if (!this._hovered)
			{
				base.gameObject.GetComponent<Renderer>().sharedMaterial = base.NormalMaterial;
			}
			if (LocalPlayer.Inventory.RightHand == this)
			{
				this._held.GetComponentInChildren<Renderer>().sharedMaterial = this._decayStatesMats[(int)this._state];
			}
			for (int i = 0; i < this._renderers.Length; i++)
			{
				this._renderers[i].sharedMaterial = base.NormalMaterial;
			}
			if (state == DecayingInventoryItemView.DecayStates.None)
			{
				LocalPlayer.ItemDecayMachine.CancelCommandFor(this);
				this._decayDoneTime = Time.time - this._decayStartTime;
			}
		}
	}
}
