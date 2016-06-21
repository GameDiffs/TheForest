using Bolt;
using System;
using TheForest.Items.Core;
using TheForest.Items.Inventory;

public class CoopSharableStorageProxy : EntityBehaviour<IPlayerState>
{
	public ItemStorage _storage;

	private bool _initialized;

	private void Start()
	{
		if (!this._initialized && this.entity.isAttached)
		{
			this.Attached();
		}
	}

	public override void Attached()
	{
		if (!this._initialized)
		{
			this._initialized = true;
			base.state.AddCallback("SharableStorage[]", new PropertyCallbackSimple(this.RefreshStorage));
			this.RefreshStorage();
		}
	}

	private void RefreshStorage()
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < base.state.SharableStorage.Length; i++)
		{
			if (i < this._storage.UsedSlots.Count)
			{
				if (base.state.SharableStorage[i] != this._storage.UsedSlots[i + num]._itemId)
				{
					if (base.state.SharableStorage[i] > 0)
					{
						this._storage.UsedSlots[i + num]._itemId = base.state.SharableStorage[i];
						flag = true;
					}
					else
					{
						this._storage.UsedSlots.RemoveAt(i);
						num--;
						flag = true;
					}
				}
			}
			else if (base.state.SharableStorage[i] > 0)
			{
				this._storage.UsedSlots.Add(new InventoryItem
				{
					_itemId = base.state.SharableStorage[i],
					_amount = 1,
					_maxAmount = 1
				});
				flag = true;
			}
		}
		if (flag)
		{
			this._storage.UpdateContentVersion();
		}
	}
}
