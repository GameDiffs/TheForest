using System;
using TheForest.Buildings.Creation;
using TheForest.Tools;
using UnityEngine;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
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
	}
}
