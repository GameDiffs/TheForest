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
					EventRegistry.Player.Unsubscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
					EventRegistry.Player.Unsubscribe(TfEvent.UsedItem, new EventRegistry.SubscriberCallback(this.OnCollectedItem));
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
					EventRegistry.Player.Unsubscribe(TfEvent.InspectedAnimal, new EventRegistry.SubscriberCallback(this.OnInspectedAnimal));
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
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			TickOffSystem.<DelayedAwake>c__Iterator18A <DelayedAwake>c__Iterator18A = new TickOffSystem.<DelayedAwake>c__Iterator18A();
			<DelayedAwake>c__Iterator18A.<>f__this = this;
			return <DelayedAwake>c__Iterator18A;
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			TickOffSystem.<OnDeserialized>c__Iterator18B <OnDeserialized>c__Iterator18B = new TickOffSystem.<OnDeserialized>c__Iterator18B();
			<OnDeserialized>c__Iterator18B.<>f__this = this;
			return <OnDeserialized>c__Iterator18B;
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
