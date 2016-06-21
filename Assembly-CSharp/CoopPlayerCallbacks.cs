using Bolt;
using FMOD.Studio;
using Steamworks;
using System;
using TheForest.Items;
using TheForest.Items.Core;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.Special;
using TheForest.Items.World;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UdpKit;
using UniLinq;
using UnityEngine;

[BoltGlobalBehaviour]
public class CoopPlayerCallbacks : GlobalEventListener
{
	public static bool WasDisconnectedFromServer;

	private static CoopTreeId[] _allTrees;

	public static CoopTreeId[] AllTrees
	{
		get
		{
			if (CoopPlayerCallbacks._allTrees == null)
			{
				CoopPlayerCallbacks._allTrees = (from x in UnityEngine.Object.FindObjectsOfType<CoopTreeId>()
				orderby x.Id
				select x).ToArray<CoopTreeId>();
			}
			return CoopPlayerCallbacks._allTrees;
		}
	}

	public override void BoltStartBegin()
	{
		CoopVoice.VoiceChannel = BoltNetwork.CreateStreamChannel("Voice", UdpChannelMode.Unreliable, 1);
	}

	public override void BoltShutdownBegin(AddCallback registerDoneCallback)
	{
		CoopPlayerCallbacks._allTrees = null;
	}

	public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
	{
		int o = 0;
		BoltEntity boltEntity = BoltNetwork.FindEntity(new NetworkId(Blit.ReadU64(data.Data, ref o)));
		if (boltEntity.IsAttached())
		{
			CoopVoice component = boltEntity.GetComponent<CoopVoice>();
			if (component)
			{
				component.ReceiveVoiceData(data.Data, o);
			}
		}
	}

	public override void OnEvent(TakeBodyApprove evnt)
	{
		LocalPlayer.AnimControl.setMutantPickUp(evnt.Body.gameObject);
		SetCorpsePosition setCorpsePosition = SetCorpsePosition.Create(GlobalTargets.OnlyServer);
		setCorpsePosition.Corpse = evnt.Body;
		setCorpsePosition.Corpse.Freeze(false);
		setCorpsePosition.Pickup = true;
		setCorpsePosition.Send();
	}

