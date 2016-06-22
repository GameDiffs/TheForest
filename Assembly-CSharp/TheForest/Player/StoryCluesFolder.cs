using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Tools;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class StoryCluesFolder : MonoBehaviour
	{
		[Serializable]
		public class ClueItem
		{
			[ItemIdPicker]
			public int _itemId;

			public GameObject[] _renderers;
		}

		public List<StoryCluesFolder.ClueItem> _clues;

		public GameObject[] _cluePages;

		public GameObject _exceedingCluesPage;

		private void Update()
		{
			bool flag = true;
			for (int i = 0; i < this._cluePages.Length; i++)
			{
				if (this._cluePages[i].activeSelf)
				{
					flag = false;
					break;
				}
			}
			if (this._exceedingCluesPage.activeSelf != flag)
			{
				this._exceedingCluesPage.SetActive(flag);
			}
		}

		private void OnDestroy()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnInventoryAddedItem));
		}

		private void OnInventoryAddedItem(object o)
		{
			this.OnInventoryAddedItem((int)o);
		}

		private void OnInventoryAddedItem(int itemId)
		{
			StoryCluesFolder.ClueItem clueItem = null;
			for (int i = 0; i < this._clues.Count; i++)
			{
				if (this._clues[i]._itemId == itemId)
				{
					clueItem = this._clues[i];
					break;
				}
			}
			if (clueItem != null)
			{
				for (int j = 0; j < clueItem._renderers.Length; j++)
				{
					clueItem._renderers[j].SetActive(true);
				}
				this._clues.Remove(clueItem);
				LocalPlayer.Tuts.ShowStoryClueTut();
				LocalPlayer.Sfx.PlayTurnPage();
			}
		}

		public void Init()
		{
			EventRegistry.Player.Subscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnInventoryAddedItem));
			Scene.ActiveMB.StartCoroutine(this.InitFromSave());
		}

		[DebuggerHidden]
		private IEnumerator InitFromSave()
		{
			StoryCluesFolder.<InitFromSave>c__Iterator18B <InitFromSave>c__Iterator18B = new StoryCluesFolder.<InitFromSave>c__Iterator18B();
			<InitFromSave>c__Iterator18B.<>f__this = this;
			return <InitFromSave>c__Iterator18B;
		}
	}
}
