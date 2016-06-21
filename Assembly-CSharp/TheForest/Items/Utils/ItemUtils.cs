using System;
using System.Reflection;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Utils
{
	public static class ItemUtils
	{
		public static void ApplyEffectsToStats(StatEffect effect, bool forward)
		{
			Type typeFromHandle = typeof(PlayerStats);
			PlayerStats stats = LocalPlayer.Stats;
			PlayerInventory inventory = LocalPlayer.Inventory;
			int num = (!forward) ? -1 : 1;
			switch (effect._type)
			{
			case StatEffect.Types.Stamina:
				stats.Stamina += effect._amount * (float)num;
				goto IL_412;
			case StatEffect.Types.Health:
				stats.Health += effect._amount * (float)num * LocalPlayer.Stats.FoodPoisoning.EffectRatio;
				goto IL_412;
			case StatEffect.Types.Energy:
				stats.Energy += effect._amount * (float)num * LocalPlayer.Stats.FoodPoisoning.EffectRatio;
				goto IL_412;
			case StatEffect.Types.Armor:
				stats.Armor += (int)effect._amount * num;
				goto IL_412;
			case StatEffect.Types.Fullness:
				stats.Fullness += effect._amount * (float)num * LocalPlayer.Stats.FoodPoisoning.EffectRatio;
				goto IL_412;
			case StatEffect.Types.BatteryCharge:
				stats.BatteryCharge = Mathf.Clamp(stats.BatteryCharge + effect._amount * (float)num, 0f, 100f);
				goto IL_412;
			case StatEffect.Types.VisibleLizardSkinArmor:
				stats.AddArmorVisible(PlayerStats.ArmorTypes.LizardSkin);
				inventory.PendingSendMessage = "CheckArmor";
				goto IL_412;
			case StatEffect.Types.Method_PoisonMe:
				if (effect._amount == 0f)
				{
					stats.PoisonMe();
				}
				else
				{
					stats.Invoke("PoisonMe", effect._amount);
				}
				goto IL_412;
			case StatEffect.Types.Method_HitFood:
				if (effect._amount == 0f)
				{
					stats.HitFood();
				}
				else
				{
					stats.Invoke("HitFood", effect._amount);
				}
				goto IL_412;
			case StatEffect.Types.VisibleDeerSkinArmor:
				stats.AddArmorVisible(PlayerStats.ArmorTypes.DeerSkin);
				inventory.PendingSendMessage = "CheckArmor";
				goto IL_412;
			case StatEffect.Types.VisibleStealthArmor:
				stats.AddArmorVisible(PlayerStats.ArmorTypes.Leaves);
				inventory.PendingSendMessage = "CheckArmor";
				goto IL_412;
			case StatEffect.Types.Stealth:
				LocalPlayer.Stats.Stealth += effect._amount * (float)num;
				goto IL_412;
			case StatEffect.Types.Thirst:
				stats.Thirst += effect._amount * (float)num * LocalPlayer.Stats.FoodPoisoning.EffectRatio;
				goto IL_412;
			case StatEffect.Types.AirRecharge:
				stats.AirBreathing.CurrentRebreatherAir = stats.AirBreathing.MaxRebreatherAirCapacity;
				goto IL_412;
			case StatEffect.Types.Method_UseRebreather:
				stats.UseRebreather(forward);
				goto IL_412;
			case StatEffect.Types.CureFoodPoisoning:
				LocalPlayer.Stats.FoodPoisoning.Cure();
				goto IL_412;
			case StatEffect.Types.CureBloodInfection:
				LocalPlayer.Stats.BloodInfection.Cure();
				goto IL_412;
			case StatEffect.Types.OvereatingPoints:
				stats.PhysicalStrength.OvereatingPointsChange(effect._amount * (float)num);
				goto IL_412;
			case StatEffect.Types.UndereatingPoints:
				stats.PhysicalStrength.UndereatingPointsChange(effect._amount * (float)num);
				goto IL_412;
			case StatEffect.Types.SnowFlotation:
				LocalPlayer.FpCharacter.snowFlotation = (num > 0);
				goto IL_412;
			case StatEffect.Types.SoundRangeDampFactor:
				if (LocalPlayer.Stats.SoundRangeDampFactor < 0.69f)
				{
					LocalPlayer.Stats.SoundRangeDampFactor = 0.7f;
				}
				LocalPlayer.Stats.SoundRangeDampFactor += effect._amount * (float)num;
				goto IL_412;
			case StatEffect.Types.VisibleBoneArmor:
				stats.AddArmorVisible(PlayerStats.ArmorTypes.Bone);
				inventory.PendingSendMessage = "CheckArmor";
				goto IL_412;
			}
			FieldInfo field = typeFromHandle.GetField(effect._type.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field.FieldType == typeof(float))
			{
				field.SetValue(stats, (float)field.GetValue(stats) + effect._amount * (float)num);
			}
			else if (field.FieldType == typeof(int))
			{
				field.SetValue(stats, (int)((float)((int)field.GetValue(stats)) + effect._amount * (float)num));
			}
			IL_412:
			LocalPlayer.Stats.CheckStats();
		}

		public static void ApplyEffectsToStats(StatEffect[] effects, bool forward)
		{
			if (effects.Length > 0)
			{
				for (int i = 0; i < effects.Length; i++)
				{
					ItemUtils.ApplyEffectsToStats(effects[i], forward);
				}
			}
		}
	}
}
