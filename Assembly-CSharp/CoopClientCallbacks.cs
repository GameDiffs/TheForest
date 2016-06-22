using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class CoopClientCallbacks : GlobalEventListener
{
	private static bool invokedServerNotResponding;

	public static Action OnDisconnected;

	public static Action ServerNotResponding;

	public static Action ServerIsResponding;

	private void Start()
	{
		base.StartCoroutine("ServerResponseTest");
	}

	public override void OnEvent(CreepHitPlayer evnt)
	{
		LocalPlayer.Transform.GetComponentInChildren<PlayerStats>().SendMessage("Explosion", evnt.damage, SendMessageOptions.DontRequireReceiver);
	}

	[DebuggerHidden]
	private IEnumerator ServerResponseTest()
	{
		return new CoopClientCallbacks.<ServerResponseTest>c__Iterator18();
	}

	private CoopComponentDisabler InitCCD(BoltEntity entity)
	{
		CoopComponentDisabler coopComponentDisabler = entity.GetComponent<CoopComponentDisabler>();
		if (!coopComponentDisabler)
		{
			coopComponentDisabler = entity.gameObject.AddComponent<CoopComponentDisabler>();
		}
		return coopComponentDisabler;
	}

	public override void Connected(BoltConnection connection)
	{
		CoopKick.Client_Banned = false;
		CoopKick.Client_KickMessage = string.Empty;
	}

	public override void Disconnected(BoltConnection connection)
	{
		CoopKickToken coopKickToken = connection.DisconnectToken as CoopKickToken;
		if (coopKickToken != null)
		{
			CoopKick.Client_Banned = coopKickToken.Banned;
			CoopKick.Client_KickMessage = coopKickToken.KickMessage;
			UnityEngine.Object.FindObjectOfType<CoopSteamClientStarter>().CancelInvoke("OnDisconnected");
			CoopClientCallbacks.OnDisconnected = null;
		}
		else
		{
			CoopKick.Client_Banned = false;
			CoopKick.Client_KickMessage = string.Empty;
			if (CoopClientCallbacks.OnDisconnected != null)
			{
				CoopClientCallbacks.OnDisconnected();
			}
		}
	}

	public override void OnEvent(ItemHolderTakeItem evnt)
	{
		if (evnt.Target)
		{
			Component[] componentsInChildren = evnt.Target.GetComponentsInChildren(typeof(MultiHolder), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as MultiHolder).TakeItemMP(null, (MultiHolder.ContentTypes)evnt.ContentType);
			}
		}
	}

	public override void OnEvent(PerformRepairBuilding evnt)
	{
		if (evnt.Building)
		{
			evnt.Building.GetComponentInChildren<BuildingHealth>().RespawnBuilding();
		}
	}

	public override void OnEvent(ItemHolderAddItem evnt)
	{
		if (evnt.Target)
		{
			Component[] componentsInChildren = evnt.Target.GetComponentsInChildren(typeof(MultiHolder), true);
			if (componentsInChildren.Length > 0)
			{
				(componentsInChildren[0] as MultiHolder).AddItemMP((MultiHolder.ContentTypes)evnt.ContentType, evnt.RaisedBy);
			}
		}
	}

	public override void OnEvent(FireAddFuelEvent evnt)
	{
		if (evnt.Target && evnt.Target.GetComponentInChildren<Fire2>())
		{
			evnt.Target.GetComponentInChildren<Fire2>().AddFuel_Complete();
		}
	}

	public override void EntityFrozen(BoltEntity entity)
	{
		if (entity.StateIs<IMutantState>() || entity.StateIs<IAnimalState>())
		{
			if (entity.StateIs<IMutantFemaleDummyState>())
			{
				return;
			}
			if (entity.StateIs<IMutantMaleDummyState>())
			{
				return;
			}
			if (!entity.transform.GetComponent<CoopMutantDummy>())
			{
				CoopComponentDisabler coopComponentDisabler = this.InitCCD(entity);
				coopComponentDisabler.DisableComponents();
			}
		}
	}

	public override void EntityThawed(BoltEntity entity)
	{
		if (entity.StateIs<IMutantState>() || entity.StateIs<IAnimalState>())
		{
			if (entity.StateIs<IMutantFemaleDummyState>())
			{
				return;
			}
			if (entity.StateIs<IMutantMaleDummyState>())
			{
				return;
			}
			if (!entity.transform.GetComponent<CoopMutantDummy>())
			{
				CoopComponentDisabler coopComponentDisabler = this.InitCCD(entity);
				coopComponentDisabler.EnableComponents();
			}
		}
	}

	public override void OnEvent(DroppedBody evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.SendMessage("clientDrop", evnt.rot, SendMessageOptions.DontRequireReceiver);
			evnt.Target.SendMessage("setRagDollDrop", SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void OnEvent(ragdollActivate evnt)
	{
		if (evnt.Target)
		{
			evnt.Target.SendMessage("enableNetRagDoll", SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void OnEvent(storeRagDollName evnt)
	{
		evnt.Target.SendMessage("getRagDollName", evnt.name, SendMessageOptions.DontRequireReceiver);
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
				evnt.Corpse.SendMessage("sendResetRagDoll", SendMessageOptions.DontRequireReceiver);
				evnt.Corpse.transform.position = new Vector3(4096f, 4096f, 4096f);
			}
			else if (evnt.Destroy)
			{
				BoltNetwork.Destroy(evnt.Corpse);
			}
			else
			{
				evnt.Corpse.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
				evnt.Corpse.SendMessage("setRagDollDrop", SendMessageOptions.DontRequireReceiver);
				evnt.Corpse.transform.position = evnt.Position;
				evnt.Corpse.transform.rotation = ((!(evnt.Rotation == default(Quaternion))) ? evnt.Rotation : Quaternion.identity);
				MultiHolder.GetTriggerChild(evnt.Corpse.transform).gameObject.SetActive(true);
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

	public override void OnEvent(playerBlock evnt)
	{
		evnt.target.gameObject.SendMessage("forcePlayerBlock", SendMessageOptions.DontRequireReceiver);
	}
}
