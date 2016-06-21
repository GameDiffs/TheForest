using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.UI
{
	public class MpBannedPlayerList : MonoBehaviour
	{
		public BannedPlayerListRow _rowPrefab;

		public UIGrid _grid;

		public UILabel _playerCount;

		private void OnEnable()
		{
			this.Refresh();
			LocalPlayer.FpCharacter.LockView(true);
		}

		private void OnDisable()
		{
			if (!Scene.HudGui.MpPlayerList.gameObject.activeInHierarchy && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
			{
				LocalPlayer.FpCharacter.UnLockView();
			}
		}

		public void Refresh()
		{
			CoopKick.CheckBanEndTimes();
			for (int i = this._grid.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this._grid.GetChild(i).gameObject);
			}
			for (int j = 0; j < CoopKick.Instance.KickedPlayers.Count; j++)
			{
				this.AddBannedPlayerRow(CoopKick.Instance.KickedPlayers[j]);
			}
			this._grid.repositionNow = true;
			this._playerCount.text = CoopKick.Instance.KickedPlayers.Count<CoopKick.KickedPlayer>().ToString();
		}

		public void UnBan(ulong steamid)
		{
			CoopKick.UnBanPlayer(steamid);
			this.Refresh();
		}

		public void BackToPlayerList()
		{
			Scene.HudGui.MpPlayerList.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}

		private void AddBannedPlayerRow(CoopKick.KickedPlayer kicked)
		{
			BannedPlayerListRow bannedPlayerListRow = UnityEngine.Object.Instantiate<BannedPlayerListRow>(this._rowPrefab);
			bannedPlayerListRow.transform.parent = this._grid.transform;
			bannedPlayerListRow.transform.localPosition = Vector3.zero;
			bannedPlayerListRow.transform.localScale = Vector3.one;
			bannedPlayerListRow._nameLabel.text = kicked.Name;
			bannedPlayerListRow._durationLabel.text = ((kicked.BanEndTime != 0L) ? this.GetRemainingTime(kicked.BanEndTime) : "permanent");
			if (BoltNetwork.isServer)
			{
				EventDelegate eventDelegate = new EventDelegate(this, "UnBan");
				eventDelegate.parameters[0] = new EventDelegate.Parameter(kicked.SteamId);
				bannedPlayerListRow._unbanButton.onClick.Add(eventDelegate);
			}
		}

		private string GetRemainingTime(long timestamp)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, Convert.ToInt32(timestamp - DateTime.UtcNow.ToUnixTimestamp()));
			if (timeSpan.TotalDays > 1.0)
			{
				return Mathf.FloorToInt(Convert.ToSingle(timeSpan.TotalDays)) + " day" + ((timeSpan.TotalDays < 2.0) ? string.Empty : "s");
			}
			if (timeSpan.TotalHours > 1.0)
			{
				return Mathf.FloorToInt(Convert.ToSingle(timeSpan.TotalHours)) + " hour" + ((timeSpan.TotalHours < 2.0) ? string.Empty : "s");
			}
			if (timeSpan.TotalMinutes > 1.0)
			{
				return Mathf.FloorToInt(Convert.ToSingle(timeSpan.TotalMinutes)) + " minute" + ((timeSpan.TotalMinutes < 2.0) ? string.Empty : "s");
			}
			return Mathf.FloorToInt(Convert.ToSingle(timeSpan.TotalSeconds)) + " second" + ((timeSpan.TotalSeconds < 2.0) ? string.Empty : "s");
		}
	}
}
