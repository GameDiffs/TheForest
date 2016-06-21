using BoltInternal;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using UnityEngine;

internal class CoopSteamServerStarter : CoopPeerStarter
{
	public static bool SaveIsLoading;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		CoopSteamServerStarter.<Start>c__Iterator29 <Start>c__Iterator = new CoopSteamServerStarter.<Start>c__Iterator29();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	public override void Connected(BoltConnection connection)
	{
		connection.SetCanReceiveEntities(false);
		if (CoopKick.IsBanned(connection.RemoteEndPoint.SteamId))
		{
			connection.Disconnect(new CoopKickToken
			{
				Banned = true,
				KickMessage = "Host banned you permanently from his games"
			});
		}
		else
		{
			CoopServerInfo.Instance.entity.Freeze(false);
		}
	}

	public override void BoltStartDone()
	{
		BoltNetwork.AddGlobalEventListener(CoopAckChecker.Instance);
		if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue && !LevelSerializer.CanResume)
		{
			TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
		}
		if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue)
		{
			AnimalSpawnController.lastUpdate = Time.realtimeSinceStartup + 60f;
		}
		base.BoltSetup();
		BoltNetwork.Instantiate(BoltPrefabs.CoopServerInfo).transform.position = new Vector3(0f, 0f, 0f);
		ICoopServerInfo state = CoopServerInfo.Instance.state;
		this.mapState = CoopPeerStarter.MapState.Begin;
	}

	private void OnGameStart()
	{
		try
		{
			AnimalSpawnController.lastUpdate = Time.realtimeSinceStartup + 30f;
			try
			{
				BoltNetwork.UpdateSceneObjectsLookup();
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
			CoopTreeGrid.Update(BoltNetwork.SceneObjects);
			this.AttachBuildings();
		}
		finally
		{
			LoadSave.OnGameStart -= new Action(this.OnGameStart);
		}
	}

	public static void AttachBuildingBoltEntity(BoltEntity entity)
	{
		if (!BoltNetwork.isServer)
		{
			return;
		}
		if (!CoopSteamServerStarter.SaveIsLoading)
		{
			return;
		}
		if (!entity)
		{
			return;
		}
		if (entity.IsAttached())
		{
			return;
		}
		BoltEntitySettingsModifier boltEntitySettingsModifier = entity.ModifySettings();
		BridgeArchitect component = entity.GetComponent<BridgeArchitect>();
		if (boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IFireState)
		{
			BoltNetwork.Attach(entity);
		}
		else if (boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IRaftState)
		{
			BoltNetwork.Attach(entity);
			if (entity && entity.isAttached && entity.StateIs<IRaftState>())
			{
				entity.GetState<IRaftState>().IsReal = true;
			}
		}
		else if (boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IMultiHolderState)
		{
			BoltNetwork.Attach(entity);
			if (entity && entity.isAttached && entity.StateIs<IMultiHolderState>())
			{
				entity.GetState<IMultiHolderState>().IsReal = true;
				MultiHolder[] componentsInChildren = entity.GetComponentsInChildren<MultiHolder>(true);
				if (componentsInChildren.Length > 0)
				{
					componentsInChildren[0]._contentActual = componentsInChildren[0]._contentAmount;
					componentsInChildren[0]._contentTypeActual = componentsInChildren[0]._content;
				}
			}
		}
		else if (component)
		{
			BoltNetwork.Attach(entity, component.CustomToken);
		}
		else if (boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IFoundationState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IBuildingState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IRabbitCage || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.ITreeBuildingState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.ITrapLargeState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IBuildingDestructibleState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IWallChunkBuildingState)
		{
			CoopBuildingEx component2 = entity.GetComponent<CoopBuildingEx>();
			WallChunkArchitect component3 = entity.GetComponent<WallChunkArchitect>();
			if (component3)
			{
				BoltNetwork.Attach(entity, component3.CustomToken);
			}
			else if (component2)
			{
				CoopConstructionExToken coopConstructionExToken = new CoopConstructionExToken();
				coopConstructionExToken.Architects = new CoopConstructionExToken.ArchitectData[component2.Architects.Length];
				for (int i = 0; i < component2.Architects.Length; i++)
				{
					coopConstructionExToken.Architects[i].PointsCount = (component2.Architects[i] as ICoopStructure).MultiPointsCount;
					coopConstructionExToken.Architects[i].PointsPositions = (component2.Architects[i] as ICoopStructure).MultiPointsPositions.ToArray();
					coopConstructionExToken.Architects[i].CustomToken = (component2.Architects[i] as ICoopStructure).CustomToken;
					if (component2.Architects[i] is FoundationArchitect)
					{
						coopConstructionExToken.Architects[i].AboveGround = ((FoundationArchitect)component2.Architects[i])._aboveGround;
					}
					if (component2.Architects[i] is RoofArchitect && (component2.Architects[i] as RoofArchitect).CurrentSupport != null)
					{
						coopConstructionExToken.Architects[i].Support = ((component2.Architects[i] as RoofArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
					}
					if (component2.Architects[i] is FloorArchitect && (component2.Architects[i] as FloorArchitect).CurrentSupport != null)
					{
						coopConstructionExToken.Architects[i].Support = ((component2.Architects[i] as FloorArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
					}
					CoopSteamServerStarter.AttachBuildingBoltEntity(coopConstructionExToken.Architects[i].Support);
				}
				BoltNetwork.Attach(entity, coopConstructionExToken);
			}
			else
			{
				BoltNetwork.Attach(entity);
			}
		}
		else if (boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IConstructionState || boltEntitySettingsModifier.serializerId == StateSerializerTypeIds.IWallChunkConstructionState)
		{
			CoopConstructionEx component4 = entity.GetComponent<CoopConstructionEx>();
			WallChunkArchitect component5 = entity.GetComponent<WallChunkArchitect>();
			if (component5)
			{
				BoltNetwork.Attach(entity, component5.CustomToken);
			}
			else if (component4)
			{
				CoopConstructionExToken coopConstructionExToken2 = new CoopConstructionExToken();
				coopConstructionExToken2.Architects = new CoopConstructionExToken.ArchitectData[component4.Architects.Length];
				for (int j = 0; j < component4.Architects.Length; j++)
				{
					coopConstructionExToken2.Architects[j].PointsCount = (component4.Architects[j] as ICoopStructure).MultiPointsCount;
					coopConstructionExToken2.Architects[j].PointsPositions = (component4.Architects[j] as ICoopStructure).MultiPointsPositions.ToArray();
					coopConstructionExToken2.Architects[j].CustomToken = (component4.Architects[j] as ICoopStructure).CustomToken;
					if (component4.Architects[j] is FoundationArchitect)
					{
						coopConstructionExToken2.Architects[j].AboveGround = ((FoundationArchitect)component4.Architects[j])._aboveGround;
					}
					if (component4.Architects[j] is RoofArchitect && (component4.Architects[j] as RoofArchitect).CurrentSupport != null)
					{
						coopConstructionExToken2.Architects[j].Support = ((component4.Architects[j] as RoofArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
					}
					if (component4.Architects[j] is FloorArchitect && (component4.Architects[j] as FloorArchitect).CurrentSupport != null)
					{
						coopConstructionExToken2.Architects[j].Support = ((component4.Architects[j] as FloorArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
					}
					CoopSteamServerStarter.AttachBuildingBoltEntity(coopConstructionExToken2.Architects[j].Support);
				}
				BoltNetwork.Attach(entity, coopConstructionExToken2);
			}
			else
			{
				BoltNetwork.Attach(entity);
			}
		}
	}

	private void AttachBuildings()
	{
		CoopSteamServerStarter.SaveIsLoading = true;
		try
		{
			BoltEntity[] array = UnityEngine.Object.FindObjectsOfType<BoltEntity>();
			for (int i = 0; i < array.Length; i++)
			{
				BoltEntity entity = array[i];
				CoopSteamServerStarter.AttachBuildingBoltEntity(entity);
			}
		}
		finally
		{
			CoopSteamServerStarter.SaveIsLoading = false;
		}
	}

	protected override void OnLoadingDone()
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.CoopBuildMission);
		boltEntity.transform.position = new Vector3(0f, 100f, 0f);
		BoltEntity boltEntity2 = BoltNetwork.Instantiate(BoltPrefabs.CoopWeatherProxy);
		boltEntity2.GetState<IWeatherState>().TimeOfDay = -1f;
		boltEntity2.transform.position = new Vector3(0f, 200f, 0f);
		if (CoopPeerStarter.DedicatedHost)
		{
			Console.WriteLine("Dedicated Server Running");
			Console.WriteLine("Address: " + CoopDedicatedServerStarter.EndPoint);
			Console.WriteLine("Max Players: " + CoopDedicatedServerStarter.Players);
			Console.WriteLine("Save Interval: " + CoopDedicatedServerStarter.AutoSaveIntervalMinutes + " minutes");
		}
	}
}
