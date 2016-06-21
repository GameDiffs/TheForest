using System;
using System.Collections.Generic;

namespace TheForest.Utils
{
	[Serializable]
	public class PlayerStatConditionList
	{
		public enum ValidationTypes
		{
			AnyTrue,
			AllTrue
		}

		public PlayerStatConditionList.ValidationTypes _validationType;

		[NameFromProperty("_stat")]
		public List<PlayerStatCondition> _conditions;

		public bool IsValid(PlayerStats playerStats)
		{
			if (this._conditions.Count > 0)
			{
				foreach (PlayerStatCondition current in this._conditions)
				{
					bool flag = current.IsValid(playerStats);
					PlayerStatConditionList.ValidationTypes validationType = this._validationType;
					if (validationType != PlayerStatConditionList.ValidationTypes.AnyTrue)
					{
						if (validationType == PlayerStatConditionList.ValidationTypes.AllTrue)
						{
							if (!flag)
							{
								bool result = false;
								return result;
							}
						}
					}
					else if (flag)
					{
						bool result = true;
						return result;
					}
				}
				return this._validationType == PlayerStatConditionList.ValidationTypes.AllTrue;
			}
			return true;
		}
	}
}