	public override void OnEvent(Chop evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.GetComponentInChildren<chopEnemy>().triggerChop();
		}
	}

	public override void OnEvent(FauxWeaponHit evnt)
	{
		GameObject original = (GameObject)Resources.Load("CoopFauxWeapon", typeof(GameObject));
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, evnt.Position, Quaternion.identity);
		gameObject.GetComponent<CoopFauxWeapon>().Damage = evnt.Damage;
	}

	public override void OnEvent(OpenSuitcase evnt)
	{
		Collider[] array = Physics.OverlapSphere(evnt.Position, 0.5f);
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = array[i];
			if (collider.gameObject.CompareTag("suitCase"))
			{
				collider.SendMessage("Hit", evnt.Damage);
			}
		}
	}

	public static void ClearTrees()
	{
		CoopPlayerCallbacks._allTrees = null;
	}

	public override void OnEvent(ItemRemoveFromPlayer evnt)
	{
		Debug.Log("RECEIVED REMOVE ITEM: " + evnt.ItemId);
		if (evnt.ItemId == 0)
		{
			LocalPlayer.GameObject.GetComponentInChildren<LogControler>().RemoveLog(false);
		}
		else
		{
			LocalPlayer.Inventory.RemoveItem(evnt.ItemId, 1, false);
		}
	}

	public override void OnEvent(DisablePickup evnt)
	{
		if (evnt.Entity && evnt.Entity.isAttached && evnt.Entity.isOwner)
		{
			evnt.Entity.SendMessage("SetPickupUsed", evnt.Num, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void OnEvent(DestroyPickUp evnt)
	{
		if (evnt.PickUpEntity)
		{
			if (evnt.PickUpEntity.isAttached)
			{
				if (evnt.PickUpEntity.isOwner)
				{
					if (evnt.PickUpEntity.StateIs<ISuitcaseState>())
					{
						if (evnt.SibblingId >= 0)
						{
							ISuitcaseState state = evnt.PickUpEntity.GetState<ISuitcaseState>();
							state.FlaresPickedUp |= 1 << evnt.SibblingId;
						}
						else
						{
							evnt.PickUpEntity.GetState<ISuitcaseState>().ClothPickedUp = true;
						}
					}
					else
					{
						if (evnt.FakeDrop)
						{
							if (evnt.PickUpPlayer == LocalPlayer.Entity)
							{
								LocalPlayer.Inventory.FakeDrop(evnt.ItemId);
							}
							else
							{
								PlayerAddItem playerAddItem = PlayerAddItem.Create(evnt.PickUpPlayer.source);
								playerAddItem.ItemId = evnt.ItemId;
								playerAddItem.Amount = 1;
								playerAddItem.Player = evnt.PickUpPlayer;
								playerAddItem.Send();
							}
						}
						PickUp componentInChildren = evnt.PickUpEntity.GetComponentInChildren<PickUp>();
						if (!componentInChildren || !componentInChildren.TryPool())
						{
							BoltNetwork.Destroy(evnt.PickUpEntity);
						}
					}
				}
				else
				{
					DestroyPickUp destroyPickUp = DestroyPickUp.Raise(evnt.PickUpEntity.source);
					destroyPickUp.PickUpEntity = evnt.PickUpEntity;
					destroyPickUp.Send();
				}
			}
		}
		else if (evnt.PickUpPlayer.isOwner)
		{
			ItemRemoveFromPlayer itemRemoveFromPlayer = ItemRemoveFromPlayer.Create(GlobalTargets.OnlySelf);
			itemRemoveFromPlayer.ItemId = itemRemoveFromPlayer.ItemId;
			itemRemoveFromPlayer.Send();
		}
		else
		{
			ItemRemoveFromPlayer itemRemoveFromPlayer2 = ItemRemoveFromPlayer.Create(evnt.PickUpPlayer.source);
			itemRemoveFromPlayer2.ItemId = itemRemoveFromPlayer2.ItemId;
			itemRemoveFromPlayer2.Send();
		}
	}

	public override void OnEvent(FmodOneShot evnt)
	{
		if (evnt.EventPath != -1)
		{
			string text = CoopAudioEventDb.FindEvent(evnt.EventPath);
			if (text != null)
			{
				FMOD_StudioSystem.instance.PlayOneShot(text, evnt.Position, null);
			}
		}
	}

	public override void OnEvent(FmodOneShotParameter evnt)
	{
		if (evnt.EventPath != -1)
		{
			string text = CoopAudioEventDb.FindEvent(evnt.EventPath);
			if (text != null)
			{
				FMOD_StudioSystem.instance.PlayOneShot(text, evnt.Position, delegate(EventInstance eventInstance)
				{
					eventInstance.setParameterValueByIndex(evnt.Index, evnt.Value);
					return true;
				});
			}
		}
	}

	public override void OnEvent(AdminAuthed evnt)
	{
		if (evnt.IsAdmin)
		{
			CoopSteamClientStarter.IsAdmin = true;
		}
	}

	public override void OnEvent(PlayerHitByEnemey evnt)
	{
		if (evnt.Target && evnt.Target.isOwner)
		{
			LocalPlayer.GameObject.SendMessage("hitFromEnemy", evnt.Damage);
		}
	}

	public override void OnEvent(ChatEvent evnt)
	{
		ChatBox.Instance.AddLine(evnt.Sender, evnt.Message);
	}

	public override void EntityAttached(BoltEntity arg)
	{
		if (arg.StateIs<IBuildMissionState>())
		{
		}
		if (arg.StateIs<IPlayerState>())
		{
			if (arg.isOwner)
			{
				DebugInfo.Ignore(arg);
				ChatBox.Instance.RegisterPlayer("You", arg.networkId);
			}
			else
			{
				arg.source.UserData = arg;
				arg.GetState<IPlayerState>().AddCallback("name", delegate
				{
					ChatBox.Instance.RegisterPlayer(arg.GetState<IPlayerState>().name, arg.networkId);
				});
			}
		}
	}

	public override void OnEvent(RequestDestroy evnt)
	{
		if (evnt.Entity && evnt.Entity.isAttached)
		{
			evnt.Entity.transform.parent = null;
			if (evnt.Entity.isOwner)
			{
				BoltNetwork.Destroy(evnt.Entity);
			}
		}
	}

	public override void EntityDetached(BoltEntity arg)
	{
		if (arg.IsAttached() && arg.StateIs<IPlayerState>())
		{
			try
			{
				ChatBox.Instance.UnregisterPlayer(arg.networkId);
			}
			catch
			{
			}
		}
	}

	public override void Connected(BoltConnection connection)
	{
		connection.SetStreamBandwidth(40000);
	}

	public override void Disconnected(BoltConnection connection)
	{
		if (BoltNetwork.isClient && CoopClientCallbacks.OnDisconnected == null)
		{
			if (CoopLobby.IsInLobby)
			{
				SteamMatchmaking.LeaveLobby(CoopLobby.Instance.Info.LobbyId);
			}
			CoopPlayerCallbacks._allTrees = null;
			CoopPlayerCallbacks.WasDisconnectedFromServer = true;
			BoltLauncher.Shutdown();
			Application.LoadLevel("TitleScene");
		}
	}

	public override void OnEvent(HitTree evnt)
	{
		CoopTreeId coopTreeId = CoopPlayerCallbacks.AllTrees.FirstOrDefault((CoopTreeId x) => x.Id == evnt.TreeId);
		if (coopTreeId)
		{
			TreeHealth currentView = coopTreeId.GetComponent<LOD_Trees>().CurrentView;
			if (currentView)
			{
				Transform transform = currentView.transform.FindChild(evnt.ChunkParent);
				if (transform)
				{
					Transform transform2 = transform.FindChild("TreeDmg" + evnt.ChunkParent);
					if (transform2)
					{
						transform2.SendMessage("ActivateFake", evnt.ChunkId);
					}
				}
			}
		}
	}

	public override void OnEvent(PlayerHealed evnt)
	{
		if (evnt.HealTarget == LocalPlayer.Entity && !Scene.Cams.DeadCam.activeSelf)
		{
			LocalPlayer.Stats.HealedMp();
		}
	}

	public override void OnEvent(StealItem evnt)
	{
		if (BoltNetwork.isServer && evnt.robbed != LocalPlayer.Entity)
		{
			StealItem stealItem = StealItem.Create(evnt.robbed.source);
			stealItem.thief = evnt.thief;
			stealItem.robbed = evnt.robbed;
			stealItem.Send();
		}
		else if (evnt.robbed == LocalPlayer.Entity)
		{
			if (!LocalPlayer.Inventory.IsRightHandEmpty())
			{
				ItemStorageProxy component = LocalPlayer.Inventory.RightHand._held.GetComponent<ItemStorageProxy>();
				if (component)
				{
					for (int i = 0; i < component._storage.UsedSlots.Count; i++)
					{
						PlayerAddItem playerAddItem = PlayerAddItem.Create(GlobalTargets.OnlyServer);
						playerAddItem.Player = evnt.thief;
						playerAddItem.ItemId = component._storage.UsedSlots[i]._itemId;
						playerAddItem.Amount = component._storage.UsedSlots[i]._amount;
						playerAddItem.Send();
					}
					component._storage.Close();
					component._storage.UsedSlots.Clear();
					component._storage.UpdateContentVersion();
					component.CheckContentVersion();
				}
				else
				{
					PlayerAddItem playerAddItem = PlayerAddItem.Create(GlobalTargets.OnlyServer);
					playerAddItem.Player = evnt.thief;
					playerAddItem.ItemId = LocalPlayer.Inventory.RightHand._itemId;
					playerAddItem.Send();
					LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, false);
				}
			}
		}
		else if (evnt.robbed.source == LocalPlayer.Entity.source)
		{
			ItemStorage component2 = evnt.robbed.GetComponent<ItemStorage>();
			if (component2)
			{
				for (int j = 0; j < component2.UsedSlots.Count; j++)
				{
					PlayerAddItem playerAddItem2 = PlayerAddItem.Create(GlobalTargets.OnlyServer);
					playerAddItem2.Player = evnt.thief;
					playerAddItem2.ItemId = component2.UsedSlots[j]._itemId;
					playerAddItem2.Amount = component2.UsedSlots[j]._amount;
					playerAddItem2.Send();
				}
				BoltNetwork.Destroy(component2.gameObject);
			}
		}
	}

	public override void OnEvent(PlayerAddItem evnt)
	{
		if (BoltNetwork.isServer && evnt.Player != null && evnt.Player != LocalPlayer.Entity)
		{
			PlayerAddItem playerAddItem = PlayerAddItem.Create(evnt.Player.source);
			playerAddItem.ItemId = evnt.ItemId;
			playerAddItem.Amount = evnt.Amount;
			playerAddItem.Send();
		}
		else
		{
			int num = (evnt.Amount <= 1) ? 1 : evnt.Amount;
			if (!LocalPlayer.Inventory.AddItem(evnt.ItemId, num, false, false, (WeaponStatUpgrade.Types)(-2)))
			{
				for (int i = 0; i < num; i++)
				{
					LocalPlayer.Inventory.FakeDrop(evnt.ItemId);
				}
			}
		}
	}

	public override void OnEvent(Sleep evnt)
	{
		if (BoltNetwork.isClient && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Sleep)
		{
			Scene.HudGui.MpSleepLabel.gameObject.SetActive(false);
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
			if (!evnt.Aborted)
			{
				LocalPlayer.Stats.GoToSleep();
				if (Grabber.FocusedItemGO)
				{
					ShelterTrigger component = Grabber.FocusedItemGO.GetComponent<ShelterTrigger>();
					if (component && component.BreakAfterSleep)
					{
						base.StartCoroutine(component.DelayedCollapse());
					}
				}
			}
			else
			{
				LocalPlayer.Stats.GoToSleepFake();
			}
		}
	}
}
