using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.UI
{
	public class LoadSaveSlotInfo : MonoBehaviour
	{
		public UILabel _labelSlot;

		public UILabel _labelStat;

		public UILabel _labelDateTime;

		public TitleScreen.GameSetup.Slots _slot;

		[HideInInspector]
		public int _slotNum;

		private void OnEnable()
		{
			this._slotNum = (int)this._slot;
			this.LoadStats();
		}

		private void LoadStats()
		{
			string localSlotPath = SaveSlotUtils.GetLocalSlotPath(this._slot);
			string path = localSlotPath + "info";
			if (!File.Exists(localSlotPath + "__RESUME__"))
			{
				this._labelSlot.text = "Slot " + this._slotNum;
				if (this._labelStat)
				{
					this._labelStat.gameObject.SetActive(false);
				}
				if (this._labelDateTime)
				{
					this._labelDateTime.gameObject.SetActive(false);
				}
				if (Application.loadedLevelName.Equals("TitleScene"))
				{
					base.transform.parent.GetComponent<Collider>().enabled = false;
				}
			}
			else
			{
				base.transform.parent.GetComponent<Collider>().enabled = true;
				try
				{
					if (this._labelStat && File.Exists(path))
					{
						GameStats.Stats gameStats = GameStats.Stats.LoadFromBytes(File.ReadAllBytes(path));
						this._labelSlot.text = string.Concat(new object[]
						{
							"Slot ",
							this._slotNum,
							": day ",
							gameStats._day
						});
						FieldInfo[] array = (from f in gameStats.GetType().GetFields()
						where (int)f.GetValue(gameStats) > 0
						select f).ToArray<FieldInfo>();
						if (array != null && array.Length > 0)
						{
							int num = UnityEngine.Random.Range(0, array.Length);
							string name = array[num].Name;
							string text;
							switch (name)
							{
							case "_treeCutDown":
								text = "Trees Cut Down: ";
								goto IL_48A;
							case "_enemiesKilled":
								text = "Enemies Killed: ";
								goto IL_48A;
							case "_rabbitKilled":
								text = "Rabbits Killed: ";
								goto IL_48A;
							case "_lizardKilled":
								text = "Lizards Killed: ";
								goto IL_48A;
							case "_raccoonKilled":
								text = "Raccoons Killed: ";
								goto IL_48A;
							case "_deerKilled":
								text = "Deer Killed: ";
								goto IL_48A;
							case "_turtleKilled":
								text = "Turtles Killed: ";
								goto IL_48A;
							case "_birdKilled":
								text = "Birds Killed: ";
								goto IL_48A;
							case "_cookedFood":
								text = "Cooked Food: ";
								goto IL_48A;
							case "_burntFood":
								text = "Burnt Food: ";
								goto IL_48A;
							case "_cancelledStructures":
								text = "Cancelled Structures: ";
								goto IL_48A;
							case "_builtStructures":
								text = "Built Structures: ";
								goto IL_48A;
							case "_destroyedStructures":
								text = "Destroyed Structures: ";
								goto IL_48A;
							case "_repairedStructures":
								text = "Repaired Structures: ";
								goto IL_48A;
							case "_edibleItemsUsed":
								text = "Edible Items Used: ";
								goto IL_48A;
							case "_itemsCrafted":
								text = "Items Crafted: ";
								goto IL_48A;
							case "_upgradesAdded":
								text = "Upgrades Added: ";
								goto IL_48A;
							case "_arrowsFired":
								text = "Arrows Fired: ";
								goto IL_48A;
							case "_litArrows":
								text = "Lit Arrows: ";
								goto IL_48A;
							case "_litWeapons":
								text = "Lit Weapons: ";
								goto IL_48A;
							case "_burntEnemies":
								text = "Burnt Enemies: ";
								goto IL_48A;
							case "_explodedEnemies":
								text = "Exploded Enemies: ";
								goto IL_48A;
							}
							text = string.Empty;
							IL_48A:
							if (string.IsNullOrEmpty(text))
							{
								this._labelStat.gameObject.SetActive(false);
							}
							else
							{
								this._labelStat.gameObject.SetActive(true);
								this._labelStat.text = text + array[num].GetValue(gameStats);
							}
						}
						else
						{
							this._labelStat.gameObject.SetActive(false);
						}
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				if (this._labelDateTime)
				{
					this._labelDateTime.text = File.GetCreationTime(localSlotPath + "__RESUME__").ToString(CultureInfo.CurrentCulture.DateTimeFormat);
					this._labelDateTime.gameObject.SetActive(true);
				}
			}
		}
	}
}
