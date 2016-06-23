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
	public class SerializableSurvivalBookTodo : MonoBehaviour
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

			public SerializableSurvivalBookTodo.TodoEntryGOs GOs
			{
				get;
				set;
			}

			public int DisplayedNum
			{
				get;
				set;
			}

			public void Prepare(SerializableSurvivalBookTodo.TodoEntryGOs gos, Action onStatusChange)
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
		public class CampTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public BuildingCondition _completeConditionStorage;

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FoodTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public BuildingCondition _availableConditionStorage;

			[SerializeThis]
			public CookFoodCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class DefensesTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public EnemyContactCondition _availableConditionStorage;

			[SerializeThis]
			public BuildingCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class RedmanTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public StoryCondition _availableConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave1TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave2TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public StoryCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave3TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave4TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave5TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave6TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave7TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave8TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave9TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave10TodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public StarLocationListCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FindClimbingAxeTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public StoryCondition _availableConditionStorage;

			[SerializeThis]
			public ItemCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FindRebreatherTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public AirBreathingCondition _airBreathingCondition;

			[SerializeThis]
			public ListAnyCondition _availableConditionStorage;

			[SerializeThis]
			public ItemCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					if (this._availableConditionStorage == null)
					{
						this._availableConditionStorage = new ListAnyCondition
						{
							_conditions = new ACondition[]
							{
								this._airBreathingCondition,
								this._completeConditionStorage
							}
						};
					}
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class SinkHoleTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ProximityCondition _availableConditionStorage;

			[SerializeThis]
			public ProximityCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class PassengersTodoTask : SerializableSurvivalBookTodo.TodoTask
		{
			[SerializeThis]
			public ItemCondition _availableConditionStorage;

			[SerializeThis]
			public PassengersCondition _completeConditionStorage;

			public override ACondition _availableCondition
			{
				get
				{
					return this._availableConditionStorage;
				}
			}

			public override ACondition _completeCondition
			{
				get
				{
					return this._completeConditionStorage;
				}
			}
		}

		public SelectPageNumber _todoListTab;

		public SelectPageNumber _statsListTab;

		public SelectPageNumber _trapsPageTab;

		public SerializableSurvivalBookTodo.TodoTask _son;

		public SerializableSurvivalBookTodo.TodoEntryGOs _sonGOs;

		public SerializableSurvivalBookTodo.CampTodoTask _camp;

		public SerializableSurvivalBookTodo.TodoEntryGOs _campGOs;

		public SerializableSurvivalBookTodo.FoodTodoTask _food;

		public SerializableSurvivalBookTodo.TodoEntryGOs _foodGOs;

		public SerializableSurvivalBookTodo.DefensesTodoTask _defenses;

		public SerializableSurvivalBookTodo.TodoEntryGOs _defensesGOs;

		public SerializableSurvivalBookTodo.TodoTask _redman;

		public SerializableSurvivalBookTodo.TodoEntryGOs _redmanGOs;

		public SerializableSurvivalBookTodo.Cave1TodoTask _cave1;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave1GOs;

		public SerializableSurvivalBookTodo.Cave2TodoTask _cave2;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave2GOs;

		public SerializableSurvivalBookTodo.Cave3TodoTask _cave3;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave3GOs;

		public SerializableSurvivalBookTodo.Cave4TodoTask _cave4;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave4GOs;

		public SerializableSurvivalBookTodo.Cave5TodoTask _cave5;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave5GOs;

		public SerializableSurvivalBookTodo.Cave6TodoTask _cave6;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave6GOs;

		public SerializableSurvivalBookTodo.Cave7TodoTask _cave7;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave7GOs;

		public SerializableSurvivalBookTodo.Cave8TodoTask _cave8;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave8GOs;

		public SerializableSurvivalBookTodo.Cave9TodoTask _cave9;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave9GOs;

		public SerializableSurvivalBookTodo.Cave10TodoTask _cave10;

		public SerializableSurvivalBookTodo.TodoEntryGOs _cave10GOs;

		public SerializableSurvivalBookTodo.FindClimbingAxeTodoTask _findClimbingAxe;

		public SerializableSurvivalBookTodo.TodoEntryGOs _findClimbingAxeGOs;

		public SerializableSurvivalBookTodo.FindRebreatherTodoTask _findRebreather;

		public SerializableSurvivalBookTodo.TodoEntryGOs _findRebreatherGOs;

		public SerializableSurvivalBookTodo.SinkHoleTodoTask _sinkHole;

		public SerializableSurvivalBookTodo.TodoEntryGOs _sinkHoleGOs;

		public SerializableSurvivalBookTodo.PassengersTodoTask _passengers;

		public SerializableSurvivalBookTodo.TodoEntryGOs _passengersGOs;

		public float _displayOffset = 1f;

		private bool _initialized;

		[HideInInspector]
		public int _lastConditionId;

		[SerializeThis]
		private int[] _doneConditions;

		private void Awake()
		{
			base.enabled = false;
			base.StartCoroutine(this.DelayedAwake());
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			SerializableSurvivalBookTodo.<DelayedAwake>c__Iterator188 <DelayedAwake>c__Iterator = new SerializableSurvivalBookTodo.<DelayedAwake>c__Iterator188();
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		public void OnEnable()
		{
			int displayedElemenntNum = 0;
			displayedElemenntNum = this.ToggleDisplay(this._son, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._camp, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._food, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._defenses, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._redman, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._findClimbingAxe, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._findRebreather, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._sinkHole, displayedElemenntNum);
			this.ToggleDisplay(this._passengers, displayedElemenntNum);
			displayedElemenntNum = 0;
			displayedElemenntNum = this.ToggleDisplay(this._cave1, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave2, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave3, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave4, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave5, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave6, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave7, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave8, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave9, displayedElemenntNum);
			displayedElemenntNum = this.ToggleDisplay(this._cave10, displayedElemenntNum);
		}

		private void OnDestroy()
		{
			this._son.Clear();
			this._camp.Clear();
			this._food.Clear();
			this._defenses.Clear();
			this._redman.Clear();
			this._cave1.Clear();
			this._cave2.Clear();
			this._cave5.Clear();
			this._findClimbingAxe.Clear();
			this._findRebreather.Clear();
			this._sinkHole.Clear();
			this._passengers.Clear();
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			SerializableSurvivalBookTodo.<OnDeserialized>c__Iterator189 <OnDeserialized>c__Iterator = new SerializableSurvivalBookTodo.<OnDeserialized>c__Iterator189();
			<OnDeserialized>c__Iterator.<>f__this = this;
			return <OnDeserialized>c__Iterator;
		}

		private void OnSerializing()
		{
			HashSet<int> hashSet = new HashSet<int>();
			this._son.SaveDone(hashSet);
			this._camp.SaveDone(hashSet);
			this._food.SaveDone(hashSet);
			this._defenses.SaveDone(hashSet);
			this._redman.SaveDone(hashSet);
			this._cave1.SaveDone(hashSet);
			this._cave2.SaveDone(hashSet);
			this._cave3.SaveDone(hashSet);
			this._cave4.SaveDone(hashSet);
			this._cave5.SaveDone(hashSet);
			this._cave6.SaveDone(hashSet);
			this._cave7.SaveDone(hashSet);
			this._cave8.SaveDone(hashSet);
			this._cave9.SaveDone(hashSet);
			this._cave10.SaveDone(hashSet);
			this._findClimbingAxe.SaveDone(hashSet);
			this._findRebreather.SaveDone(hashSet);
			this._sinkHole.SaveDone(hashSet);
			this._passengers.SaveDone(hashSet);
			this._doneConditions = new int[hashSet.Count];
			hashSet.CopyTo(this._doneConditions);
		}

		private int ToggleDisplay(SerializableSurvivalBookTodo.TodoTask elem, int displayedElemenntNum)
		{
			if (elem._available)
			{
				if (elem.DisplayedNum != displayedElemenntNum)
				{
					elem.DisplayedNum = displayedElemenntNum;
					Vector3 localPosition = elem.GOs._text.transform.localPosition;
					localPosition.y = this._displayOffset * (float)displayedElemenntNum;
					elem.GOs._text.transform.localPosition = localPosition;
				}
				if (!elem.GOs._text.activeSelf)
				{
					elem.GOs._text.SetActive(true);
				}
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
				displayedElemenntNum++;
			}
			else if (elem.GOs._text.activeSelf)
			{
				elem.GOs._text.SetActive(false);
			}
			return displayedElemenntNum;
		}

		private void OnStatusChange()
		{
			this._todoListTab.Highlight(null);
		}

		private void OnDefenseTaskAvailable()
		{
			this._defenses.OnStatusChange = new Action(this.OnStatusChange);
			this.OnStatusChange();
			this._trapsPageTab.Highlight(null);
		}
	}
}
