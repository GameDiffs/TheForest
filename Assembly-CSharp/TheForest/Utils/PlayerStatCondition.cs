using System;
using UnityEngine;

namespace TheForest.Utils
{
	[Serializable]
	public class PlayerStatCondition
	{
		public enum Operators
		{
			Superior,
			SuperiorOrEqual,
			Equal,
			Inferior,
			InferiorOrEqual,
			Different
		}

		public enum Stats
		{
			Stamina,
			Health,
			Energy,
			Armor,
			Fullness,
			BatteryCharge
		}

		public PlayerStatCondition.Stats _stat;

		public PlayerStatCondition.Operators _op;

		public float _value;

		public bool IsValid(PlayerStats playerStats)
		{
			float num;
			switch (this._stat)
			{
			case PlayerStatCondition.Stats.Stamina:
				num = playerStats.Stamina;
				break;
			case PlayerStatCondition.Stats.Health:
				num = playerStats.Health;
				break;
			case PlayerStatCondition.Stats.Energy:
				num = playerStats.Energy;
				break;
			case PlayerStatCondition.Stats.Armor:
				num = (float)playerStats.Armor;
				break;
			case PlayerStatCondition.Stats.Fullness:
				num = playerStats.Fullness;
				break;
			case PlayerStatCondition.Stats.BatteryCharge:
				num = playerStats.BatteryCharge;
				break;
			default:
				return false;
			}
			switch (this._op)
			{
			case PlayerStatCondition.Operators.Superior:
				return num > this._value;
			case PlayerStatCondition.Operators.SuperiorOrEqual:
				return num >= this._value;
			case PlayerStatCondition.Operators.Equal:
				return Mathf.Approximately(num, this._value);
			case PlayerStatCondition.Operators.Inferior:
				return num <= this._value;
			case PlayerStatCondition.Operators.InferiorOrEqual:
				return num < this._value;
			case PlayerStatCondition.Operators.Different:
				return !Mathf.Approximately(num, this._value);
			default:
				return false;
			}
		}
	}
}
