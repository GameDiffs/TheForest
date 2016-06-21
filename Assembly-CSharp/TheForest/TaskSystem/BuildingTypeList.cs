using System;
using TheForest.Buildings.Creation;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class BuildingTypeList
	{
		public Create.BuildingTypes[] _types;

		[SerializeThis]
		public bool _done;
	}
}
