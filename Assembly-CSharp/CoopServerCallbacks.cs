using Bolt;
using System;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using TheForest.Utils;
using TheForest.World;
using UniLinq;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class CoopServerCallbacks : GlobalEventListener
{
	private Vector3[] player_positions;

	private CachedGlobal<sceneTracker> tracker;

	public override void OnEvent(RackAdd evnt)
	{
		if (evnt.Rack && evnt.Slot >= 0)
		{
			evnt.Rack.GetState<IWeaponRackState>().Slots[evnt.Slot] = evnt.ItemId;
		}
	}

	public override void OnEvent(AdminCommand evnt)
	{
		if (evnt.RaisedBy.IsDedicatedServerAdmin())
		{
			CoopAdminCommand.Recv(evnt.Command, evnt.Data);
		}
	}

	public override void OnEvent(RackRemove evnt)
	{
		if (evnt.Rack && evnt.Slot >= 0)
		{
			evnt.Rack.GetState<IWeaponRackState>().Slots[evnt.Slot] = 0;
		}
	}

	public override void OnEvent(DestroyTree evnt)
	{
		BoltEntity tree = evnt.Tree;
		if (tree && tree.isAttached)
		{
			if (tree.canFreeze)
			{
				tree.Freeze(false);
			}
			ITreeCutState state = tree.GetState<ITreeCutState>();
			if (state.State == 0)
			{
				state.State = 1;
			}
			state.Damage = 16;
		}
	}

	public override void OnEvent(AddItemToDoor evnt)
	{
		if (!CoopHellDoors.Instance)
		{
			throw new Exception("could not find hell doors root object");
		}
		if (CoopHellDoors.Instance.state.NewProperty[evnt.Door].Items[evnt.Slot] != evnt.Item)
		{
			CoopHellDoors.Instance.state.NewProperty[evnt.Door].Items[evnt.Slot] = evnt.Item;
		}
	}

	public override void OnEvent(Burn evnt)
	{
		if (evnt.Entity)
		{
			if (evnt.Entity.StateIs<IMutantState>())
			{
				EnemyHealth[] componentsInChildren = evnt.Entity.GetComponentsInChildren<EnemyHealth>(true);
				if (componentsInChildren.Length > 0)
				{
					componentsInChildren[0].Burn();
				}
			}
			else if (evnt.Entity.StateIs<IAnimalState>())
			{
				animalHealth[] componentsInChildren2 = evnt.Entity.GetComponentsInChildren<animalHealth>(true);
				if (componentsInChildren2.Length > 0)
				{
					componentsInChildren2[0].Burn();
				}
			}
		}
	}

	public override void OnEvent(Poison evnt)
	{
		if (evnt.Entity)
		{
			if (evnt.Entity.StateIs<IMutantState>())
			{
				EnemyHealth[] componentsInChildren = evnt.Entity.GetComponentsInChildren<EnemyHealth>(true);
				if (componentsInChildren.Length > 0)
				{
					componentsInChildren[0].Poison();
				}
			}
			else if (evnt.Entity.StateIs<IAnimalState>())
			{
				animalHealth[] componentsInChildren2 = evnt.Entity.GetComponentsInChildren<animalHealth>(true);
				if (componentsInChildren2.Length > 0)
				{
					componentsInChildren2[0].Poison();
				}
			}
		}
	}

	public override void OnEvent(AddEffigyPart evnt)
	{
		if (evnt.Effigy)
		{
			Component[] componentsInChildren = evnt.Effigy.GetComponentsInChildren(typeof(EffigyArchitect), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as EffigyArchitect).SpawnPartReal(new EffigyArchitect.Part
				{
					_itemId = evnt.ItemId,
					_position = evnt.Position,
					_rotation = evnt.Rotation
				}, true);
			}
		}
	}

	public override void OnEvent(LocalizedHit evnt)
	{
		if (evnt.Building)
		{
			LocalizedHitData data = default(LocalizedHitData);
			data._damage = evnt.Damage;
			data._position = evnt.Position;
			if (evnt.Chunk == -1)
			{
				evnt.Building.GetComponent<BuildingHealth>().LocalizedHitReal(data);
			}
			else
			{
				evnt.Building.GetComponent<BuildingHealth>().GetChunk(evnt.Chunk).LocalizedHitReal(data);
			}
		}
	}

	public override void OnEvent(PlaceTrophy evnt)
	{
		if (evnt.Maker)
		{
			TrophyMaker[] componentsInChildren = evnt.Maker.GetComponentsInChildren<TrophyMaker>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].PlaceTrophy(evnt.ItemId);
			}
		}
	}

	public override void OnEvent(ResetTrap evnt)
	{
		if (evnt.TargetTrap && evnt.TargetTrap.isAttached && evnt.TargetTrap.isOwner)
		{
			try
			{
				ResetTraps componentInChildren = evnt.TargetTrap.GetComponentInChildren<ResetTraps>();
				if (componentInChildren)
				{
					if (!componentInChildren.gameObject.activeSelf)
					{
						return;
					}
					componentInChildren.RestoreSafe();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			BoltNetwork.Instantiate(evnt.TargetTrap.prefabId, evnt.TargetTrap.transform.position, evnt.TargetTrap.transform.rotation);
			BoltNetwork.Destroy(evnt.TargetTrap);
		}
	}

	public override void OnEvent(HitCorpse evnt)
	{
		if (evnt.Entity)
		{
			NetworkArray_Integer bodyPartsDamage;
			NetworkArray_Values<int> expr_20 = bodyPartsDamage = evnt.Entity.GetComponentInChildren<CoopSliceAndDiceMutant>().BodyPartsDamage;
			int num;
			int expr_28 = num = evnt.BodyPartIndex;
			num = bodyPartsDamage[num];
			expr_20[expr_28] = num - evnt.Damage;
		}
	}

	public override void Connected(BoltConnection connection)
	{
		connection.SetCanReceiveEntities(false);
		if (CoopPeerStarter.DedicatedHost)
		{
			if (!string.IsNullOrEmpty(CoopDedicatedServerStarter.ServerPassword))
			{
				CoopJoinDedicatedServerToken coopJoinDedicatedServerToken = connection.ConnectToken as CoopJoinDedicatedServerToken;
				if (coopJoinDedicatedServerToken != null && coopJoinDedicatedServerToken.ServerPassword != CoopDedicatedServerStarter.ServerPassword)
				{
					connection.Disconnect(new CoopJoinDedicatedServerFailed
					{
						Error = "Incorrect server password"
					});
					return;
				}
			}
			if (!string.IsNullOrEmpty(CoopDedicatedServerStarter.AdminPassword))
			{
				CoopJoinDedicatedServerToken coopJoinDedicatedServerToken2 = connection.ConnectToken as CoopJoinDedicatedServerToken;
				if (coopJoinDedicatedServerToken2 != null && coopJoinDedicatedServerToken2.AdminPassword == CoopDedicatedServerStarter.AdminPassword)
				{
					AdminAuthed adminAuthed = AdminAuthed.Create(connection);
					adminAuthed.IsAdmin = true;
					adminAuthed.Send();
				}
			}
		}
	}

	public override void OnEvent(FoundationExLocalizedHit evnt)
	{
		if (evnt.Entity)
		{
			LocalizedHitData data = default(LocalizedHitData);
			data._damage = evnt.HitDamage;
			data._position = evnt.HitPosition;
			evnt.Entity.GetComponent<FoundationArchitect>().GetChunk(evnt.Chunk).LocalizedHitReal(data);
		}
	}

	public override void OnEvent(FoundationExLookAtExplosion evnt)
	{
		if (evnt.Entity)
		{
			evnt.Entity.GetComponent<FoundationArchitect>().GetChunk(evnt.Chunk).LookAtExplosionReal(evnt.Position);
		}
	}

	public override void OnEvent(ToggleWallAddition evnt)
	{
		if (evnt.Wall)
		{
			evnt.Wall.GetComponent<WallChunkArchitect>().PerformToggleAddition();
		}
	}

	public override void OnEvent(ToggleWallDoor evnt)
	{
		if (evnt.Entity)
		{
			evnt.Entity.GetComponentInChildren<WallDoor>().ToggleDoorStatusAction(false);
		}
	}

	public override void OnEvent(PlaceWallChunk evnt)
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(evnt.prefab, evnt.token);
		if (evnt.parent)
		{
			boltEntity.transform.parent = evnt.parent.transform;
		}
		LocalPlayer.Create.RefreshGrabber();
	}

	public override void OnEvent(DestroyWithTag evnt)
	{
		if (evnt.Entity)
		{
			DestroyOnContactWithTag componentInChildren = evnt.Entity.GetComponentInChildren<DestroyOnContactWithTag>();
			if (componentInChildren)
			{
				if (componentInChildren._destroyTarget)
				{
					foreach (Transform transform in componentInChildren._destroyTarget.transform)
					{
						if (transform.GetComponent<BoltEntity>())
						{
							transform.parent = null;
							BoltNetwork.Destroy(transform.gameObject);
						}
					}
				}
				componentInChildren.Perform(true);
				BoltNetwork.Destroy(evnt.Entity, new CoopDestroyTagToken());
			}
			else
			{
				BoltNetwork.Destroy(evnt.Entity);
			}
		}
	}

	public override void OnEvent(DamageTree evnt)
	{
		if (evnt.TreeEntity)
		{
			ITreeCutState state = evnt.TreeEntity.GetState<ITreeCutState>();
			int num = Mathf.Max(1, evnt.Damage);
			state.Damage += num;
			this.UpdateChunks(state, evnt.DamageIndex, num);
		}
	}

	public override void OnEvent(BurnTree evnt)
	{
		if (evnt.Entity && evnt.Entity.isOwner)
		{
			ITreeCutState state = evnt.Entity.GetState<ITreeCutState>();
			state.Burning = evnt.IsBurning;
			evnt.Entity.Freeze(false);
		}
	}

	private void UpdateChunks(ITreeCutState state, int chunk, int damage)
	{
		int num = 10;
		while (damage > 0 && --num >= 0)
		{
			switch (chunk)
			{
			case 1:
				if (state.Chunk1 < 4)
				{
					state.Chunk1++;
					damage--;
				}
				else
				{
					chunk = 2;
				}
				break;
			case 2:
				if (state.Chunk2 < 4)
				{
					damage--;
					state.Chunk2++;
				}
				else
				{
					chunk = 3;
				}
				break;
			case 3:
				if (state.Chunk3 < 4)
				{
					damage--;
					state.Chunk3++;
				}
				else
				{
					chunk = 4;
				}
				break;
			case 4:
				if (state.Chunk4 < 4)
				{
					damage--;
					state.Chunk4++;
				}
				break;
			}
		}
	}

	public override void OnEvent(LightEffigy evnt)
	{
		if (evnt.Effigy)
		{
			evnt.Effigy.GetState<IBuildingEffigyState>().Lit = true;
		}
	}

	public override void OnEvent(CutTriggerActivated evnt)
	{
		Debug.Log("starting cutTriggerActivated");
		if (evnt.Trap)
		{
			Component[] componentsInChildren = evnt.Trap.GetComponentsInChildren(typeof(trapTrigger), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as trapTrigger).releaseNooseTrap();
				Debug.Log("sending cutTriggerActivated");
			}
		}
	}

	public override void OnEvent(doReleaseFromTrap evnt)
	{
		if (evnt.target)
		{
		}
	}

	public override void OnEvent(SetCorpsePosition evnt)
	{
		if (evnt.Corpse)
		{
			if (evnt.Corpse.transform.parent != null)
			{
				evnt.Corpse.gameObject.SendMessageUpwards("releaseNooseTrapMP", SendMessageOptions.DontRequireReceiver);
			}
			evnt.Corpse.transform.parent = null;
			evnt.Corpse.Freeze(false);
			if (evnt.Pickup)
			{
				evnt.Corpse.transform.position = new Vector3(4096f, 4096f, 4096f);
			}
			else if (evnt.Destroy)
			{
				BoltNetwork.Destroy(evnt.Corpse);
			}
			else
			{
				evnt.Corpse.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
				evnt.Corpse.transform.position = evnt.Position;
				evnt.Corpse.transform.rotation = ((!(evnt.Rotation == default(Quaternion))) ? evnt.Rotation : Quaternion.identity);
				MultiHolder.GetTriggerChild(evnt.Corpse.transform).gameObject.SetActive(true);
			}
		}
	}

	public override void OnEvent(SpawnCutTree evnt)
	{
		CoopTreeId coopTreeId = CoopPlayerCallbacks.AllTrees.FirstOrDefault((CoopTreeId x) => x.Id == evnt.TreeId);
		if (coopTreeId && coopTreeId.state.State == 0)
		{
			coopTreeId.state.State = 1;
			coopTreeId.entity.Freeze(false);
		}
	}

	public override void OnEvent(DropItem evnt)
	{
		BoltNetwork.Instantiate(evnt.PrefabId, evnt.Position, evnt.Rotation);
	}

	public override void OnEvent(PlaceConstruction evnt)
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(evnt.PrefabId, evnt.Position, evnt.Rotation);
		if (boltEntity.GetComponent<TreeStructure>())
		{
			boltEntity.GetComponent<TreeStructure>().TreeId = evnt.TreeIndex;
		}
		if (evnt.Parent)
		{
			boltEntity.transform.parent = evnt.Parent.transform;
		}
		if (evnt.AboveGround)
		{
			FoundationArchitect component = boltEntity.GetComponent<FoundationArchitect>();
			if (component)
			{
				component._aboveGround = evnt.AboveGround;
			}
		}
		boltEntity.SendMessage("OnDeserialized", SendMessageOptions.DontRequireReceiver);
		LocalPlayer.Create.RefreshGrabber();
	}

	public override void OnEvent(FireLightEvent evnt)
	{
		if (evnt.Target)
		{
			if (evnt.Target.GetComponentInChildren<Fire2>())
			{
				evnt.Target.GetComponentInChildren<Fire2>().Action_LightFire();
			}
			if (evnt.Target.GetComponentInChildren<FireStand>())
			{
				evnt.Target.GetComponentInChildren<FireStand>().LightFireMP();
			}
		}
	}

	public override void OnEvent(FireAddFuelEvent evnt)
	{
		if (evnt.Target)
		{
			if (evnt.Target.GetComponentInChildren<Fire2>())
			{
				evnt.Target.GetComponentInChildren<Fire2>().Action_AddFuel();
			}
			if (evnt.Target.GetComponentInChildren<FireStand>())
			{
				evnt.Target.GetComponentInChildren<FireStand>().AddToFuelMP();
			}
		}
	}

	public override void OnEvent(RemoveWater evnt)
	{
		if (evnt.Entity)
		{
			WaterSource[] componentsInChildren = evnt.Entity.GetComponentsInChildren<WaterSource>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].RemoveWater(evnt.Amount);
			}
		}
	}

	public override void OnEvent(BreakPlank evnt)
	{
		CoopWeatherProxy.Instance.state.BreakableWalls[evnt.Index] = 1;
	}

	public override void OnEvent(PlaceFoundationEx evnt)
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(evnt.Prefab, evnt.Token);
		boltEntity.transform.position = evnt.Position;
		if (evnt.Parent)
		{
			boltEntity.transform.parent = evnt.Parent.transform;
		}
		LocalPlayer.Create.RefreshGrabber();
	}

	public override void OnEvent(GrowGarden evnt)
	{
		if (evnt.Garden)
		{
			Garden[] componentsInChildren = evnt.Garden.GetComponentsInChildren<Garden>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].PlantSeed_Real(evnt.SeedNum);
			}
		}
	}

	public override void OnEvent(TriggerLargeTrap evnt)
	{
		if (evnt.Trap)
		{
			evnt.Trap.GetState<ITrapLargeState>().Sprung = true;
			Component[] componentsInChildren = evnt.Trap.GetComponentsInChildren(typeof(trapTrigger), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as trapTrigger).TriggerLargeTrap(null);
			}
		}
	}

	public override void OnEvent(ClientSuitcasePush evnt)
	{
		if (evnt.Suitcase)
		{
			evnt.Suitcase.GetComponentInChildren<Rigidbody>().velocity = evnt.Direction;
			evnt.Suitcase.GetComponent<CoopSuitcase>().enabled = true;
		}
	}

	public override void OnEvent(PushRaft evnt)
	{
		if (evnt.Raft.IsAttached())
		{
			Component[] componentsInChildren = evnt.Raft.GetComponentsInChildren(typeof(CoopRaftPusher2), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as CoopRaftPusher2).PushRaft(evnt.Direction);
			}
		}
	}

	public override void OnEvent(RaftGrab evnt)
	{
		if (evnt.Raft)
		{
			IRaftState state = evnt.Raft.GetState<IRaftState>();
			bool flag = state.GrabbedBy;
			bool flag2 = evnt.Player;
			if (!flag || (!flag2 && evnt.RaisedBy == state.GrabbedBy.source))
			{
				state.GrabbedBy = evnt.Player;
				if (!flag2)
				{
					Scene.ActiveMB.StartCoroutine(evnt.Raft.GetComponent<Buoyancy>().ResetRigidbody());
				}
			}
		}
	}

	public override void OnEvent(RaftControl evnt)
	{
		if (evnt.Raft)
		{
			RaftPushMP component = evnt.Raft.GetComponent<RaftPushMP>();
			component.ReceivedCommand((RaftPush.MoveDirection)evnt.Movement, evnt.Rotation);
		}
	}

	public override void OnEvent(SledGrab evnt)
	{
		if (evnt.Player && evnt.Sled)
		{
			IMultiHolderState state = evnt.Sled.GetState<IMultiHolderState>();
			if (!state.GrabbedBy)
			{
				state.GrabbedBy = evnt.Player;
			}
		}
	}

	public override void OnEvent(BreakCrateEvent evnt)
	{
		if (CoopWorldCrates.Instance)
		{
			CoopWorldCrates.Instance.state.Broken[evnt.Index] = 1;
		}
	}

	public override void OnEvent(PlayerHitEnemy ev)
	{
		if (!ev.Target)
		{
			return;
		}
		if (ev.Hit == 0)
		{
			return;
		}
		try
		{
			EnemyHealth.CurrentAttacker = (ev.RaisedBy.UserData as BoltEntity);
			Transform transform;
			if (ev.Target.GetComponent<animalHealth>())
			{
				transform = ev.Target.transform;
			}
			else
			{
				transform = ev.Target.transform.GetChild(0);
			}
			if (ev.HitHead)
			{
				transform.SendMessage("HitHead", SendMessageOptions.DontRequireReceiver);
			}
			if (ev.getStealthAttack)
			{
				transform.SendMessage("getStealthAttack", SendMessageOptions.DontRequireReceiver);
			}
			transform.SendMessage("getAttacker", (ev.RaisedBy.UserData as BoltEntity).gameObject, SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("getAttackerType", ev.getAttackerType, SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("getAttackDirection", ev.getAttackDirection, SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("getCombo", ev.getCombo, SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("takeDamage", ev.takeDamage, SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("setSkinDamage", UnityEngine.Random.Range(0, 3), SendMessageOptions.DontRequireReceiver);
			transform.SendMessage("HitReal", ev.Hit, SendMessageOptions.DontRequireReceiver);
			if (ev.HitAxe)
			{
				transform.SendMessage("HitAxe", SendMessageOptions.DontRequireReceiver);
			}
			if (ev.Burn)
			{
				transform.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
			}
		}
		finally
		{
			EnemyHealth.CurrentAttacker = null;
		}
	}

	public override void OnEvent(SendMessageEvent evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.gameObject.SendMessage(evnt.Message);
		}
	}

	public override void OnEvent(CancelBluePrint evnt)
	{
		if (evnt.BluePrint)
		{
			Craft_Structure componentInChildren = evnt.BluePrint.GetComponentInChildren<Craft_Structure>();
			if (componentInChildren)
			{
				componentInChildren.CancelBlueprintSafe();
			}
		}
	}

	public override void OnEvent(AddRepairMaterial evnt)
	{
		if (evnt.Building)
		{
			evnt.Building.SendMessage("AddRepairMaterialReal", evnt.IsLog);
		}
	}

	public override void OnEvent(ItemHolderAddItem evnt)
	{
		if (evnt.Target)
		{
			rockThrowerItemHolder componentInChildren = evnt.Target.GetComponentInChildren<rockThrowerItemHolder>();
			if (componentInChildren)
			{
				componentInChildren.AddItemMP(evnt.ContentType);
				return;
			}
			MultiThrowerItemHolder componentInChildren2 = evnt.Target.GetComponentInChildren<MultiThrowerItemHolder>();
			if (componentInChildren2)
			{
				componentInChildren2.AddItemMP(evnt.ContentType);
				return;
			}
			LogHolder componentInChildren3 = evnt.Target.GetComponentInChildren<LogHolder>();
			if (componentInChildren3)
			{
				componentInChildren3.AddItemMP();
			}
			else
			{
				ItemHolder componentInChildren4 = evnt.Target.GetComponentInChildren<ItemHolder>();
				if (componentInChildren4)
				{
					componentInChildren4.AddItemMP();
				}
				else
				{
					MultiHolder[] componentsInChildren = evnt.Target.GetComponentsInChildren<MultiHolder>(true);
					if (componentsInChildren.Length > 0)
					{
						componentsInChildren[0].AddItemMP((MultiHolder.ContentTypes)evnt.ContentType);
					}
				}
			}
		}
	}

	public override void OnEvent(TakeBody evnt)
	{
		if (evnt.Sled && evnt.Body)
		{
			MultiHolder[] componentsInChildren = evnt.Sled.GetComponentsInChildren<MultiHolder>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].TakeBodyMP(evnt.Body, evnt.RaisedBy);
			}
		}
	}

	public override void OnEvent(AddBody evnt)
	{
		if (evnt.Sled && evnt.Body)
		{
			MultiHolder[] componentsInChildren = evnt.Sled.GetComponentsInChildren<MultiHolder>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].AddBodyMP(evnt.Body);
			}
		}
	}

	public override void OnEvent(OpenSuitcase2 evnt)
	{
		if (evnt.Suitcase)
		{
			evnt.Suitcase.GetState<ISuitcaseState>().Open = true;
		}
	}

	public override void OnEvent(ItemHolderTakeItem evnt)
	{
		if (evnt.Target)
		{
			rockThrowerItemHolder componentInChildren = evnt.Target.GetComponentInChildren<rockThrowerItemHolder>();
			if (componentInChildren)
			{
				componentInChildren.TakeItemMP(evnt.Player, evnt.ContentType);
				return;
			}
			MultiThrowerItemHolder componentInChildren2 = evnt.Target.GetComponentInChildren<MultiThrowerItemHolder>();
			if (componentInChildren2)
			{
				componentInChildren2.TakeItemMP(evnt.Player, evnt.ContentType);
				return;
			}
			LogHolder componentInChildren3 = evnt.Target.GetComponentInChildren<LogHolder>();
			if (componentInChildren3)
			{
				componentInChildren3.TakeItemMP(evnt.Player);
				return;
			}
			ItemHolder componentInChildren4 = evnt.Target.GetComponentInChildren<ItemHolder>();
			if (componentInChildren4)
			{
				componentInChildren4.TakeItemMP(evnt.Player);
				return;
			}
			MultiHolder[] componentsInChildren = evnt.Target.GetComponentsInChildren<MultiHolder>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].TakeItemMP(evnt.Player, (MultiHolder.ContentTypes)evnt.ContentType);
				return;
			}
		}
	}

	public override void OnEvent(RockThrowerRemoveItem evnt)
	{
		if (evnt.Target)
		{
			rockThrowerItemHolder componentInChildren = evnt.Target.GetComponentInChildren<rockThrowerItemHolder>();
			if (componentInChildren)
			{
				componentInChildren.loadItemIntoBasket(evnt.ContentType);
				return;
			}
			MultiThrowerItemHolder componentInChildren2 = evnt.Target.GetComponentInChildren<MultiThrowerItemHolder>();
			if (componentInChildren2)
			{
				componentInChildren2.loadItemIntoBasket(evnt.ContentType);
				return;
			}
		}
	}

	public override void OnEvent(RockThrowerActivated evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.SendMessage("disableTrigger");
		}
	}

	public override void OnEvent(RockThrowerDeActivated evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.SendMessage("enableTrigger");
		}
	}

	public override void OnEvent(RockThrowerAnimate evnt)
	{
		if (evnt.Target)
		{
			coopRockThrower component = evnt.Target.GetComponent<coopRockThrower>();
			if (component)
			{
				component.setAnimator(evnt.animVar, evnt.onoff);
			}
		}
	}

	public override void OnEvent(RockThrowerResetAmmo evnt)
	{
		if (evnt.Target)
		{
			rockThrowerItemHolder componentInChildren = evnt.Target.GetComponentInChildren<rockThrowerItemHolder>();
			if (componentInChildren)
			{
				componentInChildren.resetBasketAmmo();
				return;
			}
			MultiThrowerItemHolder componentInChildren2 = evnt.Target.GetComponentInChildren<MultiThrowerItemHolder>();
			if (componentInChildren2)
			{
				componentInChildren2.resetBasketAmmo();
			}
		}
	}

	public override void OnEvent(RockThrowerLandTarget evnt)
	{
		if (evnt.Target)
		{
			rockThrowerAnimEvents componentInChildren = evnt.Target.GetComponentInChildren<rockThrowerAnimEvents>();
			if (componentInChildren)
			{
				componentInChildren.landTarget = evnt.landPos;
			}
		}
	}

	public override void OnEvent(ChatEvent evnt)
	{
		ChatEvent chatEvent = ChatEvent.Raise(GlobalTargets.AllClients);
		chatEvent.Sender = evnt.Sender;
		chatEvent.Message = evnt.Message;
		chatEvent.Send();
	}

	public override void OnEvent(AddIngredient evnt)
	{
		if (evnt.Construction)
		{
			evnt.Construction.GetComponentInChildren<Craft_Structure>().AddIngrendient_Actual(evnt.IngredientNum, false);
		}
	}

	public override void OnEvent(DestroyBuilding evnt)
	{
		if (evnt.BuildingEntity)
		{
			BoltNetwork.Destroy(evnt.BuildingEntity);
		}
	}

	public override void OnEvent(RabbitAdd evnt)
	{
		if (evnt.Cage)
		{
			evnt.Cage.GetState<IRabbitCage>().RabbitCount++;
		}
	}

	public override void OnEvent(RabbitTake evnt)
	{
		if (evnt.Cage)
		{
			evnt.Cage.GetState<IRabbitCage>().RabbitCount--;
		}
	}

	public override void OnEvent(SpawnBunny evnt)
	{
		Transform transform = (UnityEngine.Object.Instantiate(Resources.Load<CoopRabbitReference>("CoopRabbitReference").Rabbit, evnt.Pos, Quaternion.identity) as GameObject).transform;
		if (!transform)
		{
			return;
		}
		if (transform)
		{
			transform.GetChild(0).eulerAngles = new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
		}
		if (transform)
		{
			transform.SendMessage("startUpdateSpawn");
		}
		AnimalSpawnController.AttachAnimalToNetwork(null, null, transform.gameObject);
	}

	public override void OnEvent(playerSwingWeapon evnt)
	{
		if (Scene.SceneTracker)
		{
			for (int i = 0; i < Scene.SceneTracker.visibleEnemies.Count; i++)
			{
				if (Scene.SceneTracker.visibleEnemies[i])
				{
					Scene.SceneTracker.visibleEnemies[i].SendMessage("setPlayerAttacking", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public override void OnEvent(playerBlock evnt)
	{
		evnt.target.gameObject.SendMessage("forcePlayerBlock", SendMessageOptions.DontRequireReceiver);
	}

	public override void EntityDetached(BoltEntity entity)
	{
		if (entity.StateIs<IPlayerState>() && Scene.SceneTracker)
		{
			if (Scene.SceneTracker.allPlayers.Contains(entity.gameObject))
			{
				Scene.SceneTracker.allPlayers.Remove(entity.gameObject);
			}
			if (Scene.SceneTracker.allPlayerEntities.Contains(entity))
			{
				Scene.SceneTracker.allPlayerEntities.Remove(entity);
			}
		}
	}

	public override void EntityReceived(BoltEntity entity)
	{
		if (entity.StateIs<IPlayerState>() && Scene.SceneTracker)
		{
			if (!Scene.SceneTracker.allPlayers.Contains(entity.gameObject))
			{
				Scene.SceneTracker.allPlayers.Add(entity.gameObject);
			}
			if (!Scene.SceneTracker.allPlayerEntities.Contains(entity))
			{
				Scene.SceneTracker.allPlayerEntities.Add(entity);
			}
		}
	}

	private void Update()
	{
		if (CoopServerInfo.Instance && CoopLobby.Instance != null && !CoopPeerStarter.Dedicated)
		{
			CoopLobby arg_46_0 = CoopLobby.Instance;
			int num = BoltNetwork.clients.Count<BoltConnection>() + 1;
			CoopServerInfo.Instance.state.PlayerCount = num;
			arg_46_0.SetCurrentMembers(num);
		}
		if (CoopServerInfo.Instance && this.tracker.Component)
		{
			if (LocalPlayer.Entity && string.IsNullOrEmpty(CoopServerInfo.Instance.state.PlayerNames[0]))
			{
				CoopServerInfo.Instance.state.PlayerNames[0] = LocalPlayer.Entity.GetState<IPlayerState>().name + " (host)";
			}
			int num2 = Mathf.Min(CoopServerInfo.Instance.state.PlayerNames.Count<string>() - 1, this.tracker.Component.allPlayerEntities.Count);
			for (int i = 0; i < num2; i++)
			{
				if (CoopServerInfo.Instance.state.PlayerNames[i + 1] != this.tracker.Component.allPlayerEntities[i].GetState<IPlayerState>().name)
				{
					CoopServerInfo.Instance.state.PlayerNames[i + 1] = this.tracker.Component.allPlayerEntities[i].GetState<IPlayerState>().name;
				}
			}
			for (int j = this.tracker.Component.allPlayerEntities.Count + 1; j < CoopServerInfo.Instance.state.PlayerNames.Length; j++)
			{
				if (!string.IsNullOrEmpty(CoopServerInfo.Instance.state.PlayerNames[j]))
				{
					CoopServerInfo.Instance.state.PlayerNames[j] = string.Empty;
				}
			}
		}
		CoopTreeGrid.AttachTrees();
		if (this.tracker.Component)
		{
			CoopTreeGrid.AttachAdjacent(this.tracker.Component.allPlayers);
		}
	}
}
