using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.TaskSystem;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class SurvivalBookTodo : MonoBehaviour
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

			public SurvivalBookTodo.TodoEntryGOs GOs
			{
				get;
				set;
			}

			public int DisplayedNum
			{
				get;
				set;
			}

			public void Prepare(SurvivalBookTodo.TodoEntryGOs gos, Action onStatusChange)
			{
				this.GOs = gos;
				this.FixSerializer();
				if (this._allowInMp || !BoltNetwork.isRunning)
				{
					this.OnStatusChange = onStatusChange;
					this.Init();
				}
				this.DisplayedNum = -1;
			}

			public virtual void FixSerializer()
			{
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
		public class CampTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				if (this._completeConditionStorage == null || this._completeConditionStorage._buildings == null)
				{
					this._completeConditionStorage = new BuildingCondition
					{
						_buildings = new BuildingTypeList[]
						{
							new BuildingTypeList
							{
								_types = new Create.BuildingTypes[]
								{
									Create.BuildingTypes.LeafShelter,
									Create.BuildingTypes.Shelter,
									Create.BuildingTypes.LogCabinMed,
									Create.BuildingTypes.LogCabin
								}
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FoodTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._buildings == null)
				{
					this._availableConditionStorage = new BuildingCondition
					{
						_buildings = new BuildingTypeList[]
						{
							new BuildingTypeList
							{
								_types = new Create.BuildingTypes[]
								{
									Create.BuildingTypes.Fire,
									Create.BuildingTypes.FireRockPit,
									Create.BuildingTypes.BonFire
								}
							}
						},
						_done = done
					};
				}
				if (this._completeConditionStorage == null)
				{
					this._completeConditionStorage = new CookFoodCondition();
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class DefensesTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				if (this._availableConditionStorage == null)
				{
					this._availableConditionStorage = new EnemyContactCondition();
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._buildings == null)
				{
					this._completeConditionStorage = new BuildingCondition
					{
						_buildings = new BuildingTypeList[]
						{
							new BuildingTypeList
							{
								_types = new Create.BuildingTypes[]
								{
									Create.BuildingTypes.TrapDeadfall,
									Create.BuildingTypes.TrapPole,
									Create.BuildingTypes.TrapRope,
									Create.BuildingTypes.TrapSimple,
									Create.BuildingTypes.TrapSpikedWall,
									Create.BuildingTypes.TrapTripWireExplosive,
									Create.BuildingTypes.TrapTripWireMolotov,
									Create.BuildingTypes.TrapLeafPile,
									Create.BuildingTypes.TrapSwingingRock
								}
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class RedmanTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._type == (GameStats.StoryElements)0)
				{
					this._availableConditionStorage = new StoryCondition
					{
						_type = GameStats.StoryElements.RedManOnYacht,
						_done = done
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave1TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave1Door",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 1
							},
							new StarLocationCondition
							{
								_starNumber = 2
							},
							new StarLocationCondition
							{
								_starNumber = 3
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave2TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._type == (GameStats.StoryElements)0)
				{
					this._availableConditionStorage = new StoryCondition
					{
						_type = GameStats.StoryElements.HangingScene,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 4
							},
							new StarLocationCondition
							{
								_starNumber = 5
							},
							new StarLocationCondition
							{
								_starNumber = 6
							},
							new StarLocationCondition
							{
								_starNumber = 7
							},
							new StarLocationCondition
							{
								_starNumber = 8
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave3TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave6Door3",
						_use2dDistance = true,
						_distance = 20f,
						_inCaveOnly = true,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 14
							},
							new StarLocationCondition
							{
								_starNumber = 15
							},
							new StarLocationCondition
							{
								_starNumber = 16
							},
							new StarLocationCondition
							{
								_starNumber = 17
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave4TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave4ClimbEntrance_Altexit",
						_use2dDistance = true,
						_distance = 10f,
						_inCaveOnly = true,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 18
							},
							new StarLocationCondition
							{
								_starNumber = 19
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave5TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave5Door",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 9
							},
							new StarLocationCondition
							{
								_starNumber = 10
							},
							new StarLocationCondition
							{
								_starNumber = 11
							},
							new StarLocationCondition
							{
								_starNumber = 12
							},
							new StarLocationCondition
							{
								_starNumber = 13
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave6TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave6Door",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 20
							},
							new StarLocationCondition
							{
								_starNumber = 21
							},
							new StarLocationCondition
							{
								_starNumber = 22
							},
							new StarLocationCondition
							{
								_starNumber = 23
							},
							new StarLocationCondition
							{
								_starNumber = 24
							},
							new StarLocationCondition
							{
								_starNumber = 25
							},
							new StarLocationCondition
							{
								_starNumber = 26
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave7TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave7Door1",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 27
							},
							new StarLocationCondition
							{
								_starNumber = 28
							},
							new StarLocationCondition
							{
								_starNumber = 29
							},
							new StarLocationCondition
							{
								_starNumber = 30
							},
							new StarLocationCondition
							{
								_starNumber = 31
							},
							new StarLocationCondition
							{
								_starNumber = 32
							},
							new StarLocationCondition
							{
								_starNumber = 33
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave8TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave7Door1",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 34
							},
							new StarLocationCondition
							{
								_starNumber = 35
							},
							new StarLocationCondition
							{
								_starNumber = 36
							},
							new StarLocationCondition
							{
								_starNumber = 37
							},
							new StarLocationCondition
							{
								_starNumber = 38
							},
							new StarLocationCondition
							{
								_starNumber = 39
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave9TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave9RopeLedgeEntrance",
						_use2dDistance = true,
						_distance = 10f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 40
							},
							new StarLocationCondition
							{
								_starNumber = 41
							},
							new StarLocationCondition
							{
								_starNumber = 42
							},
							new StarLocationCondition
							{
								_starNumber = 43
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class Cave10TodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "Cave10ClimbEntrance",
						_use2dDistance = true,
						_distance = 20f,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._starConditions == null || this._completeConditionStorage._starConditions.Length == 0 || this._completeConditionStorage._starConditions[0]._starNumber == 0)
				{
					this._completeConditionStorage = new StarLocationListCondition
					{
						_starConditions = new StarLocationCondition[]
						{
							new StarLocationCondition
							{
								_starNumber = 44
							},
							new StarLocationCondition
							{
								_starNumber = 45
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FindClimbingAxeTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._type == (GameStats.StoryElements)0)
				{
					this._availableConditionStorage = new StoryCondition
					{
						_type = GameStats.StoryElements.FoundClimbWall,
						_done = done
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._items == null)
				{
					this._completeConditionStorage = new ItemCondition
					{
						_items = new ItemIdList[]
						{
							new ItemIdList
							{
								_itemIds = new int[]
								{
									138
								}
							}
						}
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class FindRebreatherTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				if (this._airBreathingCondition == null || this._airBreathingCondition._threshold == 0f)
				{
					this._airBreathingCondition = new AirBreathingCondition
					{
						_threshold = 0.4f
					};
				}
				if (this._completeConditionStorage == null || this._completeConditionStorage._items == null || this._completeConditionStorage._items.Length == 0)
				{
					this._completeConditionStorage = new ItemCondition
					{
						_items = new ItemIdList[]
						{
							new ItemIdList
							{
								_itemIds = new int[]
								{
									143
								}
							}
						}
					};
				}
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._conditions == null || this._availableConditionStorage._conditions.Length == 0)
				{
					this._availableConditionStorage = new ListAnyCondition
					{
						_conditions = new ACondition[]
						{
							this._airBreathingCondition,
							this._completeConditionStorage
						},
						_done = done
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class SinkHoleTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				if (this._availableConditionStorage == null || string.IsNullOrEmpty(this._availableConditionStorage._objectTag))
				{
					this._availableConditionStorage = new ProximityCondition
					{
						_objectTag = "SinkHoleCenter",
						_isTag = true,
						_use2dDistance = true,
						_distance = 250f
					};
				}
				if (this._completeConditionStorage == null || string.IsNullOrEmpty(this._completeConditionStorage._objectTag))
				{
					this._completeConditionStorage = new ProximityCondition
					{
						_objectTag = "SinkHoleCenter",
						_isTag = true,
						_distance = 135f
					};
				}
				base.FixSerializer();
			}
		}

		[DoNotSerializePublic]
		[Serializable]
		public class PassengersTodoTask : SurvivalBookTodo.TodoTask
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

			public override void FixSerializer()
			{
				bool done = this._availableConditionStorage != null && this._availableConditionStorage._done;
				if (this._availableConditionStorage == null || this._availableConditionStorage._items == null)
				{
					this._availableConditionStorage = new ItemCondition
					{
						_items = new ItemIdList[]
						{
							new ItemIdList
							{
								_itemIds = new int[]
								{
									197
								}
							}
						},
						_done = done
					};
				}
				if (this._completeConditionStorage == null)
				{
					this._completeConditionStorage = new PassengersCondition();
				}
				base.FixSerializer();
			}
		}

		public SelectPageNumber _todoListTab;

		public SelectPageNumber _statsListTab;

		public SelectPageNumber _trapsPageTab;

		[SerializeThis]
		public SurvivalBookTodo.TodoTask _son;

		public SurvivalBookTodo.TodoEntryGOs _sonGOs;

		[SerializeThis]
		public SurvivalBookTodo.CampTodoTask _camp;

		public SurvivalBookTodo.TodoEntryGOs _campGOs;

		[SerializeThis]
		public SurvivalBookTodo.FoodTodoTask _food;

		public SurvivalBookTodo.TodoEntryGOs _foodGOs;

		[SerializeThis]
		public SurvivalBookTodo.DefensesTodoTask _defenses;

		public SurvivalBookTodo.TodoEntryGOs _defensesGOs;

		[SerializeThis]
		public SurvivalBookTodo.TodoTask _redman;

		public SurvivalBookTodo.TodoEntryGOs _redmanGOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave1TodoTask _cave1;

		public SurvivalBookTodo.TodoEntryGOs _cave1GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave2TodoTask _cave2;

		public SurvivalBookTodo.TodoEntryGOs _cave2GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave3TodoTask _cave3;

		public SurvivalBookTodo.TodoEntryGOs _cave3GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave4TodoTask _cave4;

		public SurvivalBookTodo.TodoEntryGOs _cave4GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave5TodoTask _cave5;

		public SurvivalBookTodo.TodoEntryGOs _cave5GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave6TodoTask _cave6;

		public SurvivalBookTodo.TodoEntryGOs _cave6GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave7TodoTask _cave7;

		public SurvivalBookTodo.TodoEntryGOs _cave7GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave8TodoTask _cave8;

		public SurvivalBookTodo.TodoEntryGOs _cave8GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave9TodoTask _cave9;

		public SurvivalBookTodo.TodoEntryGOs _cave9GOs;

		[SerializeThis]
		public SurvivalBookTodo.Cave10TodoTask _cave10;

		public SurvivalBookTodo.TodoEntryGOs _cave10GOs;

		[SerializeThis]
		public SurvivalBookTodo.FindClimbingAxeTodoTask _findClimbingAxe;

		public SurvivalBookTodo.TodoEntryGOs _findClimbingAxeGOs;

		[SerializeThis]
		public SurvivalBookTodo.FindRebreatherTodoTask _findRebreather;

		public SurvivalBookTodo.TodoEntryGOs _findRebreatherGOs;

		[SerializeThis]
		public SurvivalBookTodo.SinkHoleTodoTask _sinkHole;

		public SurvivalBookTodo.TodoEntryGOs _sinkHoleGOs;

		[SerializeThis]
		public SurvivalBookTodo.PassengersTodoTask _passengers;

		public SurvivalBookTodo.TodoEntryGOs _passengersGOs;

		public float _displayOffset = 1f;

		private bool _initialized;

		private void Awake()
		{
			base.enabled = false;
			if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.New)
			{
				base.StartCoroutine(this.DelayedAwake());
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			SurvivalBookTodo.<DelayedAwake>c__Iterator18F <DelayedAwake>c__Iterator18F = new SurvivalBookTodo.<DelayedAwake>c__Iterator18F();
			<DelayedAwake>c__Iterator18F.<>f__this = this;
			return <DelayedAwake>c__Iterator18F;
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
			SurvivalBookTodo.<OnDeserialized>c__Iterator190 <OnDeserialized>c__Iterator = new SurvivalBookTodo.<OnDeserialized>c__Iterator190();
			<OnDeserialized>c__Iterator.<>f__this = this;
			return <OnDeserialized>c__Iterator;
		}

		private int ToggleDisplay(SurvivalBookTodo.TodoTask elem, int displayedElemenntNum)
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
