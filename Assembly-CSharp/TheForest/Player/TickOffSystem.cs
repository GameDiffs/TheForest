using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Tools;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class TickOffSystem : MonoBehaviour
	{
		[Serializable]
		public class Entry
		{
			public TickOffSystem.EntryType _type;

			public int _id;

			[ItemIdPicker]
			public int _itemId;

			public AnimalType _animalType;

			public GameObject _tickGo;

			public bool _ticked;

			public void Init()
			{
				TickOffSystem.EntryType type = this._type;
				if (type != TickOffSystem.EntryType.CollectItem)
				{
					if (type == TickOffSystem.EntryType.InspectAnimal)
					{
						EventRegistry.Player.Subscribe(TfEvent.InspectedAnimal, new EventRegistry.SubscriberCallback(this.OnInspectedAnimal));
					}
				}
				else
				{
					EventRegistry.Player.Subscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
					EventRegistry.Player.Subscribe(TfEvent.UsedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
				}
			}

			public void Clear()
			{
				TickOffSystem.EntryType type = this._type;
				if (type != TickOffSystem.EntryType.CollectItem)
				{
					if (type == TickOffSystem.EntryType.InspectAnimal)
					{
						EventRegistry.Player.Unsubscribe(TfEvent.InspectedAnimal, new EventRegistry.SubscriberCallback(this.OnInspectedAnimal));
					}
				}
				else
				{
					EventRegistry.Player.Unsubscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
					EventRegistry.Player.Unsubscribe(TfEvent.UsedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
				}
				this._tickGo = null;
			}

			private void OnCollectedItem(object o)
			{
				int num = (int)o;
				if (num == this._itemId)
				{
					this._ticked = true;
					if (this._tickGo)
					{
						this._tickGo.SetActive(true);
					}
					else
					{
						UnityEngine.Debug.LogError(string.Concat(new object[]
						{
							"Missing tick go for TickOff entry: ",
							this._id,
							" (",
							this._type,
							")"
						}));
					}
					this.Clear();
					EventRegistry.Player.Publish(TfEvent.TickedOffEntry, this);
				}
			}

			private void OnInspectedAnimal(object o)
			{
				AnimalType animalType = (AnimalType)((int)o);
				if (animalType == this._animalType)
				{
					this._ticked = true;
					this._tickGo.SetActive(true);
					this.Clear();
					EventRegistry.Player.Publish(TfEvent.TickedOffEntry, this);
				}
			}
		}

		public enum EntryType
		{
			CollectItem,
			InspectAnimal
		}

		public SelectPageNumber _tickOffTab;

		public string _tickedEntryMessage;

		public TickOffSystem.Entry[] _entries;

		[SerializeThis]
		private int[] _tickedEntries;

		private bool _initialized;

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				base.StartCoroutine(this.DelayedAwake());
			}
		}

		private void OnDestroy()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.TickedOffEntry, new EventRegistry.SubscriberCallback(this.DoneMessage));
			TickOffSystem.Entry[] entries = this._entries;
			for (int i = 0; i < entries.Length; i++)
			{
				TickOffSystem.Entry entry = entries[i];
				entry.Clear();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			TickOffSystem.<DelayedAwake>c__Iterator192 <DelayedAwake>c__Iterator = new TickOffSystem.<DelayedAwake>c__Iterator192();
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			TickOffSystem.<OnDeserialized>c__Iterator193 <OnDeserialized>c__Iterator = new TickOffSystem.<OnDeserialized>c__Iterator193();
			<OnDeserialized>c__Iterator.<>f__this = this;
			return <OnDeserialized>c__Iterator;
		}

		private void OnSerializing()
		{
			this._tickedEntries = (from e in this._entries
			where e._ticked
			select e._id).ToArray<int>();
		}

		public void LogMessage(string message)
		{
			Scene.HudGui.ShowTodoListMessage(message);
		}

		public void DoneMessage(object o)
		{
			this.LogMessage(this._tickedEntryMessage);
			if (this._tickOffTab)
			{
				this._tickOffTab.Highlight(null);
			}
			LocalPlayer.Sfx.PlayTaskCompleted();
		}
	}
}
