using System;
using UnityEngine;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class StarLocationListCondition : ListAllCondition
	{
		[SerializeThis, Header("StarLocation List")]
		public StarLocationCondition[] _starConditions;

		public override void Init()
		{
			this._conditions = this._starConditions;
			base.Init();
		}
	}
}
