using System;
using System.Collections.Generic;
using TheForest.Buildings.Creation;
using TheForest.Tools;
using UnityEngine;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class BuildingCondition : ACondition
	{
		public BuildingTypeList[] _buildings;

		public override void Init()
		{
			EventRegistry.Player.Subscribe(TfEvent.BuiltStructure, new EventRegistry.SubscriberCallback(this.OnStructureBuild));
		}

		public override void Clear()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.BuiltStructure, new EventRegistry.SubscriberCallback(this.OnStructureBuild));
			base.Clear();
		}

		public void OnStructureBuild(object o)
		{
			bool flag = true;
			if (this._buildings != null)
			{
				Create.BuildingTypes buildingTypes = (Create.BuildingTypes)((int)o);
				for (int i = 0; i < this._buildings.Length; i++)
				{
					BuildingTypeList buildingTypeList = this._buildings[i];
					if (!buildingTypeList._done)
					{
						for (int j = 0; j < buildingTypeList._types.Length; j++)
						{
							if (buildingTypeList._types[j] == buildingTypes)
							{
								buildingTypeList._done = true;
								break;
							}
						}
						if (!buildingTypeList._done)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.SetDone();
					this.Clear();
				}
			}
			else
			{
				Debug.LogError("Broken BuildingCondition, likely serializer didn't load it correctly");
			}
		}

		public override void SaveDone(ICollection<int> doneConditions)
		{
			if (this._done)
			{
				doneConditions.Add(this._id);
			}
			for (int i = 0; i < this._buildings.Length; i++)
			{
				this._buildings[i].SaveDone(doneConditions);
			}
		}

		public override void LoadDone(ICollection<int> doneConditions)
		{
			if (doneConditions.Contains(this._id))
			{
				this._done = true;
			}
			for (int i = 0; i < this._buildings.Length; i++)
			{
				this._buildings[i].LoadDone(doneConditions);
			}
		}
	}
}
