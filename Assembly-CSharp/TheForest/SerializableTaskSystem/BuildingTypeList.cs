using System;
using TheForest.Buildings.Creation;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class BuildingTypeList : ACondition
	{
		public Create.BuildingTypes[] _types;

		public override void Init()
		{
		}
	}
}
