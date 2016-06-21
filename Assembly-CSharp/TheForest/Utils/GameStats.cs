using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TheForest.Buildings.Creation;
using TheForest.TaskSystem;
using TheForest.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Utils
{
	public class GameStats : MonoBehaviour
	{
		public class ItemEvent : UnityEvent<int>
		{
		}

		public class UpgradeEvent : UnityEvent<int>
		{
		}

		public class BuildEvent : UnityEvent<Create.BuildingTypes>
		{
		}

		public class StoryEvent : UnityEvent<GameStats.StoryElements>
		{
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Stats
		{
			public int _day;

			public int _treeCutDown;

			public int _enemiesKilled;

			public int _rabbitKilled;

			public int _lizardKilled;

			public int _raccoonKilled;

			public int _deerKilled;

			public int _turtleKilled;

			public int _birdKilled;

			public int _cookedFood;

			public int _burntFood;

			public int _cancelledStructures;

			public int _builtStructures;

			public int _destroyedStructures;

			public int _repairedStructures;

			public int _edibleItemsUsed;

			public int _itemsCrafted;

			public int _upgradesAdded;

			public int _arrowsFired;

			public int _litArrows;

			public int _litWeapons;

			public int _burntEnemies;

			public int _explodedEnemies;

			public int _openedSuitcases;

			public int _passengersFound;

			public GameStats.StoryElements _storyElements;

			public int _infections;

			public static GameStats.Stats LoadFromBytes(byte[] bytes)
			{
				GameStats.Stats result;
				try
				{
					IFormatter formatter = new BinaryFormatter();
					using (MemoryStream memoryStream = new MemoryStream(bytes))
					{
						result = (GameStats.Stats)formatter.Deserialize(memoryStream);
					}
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					result = default(GameStats.Stats);
				}
				return result;
			}
		}

		[Flags]
		public enum StoryElements
		{
			HangingScene = 1,
			RedManOnYacht = 32,
			FoundClimbWall = 256
		}

		public GameStats.Stats _stats;

		public static UnityEvent TreeCutDown = new UnityEvent();

		public static UnityEvent EnemyKilled = new UnityEvent();

		public static UnityEvent RabbitKilled = new UnityEvent();

		public static UnityEvent LizardKilled = new UnityEvent();

		public static UnityEvent RaccoonKilled = new UnityEvent();

		public static UnityEvent DeerKilled = new UnityEvent();

		public static UnityEvent TurtleKilled = new UnityEvent();

		public static UnityEvent BirdKilled = new UnityEvent();

		public static UnityEvent CookedFood = new UnityEvent();

		public static UnityEvent BurntFood = new UnityEvent();

		public static UnityEvent CancelledStructure = new UnityEvent();

		public static UnityEvent DestroyedStructure = new UnityEvent();

		public static UnityEvent RepairedStructure = new UnityEvent();

		public static GameStats.ItemEvent EdibleItemUsed = new GameStats.ItemEvent();

		public static GameStats.ItemEvent ItemCrafted = new GameStats.ItemEvent();

		public static GameStats.UpgradeEvent UpgradesAdded = new GameStats.UpgradeEvent();

		public static UnityEvent ArrowFired = new UnityEvent();

		public static UnityEvent LitArrow = new UnityEvent();

		public static UnityEvent LitWeapon = new UnityEvent();

		public static UnityEvent BurntEnemy = new UnityEvent();

		public static UnityEvent ExplodedEnemy = new UnityEvent();

		public static UnityEvent OpenedSuitcase = new UnityEvent();

		public static UnityEvent FoundPassenger = new UnityEvent();

		public static UnityEvent Infected = new UnityEvent();

		private void Awake()
		{
			GameStats.TreeCutDown.AddListener(delegate
			{
				this._stats._treeCutDown = this._stats._treeCutDown + 1;
			});
			GameStats.EnemyKilled.AddListener(delegate
			{
				this._stats._enemiesKilled = this._stats._enemiesKilled + 1;
			});
			GameStats.RabbitKilled.AddListener(delegate
			{
				this._stats._rabbitKilled = this._stats._rabbitKilled + 1;
			});
			GameStats.LizardKilled.AddListener(delegate
			{
				this._stats._lizardKilled = this._stats._lizardKilled + 1;
			});
			GameStats.RaccoonKilled.AddListener(delegate
			{
				this._stats._raccoonKilled = this._stats._raccoonKilled + 1;
			});
			GameStats.DeerKilled.AddListener(delegate
			{
				this._stats._deerKilled = this._stats._deerKilled + 1;
			});
			GameStats.TurtleKilled.AddListener(delegate
			{
				this._stats._turtleKilled = this._stats._turtleKilled + 1;
			});
			GameStats.BirdKilled.AddListener(delegate
			{
				this._stats._birdKilled = this._stats._birdKilled + 1;
			});
			GameStats.CookedFood.AddListener(delegate
			{
				this._stats._cookedFood = this._stats._cookedFood + 1;
			});
			GameStats.BurntFood.AddListener(delegate
			{
				this._stats._burntFood = this._stats._burntFood + 1;
			});
			GameStats.CancelledStructure.AddListener(delegate
			{
				this._stats._cancelledStructures = this._stats._cancelledStructures + 1;
			});
			EventRegistry.Player.Subscribe(typeof(BuildingCondition), new EventRegistry.SubscriberCallback(this.OnStructureBuilt));
			GameStats.DestroyedStructure.AddListener(delegate
			{
				this._stats._destroyedStructures = this._stats._destroyedStructures + 1;
			});
			GameStats.RepairedStructure.AddListener(delegate
			{
				this._stats._repairedStructures = this._stats._repairedStructures + 1;
			});
			GameStats.EdibleItemUsed.AddListener(delegate(int itemId)
			{
				this._stats._edibleItemsUsed = this._stats._edibleItemsUsed + 1;
			});
			GameStats.ItemCrafted.AddListener(delegate(int itemId)
			{
				this._stats._itemsCrafted = this._stats._itemsCrafted + 1;
			});
			GameStats.UpgradesAdded.AddListener(delegate(int amount)
			{
				this._stats._upgradesAdded = this._stats._upgradesAdded + amount;
			});
			GameStats.ArrowFired.AddListener(delegate
			{
				this._stats._arrowsFired = this._stats._arrowsFired + 1;
			});
			GameStats.LitArrow.AddListener(delegate
			{
				this._stats._litArrows = this._stats._litArrows + 1;
			});
			GameStats.LitWeapon.AddListener(delegate
			{
				this._stats._litWeapons = this._stats._litWeapons + 1;
			});
			GameStats.BurntEnemy.AddListener(delegate
			{
				this._stats._burntEnemies = this._stats._burntEnemies + 1;
			});
			GameStats.ExplodedEnemy.AddListener(delegate
			{
				this._stats._explodedEnemies = this._stats._explodedEnemies + 1;
			});
			GameStats.OpenedSuitcase.AddListener(delegate
			{
				this._stats._openedSuitcases = this._stats._openedSuitcases + 1;
			});
			GameStats.FoundPassenger.AddListener(delegate
			{
				this._stats._passengersFound = this._stats._passengersFound + 1;
			});
			EventRegistry.Player.Subscribe(typeof(StoryCondition), new EventRegistry.SubscriberCallback(this.OnStoryProgress));
			GameStats.Infected.AddListener(delegate
			{
				this._stats._infections = this._stats._infections + 1;
			});
		}

		private void OnSerializing()
		{
			this._stats._day = Clock.Day;
			string localSlotPath = SaveSlotUtils.GetLocalSlotPath();
			string path = localSlotPath + "info";
			string filename = SaveSlotUtils.GetCloudSlotPath() + "info";
			if (!Directory.Exists(localSlotPath))
			{
				Directory.CreateDirectory(localSlotPath);
			}
			IFormatter formatter = new BinaryFormatter();
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, this._stats);
				array = memoryStream.ToArray();
			}
			File.WriteAllBytes(path, array);
			CoopSteamCloud.CloudSave(filename, array);
		}

		private void OnStructureBuilt(object o)
		{
			this._stats._builtStructures = this._stats._builtStructures + 1;
		}

		private void OnStoryProgress(object o)
		{
			GameStats.StoryElements storyElements = (GameStats.StoryElements)((int)o);
			this._stats._storyElements = (this._stats._storyElements | storyElements);
		}
	}
}
