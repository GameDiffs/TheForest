using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Player;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class rockThrowerItemHolder : EntityEventListener<IRockThrowerState>
{
	[ItemIdPicker]
	public int _itemid;

	[ItemIdPicker]
	public int _headid;

	public GameObject[] ItemsRender;

	public GameObject TakeIcon;

	public GameObject AddIcon;

	public Transform lever;

	[Header("FMOD")]
	public string addItemEvent;

	[SerializeThis]
	public int Items;

	private bool hasPreloaded;

	public int ammoLoaded;

	private rockThrowerAnimEvents am;

	private int lastAmmoLoadedType;

	private int lastContentType;

	private void Awake()
	{
		base.enabled = false;
		this.am = base.transform.parent.GetComponentInChildren<rockThrowerAnimEvents>();
	}

	private void OnEnable()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.addItemEvent
		});
		this.hasPreloaded = true;
	}

	private void OnDisable()
	{
		if (this.hasPreloaded)
		{
			FMODCommon.UnloadEvents(new string[]
			{
				this.addItemEvent
			});
			this.hasPreloaded = false;
		}
	}

	private void Update()
	{
		if (BoltNetwork.isServer)
		{
			base.state.ItemCount = this.Items;
			base.state.ammoCount = this.ammoLoaded;
			base.state.leverRotate = this.lever.localRotation.y;
		}
		if (BoltNetwork.isClient)
		{
			this.Items = base.state.ItemCount;
			this.ammoLoaded = base.state.ammoCount;
			if (this.ammoLoaded > 0)
			{
				for (int i = 0; i < this.ammoLoaded; i++)
				{
					this.am.rockAmmo[i].SetActive(true);
				}
			}
			else
			{
				GameObject[] rockAmmo = this.am.rockAmmo;
				for (int j = 0; j < rockAmmo.Length; j++)
				{
					GameObject gameObject = rockAmmo[j];
					gameObject.SetActive(false);
				}
			}
		}
		if (!LocalPlayer.AnimControl.onRockThrower && BoltNetwork.isRunning)
		{
			Vector3 localEulerAngles = this.lever.localEulerAngles;
			localEulerAngles.y = base.state.leverRotate;
			this.lever.localEulerAngles = localEulerAngles;
		}
		if (this.Items > 0)
		{
			this.TakeIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				int contentType = 0;
				bool flag = false;
				if (LocalPlayer.Inventory.AddItem(this._itemid, 1, false, false, (WeaponStatUpgrade.Types)(-2)))
				{
					flag = true;
				}
				if (flag)
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
						itemHolderTakeItem.Target = this.entity;
						itemHolderTakeItem.ContentType = contentType;
						itemHolderTakeItem.Player = LocalPlayer.Entity;
						itemHolderTakeItem.Send();
					}
					else
					{
						this.ItemsRender[this.Items - 1].SetActive(false);
						this.Items--;
					}
				}
			}
		}
		else if (this.TakeIcon.activeSelf)
		{
			this.TakeIcon.SetActive(false);
		}
		if (this.Items < this.ItemsRender.Length && LocalPlayer.Inventory.Owns(this._itemid))
		{
			this.AddIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Craft"))
			{
				if (this.addItemEvent.Length > 0)
				{
					FMODCommon.PlayOneshot(this.addItemEvent, base.transform);
				}
				else
				{
					LocalPlayer.Sfx.PlayPutDown(base.gameObject);
				}
				int num = 0;
				bool flag2 = false;
				if (LocalPlayer.Inventory.RemoveItem(this._itemid, 1, false))
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
						itemHolderAddItem.ContentType = num;
						itemHolderAddItem.Target = this.entity;
						itemHolderAddItem.Send();
					}
					else
					{
						this.Items++;
						this.ItemsRender[this.Items - 1].SetActive(true);
						this.ItemsRender[this.Items - 1].SendMessage("setupAmmoType", num);
					}
				}
			}
		}
		else
		{
			this.AddIcon.SetActive(false);
		}
	}

	private void OnDeserialized()
	{
		if (!BoltNetwork.isClient)
		{
			for (int i = 0; i < this.ItemsRender.Length; i++)
			{
				GameObject gameObject = this.ItemsRender[i];
				bool flag = i < this.Items;
				if (gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
			}
		}
	}

	private void GrabEnter()
	{
		base.enabled = (!BoltNetwork.isRunning || this.entity.isAttached);
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.AddIcon.SetActive(false);
		this.TakeIcon.SetActive(false);
	}

	public override void Attached()
	{
		base.state.AddCallback("ItemCount", new PropertyCallbackSimple(this.ItemCountChangedMP));
		base.state.AddCallback("ammoCount", new PropertyCallbackSimple(this.AmmoCountChangedMP));
		base.state.AddCallback("leverRotate", new PropertyCallbackSimple(this.leverRotateChangedMP));
		if (BoltNetwork.isServer)
		{
			base.state.ItemCount = this.Items;
		}
	}

	public void loadItemIntoBasket(int type)
	{
		if (BoltNetwork.isRunning)
		{
			if (this.Items > 0)
			{
				this.entity.Freeze(false);
				base.state.ItemCount = (this.Items = Mathf.Max(0, this.Items - 1));
				this.am.rockAmmo[this.ammoLoaded].SetActive(true);
				this.am.rockAmmo[this.ammoLoaded].SendMessage("setupAmmoType", type);
				this.lastAmmoLoadedType = type;
				base.state.ammoCount = (this.ammoLoaded = Mathf.Min(3, this.ammoLoaded + 1));
				this.am.ammoCount = this.ammoLoaded;
			}
		}
		else
		{
			this.Items = Mathf.Max(0, this.Items - 1);
			this.am.rockAmmo[this.ammoLoaded].SetActive(true);
			this.am.rockAmmo[this.ammoLoaded].SendMessage("setupAmmoType", type);
			this.ammoLoaded = Mathf.Min(3, this.ammoLoaded + 1);
			this.am.ammoCount = this.ammoLoaded;
		}
	}

	public void resetBasketAmmo()
	{
		base.state.ammoCount = (this.ammoLoaded = 0);
		this.am.ammoCount = 0;
		GameObject[] rockAmmo = this.am.rockAmmo;
		for (int i = 0; i < rockAmmo.Length; i++)
		{
			GameObject gameObject = rockAmmo[i];
			gameObject.SetActive(false);
		}
	}

	public void TakeItemMP(BoltEntity targetPlayer, int content = -1)
	{
		if (this.Items > 0)
		{
			this.entity.Freeze(false);
			base.state.ItemCount = (this.Items = Mathf.Max(0, this.Items - 1));
		}
		else
		{
			ItemRemoveFromPlayer itemRemoveFromPlayer;
			if (targetPlayer.isOwner)
			{
				itemRemoveFromPlayer = ItemRemoveFromPlayer.Create(GlobalTargets.OnlySelf);
			}
			else
			{
				itemRemoveFromPlayer = ItemRemoveFromPlayer.Create(targetPlayer.source);
			}
			itemRemoveFromPlayer.ItemId = this._itemid;
			itemRemoveFromPlayer.Send();
		}
	}

	public void AddItemMP(int type)
	{
		if (this.Items < this.ItemsRender.Length)
		{
			base.state.ItemCount = (this.Items = Mathf.Min(this.Items + 1, this.ItemsRender.Length));
			this.lastContentType = type;
			this.entity.Freeze(false);
		}
	}

	private void ItemCountChangedMP()
	{
		if (BoltNetwork.isClient)
		{
			this.Items = base.state.ItemCount;
		}
		for (int i = 0; i < this.ItemsRender.Length; i++)
		{
			this.ItemsRender[i].SetActive(false);
		}
		for (int j = 0; j < this.Items; j++)
		{
			this.ItemsRender[j].SetActive(true);
			if (j == this.Items - 1)
			{
			}
		}
	}

	private void AmmoCountChangedMP()
	{
		if (BoltNetwork.isClient)
		{
			this.ammoLoaded = base.state.ammoCount;
		}
		if (this.ammoLoaded > 0)
		{
			for (int i = 0; i < this.ammoLoaded; i++)
			{
				if (!this.am.rockAmmo[i].activeSelf)
				{
					this.am.rockAmmo[i].SetActive(true);
					Debug.Log("enabling ammo, amount = " + this.ammoLoaded);
				}
			}
		}
		else
		{
			GameObject[] rockAmmo = this.am.rockAmmo;
			for (int j = 0; j < rockAmmo.Length; j++)
			{
				GameObject gameObject = rockAmmo[j];
				gameObject.SetActive(false);
			}
		}
	}

	private void leverRotateChangedMP()
	{
		if (!LocalPlayer.AnimControl.onRockThrower)
		{
			Vector3 localEulerAngles = this.lever.localEulerAngles;
			localEulerAngles.y = base.state.leverRotate;
			this.lever.localEulerAngles = localEulerAngles;
		}
	}

	public void disableTriggerMP()
	{
		if (BoltNetwork.isRunning)
		{
			RockThrowerActivated rockThrowerActivated = RockThrowerActivated.Create(GlobalTargets.Everyone);
			rockThrowerActivated.Target = this.entity;
			rockThrowerActivated.Send();
		}
	}

	public void enableTriggerMP()
	{
		if (BoltNetwork.isRunning)
		{
			RockThrowerDeActivated rockThrowerDeActivated = RockThrowerDeActivated.Create(GlobalTargets.Everyone);
			rockThrowerDeActivated.Target = this.entity;
			rockThrowerDeActivated.Send();
		}
	}

	public void forceRemoveItem()
	{
		if (BoltNetwork.isRunning)
		{
			RockThrowerRemoveItem rockThrowerRemoveItem = RockThrowerRemoveItem.Create(GlobalTargets.OnlyServer);
			setAmmoType component = this.ItemsRender[this.Items - 1].GetComponent<setAmmoType>();
			if (component)
			{
				rockThrowerRemoveItem.ContentType = component.ammoType;
			}
			rockThrowerRemoveItem.Target = this.entity;
			rockThrowerRemoveItem.Player = LocalPlayer.Entity;
			rockThrowerRemoveItem.Send();
		}
		else
		{
			setAmmoType component2 = this.ItemsRender[this.Items - 1].GetComponent<setAmmoType>();
			this.ItemsRender[this.Items - 1].SetActive(false);
			if (component2)
			{
				this.loadItemIntoBasket(component2.ammoType);
			}
			if (this.Items > 0)
			{
				LocalPlayer.Sfx.PlayWhoosh();
			}
		}
	}

	public void sendResetAmmoMP()
	{
		if (BoltNetwork.isRunning)
		{
			RockThrowerResetAmmo rockThrowerResetAmmo = RockThrowerResetAmmo.Create(GlobalTargets.Everyone);
			rockThrowerResetAmmo.Target = this.entity;
			rockThrowerResetAmmo.Send();
		}
		else
		{
			this.ammoLoaded = 0;
			this.am.ammoCount = 0;
			GameObject[] rockAmmo = this.am.rockAmmo;
			for (int i = 0; i < rockAmmo.Length; i++)
			{
				GameObject gameObject = rockAmmo[i];
				gameObject.SetActive(false);
			}
		}
	}

	public void sendAnimVars(int var, bool onoff)
	{
		RockThrowerAnimate rockThrowerAnimate = RockThrowerAnimate.Create(GlobalTargets.Everyone);
		rockThrowerAnimate.animVar = var;
		rockThrowerAnimate.onoff = onoff;
		rockThrowerAnimate.Target = this.entity;
		rockThrowerAnimate.Send();
	}

	public void sendLandTarget()
	{
		RockThrowerLandTarget rockThrowerLandTarget = RockThrowerLandTarget.Create(GlobalTargets.OnlyServer);
		rockThrowerLandTarget.landPos = this.am.throwPos.GetComponent<rockThrowerAimingReticle>()._currentLandTarget;
		rockThrowerLandTarget.Target = this.entity;
		rockThrowerLandTarget.Send();
	}
}
