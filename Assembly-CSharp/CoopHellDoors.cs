using Bolt;
using System;
using TheForest.World;
using UnityEngine;

public class CoopHellDoors : EntityBehaviour<IWorldHellDoors>
{
	private float _lastSend;

	public static CoopHellDoors Instance;

	public DoorWeightOpener[] Doors;

	private void Awake()
	{
		CoopHellDoors.Instance = this;
	}

	public override void Attached()
	{
		base.state.AddCallback("NewProperty[]", new PropertyCallback(this.DoorItemsChanged));
	}

	private void OnWeightChanged()
	{
		if (this.entity.IsAttached())
		{
			for (int i = 0; i < base.state.NewProperty.Length; i++)
			{
				for (int j = 0; j < base.state.NewProperty[i].Items.Length; j++)
				{
					if (base.state.NewProperty[i].Items[j] != this.Doors[i]._slots[j].StoredItemId)
					{
						if (this.Doors[i]._slots[j]._removed)
						{
							this.Doors[i]._slots[j]._removed = false;
							AddItemToDoor addItemToDoor = AddItemToDoor.Create(GlobalTargets.OnlyServer);
							addItemToDoor.Door = i;
							addItemToDoor.Slot = j;
							addItemToDoor.Item = -1;
							addItemToDoor.Send();
						}
						else if (this.Doors[i]._slots[j]._added)
						{
							this.Doors[i]._slots[j]._added = false;
							AddItemToDoor addItemToDoor2 = AddItemToDoor.Create(GlobalTargets.OnlyServer);
							addItemToDoor2.Door = i;
							addItemToDoor2.Slot = j;
							addItemToDoor2.Item = this.Doors[i]._slots[j].StoredItemId;
							addItemToDoor2.Send();
						}
						this._lastSend = Time.time;
					}
				}
			}
		}
	}

	private void DoorItemsChanged(IState _, string propertyPath, ArrayIndices arrayIndices)
	{
		for (int i = 0; i < base.state.NewProperty.Length; i++)
		{
			for (int j = 0; j < base.state.NewProperty[i].Items.Length; j++)
			{
				this.Doors[i]._slots[j].ItemIdChanged_Network(base.state.NewProperty[i].Items[j]);
			}
		}
	}
}
