using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class LogHolder : EntityBehaviour<IItemHolderState>
{
	public GameObject[] LogRender;

	public GameObject TakeIcon;

	public GameObject AddIcon;

	public bool Pushable;

	[SerializeThis]
	private int Logs;

	private FMOD_StudioEventEmitter emitter;

	private float _originalMass;

	private float _originalDrag;

	private void Awake()
	{
		base.enabled = false;
		this.emitter = base.GetComponent<FMOD_StudioEventEmitter>();
		if (this.Pushable)
		{
			this._originalMass = base.transform.parent.GetComponent<Rigidbody>().mass;
			this._originalDrag = base.transform.parent.GetComponent<Rigidbody>().drag;
		}
	}

	private void Update()
	{
		if (this.Logs > 0 && LocalPlayer.Inventory.Logs.Amount < 2 && (!BoltNetwork.isRunning || this.entity.isAttached))
		{
			this.TakeIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				if (!BoltNetwork.isRunning)
				{
					if (LocalPlayer.Inventory.Logs.Lift())
					{
						this.LogRender[this.Logs - 1].SetActive(false);
						this.Logs--;
						this.RefreshMassAndDrag();
					}
				}
				else
				{
					ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
					itemHolderTakeItem.Target = this.entity;
					itemHolderTakeItem.Player = LocalPlayer.Entity;
					itemHolderTakeItem.Send();
				}
			}
		}
		else
		{
			this.TakeIcon.SetActive(false);
		}
		if (this.Logs < 7 && LocalPlayer.Inventory.Logs.Amount > 0 && (!BoltNetwork.isRunning || this.entity.isAttached))
		{
			this.AddIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Craft"))
			{
				LocalPlayer.Inventory.Logs.PutDown(false, false, true);
				if (BoltNetwork.isRunning)
				{
					ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
					itemHolderAddItem.Target = this.entity;
					itemHolderAddItem.Send();
				}
				else
				{
					this.Logs++;
					this.LogRender[this.Logs - 1].SetActive(true);
					this.RefreshMassAndDrag();
				}
				this.emitter.Play();
			}
		}
		else
		{
			this.AddIcon.SetActive(false);
		}
	}

	private void OnDeserialized()
	{
		this.RefreshMassAndDrag();
	}

	private void GrabEnter()
	{
		LocalPlayer.Inventory.DontShowDrop = true;
		base.enabled = true;
	}

	private void GrabExit()
	{
		LocalPlayer.Inventory.DontShowDrop = false;
		base.enabled = false;
		this.AddIcon.SetActive(false);
		this.TakeIcon.SetActive(false);
	}

	private void RefreshMassAndDrag()
	{
		if (this.Pushable)
		{
			base.transform.parent.GetComponent<Rigidbody>().mass = this._originalMass + (float)(10 * this.Logs);
			base.transform.parent.GetComponent<Rigidbody>().drag = this._originalDrag + 0.5f * (float)this.Logs;
		}
	}

	public override void Attached()
	{
		base.state.AddCallback("ItemCount", new PropertyCallbackSimple(this.ItemCountChangedMP));
		if (BoltNetwork.isServer)
		{
			base.state.ItemCount = this.Logs;
		}
	}

	public void TakeItemMP(BoltEntity targetPlayer)
	{
		if (base.state.ItemCount > 0)
		{
			base.state.ItemCount = base.state.ItemCount - 1;
			this.RefreshMassAndDrag();
			PlayerAddItem playerAddItem;
			if (targetPlayer.isOwner)
			{
				playerAddItem = PlayerAddItem.Create(GlobalTargets.OnlySelf);
			}
			else
			{
				playerAddItem = PlayerAddItem.Create(targetPlayer.source);
			}
			playerAddItem.ItemId = LocalPlayer.Inventory.Logs._logItemId;
			playerAddItem.Send();
		}
	}

	public void AddItemMP()
	{
		base.state.ItemCount = Mathf.Min(base.state.ItemCount + 1, this.LogRender.Length);
		this.RefreshMassAndDrag();
	}

	private void ItemCountChangedMP()
	{
		this.Logs = base.state.ItemCount;
		for (int i = 0; i < this.LogRender.Length; i++)
		{
			this.LogRender[i].SetActive(false);
		}
		for (int j = 0; j < this.Logs; j++)
		{
			this.LogRender[j].SetActive(true);
		}
	}
}
