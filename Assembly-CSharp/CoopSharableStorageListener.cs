using Bolt;
using System;
using TheForest.Items.Core;

public class CoopSharableStorageListener : EntityBehaviour<IPlayerState>
{
	public ItemStorage _storage;

	private int _contentVersion = -1;

	private void Awake()
	{
		base.enabled = BoltNetwork.isRunning;
	}

	private void Update()
	{
		if (this.entity && this.entity.isAttached)
		{
			this.CheckContentVersion();
		}
	}

	public void CheckContentVersion()
	{
		if (this._contentVersion != this._storage.ContentVersion)
		{
			this.RefreshState();
			this._contentVersion = this._storage.ContentVersion;
		}
	}

	private void RefreshState()
	{
		int i;
		for (i = 0; i < this._storage.UsedSlots.Count; i++)
		{
			base.state.SharableStorage[i] = this._storage.UsedSlots[i]._itemId;
		}
		while (i < base.state.SharableStorage.Length)
		{
			base.state.SharableStorage[i] = -1;
			i++;
		}
	}
}
