using System;

namespace TheForest.Items
{
	[Serializable]
	public class StatEffect
	{
		public enum Types
		{
			Stamina,
			Health,
			Energy,
			Armor,
			Hunger,
			InfectionChance,
			BleedChance,
			Ate,
			ColdAmt,
			Fullness,
			Tired,
			EnergyEx,
			BatteryCharge,
			VisibleLizardSkinArmor,
			EnergyTemp,
			Method_PoisonMe,
			Method_HitFood,
			VisibleDeerSkinArmor,
			ColdArmor,
			VisibleStealthArmor,
			Stealth,
			Thirst,
			AirRecharge,
			Method_UseRebreather,
			Flammable,
			CureFoodPoisoning,
			CureBloodInfection,
			OvereatingPoints,
			UndereatingPoints,
			SnowFlotation,
			SoundRangeDampFactor,
			VisibleBoneArmor
		}

		public StatEffect.Types _type;

		public float _amount;
	}
}
