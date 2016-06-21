using System;
using UnityEngine;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class StarLocationListCondition : ListAllCondition
	{
		[Header("StarLocation List")]
		public StarLocationCondition[] _starConditions;

		public override ACondition[] Conditions
		{
			get
			{
				return this._starConditions;
			}
		}

		public override void Init()
		{
			base.Init();
		}
	}
}
