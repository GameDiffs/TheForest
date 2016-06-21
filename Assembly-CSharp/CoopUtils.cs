using BoltInternal;
using System;
using TheForest.Utils;
using UnityEngine;

public static class CoopUtils
{
	public static bool IsDedicatedServerAdmin(this BoltConnection connection)
	{
		if (connection == null)
		{
			return false;
		}
		CoopJoinDedicatedServerToken coopJoinDedicatedServerToken = connection.ConnectToken as CoopJoinDedicatedServerToken;
		return coopJoinDedicatedServerToken != null && !string.IsNullOrEmpty(coopJoinDedicatedServerToken.AdminPassword) && !string.IsNullOrEmpty(CoopDedicatedServerStarter.AdminPassword) && coopJoinDedicatedServerToken.AdminPassword == CoopDedicatedServerStarter.AdminPassword;
	}

	public static float CalculatePriorityFor(BoltConnection connection, BoltEntity entity, float multiplier, int skipped)
	{
		if (!entity || !entity.isAttached)
		{
			return 0f;
		}
		skipped = Mathf.Max(1, skipped);
		if (BoltNetwork.isClient)
		{
			return 256f;
		}
		if (entity.StateIs<IPlayerState>())
		{
			return 256f * multiplier * (float)skipped;
		}
		if (entity.StateIs<ITreeCutState>())
		{
			return 256f * multiplier * (float)skipped;
		}
		if (entity.StateIs<ITreeFallState>())
		{
			return 256f * multiplier * (float)skipped;
		}
		if (entity.StateIs<IMutantFemaleDummyState>())
		{
			return 256f * multiplier * (float)skipped;
		}
		if (entity.StateIs<IMutantMaleDummyState>())
		{
			return 256f * multiplier * (float)skipped;
		}
		BoltEntity boltEntity = connection.UserData as BoltEntity;
		if (boltEntity != null)
		{
			float num = Vector3.Distance(entity.transform.position, boltEntity.transform.position);
			if (num < 256f)
			{
				if (entity.StateIs<IMutantState>())
				{
					return 256f * multiplier * (float)skipped;
				}
				return Mathf.Clamp(256f - num, 0f, 256f) * multiplier * (float)skipped;
			}
		}
		return 0f;
	}

	public static BoltEntity AttachLocalPlayer(GameObject go, string name)
	{
		return CoopUtils.AttachLocalPlayer(go, name, false);
	}

	public static BoltEntity AttachLocalPlayer(GameObject go, string name, bool attachToRespawn)
	{
		if (go.GetComponent<BoltEntity>() && go.GetComponent<BoltEntity>().isAttached)
		{
			return go.GetComponent<BoltEntity>();
		}
		go.AddComponent<BoltPlayerSetup>();
		go.AddComponent<BoltEntity>();
		using (BoltEntitySettingsModifier boltEntitySettingsModifier = go.GetComponent<BoltEntity>().ModifySettings())
		{
			boltEntitySettingsModifier.prefabId = BoltPrefabs.player_net;
			boltEntitySettingsModifier.serializerId = StateSerializerTypeIds.IPlayerState;
			boltEntitySettingsModifier.allowInstantiateOnClient = true;
			boltEntitySettingsModifier.persistThroughSceneLoads = true;
			boltEntitySettingsModifier.clientPredicted = false;
			boltEntitySettingsModifier.updateRate = 1;
		}
		BoltEntity component = BoltNetwork.Attach(go).GetComponent<BoltEntity>();
		component.GetState<IPlayerState>().name = ((!string.IsNullOrEmpty(name)) ? name : "UNKNOWN");
		LocalPlayer.Entity = component;
		BoltNetwork.SetCanReceiveEntities(true);
		return component;
	}

	public static BoltEntity AttachLocalPlayer(string name)
	{
		return CoopUtils.AttachLocalPlayer(LocalPlayer.GameObject, name);
	}
}
