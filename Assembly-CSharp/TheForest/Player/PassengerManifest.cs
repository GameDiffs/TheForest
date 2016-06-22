using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class PassengerManifest : MonoBehaviour
	{
		public PassengerDatabase _db;

		public GameObject[] _foundGOs;

		public TextMesh _countText;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _itemId;

		[SerializeThis]
		private List<int> _foundPassengersIds = new List<int>();

		[SerializeThis]
		private int _foundPassengersIdsCount;

		private void OnDeserialized()
		{
			this._foundPassengersIds.RemoveRange(this._foundPassengersIdsCount, this._foundPassengersIds.Count - this._foundPassengersIdsCount);
			for (int i = 0; i < this._foundPassengersIds.Count; i++)
			{
				int passengerNum = PassengerDatabase.Instance.GetPassengerNum(this._foundPassengersIds[i]);
				if (passengerNum >= 0)
				{
					this._foundGOs[passengerNum].SetActive(true);
				}
			}
			this._countText.text = this._foundPassengersIdsCount + "/" + this._foundGOs.Length;
		}

		public void FoundPassenger(int passengerId)
		{
			if (!LocalPlayer.AnimControl.upsideDown && LocalPlayer.Inventory.Owns(this._itemId) && !this._foundPassengersIds.Contains(passengerId))
			{
				this._foundPassengersIds.Add(passengerId);
				this._foundPassengersIdsCount++;
				if (this._foundPassengersIdsCount == 1 && !LocalPlayer.Vis.currentlyTargetted)
				{
					base.StartCoroutine(this.ToggleManifest());
				}
				Scene.HudGui.ShowFoundPassenger(passengerId);
				int passengerNum = PassengerDatabase.Instance.GetPassengerNum(passengerId);
				if (passengerNum >= 0)
				{
					this._foundGOs[passengerNum].SetActive(true);
				}
				this._countText.text = this._foundPassengersIdsCount + "/" + this._foundGOs.Length;
				GameStats.FoundPassenger.Invoke();
			}
		}

		[DebuggerHidden]
		private IEnumerator ToggleManifest()
		{
			PassengerManifest.<ToggleManifest>c__Iterator185 <ToggleManifest>c__Iterator = new PassengerManifest.<ToggleManifest>c__Iterator185();
			<ToggleManifest>c__Iterator.<>f__this = this;
			return <ToggleManifest>c__Iterator;
		}
	}
}
