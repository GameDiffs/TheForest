using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.SerializableTaskSystem;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class SurvivalBookBestiary : MonoBehaviour
	{
		[DoNotSerializePublic]
		[Serializable]
		public class TodoEntryGOs
		{
			public GameObject _text;

			public GameObject _done;
		}

		[DoNotSerializePublic]
		[Serializable]
		public class TodoTask : Task
		{
			public string _availableMessage = "new item added to todo list";

			public string _doneMessage = "to do list updated";

			public SurvivalBookBestiary.TodoEntryGOs GOs
			{
				get;
				set;
			}

			public int DisplayedNum
			{
				get;
				set;
			}

			public virtual void Prepare(SurvivalBookBestiary.TodoEntryGOs gos, Action onStatusChange)
			{
				this.GOs = gos;
				if (this._allowInMp || !BoltNetwork.isRunning)
				{
					this.OnStatusChange = onStatusChange;
					this.Init();
				}
				this.DisplayedNum = -1;
			}

			public override void SetAvailable()
			{
				if (!this._available)
				{
					this.AvailableMessage();
					base.SetAvailable();
				}
			}

			public override void SetDone()
			{
				if (!this._done)
				{
					this.DoneMessage();
					base.SetDone();
				}
			}

			public void LogMessage(string message)
			{
				Scene.HudGui.ShowTodoListMessage(message);
			}

			public void AvailableMessage()
			{
				this.LogMessage(this._availableMessage);
				LocalPlayer.Sfx.PlayTaskAvailable();
			}

			public void DoneMessage()
			{
				this.LogMessage(this._doneMessage);
				LocalPlayer.Sfx.PlayTaskCompleted();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FoundEnemyInfo : SurvivalBookBestiary.TodoTask
		{
			[SerializeThis]
			public EnemyContactCondition _availableConditionStorage;

			public int CurrentUnlockLevel;

			private Action OnStatusChangeOuter;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override void Prepare(SurvivalBookBestiary.TodoEntryGOs gos, Action onStatusChange)
			{
				this.OnStatusChangeOuter = onStatusChange;
				base.Prepare(gos, new Action(this.OnStatusChangeInner));
			}

			private void OnStatusChangeInner()
			{
				if (this.OnStatusChangeOuter != null)
				{
					this.OnStatusChangeOuter();
				}
				this.CurrentUnlockLevel++;
			}
		}

		public SelectPageNumber _tab;

		public SurvivalBookBestiary.FoundEnemyInfo[] _foundEnemyInfos;

		public SurvivalBookBestiary.TodoEntryGOs[] _foundEnemyInfosGOs;

		public float _displayOffset = 1f;

		private bool _initialized;

		[HideInInspector]
		public int _lastConditionId;

		[SerializeThis]
		private int[] _doneConditions;

		private void Awake()
		{
			if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.New)
			{
				Scene.ActiveMB.StartCoroutine(this.DelayedAwake());
			}
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			SurvivalBookBestiary.<DelayedAwake>c__Iterator18C <DelayedAwake>c__Iterator18C = new SurvivalBookBestiary.<DelayedAwake>c__Iterator18C();
			<DelayedAwake>c__Iterator18C.<>f__this = this;
			return <DelayedAwake>c__Iterator18C;
		}

		public void OnEnable()
		{
			int displayedElemenntNum = 0;
			for (int i = 0; i < this._foundEnemyInfos.Length; i++)
			{
				displayedElemenntNum = this.ToggleDisplay(this._foundEnemyInfos[i], displayedElemenntNum);
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < this._foundEnemyInfos.Length; i++)
			{
				this._foundEnemyInfos[i].Clear();
			}
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			SurvivalBookBestiary.<OnDeserialized>c__Iterator18D <OnDeserialized>c__Iterator18D = new SurvivalBookBestiary.<OnDeserialized>c__Iterator18D();
			<OnDeserialized>c__Iterator18D.<>f__this = this;
			return <OnDeserialized>c__Iterator18D;
		}

		private void OnSerializing()
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < this._foundEnemyInfos.Length; i++)
			{
				this._foundEnemyInfos[i].SaveDone(hashSet);
			}
			this._doneConditions = new int[hashSet.Count];
			hashSet.CopyTo(this._doneConditions);
		}

		private int ToggleDisplay(SurvivalBookBestiary.TodoTask elem, int displayedElemenntNum)
		{
			if (elem.GOs != null && elem.GOs._text)
			{
				if (elem._available)
				{
					if (!elem.GOs._text.activeSelf)
					{
						elem.GOs._text.SetActive(true);
					}
					if (elem.GOs._done)
					{
						if (elem._done)
						{
							if (!elem.GOs._done.activeSelf)
							{
								elem.GOs._done.SetActive(true);
							}
						}
						else if (elem.GOs._done.activeSelf)
						{
							elem.GOs._done.SetActive(false);
						}
					}
					displayedElemenntNum++;
				}
				else if (elem.GOs._text.activeSelf)
				{
					elem.GOs._text.SetActive(false);
				}
			}
			return displayedElemenntNum;
		}

		private void OnStatusChange()
		{
			if (this._tab)
			{
				this._tab.Highlight(null);
			}
		}
	}
}
