using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.UI
{
	public class MpPlayerList : MonoBehaviour
	{
		public PlayerListRow _rowPrefab;

		public UIGrid _grid;

		public UILabel _playerCount;

		public GameObject _banListButton;

		private void OnEnable()
		{
			LocalPlayer.FpCharacter.LockView(true);
			if (BoltNetwork.isClient)
			{
				this._banListButton.SetActive(false);
			}
			this.Refresh();
		}

		private void OnDisable()
		{
			if (!Scene.HudGui.MpBannedPlayerList.gameObject.activeInHierarchy && (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World || LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Sleep))
			{
				LocalPlayer.FpCharacter.UnLockView();
			}
		}

		public void Refresh()
		{
			for (int i = this._grid.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this._grid.GetChild(i).gameObject);
			}
			IPlayerState state = LocalPlayer.Entity.GetState<IPlayerState>();
			this.AddPlayerRow(state.name, LocalPlayer.Entity, false);
			int num = 1;
			for (int j = 0; j < Scene.SceneTracker.allPlayerEntities.Count; j++)
			{
				state = Scene.SceneTracker.allPlayerEntities[j].GetState<IPlayerState>();
				this.AddPlayerRow(state.name, Scene.SceneTracker.allPlayerEntities[j], BoltNetwork.isServer);
				num++;
			}
			this._grid.repositionNow = true;
			this._playerCount.text = num + "/" + CoopLobby.Instance.Info.MemberLimit;
		}

		public void Kick(ulong steamid)
		{
			BoltEntity entityFromSteamID = this.GetEntityFromSteamID(steamid);
			if (entityFromSteamID)
			{
				CoopKick.KickPlayer(entityFromSteamID, -1, "Host kicked you from the game");
				base.Invoke("Refresh", 0.1f);
			}
		}

		public void Ban(ulong steamid)
		{
			BoltEntity entityFromSteamID = this.GetEntityFromSteamID(steamid);
			if (entityFromSteamID)
			{
				CoopKick.BanPlayer(entityFromSteamID);
				base.Invoke("Refresh", 0.1f);
			}
		}

		public void Close()
		{
			Scene.HudGui.MpPlayerListCamGo.SetActive(false);
		}

		public void ToBannedPlayerList()
		{
			Scene.HudGui.MpBannedPlayerList.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}

		private void AddPlayerRow(string name, BoltEntity client, bool showButtons)
		{
			PlayerListRow playerListRow = UnityEngine.Object.Instantiate<PlayerListRow>(this._rowPrefab);
			playerListRow.transform.parent = this._grid.transform;
			playerListRow.transform.localPosition = Vector3.zero;
			playerListRow.transform.localScale = Vector3.one;
			playerListRow._entity = client;
			playerListRow._overlay._name.text = name;
			if (showButtons && client)
			{
				EventDelegate eventDelegate = new EventDelegate(this, "Kick");
				eventDelegate.parameters[0] = new EventDelegate.Parameter(client.source.RemoteEndPoint.SteamId.Id);
				playerListRow._kickButton.onClick.Add(eventDelegate);
				playerListRow._kickButton.gameObject.SetActive(true);
				EventDelegate eventDelegate2 = new EventDelegate(this, "Ban");
				eventDelegate2.parameters[0] = new EventDelegate.Parameter(client.source.RemoteEndPoint.SteamId.Id);
				playerListRow._banButton.onClick.Add(eventDelegate2);
				playerListRow._banButton.gameObject.SetActive(true);
			}
			else
			{
				playerListRow._kickButton.gameObject.SetActive(false);
				playerListRow._banButton.gameObject.SetActive(false);
			}
		}

		private BoltEntity GetEntityFromSteamID(ulong steamid)
		{
			return Scene.SceneTracker.allPlayerEntities.FirstOrDefault((BoltEntity e) => e.source.RemoteEndPoint.SteamId.Id == steamid);
		}
	}
}
