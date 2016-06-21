using Bolt;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI.Multiplayer
{
	public class ChatBox : MonoBehaviour
	{
		public class Player
		{
			public string _name;

			public Color _color;
		}

		public UICamera _eventHandler;

		public UITable _grid;

		public UIInput _input;

		public ChatMessageRow _messageRowPrefab;

		public float _visibleDuration = 10f;

		public int _historySize = 10;

		private Dictionary<NetworkId, ChatBox.Player> _players;

		private Queue<ChatMessageRow> _messageRows;

		private float _lastInteractionTime;

		private int _lastMessageId;

		private bool _mustOpen;

		private bool _mustClose;

		private bool _skipNextOpen;

		private NetworkId _localPlayerId;

		public static ChatBox Instance
		{
			get;
			set;
		}

		public static bool IsChatOpen
		{
			get;
			private set;
		}

		public Dictionary<NetworkId, ChatBox.Player> Players
		{
			get
			{
				return this._players;
			}
		}

		private void Awake()
		{
			if (BoltNetwork.isRunning)
			{
				this._players = new Dictionary<NetworkId, ChatBox.Player>(NetworkId.EqualityComparer.Instance);
				this._messageRows = new Queue<ChatMessageRow>();
				this._input.value = null;
				this._input.gameObject.SetActive(false);
				this._messageRowPrefab.gameObject.SetActive(false);
				this._eventHandler.enabled = false;
				ChatBox.Instance = this;
			}
			else
			{
				UnityEngine.Object.Destroy(this._eventHandler);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			ChatBox.Instance = null;
		}

		private void SendLine(string line)
		{
			if (BoltNetwork.isRunning)
			{
				ChatEvent chatEvent = ChatEvent.Raise(GlobalTargets.OnlyServer);
				chatEvent.Message = line;
				chatEvent.Sender = LocalPlayer.Entity.networkId;
				chatEvent.Send();
			}
		}

		private void Update()
		{
			if (this._lastInteractionTime + this._visibleDuration < Time.time && !this._input.gameObject.activeSelf)
			{
				this._grid.gameObject.SetActive(false);
			}
			if (!this._input.gameObject.activeSelf && !this._mustClose && TheForest.Utils.Input.GetButtonDown("OpenChat"))
			{
				if (this._skipNextOpen)
				{
					this._skipNextOpen = false;
				}
				else
				{
					this._eventHandler.enabled = true;
					this._mustOpen = true;
				}
			}
			else if (TheForest.Utils.Input.GetButtonDown("CloseChat"))
			{
				this._mustClose = true;
			}
			else if (this._mustOpen)
			{
				ChatBox.IsChatOpen = true;
				TheForest.Utils.Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, "Chat");
				TheForest.Utils.Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Default");
				this._mustOpen = false;
				this._grid.gameObject.SetActive(true);
				this._input.gameObject.SetActive(true);
				this._input.isSelected = true;
				LocalPlayer.Inventory.enabled = false;
				if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
				{
					LocalPlayer.FpCharacter.Locked = true;
				}
			}
			else if (this._mustClose || (this._input.gameObject.activeSelf && !this._input.isSelected))
			{
				this._mustClose = false;
				base.StartCoroutine(this.Close());
				if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
				{
					LocalPlayer.FpCharacter.Locked = false;
				}
			}
		}

		[DebuggerHidden]
		public IEnumerator Close()
		{
			ChatBox.<Close>c__Iterator1B3 <Close>c__Iterator1B = new ChatBox.<Close>c__Iterator1B3();
			<Close>c__Iterator1B.<>f__this = this;
			return <Close>c__Iterator1B;
		}

		public void RegisterPlayer(string name, NetworkId id)
		{
			Color color = new Color(UnityEngine.Random.Range(0.25f, 1f), UnityEngine.Random.Range(0.25f, 1f), UnityEngine.Random.Range(0.25f, 1f), 1f);
			color.r += 0.1f;
			color.g += 0.1f;
			color.b += 0.1f;
			this._players[id] = new ChatBox.Player
			{
				_name = name,
				_color = color
			};
		}

		public void UnregisterPlayer(NetworkId id)
		{
			this._players.Remove(id);
		}

		public void AddLine(NetworkId playerId, string message)
		{
			if (this._players.ContainsKey(playerId))
			{
				ChatMessageRow chatMessageRow = UnityEngine.Object.Instantiate<ChatMessageRow>(this._messageRowPrefab);
				chatMessageRow._name.text = this._players[playerId]._name + " : ";
				chatMessageRow._name.color = this._players[playerId]._color;
				chatMessageRow._message.text = NGUIText.StripSymbols(message);
				chatMessageRow.name = this._lastMessageId++ + chatMessageRow.name;
				chatMessageRow.gameObject.SetActive(true);
				chatMessageRow.transform.parent = this._grid.transform;
				chatMessageRow.transform.localPosition = Vector3.zero;
				chatMessageRow.transform.localScale = Vector3.one;
				this._messageRows.Enqueue(chatMessageRow);
				if (this._messageRows.Count > this._historySize)
				{
					UnityEngine.Object.Destroy(this._messageRows.Dequeue().gameObject);
				}
				this._grid.repositionNow = true;
				this._grid.gameObject.SetActive(true);
				this._lastInteractionTime = Time.time;
			}
		}

		public void OnSubmit()
		{
			if (!string.IsNullOrEmpty(this._input.value))
			{
				this.SendLine(this._input.value);
				this._input.value = null;
			}
			this._mustClose = true;
			this._lastInteractionTime = Time.time;
		}
	}
}
