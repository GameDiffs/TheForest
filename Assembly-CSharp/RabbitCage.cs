using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class RabbitCage : EntityBehaviour<IRabbitCage>
{
	public GameObject[] RabbitRender;

	public GameObject TakeIcon;

	public GameObject AddIcon;

	[ItemIdPicker(Item.Types.Equipment)]
	public int _rabbitAliveItemId;

	private PlayerInventory Player;

	[SerializeThis]
	private int Rabbits;

	private int RabbitsReal
	{
		get
		{
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				return base.state.RabbitCount;
			}
			return this.Rabbits;
		}
		set
		{
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				base.state.RabbitCount = value;
			}
			this.Rabbits = value;
		}
	}

	public override void Attached()
	{
		if (this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			base.state.RabbitCount = this.Rabbits;
		}
		base.state.AddCallback("RabbitCount", new PropertyCallbackSimple(this.RabbitCountChanged));
	}

	private void RabbitCountChanged()
	{
		this.Rabbits = base.state.RabbitCount;
		for (int i = 0; i < this.RabbitRender.Length; i++)
		{
			this.RabbitRender[i].SetActive(false);
		}
		for (int j = 0; j < this.RabbitsReal; j++)
		{
			this.RabbitRender[j].SetActive(true);
		}
	}

	private void Awake()
	{
		base.enabled = false;
	}

	private void OnDeserialized()
	{
		for (int i = 0; i < this.RabbitsReal; i++)
		{
			this.RabbitRender[i].SetActive(true);
		}
	}

	private void GrabEnter()
	{
		base.enabled = true;
	}

	private void GrabExit()
	{
		base.enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((this.Player == null || this.Player.transform != other.transform.root) && other.transform.root.CompareTag("Player"))
		{
			this.Player = other.transform.root.GetComponent<PlayerInventory>();
		}
	}

	private void Update()
	{
		bool flag = this.Player.HasInSlot(Item.EquipmentSlot.RightHand, this._rabbitAliveItemId);
		if (this.RabbitsReal > 0 && !flag)
		{
			this.TakeIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				LocalPlayer.Sfx.PlayWhoosh();
				this.Player.AddItem(this._rabbitAliveItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
				if (BoltNetwork.isRunning)
				{
					RabbitTake rabbitTake = RabbitTake.Create(GlobalTargets.OnlyServer);
					rabbitTake.Cage = this.entity;
					rabbitTake.Send();
				}
				else
				{
					this.RabbitRender[this.RabbitsReal - 1].SetActive(false);
					this.RabbitsReal--;
				}
			}
		}
		else
		{
			this.TakeIcon.SetActive(false);
		}
		if (this.RabbitsReal < 7 && flag)
		{
			this.AddIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Craft"))
			{
				this.Player.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
				if (BoltNetwork.isRunning)
				{
					RabbitAdd rabbitAdd = RabbitAdd.Create(GlobalTargets.OnlyServer);
					rabbitAdd.Cage = this.entity;
					rabbitAdd.Send();
				}
				else
				{
					this.RabbitsReal++;
					this.RabbitRender[this.RabbitsReal - 1].SetActive(true);
				}
			}
		}
		else
		{
			this.AddIcon.SetActive(false);
		}
	}
}
