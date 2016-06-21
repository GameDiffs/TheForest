using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class SurvivalBookStats : MonoBehaviour
	{
		[DoNotSerializePublic]
		[Serializable]
		public class DisplayElement
		{
			public enum FeelingCalculationMethods
			{
				One,
				Inverse,
				Over100,
				Over100Inverse
			}

			public GameObject _go;

			public TextMesh _textMesh;

			public bool _feelingCheck;

			public SurvivalBookStats.DisplayElement.FeelingCalculationMethods _feelingCalculationMethod;

			public string _feelingName;

			public string Format
			{
				get;
				set;
			}

			public string StrVal
			{
				get;
				set;
			}

			public int IntVal
			{
				get;
				set;
			}

			public float FloatVal
			{
				get;
				set;
			}

			public float FloatVal2
			{
				get;
				set;
			}

			public void Init()
			{
				if (this._textMesh)
				{
					this.Format = this._textMesh.text;
				}
				this.StrVal = string.Empty;
				this.IntVal = -1;
				this.FloatVal = -1f;
				this.FloatVal2 = -1f;
			}
		}

		public SelectPageNumber _todoListTab;

		public SurvivalBookStats.DisplayElement _days;

		public SurvivalBookStats.DisplayElement _foodPoisoning;

		public SurvivalBookStats.DisplayElement _bloodInfection;

		public SurvivalBookStats.DisplayElement _enemies;

		public SurvivalBookStats.DisplayElement _weight;

		public SurvivalBookStats.DisplayElement _weightUp;

		public SurvivalBookStats.DisplayElement _weightStable;

		public SurvivalBookStats.DisplayElement _weightDown;

		public SurvivalBookStats.DisplayElement _sanity;

		public SurvivalBookStats.DisplayElement _athleticism;

		public SurvivalBookStats.DisplayElement _athleticismMalus;

		public SurvivalBookStats.DisplayElement _strength;

		public SurvivalBookStats.DisplayElement _strengthUp;

		public SurvivalBookStats.DisplayElement _strengthDown;

		public SurvivalBookStats.DisplayElement _strengthMalus;

		public SurvivalBookStats.DisplayElement _hunger;

		public SurvivalBookStats.DisplayElement _thirst;

		public SurvivalBookStats.DisplayElement _tiredness;

		public SurvivalBookStats.DisplayElement _health;

		public SurvivalBookStats.DisplayElement _cold;

		public SurvivalBookStats.DisplayElement _armor;

		public SurvivalBookStats.DisplayElement _coldarmor;

		public SurvivalBookStats.DisplayElement _feeling;

		public string _defaultFeeling = "good";

		public string _listText = "Days: {days}\nEnemies Down: {enemies}\nStructures\nCaves explored";

		private SurvivalBookStats.DisplayElement _currentFeeling;

		private float _currentFeelingAlpha;

		private void Awake()
		{
			this._days.Init();
			this._foodPoisoning.Init();
			this._bloodInfection.Init();
			this._weight.Init();
			this._weightUp.Init();
			this._weightStable.Init();
			this._weightDown.Init();
			this._sanity.Init();
			this._athleticism.Init();
			this._athleticismMalus.Init();
			this._strength.Init();
			this._strengthUp.Init();
			this._strengthDown.Init();
			this._strengthMalus.Init();
			this._hunger.Init();
			this._thirst.Init();
			this._tiredness.Init();
			this._health.Init();
			this._cold.Init();
			this._armor.Init();
			this._coldarmor.Init();
			this._feeling.Init();
			if (!LevelSerializer.IsDeserializing)
			{
				this._todoListTab.Highlight(null);
			}
		}

		private void OnEnable()
		{
			this._currentFeelingAlpha = 3.40282347E+38f;
			this.UpdateDisplay(this._days, Mathf.FloorToInt(LocalPlayer.Stats.DaySurvived));
			this.UpdateDisplay(this._foodPoisoning, LocalPlayer.Stats.FoodPoisoning.Infected);
			this.UpdateDisplay(this._bloodInfection, LocalPlayer.Stats.BloodInfection.Infected);
			this.UpdateDisplay(this._weight, Mathf.FloorToInt(LocalPlayer.Stats.PhysicalStrength.CurrentWeightScaled));
			int weightTrend = LocalPlayer.Stats.PhysicalStrength.GetWeightTrend();
			this.UpdateDisplay(this._weightUp, weightTrend > 0);
			this.UpdateDisplay(this._weightStable, weightTrend == 0);
			this.UpdateDisplay(this._weightDown, weightTrend < 0);
			this.UpdateDisplay(this._sanity, Mathf.FloorToInt(LocalPlayer.Stats.Sanity.CurrentSanity));
			this.UpdateDisplay(this._athleticism, Mathf.FloorToInt((float)LocalPlayer.Stats.Skills.AthleticismSkillLevel));
			this.UpdateDisplay(this._athleticismMalus, LocalPlayer.Stats.BloodInfection.Infected);
			this.UpdateDisplay(this._strength, LocalPlayer.Stats.PhysicalStrength.CurrentStrengthScaled);
			int strengthTrend = LocalPlayer.Stats.PhysicalStrength.GetStrengthTrend();
			this.UpdateDisplay(this._strengthUp, strengthTrend > 0);
			this.UpdateDisplay(this._strengthDown, strengthTrend < 0);
			this.UpdateDisplay(this._strengthMalus, LocalPlayer.Stats.FoodPoisoning.Infected);
			this.UpdateDisplay(this._hunger, 1f - LocalPlayer.Stats.Fullness);
			this.UpdateDisplay(this._thirst, LocalPlayer.Stats.Thirst);
			this.UpdateDisplay(this._tiredness, 1f - LocalPlayer.Stats.Energy / 100f);
			this.UpdateDisplay(this._health, LocalPlayer.Stats.Health / 100f);
			this.UpdateDisplay(this._cold, LocalPlayer.Stats.FrostScript.coverage * 2f);
			this.UpdateDisplay(this._armor, (float)LocalPlayer.Stats.Armor / 200f);
			this.UpdateDisplay(this._coldarmor, LocalPlayer.Stats.ColdArmor);
			this.UpdateDisplay(this._feeling, this.GetCurrentFeeling());
		}

		private void UpdateDisplay(SurvivalBookStats.DisplayElement elem, string value)
		{
			if (elem.StrVal != value)
			{
				elem.StrVal = value;
				if (elem._textMesh)
				{
					elem._textMesh.text = string.Format(elem.Format, elem.StrVal);
				}
			}
		}

		private void UpdateDisplay(SurvivalBookStats.DisplayElement elem, int value)
		{
			if (elem.IntVal != value)
			{
				elem.IntVal = value;
				if (elem._textMesh)
				{
					elem._textMesh.text = string.Format(elem.Format, elem.IntVal);
				}
			}
			this.FeelingCheck<int>(elem, value);
		}

		private void UpdateDisplay(SurvivalBookStats.DisplayElement elem, float value)
		{
			if (elem.FloatVal != value)
			{
				elem.FloatVal = value;
				if (elem._textMesh)
				{
					elem._textMesh.text = string.Format(elem.Format, elem.FloatVal);
				}
			}
			this.FeelingCheck<float>(elem, value);
		}

		private void UpdateDisplay(SurvivalBookStats.DisplayElement elem, bool value)
		{
			if (elem._textMesh && elem._textMesh.gameObject.activeSelf != value)
			{
				elem._textMesh.gameObject.SetActive(value);
			}
			if (elem._go && elem._go.activeSelf != value)
			{
				elem._go.SetActive(value);
			}
		}

		private void UpdateDisplay(SurvivalBookStats.DisplayElement elem, float value, float value2)
		{
			if (elem.FloatVal != value || elem.FloatVal2 != value2)
			{
				elem.FloatVal = value;
				elem.FloatVal2 = value2;
				if (elem._textMesh)
				{
					elem._textMesh.text = string.Format(elem.Format, elem.FloatVal, elem.FloatVal2);
				}
			}
			this.FeelingCheck<float>(elem, value);
		}

		private void FeelingCheck<T>(SurvivalBookStats.DisplayElement elem, T value)
		{
			if (elem._feelingCheck)
			{
				float num = 3.40282347E+38f;
				float num2 = Convert.ToSingle(value);
				switch (elem._feelingCalculationMethod)
				{
				case SurvivalBookStats.DisplayElement.FeelingCalculationMethods.One:
					num = num2;
					break;
				case SurvivalBookStats.DisplayElement.FeelingCalculationMethods.Inverse:
					num = 1f - num2;
					break;
				case SurvivalBookStats.DisplayElement.FeelingCalculationMethods.Over100:
					num = num2 / 100f;
					break;
				case SurvivalBookStats.DisplayElement.FeelingCalculationMethods.Over100Inverse:
					num = 1f - num2 / 100f;
					break;
				}
				if (num < 0.5f && num < this._currentFeelingAlpha)
				{
					this._currentFeelingAlpha = num;
					this._currentFeeling = elem;
				}
			}
		}

		private string GetCurrentFeeling()
		{
			if (this._currentFeelingAlpha < 0.5f)
			{
				return this._currentFeeling._feelingName;
			}
			return this._defaultFeeling;
		}
	}
}
