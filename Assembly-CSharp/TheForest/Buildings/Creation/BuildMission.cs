using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class BuildMission : EntityBehaviour<IBuildMissionState>
	{
		private int _itemId;

		private int _amountNeeded;

		private BuildLog _buildLog;

		public static Dictionary<int, BuildMission> ActiveMissions = new Dictionary<int, BuildMission>();

		public int ItemId
		{
			get
			{
				return this._itemId;
			}
			set
			{
				if (BoltNetwork.isServer)
				{
					base.state.ItemId = value;
				}
				this._itemId = value;
				this.Init(value);
			}
		}

		public int AmountNeeded
		{
			get
			{
				return this._amountNeeded;
			}
			set
			{
				this._amountNeeded = value;
				if (BoltNetwork.isServer)
				{
					base.state.AmountNeeded = value;
				}
				if (this._buildLog)
				{
					if (this._amountNeeded > 0)
					{
						this._buildLog._needed = this._amountNeeded;
						if (!this._buildLog.transform.parent.gameObject.activeSelf)
						{
							this._buildLog.transform.parent.gameObject.SetActive(true);
							Scene.HudGui.Grid.Reposition();
						}
					}
					else
					{
						this._buildLog.transform.parent.gameObject.SetActive(false);
						Scene.HudGui.Grid.Reposition();
					}
				}
				else
				{
					UnityEngine.Debug.LogError(string.Concat(new object[]
					{
						"No BuildLog for BuildMission itemId=",
						this._itemId,
						" (",
						ItemDatabase.ItemById(this._itemId),
						")"
					}));
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			BuildMission.<Start>c__Iterator12C <Start>c__Iterator12C = new BuildMission.<Start>c__Iterator12C();
			<Start>c__Iterator12C.<>f__this = this;
			return <Start>c__Iterator12C;
		}

		private void Init(int itemId)
		{
			this._itemId = itemId;
			this._buildLog = Scene.HudGui.GetBuildMissionLogForItem(itemId);
			if (this._buildLog)
			{
				this._buildLog._needed = this._amountNeeded;
				this._buildLog.transform.parent.gameObject.SetActive(true);
				Scene.HudGui.Grid.Reposition();
				BuildMission.ActiveMissions[itemId] = this;
				if (BoltNetwork.isClient)
				{
					this.AmountNeeded = this._amountNeeded;
				}
			}
			else
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"BuildLog not found for itemId=",
					itemId,
					" (",
					ItemDatabase.ItemById(itemId)._name,
					")"
				}));
			}
		}

		public static void AddNeededToBuildMission(int itemId, int amount)
		{
			if (!BoltNetwork.isClient)
			{
				BuildMission buildMission;
				if (!BuildMission.ActiveMissions.TryGetValue(itemId, out buildMission))
				{
					if (BoltNetwork.isRunning)
					{
						if (!BoltNetwork.isServer)
						{
							return;
						}
						buildMission = BoltNetwork.Instantiate(Prefabs.Instance.BuildMissionPrefab.gameObject).GetComponent<BuildMission>();
					}
					else
					{
						buildMission = UnityEngine.Object.Instantiate<BuildMission>(Prefabs.Instance.BuildMissionPrefab);
					}
					if (!buildMission)
					{
						return;
					}
					buildMission.ItemId = itemId;
					buildMission.AmountNeeded = amount;
				}
				else
				{
					buildMission.AmountNeeded = Mathf.Max(buildMission._amountNeeded + amount, 0);
				}
			}
		}

		public override void Attached()
		{
			if (!this.entity.isOwner)
			{
				base.state.AddCallback("ItemId", delegate
				{
					this.ItemId = base.state.ItemId;
				});
				base.state.AddCallback("AmountNeeded", delegate
				{
					this.AmountNeeded = base.state.AmountNeeded;
				});
			}
		}
	}
}
