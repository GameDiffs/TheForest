using System;

namespace TheForest.Items.Craft
{
	[Serializable]
	public class WeaponStatUpgrade
	{
		public enum Types
		{
			weaponDamage,
			smashDamage,
			weaponSpeed,
			tiredSpeed,
			staminaDrain,
			soundDetectRange,
			weaponRange,
			BurningWeapon,
			StickyProjectile,
			WalkmanTrack,
			BurningAmmo,
			Paint_Green,
			Paint_Orange,
			DirtyWater,
			CleanWater,
			Cooked,
			ItemPart,
			BatteryCharge,
			FlareGunAmmo,
			SetWeaponAmmoBonus,
			blockStaminaDrain,
			AddMaxAmountBonus,
			SetMaxAmountBonus,
			PoisonnedAmmo,
			SetArrowMaxAmountBonus,
			BurningWeaponExtra,
			Incendiary
		}

		public const WeaponStatUpgrade.Types NoBonus = (WeaponStatUpgrade.Types)(-1);

		public const WeaponStatUpgrade.Types UnspecifiedBonus = (WeaponStatUpgrade.Types)(-2);

		public WeaponStatUpgrade.Types _type;

		public float _amount;
	}
}
