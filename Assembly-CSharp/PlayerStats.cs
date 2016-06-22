using Bolt;
using FMOD.Studio;
using HutongGames.PlayMaker;
using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.Utils;
using TheForest.Save;
using TheForest.Tools;
using TheForest.UI;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.ImageEffects;

[DoNotSerializePublic]
public class PlayerStats : MonoBehaviour
{
	public enum DamageType
	{
		Physical,
		Poison,
		Drowning,
		Fire
	}

	[Flags]
	public enum ArmorTypes
	{
		None = 0,
		LizardSkin = 1,
		DeerSkin = 2,
		Leaves = 4,
		Bone = 8
	}

	[Serializable]
	public class ArmorSet
	{
		public PlayerStats.ArmorTypes Type;

		public PlayerStats.ArmorTypes ModelType = PlayerStats.ArmorTypes.LizardSkin;

		[ItemIdPicker(Item.Types.Edible)]
		public int ItemId;

		public int HP;

		[NameFromProperty("_type")]
		public StatEffect[] Effects;

		public Material Mat;
	}

	[Serializable]
	public class StarvationSettingsData
	{
		public int StartDay;

		public int Damage = 10;

		[UnityEngine.Tooltip("Duration in game time days")]
		public float Duration = 2f;

		public float DurationDecay = 0.3f;

		public bool TakingDamage;

		public float SleepingFullnessThreshold = 0.4f;
	}

	[Serializable]
	public class ThirstSettingsData
	{
		public int StartDay;

		[UnityEngine.Tooltip("Duration in game time days")]
		public float Duration = 1f;

		public int Damage = 2;

		public RandomRange DamageChance = new RandomRange
		{
			_max = 120
		};

		public float TutorialThreshold;

		public bool TakingDamage;

		public float SleepingThirstThreshold = 0.6f;
	}

	[Serializable]
	public class FrostSettingsData
	{
		public int StartDay;

		[UnityEngine.Tooltip("Duration in game time days at max cold before damage kicks in")]
		public float Duration = 0.5f;

		[UnityEngine.Tooltip("Current time in game day time that player has been at max cold")]
		public float CurrentTimer;

		[UnityEngine.Tooltip("Total damage per game day")]
		public int Damage = 5;

		public RandomRange DamageChance = new RandomRange
		{
			_max = 150
		};

		public bool TakingDamage;

		[UnityEngine.Tooltip("Cold value at which de-frost stops and returns to normal cold routine")]
		public float DeFrostThreshold = 0.45f;

		[UnityEngine.Tooltip("Time it takes for full screen frost to fade off after taking cold damage")]
		public float DeFrostDuration = 2.5f;

		public bool DoDeFrost;
	}

	[DoNotSerializePublic]
	[Serializable]
	public class AirBreathingData
	{
		[UnityEngine.Tooltip("The % of screen covered by water before air breathing countdown starts")]
		public float ScreenCoverageThreshold = 0.5f;

		[UnityEngine.Tooltip("In real life seconds")]
		public float MaxLungAirCapacity = 25f;

		[UnityEngine.Tooltip("In real life seconds")]
		public float CurrentLungAir = 25f;

		[UnityEngine.Tooltip("In real life seconds")]
		public float MaxRebreatherAirCapacity = 600f;

		[SerializeThis, UnityEngine.Tooltip("In real life seconds")]
		public float CurrentRebreatherAir;

		public Stopwatch CurrentLungAirTimer = new Stopwatch();

		public bool UseRebreather;

		public bool RebreatherIsEquipped;

		[UnityEngine.Tooltip("In real life seconds")]
		public float OutOfAirWarningThreshold = 30f;

		[UnityEngine.Tooltip("In real life seconds")]
		public int Damage = 10;

		public RandomRange DamageChance = new RandomRange
		{
			_max = 100
		};

		public bool TakingDamage;

		public float CurrentAirPercent
		{
			get
			{
				return (this.CurrentLungAir - (float)this.CurrentLungAirTimer.Elapsed.TotalSeconds * LocalPlayer.Stats.Skills.LungBreathingRatio) / this.MaxLungAirCapacity;
			}
		}
	}

	[DoNotSerializePublic]
	[Serializable]
	public class CarriedWeightData
	{
		public float CurrentWeight;

		public string WeightUnit = "lb";

		public float WeightToUnitRatio = 120f;

		public float MaxWeight
		{
			get
			{
				return 1f;
			}
		}
	}

	[DoNotSerializePublic]
	[Serializable]
	public class PhysicalStrengthData
	{
		[Header("Strength")]
		public float StartStrength = 20f;

		public float MinStrength = 10f;

		public float MaxStrength = 99f;

		[Header("Trees cut down")]
		public float CutDownTreeMax = 100f;

		public float CutDownTreePerSecond = -0.0075f;

		[Header("Nutrition"), UnityEngine.Tooltip("Bellow that player is growing fat")]
		public float MealScheduleMinTime = 0.3f;

		[UnityEngine.Tooltip("Above that player is getting weak")]
		public float MealScheduleMaxTime = 0.55f;

		public float GoodNutritionStartValue = 5f;

		public float GoodNutritionPerDay = -1.25f;

		public float BadNutritionPerDay = -0.5f;

		public float UndereatingPointsPerDay = 3f;

		[Header("Weight")]
		public float BaseWeight = 205f;

		public float MinWeight = 155f;

		public float MaxWeight = 305f;

		public float OvereatingWeightPerDay = 2f;

		public float UndereatingWeightPerDay = -1f;

		[SerializeThis, Header("Runtime data")]
		public int LastStrengthUpdateDay;

		[SerializeThis]
		public float CurrentStrength;

		[SerializeThis]
		public float PreviousStrength;

		[SerializeThis]
		public float CurrentWeight;

		[SerializeThis]
		public float CurrentCutDownTreeCount;

		[SerializeThis]
		public float LastMealTime;

		[SerializeThis]
		public bool IsHungry;

		[SerializeThis]
		public float GoodNutritionPoints;

		[SerializeThis]
		public float OvereatingPoints;

		[SerializeThis]
		public float UndereatingPoints;

		public float CurrentStrengthScaled
		{
			get
			{
				return this.CurrentStrength * LocalPlayer.Stats.BloodInfection.EffectRatio;
			}
		}

		public float CurrentStrengthUnscaled
		{
			get
			{
				return this.CurrentStrength;
			}
		}

		public float PreviousStrengthUnscaled
		{
			get
			{
				return this.PreviousStrength;
			}
		}

		public float CurrentWeightScaled
		{
			get
			{
				return this.CurrentWeight;
			}
		}

		private float BadNutritionPoints
		{
			get
			{
				return this.OvereatingPoints + this.UndereatingPoints;
			}
		}

		public void Initialize()
		{
			if (!LevelSerializer.IsDeserializing || this.CurrentStrength < this.MinStrength)
			{
				this.CurrentWeight = 205f;
				this.CurrentStrength = (this.PreviousStrength = this.StartStrength);
				this.GoodNutritionPoints = this.GoodNutritionStartValue;
				this.OvereatingPoints = 1f;
				this.UndereatingPoints = 1f;
				this.CurrentCutDownTreeCount = 0f;
			}
			TreeHealth.OnTreeCutDown.AddListener(new UnityAction<Vector3>(this.OnTreeCutDown));
		}

		public void Refresh()
		{
			if (Time.timeScale > 0f)
			{
				this.CutDownTreeChange(this.CutDownTreePerSecond * Time.deltaTime);
				this.GoodNutritionPointsChange(Scene.Atmosphere.DeltaTimeOfDay * this.GoodNutritionPerDay);
				this.OvereatingPointsChange(Scene.Atmosphere.DeltaTimeOfDay * this.BadNutritionPerDay);
				if (LocalPlayer.Stats.Fullness < 0.5f)
				{
					this.IsHungry = true;
					this.UndereatingPointsChange(Scene.Atmosphere.DeltaTimeOfDay * this.UndereatingPointsPerDay);
				}
				if (this.GoodNutritionPoints < this.BadNutritionPoints)
				{
					if (this.OvereatingPoints > this.UndereatingPoints * 2f)
					{
						this.WeightChange(this.OvereatingWeightPerDay * Scene.Atmosphere.DeltaTimeOfDay);
					}
					else if (this.UndereatingPoints > this.OvereatingPoints * 2f)
					{
						this.WeightChange(this.UndereatingWeightPerDay * Scene.Atmosphere.DeltaTimeOfDay);
					}
				}
				else if (this.CurrentWeight > this.BaseWeight)
				{
					this.WeightChange(this.UndereatingWeightPerDay * 0.15f * Scene.Atmosphere.DeltaTimeOfDay);
				}
				else if (this.CurrentWeight < this.BaseWeight)
				{
					this.WeightChange(this.OvereatingWeightPerDay * 0.15f * Scene.Atmosphere.DeltaTimeOfDay);
				}
				if (Mathf.FloorToInt(LocalPlayer.Stats.DaySurvived) > this.LastStrengthUpdateDay)
				{
					this.LastStrengthUpdateDay = Mathf.FloorToInt(LocalPlayer.Stats.DaySurvived);
					if (this.CurrentCutDownTreeCount > 5f)
					{
						if (this.GoodNutritionPoints > this.BadNutritionPoints)
						{
							this.StrengthChange(1f);
						}
						else if (this.GoodNutritionPoints < this.UndereatingPoints && this.CurrentCutDownTreeCount > 10f)
						{
							this.StrengthChange(-1f);
						}
					}
				}
			}
		}

		public int GetStrengthTrend()
		{
			if (this.CurrentCutDownTreeCount > 5f)
			{
				if (this.GoodNutritionPoints > this.UndereatingPoints)
				{
					return 1;
				}
				if (this.GoodNutritionPoints < this.UndereatingPoints && this.CurrentCutDownTreeCount > 10f)
				{
					return -1;
				}
			}
			return 0;
		}

		public int GetWeightTrend()
		{
			if (this.GoodNutritionPoints < this.BadNutritionPoints)
			{
				if (this.OvereatingPoints > this.UndereatingPoints * 2f)
				{
					return 1;
				}
				if (this.UndereatingPoints > this.OvereatingPoints * 2f)
				{
					return -1;
				}
			}
			return 0;
		}

		public void OnTreeCutDown(Vector3 position)
		{
			if (Vector3.Distance(LocalPlayer.Transform.position, position) < 10f)
			{
				this.CutDownTreeChange(1f);
			}
		}

		public void OnAteFood()
		{
			float num = LocalPlayer.Stats.DaySurvived - this.LastMealTime;
			float num2 = Mathf.Lerp(0.5f, 0.75f, (this.CurrentStrength - this.MinStrength) / (this.MaxStrength - this.MinStrength));
			if (LocalPlayer.Stats.Fullness > num2)
			{
				this.LastMealTime = LocalPlayer.Stats.DaySurvived;
			}
			if (num > this.MealScheduleMinTime)
			{
				if (num < this.MealScheduleMaxTime)
				{
					this.GoodNutritionPointsChange(1f);
					this.OvereatingPointsChange(-0.5f);
					this.UndereatingPointsChange(-1f);
				}
			}
			else if (LocalPlayer.Stats.Fullness > num2)
			{
				if (this.IsHungry)
				{
					this.IsHungry = false;
				}
				else
				{
					this.OvereatingPointsChange(1f);
					this.UndereatingPointsChange(-0.5f);
				}
			}
		}

		private void CutDownTreeChange(float value)
		{
			this.CurrentCutDownTreeCount = Mathf.Clamp(this.CurrentCutDownTreeCount + value, 0f, this.CutDownTreeMax);
		}

		private void GoodNutritionPointsChange(float value)
		{
			this.GoodNutritionPoints = Mathf.Max(this.GoodNutritionPoints + value, 1f);
		}

		public void OvereatingPointsChange(float value)
		{
			this.OvereatingPoints = Mathf.Max(this.OvereatingPoints + value, 1f);
		}

		public void UndereatingPointsChange(float value)
		{
			this.UndereatingPoints = Mathf.Max(this.UndereatingPoints + value, 1f);
		}

		private void WeightChange(float value)
		{
			this.CurrentWeight = Mathf.Clamp(this.CurrentWeight + value, this.MinWeight, this.MaxWeight);
		}

		private void StrengthChange(float value)
		{
			this.PreviousStrength = this.CurrentStrength;
			this.CurrentStrength = Mathf.Clamp(this.CurrentStrength + value, this.MinStrength, this.MaxStrength);
		}
	}

	[DoNotSerializePublic]
	[Serializable]
	public class SkillData
	{
		[UnityEngine.Tooltip("In real life seconds")]
		public float RunSkillLevelDuration = 7200f;

		[UnityEngine.Tooltip("0.1 = 10% less stamina cost")]
		public float RunSkillLevelBonus = 0.1f;

		[UnityEngine.Tooltip("In real life seconds (of underwater lung breathing)")]
		public float BreathingSkillLevelDuration = 1800f;

		[UnityEngine.Tooltip("0.1 = 10% more underwater lunq breathing duration")]
		public float BreathingSkillLevelBonus = 0.1f;

		public float OverweightThreshold = 210f;

		public float OverweightMaxRatio = 0.4f;

		[SerializeThis]
		public float TotalRunDuration;

		[SerializeThis]
		public float TotalLungBreathingDuration;

		private int AthleticismSkill;

		public float RunStaminaRatio
		{
			get;
			private set;
		}

		public float LungBreathingRatio
		{
			get;
			private set;
		}

		public int AthleticismSkillLevel
		{
			get
			{
				return this.AthleticismSkill;
			}
		}

		public void CalcSkills()
		{
			this.AthleticismSkill = Mathf.FloorToInt(this.TotalRunDuration / this.RunSkillLevelDuration) + Mathf.FloorToInt(this.TotalLungBreathingDuration / this.BreathingSkillLevelDuration);
			if (LocalPlayer.Stats.PhysicalStrength.CurrentWeightScaled > this.OverweightThreshold)
			{
				float num = LocalPlayer.Stats.PhysicalStrength.CurrentWeightScaled - this.OverweightThreshold;
				float num2 = LocalPlayer.Stats.PhysicalStrength.MaxWeight - this.OverweightThreshold;
				this.AthleticismSkill = Mathf.FloorToInt((float)this.AthleticismSkill * Mathf.Lerp(1f, this.OverweightMaxRatio, num / num2));
			}
			this.RunStaminaRatio = Mathf.Max(1f - (float)this.AthleticismSkill * this.RunSkillLevelBonus, 0.5f);
			this.LungBreathingRatio = Mathf.Max(1f - (float)this.AthleticismSkill * this.BreathingSkillLevelBonus, 0.5f);
		}
	}

	[DoNotSerializePublic]
	[Serializable]
	public class SanityData
	{
		[SerializeThis]
		public float CurrentSanity = 100f;

		public float SanityPerKill = -0.5f;

		public float SanityPerLimbCutOff = -1f;

		public float SanityPerCannibalism = -3f;

		public float SanityPerSecondInCave = -0.001f;

		public float SanityPerSecondOfMusic = 0.01f;

		public float SanityPerSecondSittedOnBench = 0.15f;

		public float SanityPerInGameHourOfSleep = 0.75f;

		public float SanityPerFreshFoodEaten = 1f;

		public void Initialize()
		{
			GameStats.EnemyKilled.AddListener(new UnityAction(this.OnEnemyKilled));
		}

		public void OnEnemyKilled()
		{
			this.SanityChange(this.SanityPerKill);
		}

		public void OnCutLimbOff()
		{
			this.SanityChange(this.SanityPerLimbCutOff);
		}

		public void OnCannibalism()
		{
			this.SanityChange(this.SanityPerCannibalism);
		}

		public void InCave()
		{
			this.SanityChange(this.SanityPerSecondInCave * Time.deltaTime);
		}

		public void ListeningToMusic()
		{
			this.SanityChange(this.SanityPerSecondOfMusic * Time.deltaTime);
		}

		public void SittingOnBench()
		{
			if (LocalPlayer.Stats.Energy < 100f)
			{
				this.SanityChange(this.SanityPerSecondSittedOnBench * Time.deltaTime);
			}
		}

		public void OnSlept(float hours)
		{
			this.SanityChange(this.SanityPerInGameHourOfSleep * hours);
		}

		public void OnAteFreshFood()
		{
			this.SanityChange(this.SanityPerFreshFoodEaten);
		}

		private void SanityChange(float value)
		{
			this.CurrentSanity = Mathf.Clamp(this.CurrentSanity + value, 0f, 100f);
		}
	}

	[DoNotSerializePublic]
	[Serializable]
	public class InfectionData
	{
		[SerializeThis]
		public bool Infected;

		public RandomRange InfectionChance = new RandomRange();

		[UnityEngine.Tooltip("In game day time")]
		public float AutoHealDelay = 3.40282347E+38f;

		public float EffectModifier = 1f;

		[SerializeThis]
		private float InfectedTime;

		public float EffectRatio
		{
			get
			{
				return (!this.Infected) ? 1f : this.EffectModifier;
			}
		}

		public void GetInfected()
		{
			this.Infected = true;
			this.InfectedTime = LocalPlayer.Stats.DaySurvived;
			GameStats.Infected.Invoke();
		}

		public void TryGetInfected()
		{
			if (this.InfectionChance == 0)
			{
				this.Infected = true;
				this.InfectedTime = LocalPlayer.Stats.DaySurvived;
				GameStats.Infected.Invoke();
			}
		}

		public void TryAutoHeal()
		{
			if (this.Infected && LocalPlayer.Stats.DaySurvived > this.InfectedTime + this.AutoHealDelay)
			{
				this.Infected = false;
			}
		}

		public void Cure()
		{
			LocalPlayer.Stats.CancelInvoke("HitPoison");
			LocalPlayer.Stats.CancelInvoke("disablePoison");
			this.Infected = false;
		}
	}

	[Space(10f)]
	public float BodyTemp = 37f;

	public int HeartRate = 70;

	public int GreyZoneThreshold = 10;

	[SerializeThis]
	public float Stamina = 100f;

	[SerializeThis]
	public float Health;

	[SerializeThis]
	public float Energy = 100f;

	[SerializeThis]
	public int Armor;

	[SerializeThis]
	private PlayerStats.ArmorTypes[] CurrentArmorTypes;

	[SerializeThis]
	private int[] CurrentArmorHP;

	[SerializeThis]
	private int ArmorVis;

	[SerializeThis, Range(0f, 1f)]
	public float ColdArmor;

	[SerializeThis]
	public float BatteryCharge = 100f;

	[SerializeThis]
	public float Stealth;

	[SerializeThis]
	public float SoundRangeDampFactor = 1f;

	[SerializeThis]
	public float Flammable = 1f;

	[SerializeThis]
	public float DaySurvived = -1f;

	[SerializeThis]
	public float NextSleepTime = 0.5f;

	public bool Dead;

	[SerializeThis]
	private bool doneHangingScene;

	[SerializeThis, Space(10f)]
	public float Fullness;

	[SerializeThis]
	public float Thirst;

	[SerializeThis]
	public float Starvation;

	[SerializeThis]
	public float StarvationCurrentDuration = 180f;

	public PlayerStats.StarvationSettingsData StarvationSettings;

	public PlayerStats.ThirstSettingsData ThirstSettings;

	public PlayerStats.FrostSettingsData FrostDamageSettings;

	[SerializeThis]
	public PlayerStats.AirBreathingData AirBreathing;

	public PlayerStats.CarriedWeightData CarriedWeight;

	[SerializeThis]
	public PlayerStats.SkillData Skills;

	[SerializeThis]
	public PlayerStats.SanityData Sanity;

	[SerializeThis]
	public PlayerStats.PhysicalStrengthData PhysicalStrength;

	[SerializeThis]
	public PlayerStats.InfectionData FoodPoisoning;

	[SerializeThis]
	public PlayerStats.InfectionData BloodInfection;

	[Space(10f)]
	public PlayerStats.ArmorSet[] ArmorSets;

	public GameObject[] ArmorModel;

	public GameObject[] LeafArmorModel;

	public GameObject[] BoneArmorModel;

	[Space(10f)]
	public GameObject WakeMusic;

	public Renderer MyArms;

	public Renderer MyBody;

	public GameObject PlayerFlames;

	public Material Muddy;

	public Material BlackManMuddy;

	public Material Bloody;

	public Material BlackManBloody;

	public Material Clean;

	public Material BlackManClean;

	public Material InfectionMat;

	public Material AxeClean;

	public Material AxeBloody;

	[Header("FMOD")]
	public string GaspForAirEvent;

	public string RebreatherEvent;

	public string DrowningEvent;

	public string DyingEvent;

	public string DragCutsceneEvent;

	public string ExtinguishEvent;

	private PlayerInventory Player;

	private int bloodDice;

	private mutantController mutantControl;

	private sceneTracker sceneInfo;

	private bool ShouldCheckArmor;

	private bool IsLit;

	private bool gotControlRefs;

	private bool IsBloody;

	private bool Run;

	private bool Asleep;

	private bool IsTired;

	private bool Cold;

	private float CaveStartSwimmingTime;

	private bool ShouldDoWetColdRoll;

	private bool ShouldDoGotCleanCheck;

	private bool SunWarmth;

	private bool FireWarmth;

	private int BuildingWarmth;

	private bool Recharge;

	private bool CheckingBlood;

	private bool IsCold;

	private bool Sitted;

	private bool isExplode;

	private bool doneDragScene;

	private bool gotCutSceneAxe;

	private int Hunger = 3000;

	private int InfectionChance;

	private int BleedChance;

	private int Ate;

	private int ColdAmt;

	private Color HealthCurrentColor;

	private Color StaminaCurrentColor;

	private Color EnergyBackingCurrentColor;

	private Color EnergyCurrentColor;

	private float NextAdrenalineRush;

	private float ArmorResult;

	private float ColdArmorResult;

	private float HealthResult;

	private float StaminaResult;

	private float EnergyResult;

	private float Tired;

	private float EnergyEx;

	private int DeadTimes;

	private float EnergyIconTemp;

	private TheForestAtmosphere Atmos;

	private PlayerTuts Tuts;

	private PlayerSfx Sfx;

	private GameObject Ocean;

	private enemyWeaponMelee currentAttacker;

	private Grayscale DyingVision;

	private PlayMakerFSM pmDamage;

	private PlayMakerFSM pmControl;

	private PlayMakerFSM pm;

	private Animator animator;

	private playerHitReactions hitReaction;

	private camFollowHead camFollow;

	private FsmFloat fsmStamina;

	private FsmFloat fsmMaxStamina;

	private HudGui Hud;

	private DeadSpotController DSpots;

	private FMOD.Studio.EventInstance SurfaceSnapshot;

	private FMOD.Studio.EventInstance CaveSnapshot;

	private FMOD.Studio.EventInstance CaveReverbSnapshot;

	private FMOD.Studio.EventInstance DyingEventInstance;

	private ParameterInstance DyingHealthParameter;

	private FMOD.Studio.EventInstance RebreatherEventInstance;

	private ParameterInstance RebreatherDepthParameter;

	private FMOD.Studio.EventInstance DrowningEventInstance;

	private FMOD.Studio.EventInstance FireExtinguishEventInstance;

	private GameObject[] CaveDoors;

	private bool BlackMan;

	private float LogWeight;

	private int explodeHash;

	private int getupHash = Animator.StringToHash("getup");

	private int enterCaveHash = Animator.StringToHash("enterCave");

	public float blockDamagePercent;

	private MaterialPropertyBlock bloodPropertyBlock;

	[SerializeThis, HideInInspector]
	public int PlayerVariation;

	[SerializeThis, HideInInspector]
	public int PlayerVariationBody;

	private Coroutine checkItemRoutine;

	private GameObject mutant1;

	private GameObject mutant2;

	private bool delayedMutantSpawnCheck;

	private bool Warm
	{
		get
		{
			return this.SunWarmth || this.FireWarmth || this.IsLit || this.BuildingWarmth > 0;
		}
	}

	public Frost FrostScript
	{
		get;
		private set;
	}

	public bool IsHealthInGreyZone
	{
		get
		{
			return this.Health <= (float)this.GreyZoneThreshold;
		}
	}

	private int GetArmorSetIndex(PlayerStats.ArmorTypes type)
	{
		switch (type)
		{
		case PlayerStats.ArmorTypes.LizardSkin:
			return 0;
		case PlayerStats.ArmorTypes.DeerSkin:
			return 1;
		case PlayerStats.ArmorTypes.Leaves:
			return 2;
		case PlayerStats.ArmorTypes.Bone:
			return 3;
		}
		return -1;
	}

	[DebuggerHidden]
	private IEnumerator OnDeserialized()
	{
		PlayerStats.<OnDeserialized>c__Iterator19A <OnDeserialized>c__Iterator19A = new PlayerStats.<OnDeserialized>c__Iterator19A();
		<OnDeserialized>c__Iterator19A.<>f__this = this;
		return <OnDeserialized>c__Iterator19A;
	}

	private void Awake()
	{
		BleedBehavior.BloodAmount = 0f;
		BleedBehavior.BloodReductionRatio = 1f;
		this.explodeHash = Animator.StringToHash("explode");
		this.DSpots = GameObject.FindWithTag("DeadSpots").GetComponent<DeadSpotController>();
		this.Hud = Scene.HudGui;
		this.Ocean = GameObject.FindWithTag("Ocean");
		this.mutantControl = Scene.MutantControler;
		this.sceneInfo = Scene.SceneTracker;
		this.Player = base.gameObject.GetComponent<PlayerInventory>();
		this.camFollow = base.GetComponentInChildren<camFollowHead>();
		this.hitReaction = base.GetComponent<playerHitReactions>();
		this.Atmos = Scene.Atmosphere;
		this.FrostScript = LocalPlayer.MainCam.GetComponent<Frost>();
		this.Tuts = LocalPlayer.Tuts;
		this.Sfx = LocalPlayer.Sfx;
		this.animator = LocalPlayer.Animator;
		this.DyingVision = LocalPlayer.MainCam.GetComponent<Grayscale>();
		this.Fullness = 1f;
		this.bloodPropertyBlock = new MaterialPropertyBlock();
		if (!LevelSerializer.IsDeserializing)
		{
			CoopPlayerVariations component = base.GetComponent<CoopPlayerVariations>();
			this.PlayerVariation = UnityEngine.Random.Range(0, component.Variations.Length);
			this.PlayerVariationBody = UnityEngine.Random.Range(0, component.BodyMaterials.Length);
		}
		if (this.CurrentArmorTypes == null || this.CurrentArmorTypes.Length != this.ArmorModel.Length)
		{
			this.CurrentArmorTypes = new PlayerStats.ArmorTypes[this.ArmorModel.Length];
			for (int i = 0; i < this.CurrentArmorTypes.Length; i++)
			{
				this.CurrentArmorTypes[i] = PlayerStats.ArmorTypes.None;
				this.ArmorModel[i].SetActive(false);
			}
		}
		if (this.CurrentArmorHP == null || this.CurrentArmorHP.Length != this.ArmorModel.Length)
		{
			this.CurrentArmorHP = new int[this.ArmorModel.Length];
		}
		this.CaveDoors = GameObject.FindGameObjectsWithTag("CaveDoor");
	}

	private void OnDestroy()
	{
		FMODCommon.ReleaseIfValid(this.SurfaceSnapshot, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.CaveSnapshot, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.CaveReverbSnapshot, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.DyingEventInstance, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.RebreatherEventInstance, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.DrowningEventInstance, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.FireExtinguishEventInstance, STOP_MODE.IMMEDIATE);
	}

	public void IsBlackMan()
	{
		this.BlackMan = true;
	}

	private void Start()
	{
		this.Atmos.FogMaxHeight = 400f;
		this.pm = LocalPlayer.ScriptSetup.pmControl;
		this.pmDamage = LocalPlayer.ScriptSetup.pmDamage;
		this.fsmStamina = LocalPlayer.ScriptSetup.pmStamina.FsmVariables.GetFsmFloat("statStamina");
		this.fsmMaxStamina = LocalPlayer.ScriptSetup.pmStamina.FsmVariables.GetFsmFloat("statMaxStamina");
		base.InvokeRepeating("CheckStats", 2f, 2f);
		base.InvokeRepeating("Life", 1f, 1f);
		base.InvokeRepeating("GetTired", 2f, 2f);
		if (Scene.Cams.SleepCam.activeSelf)
		{
			this.Health = 18f;
			this.Energy = 11f;
			this.Fullness = 0f;
			this.CheckingBlood = true;
			base.Invoke("CheckBlood", 10f);
			this.IsBloody = true;
			if (this.BlackMan)
			{
				this.MyArms.sharedMaterial = this.BlackManBloody;
			}
			else
			{
				this.MyArms.sharedMaterial = this.Bloody;
			}
			base.Invoke("CheckArmsStart", 2f);
			Scene.Cams.SleepCam.SetActive(false);
		}
		this.isExplode = false;
		this.resetSkinDamage();
		if (FMOD_StudioSystem.instance)
		{
			this.SurfaceSnapshot = FMOD_StudioSystem.instance.GetEvent("snapshot:/Surface");
			this.CaveSnapshot = FMOD_StudioSystem.instance.GetEvent("snapshot:/Cave");
			this.CaveReverbSnapshot = FMOD_StudioSystem.instance.GetEvent("snapshot:/cave_reverb");
			this.SetInCave(false);
			this.UpdateSnapshotPositions();
			this.DyingEventInstance = FMOD_StudioSystem.instance.GetEvent(this.DyingEvent);
			UnityUtil.ERRCHECK(this.DyingEventInstance.getParameter("health", out this.DyingHealthParameter));
			UnityUtil.ERRCHECK(this.DyingHealthParameter.setValue(this.Health));
			UnityUtil.ERRCHECK(this.DyingEventInstance.start());
			this.RebreatherEventInstance = FMOD_StudioSystem.instance.GetEvent(this.RebreatherEvent);
			if (this.RebreatherEventInstance != null)
			{
				UnityUtil.ERRCHECK(this.RebreatherEventInstance.getParameter("depth", out this.RebreatherDepthParameter));
			}
			this.DrowningEventInstance = FMOD_StudioSystem.instance.GetEvent(this.DrowningEvent);
			this.FireExtinguishEventInstance = FMOD_StudioSystem.instance.GetEvent(this.ExtinguishEvent);
			base.InvokeRepeating("UpdateSnapshotPositions", 0.5f, 0.5f);
		}
		else
		{
			UnityEngine.Debug.LogError("FMOD_StudioSystem.instance is null, could not initialize PlayerStat SFX");
		}
		if (!LevelSerializer.IsDeserializing && this.DaySurvived == -1f)
		{
			this.DaySurvived = (float)Clock.Day;
		}
		this.Skills.CalcSkills();
		this.Sanity.Initialize();
		this.PhysicalStrength.Initialize();
	}

	private static void StartIfNotPlaying(FMOD.Studio.EventInstance evt)
	{
		if (evt == null)
		{
			return;
		}
		PLAYBACK_STATE pLAYBACK_STATE;
		UnityUtil.ERRCHECK(evt.getPlaybackState(out pLAYBACK_STATE));
		if (pLAYBACK_STATE != PLAYBACK_STATE.STARTING && pLAYBACK_STATE != PLAYBACK_STATE.PLAYING)
		{
			UnityUtil.ERRCHECK(evt.start());
		}
	}

	private static void StopIfPlaying(FMOD.Studio.EventInstance evt)
	{
		if (evt == null)
		{
			return;
		}
		PLAYBACK_STATE pLAYBACK_STATE;
		UnityUtil.ERRCHECK(evt.getPlaybackState(out pLAYBACK_STATE));
		if (pLAYBACK_STATE != PLAYBACK_STATE.STOPPING && pLAYBACK_STATE != PLAYBACK_STATE.STOPPED)
		{
			UnityUtil.ERRCHECK(evt.stop(STOP_MODE.ALLOWFADEOUT));
		}
	}

	private void SetInCave(bool inCave)
	{
		if (inCave)
		{
			PlayerStats.StartIfNotPlaying(this.CaveSnapshot);
			PlayerStats.StartIfNotPlaying(this.CaveReverbSnapshot);
			PlayerStats.StopIfPlaying(this.SurfaceSnapshot);
		}
		else
		{
			PlayerStats.StartIfNotPlaying(this.SurfaceSnapshot);
			PlayerStats.StopIfPlaying(this.CaveSnapshot);
			PlayerStats.StopIfPlaying(this.CaveReverbSnapshot);
		}
	}

	private void setStamina(float val)
	{
		this.Stamina += val;
	}

	private void UpdateRebreatherEvent()
	{
		if (this.RebreatherEventInstance != null && this.RebreatherEventInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.RebreatherEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			if (this.RebreatherDepthParameter != null && this.RebreatherDepthParameter.isValid())
			{
				UnityUtil.ERRCHECK(this.RebreatherDepthParameter.setValue(LocalPlayer.WaterViz.CalculateDepthParameter()));
			}
			PlayerStats.StartIfNotPlaying(this.RebreatherEventInstance);
		}
	}

	private void UpdateDrowningEvent()
	{
		if (this.DrowningEventInstance != null && this.DrowningEventInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.DrowningEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			PlayerStats.StartIfNotPlaying(this.DrowningEventInstance);
		}
	}

	private void UpdateExtinguishEvent()
	{
		if (this.IsLit && this.FireExtinguishEventInstance != null && this.FireExtinguishEventInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.FireExtinguishEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			PlayerStats.StartIfNotPlaying(this.FireExtinguishEventInstance);
		}
	}

	public bool IsInNorthColdArea()
	{
		return base.transform.position.y > 160f && base.transform.position.z < -300f;
	}

	private void Update()
	{
		LocalPlayer.Stats.DaySurvived += Scene.Atmosphere.DeltaTimeOfDay;
		if (this.Run && this.HeartRate < 170)
		{
			this.HeartRate++;
		}
		else if (!this.Run && this.HeartRate > 70)
		{
			this.HeartRate--;
		}
		if (this.Sitted)
		{
			this.Energy += 3f * Time.deltaTime;
		}
		if (!Clock.Dark && this.IsCold && !Clock.InCave && !this.IsInNorthColdArea())
		{
			this.IsCold = false;
			this.FrostScript.coverage = 0f;
		}
		if (this.IsInNorthColdArea() && !this.Warm)
		{
			this.IsCold = true;
		}
		if (this.ShouldDoWetColdRoll && !this.IsCold && (Clock.InCave || Clock.Dark))
		{
			if (!LocalPlayer.Buoyancy.InWater)
			{
				this.ShouldDoWetColdRoll = false;
			}
			else if (Clock.InCave)
			{
				if (LocalPlayer.AnimControl.swimming)
				{
					if (Time.time - this.CaveStartSwimmingTime > 12f)
					{
						this.IsCold = true;
						this.ShouldDoWetColdRoll = false;
					}
				}
				else
				{
					this.CaveStartSwimmingTime = Time.time;
				}
			}
			else if (LocalPlayer.Transform.position.y - LocalPlayer.Buoyancy.WaterLevel < 1f)
			{
				if (UnityEngine.Random.Range(0, 100) < 30)
				{
					this.IsCold = true;
				}
				this.ShouldDoWetColdRoll = false;
			}
		}
		if (this.ShouldDoGotCleanCheck)
		{
			if (!LocalPlayer.Buoyancy.InWater)
			{
				this.ShouldDoGotCleanCheck = false;
			}
			else if (LocalPlayer.ScriptSetup.hipsJnt.position.y - LocalPlayer.Buoyancy.WaterLevel < -0.5f)
			{
				this.ShouldDoGotCleanCheck = false;
				this.GotCleanReal();
			}
		}
		if (this.Health <= (float)this.GreyZoneThreshold && AudioListener.volume > 0.2f)
		{
			AudioListener.volume -= 0.1f * Time.deltaTime;
		}
		else if (AudioListener.volume < 1f)
		{
			AudioListener.volume += 0.1f * Time.deltaTime;
		}
		if (this.IsHealthInGreyZone)
		{
			this.Tuts.LowHealthTutorial();
		}
		else
		{
			this.Tuts.CloseLowHealthTutorial();
		}
		if (this.Energy < 30f)
		{
			this.Tuts.LowEnergyTutorial();
		}
		else
		{
			this.Tuts.CloseLowEnergyTutorial();
		}
		if (this.Stamina <= 10f && !this.IsTired)
		{
			base.SendMessage("PlayStaminaBreath");
			this.IsTired = true;
			this.Run = false;
		}
		if (this.Stamina > 10f && this.IsTired)
		{
			this.IsTired = false;
		}
		this.fsmStamina.Value = this.Stamina;
		this.fsmMaxStamina.Value = this.Energy;
		this.HealthResult = this.Health / 100f + (100f - this.Health) / 100f * 0.5f;
		this.StaminaResult = this.Stamina / 100f + (100f - this.Stamina) / 100f * 0.5f;
		this.EnergyResult = this.Energy / 100f + (100f - this.Energy) / 100f * 0.5f;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.CurrentArmorTypes.Length; i++)
		{
			switch (this.CurrentArmorTypes[i])
			{
			case PlayerStats.ArmorTypes.LizardSkin:
			case PlayerStats.ArmorTypes.Leaves:
			case PlayerStats.ArmorTypes.Bone:
				num++;
				break;
			case PlayerStats.ArmorTypes.DeerSkin:
				num2++;
				break;
			}
		}
		this.ColdArmorResult = (float)num2 / 10f / 2f + 0.5f;
		this.ArmorResult = (float)num / 10f / 2f + this.ColdArmorResult;
		this.Hud.ColdArmorBar.fillAmount = this.ColdArmorResult;
		this.Hud.ArmorBar.fillAmount = this.ArmorResult;
		this.Hud.StaminaBar.fillAmount = this.StaminaResult;
		this.Hud.HealthBar.fillAmount = this.HealthResult;
		this.Hud.EnergyBar.fillAmount = this.EnergyResult;
		this.Hud.Stomach.fillAmount = this.Fullness;
		if ((double)this.Fullness < 0.5)
		{
			this.Hud.StomachOutline.SetActive(true);
			if (!this.Hud.Tut_Hungry.activeSelf)
			{
				this.Tuts.HungryTutorial();
			}
		}
		else
		{
			if (this.Hud.Tut_Hungry.activeSelf)
			{
				this.Tuts.CloseHungryTutorial();
			}
			this.Hud.StomachOutline.SetActive(false);
		}
		if (!Scene.Atmosphere.Sleeping || this.Fullness > this.StarvationSettings.SleepingFullnessThreshold)
		{
			this.Fullness -= Scene.Atmosphere.DeltaTimeOfDay * 1.35f;
		}
		if (this.Fullness < 0.2f)
		{
			if (this.Fullness < 0.19f)
			{
				this.Fullness = 0.19f;
			}
			if (this.DaySurvived >= (float)this.StarvationSettings.StartDay && !this.Dead && !Scene.Atmosphere.Sleeping && LocalPlayer.Inventory.enabled)
			{
				if (!Scene.HudGui.StomachStarvation.gameObject.activeSelf)
				{
					if (this.Starvation == 0f)
					{
						this.StarvationCurrentDuration = this.StarvationSettings.Duration;
					}
					Scene.HudGui.StomachStarvation.gameObject.SetActive(true);
				}
				this.Starvation += Scene.Atmosphere.DeltaTimeOfDay / this.StarvationCurrentDuration;
				if (this.Starvation >= 1f)
				{
					if (!this.StarvationSettings.TakingDamage)
					{
						this.StarvationSettings.TakingDamage = true;
						LocalPlayer.Tuts.ShowStarvationTut();
					}
					this.Hit(this.StarvationSettings.Damage, true, PlayerStats.DamageType.Physical);
					this.Starvation = 0f;
					this.StarvationCurrentDuration *= this.StarvationSettings.DurationDecay;
				}
				Scene.HudGui.StomachStarvation.fillAmount = this.Starvation;
			}
		}
		else if (this.Starvation > 0f || Scene.HudGui.StomachStarvation.gameObject.activeSelf)
		{
			this.Starvation = 0f;
			this.StarvationCurrentDuration = this.StarvationSettings.Duration;
			this.StarvationSettings.TakingDamage = false;
			Scene.HudGui.StomachStarvation.gameObject.SetActive(false);
		}
		if (this.Fullness > 1f)
		{
			this.Fullness = 1f;
		}
		if (this.DaySurvived >= (float)this.ThirstSettings.StartDay && !this.Dead && LocalPlayer.Inventory.enabled)
		{
			if (this.Thirst >= 1f)
			{
				if (this.ThirstSettings.DamageChance == 0)
				{
					if (!this.ThirstSettings.TakingDamage)
					{
						this.ThirstSettings.TakingDamage = true;
						LocalPlayer.Tuts.ShowThirstTut();
					}
					this.Hit(this.ThirstSettings.Damage, true, PlayerStats.DamageType.Physical);
				}
			}
			else if (this.Thirst < 0f)
			{
				this.Thirst = 0f;
			}
			else
			{
				if (!Scene.Atmosphere.Sleeping || this.Thirst < this.ThirstSettings.SleepingThirstThreshold)
				{
					this.Thirst += Scene.Atmosphere.DeltaTimeOfDay / this.ThirstSettings.Duration;
				}
				if (this.Thirst > this.ThirstSettings.TutorialThreshold)
				{
					LocalPlayer.Tuts.ShowThirstyTut();
					Scene.HudGui.ThirstOutline.SetActive(true);
				}
				else
				{
					LocalPlayer.Tuts.HideThirstyTut();
					Scene.HudGui.ThirstOutline.SetActive(false);
					this.ThirstSettings.TakingDamage = false;
				}
			}
			Scene.HudGui.Thirst.fillAmount = 1f - this.Thirst;
		}
		bool flag = false;
		bool flag2 = false;
		if (LocalPlayer.WaterViz.ScreenCoverage > this.AirBreathing.ScreenCoverageThreshold && !this.Dead)
		{
			if (!Scene.HudGui.AirReserve.gameObject.activeSelf)
			{
				Scene.HudGui.AirReserve.gameObject.SetActive(true);
			}
			if (!this.AirBreathing.UseRebreather && this.AirBreathing.RebreatherIsEquipped && this.AirBreathing.CurrentRebreatherAir > 0f)
			{
				this.AirBreathing.UseRebreather = true;
			}
			if (this.AirBreathing.UseRebreather)
			{
				flag = true;
				this.AirBreathing.CurrentRebreatherAir -= Time.deltaTime;
				Scene.HudGui.AirReserve.fillAmount = this.AirBreathing.CurrentRebreatherAir / this.AirBreathing.MaxRebreatherAirCapacity;
				if (this.AirBreathing.CurrentRebreatherAir < 0f)
				{
					this.AirBreathing.CurrentLungAir = 0f;
					this.AirBreathing.UseRebreather = false;
				}
				else if (this.AirBreathing.CurrentRebreatherAir < this.AirBreathing.OutOfAirWarningThreshold)
				{
					if (!Scene.HudGui.AirReserveOutline.activeSelf)
					{
						Scene.HudGui.AirReserveOutline.SetActive(true);
					}
				}
				else if (Scene.HudGui.AirReserveOutline.activeSelf)
				{
					Scene.HudGui.AirReserveOutline.SetActive(false);
				}
			}
			else
			{
				if (Time.timeScale > 0f)
				{
					if (!this.AirBreathing.CurrentLungAirTimer.IsRunning)
					{
						this.AirBreathing.CurrentLungAirTimer.Start();
					}
				}
				else if (this.AirBreathing.CurrentLungAirTimer.IsRunning)
				{
					this.AirBreathing.CurrentLungAirTimer.Stop();
				}
				if ((double)this.AirBreathing.CurrentLungAir > this.AirBreathing.CurrentLungAirTimer.Elapsed.TotalSeconds * (double)this.Skills.LungBreathingRatio)
				{
					this.Skills.TotalLungBreathingDuration += Time.deltaTime;
					Scene.HudGui.AirReserve.fillAmount = Mathf.Lerp(Scene.HudGui.AirReserve.fillAmount, this.AirBreathing.CurrentAirPercent, Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime));
					if (!Scene.HudGui.AirReserveOutline.activeSelf)
					{
						Scene.HudGui.AirReserveOutline.SetActive(true);
					}
				}
				else
				{
					flag2 = true;
					if (this.AirBreathing.DamageChance == 0)
					{
						this.Hit(this.AirBreathing.Damage, true, PlayerStats.DamageType.Drowning);
					}
					if (this.Dead)
					{
						this.DeadTimes++;
						Scene.HudGui.AirReserve.gameObject.SetActive(false);
						Scene.HudGui.AirReserveOutline.SetActive(false);
					}
					else if (!Scene.HudGui.AirReserveOutline.activeSelf)
					{
						Scene.HudGui.AirReserveOutline.SetActive(true);
					}
				}
			}
		}
		else if (this.AirBreathing.CurrentLungAir < this.AirBreathing.MaxLungAirCapacity || Scene.HudGui.AirReserve.gameObject.activeSelf)
		{
			if (this.GaspForAirEvent.Length > 0 && FMOD_StudioSystem.instance)
			{
				FMOD_StudioSystem.instance.PlayOneShot(this.GaspForAirEvent, base.transform.position, delegate(FMOD.Studio.EventInstance instance)
				{
					float value = 85f;
					if (!this.AirBreathing.UseRebreather)
					{
						value = (this.AirBreathing.CurrentLungAir - (float)this.AirBreathing.CurrentLungAirTimer.Elapsed.TotalSeconds) / this.AirBreathing.MaxLungAirCapacity * 100f;
					}
					UnityUtil.ERRCHECK(instance.setParameterValue("oxygen", value));
					return true;
				});
			}
			this.AirBreathing.CurrentLungAirTimer.Stop();
			this.AirBreathing.CurrentLungAirTimer.Reset();
			this.AirBreathing.CurrentLungAir = this.AirBreathing.MaxLungAirCapacity;
			Scene.HudGui.AirReserve.gameObject.SetActive(false);
			Scene.HudGui.AirReserveOutline.SetActive(false);
		}
		if (flag)
		{
			this.UpdateRebreatherEvent();
		}
		else
		{
			PlayerStats.StopIfPlaying(this.RebreatherEventInstance);
		}
		if (flag2)
		{
			this.UpdateDrowningEvent();
		}
		else
		{
			PlayerStats.StopIfPlaying(this.DrowningEventInstance);
		}
		if (this.Energy > 100f)
		{
			this.Energy = 100f;
		}
		if (this.Energy < 10f)
		{
			this.Energy = 10f;
		}
		if (this.Health < 0f)
		{
			this.Health = 0f;
		}
		if (this.Health > 100f)
		{
			this.Health = 100f;
		}
		if (this.Health < 20f)
		{
			this.Hud.HealthBarOutline.SetActive(true);
		}
		else
		{
			this.Hud.HealthBarOutline.SetActive(false);
		}
		if (this.Energy < 40f || this.IsCold)
		{
			this.Hud.EnergyBarOutline.SetActive(true);
		}
		else
		{
			this.Hud.EnergyBarOutline.SetActive(false);
		}
		if (this.Stamina < 30f)
		{
			this.Hud.StaminaBarOutline.SetActive(true);
		}
		else
		{
			this.Hud.StaminaBarOutline.SetActive(false);
		}
		if (this.Stamina < 0f)
		{
			this.Stamina = 0f;
		}
		if (this.Stamina < this.Energy)
		{
			if (!LocalPlayer.FpCharacter.running)
			{
				this.Stamina += 6f * Time.deltaTime;
			}
		}
		else
		{
			this.Stamina = this.Energy;
		}
		if (this.CheckingBlood && LocalPlayer.ScriptSetup.proxyAttackers.arrayList.Count > 0)
		{
			this.StopBloodCheck();
		}
		if (this.IsCold && !this.Warm && LocalPlayer.Inventory.enabled)
		{
			if (this.BodyTemp > 14f)
			{
				this.BodyTemp -= 1f * (1f - this.ColdArmor);
			}
			if (this.FrostDamageSettings.DoDeFrost)
			{
				if (this.FrostScript.coverage > this.FrostDamageSettings.DeFrostThreshold)
				{
					this.FrostScript.coverage -= Time.deltaTime / this.FrostDamageSettings.DeFrostDuration;
				}
				else
				{
					this.FrostDamageSettings.DoDeFrost = false;
				}
			}
			else if (this.FrostScript.coverage < 0.49f)
			{
				this.FrostScript.coverage += 0.01f * Time.deltaTime * (1f - this.ColdArmor);
				if (this.FrostScript.coverage > 0.492f)
				{
					this.FrostScript.coverage = 0.491f;
				}
			}
			else if (Scene.Clock.ElapsedGameTime >= (float)this.FrostDamageSettings.StartDay)
			{
				if (this.FrostDamageSettings.CurrentTimer >= this.FrostDamageSettings.Duration)
				{
					if (this.FrostDamageSettings.DamageChance == 0)
					{
						this.Hit(this.FrostDamageSettings.Damage, true, PlayerStats.DamageType.Physical);
						this.FrostScript.coverage = 0.51f;
						this.FrostDamageSettings.DoDeFrost = true;
						if (!this.FrostDamageSettings.TakingDamage)
						{
							this.FrostDamageSettings.TakingDamage = true;
							LocalPlayer.Tuts.ShowColdDamageTut();
						}
					}
				}
				else
				{
					this.FrostDamageSettings.CurrentTimer += Scene.Atmosphere.DeltaTimeOfDay;
				}
			}
		}
		if (this.Warm)
		{
			if (this.BodyTemp < 37f)
			{
				this.BodyTemp += 1f * (1f + this.ColdArmor);
			}
			if (this.FrostScript.coverage > 0f)
			{
				this.FrostScript.coverage -= 0.01f * Time.deltaTime * (1f + this.ColdArmor);
			}
			else
			{
				this.FrostDamageSettings.TakingDamage = false;
			}
			this.FrostDamageSettings.CurrentTimer = 0f;
		}
		if (Clock.InCave)
		{
			this.Sanity.InCave();
		}
		if (PlayerSfx.MusicPlaying)
		{
			this.Sanity.ListeningToMusic();
		}
		if (this.Sitted)
		{
			this.Sanity.SittingOnBench();
		}
		this.PhysicalStrength.Refresh();
		if (this.DyingEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.DyingEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			UnityUtil.ERRCHECK(this.DyingHealthParameter.setValue(this.Health));
		}
		if (this.FireExtinguishEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.FireExtinguishEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
		}
		if (Cheats.InfiniteEnergy)
		{
			this.Energy = 100f;
			this.Stamina = 100f;
		}
	}

	private void UpdateSnapshotPositions()
	{
		GameObject gameObject = null;
		float num = 3.40282347E+38f;
		for (int i = 0; i < this.CaveDoors.Length; i++)
		{
			GameObject gameObject2 = this.CaveDoors[i];
			float sqrMagnitude = (base.transform.position - gameObject2.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				gameObject = gameObject2;
			}
		}
		if (gameObject)
		{
			ATTRIBUTES_3D attributes = UnityUtil.to3DAttributes(gameObject, null);
			attributes.position = gameObject.transform.TransformPoint(0f, 0f, -0.5f).toFMODVector();
			UnityUtil.ERRCHECK(this.CaveSnapshot.set3DAttributes(attributes));
			attributes.position = gameObject.transform.TransformPoint(0f, 0f, 0.5f).toFMODVector();
			UnityUtil.ERRCHECK(this.SurfaceSnapshot.set3DAttributes(attributes));
		}
	}

	private void Running()
	{
		this.Tuts.CloseSprint();
		this.Run = true;
	}

	private void StoppedRunning()
	{
		this.Run = false;
	}

	private void UsedEnergy()
	{
		base.CancelInvoke("StopWaiting");
		this.Energy -= this.EnergyEx;
		if (this.Stamina > 30f)
		{
			base.Invoke("HeartOff", 1f);
		}
	}

	private void HeartOff()
	{
	}

	private void StopWaiting()
	{
	}

	public void UsedStick()
	{
		this.EnergyEx = 0.025f;
		this.UsedEnergy();
	}

	private void GotMeds()
	{
		base.SendMessage("AddedMeds");
	}

	public void AteMeds()
	{
		this.Health += 60f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f * 1.5f;
	}

	public void AteAloe()
	{
		this.Health += 6f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
	}

	public void UsedRock()
	{
		this.EnergyEx = 0.05f;
		this.UsedEnergy();
	}

	public void UsedAxe()
	{
		this.EnergyEx = 0.05f;
		this.UsedEnergy();
	}

	private void GotMud()
	{
		if (this.BlackMan)
		{
			this.MyArms.sharedMaterial = this.BlackManMuddy;
		}
		else
		{
			this.MyArms.sharedMaterial = this.Muddy;
		}
		LocalPlayer.TargetFunctions.coveredInMud = true;
		this.pm.SendEvent("toCheckArms");
	}

	private void GotBloody()
	{
		this.bloodDice = UnityEngine.Random.Range(0, 4);
		if (this.bloodDice == 2 && !this.CheckingBlood)
		{
			this.CheckingBlood = true;
			base.Invoke("CheckBlood", 10f);
			this.IsBloody = true;
			if (this.BlackMan)
			{
				this.MyArms.sharedMaterial = this.BlackManBloody;
			}
			else
			{
				this.MyArms.sharedMaterial = this.Bloody;
			}
			LocalPlayer.Inventory.BloodyWeapon();
		}
	}

	private void StopBloodCheck()
	{
		base.CancelInvoke("CheckBlood");
		base.Invoke("CheckBlood", 5f);
	}

	public void CheckArmor()
	{
		this.pm.SendEvent("toCheckArms");
	}

	public bool CheckItem(Item.EquipmentSlot equipmentSlot)
	{
		bool flag = this.checkItemRoutine != null;
		if (flag)
		{
			base.StopCoroutine(this.checkItemRoutine);
		}
		this.checkItemRoutine = base.StartCoroutine(this.resetCheckItem(equipmentSlot));
		return !flag;
	}

	[DebuggerHidden]
	private IEnumerator resetCheckItem(Item.EquipmentSlot equipmentSlot)
	{
		PlayerStats.<resetCheckItem>c__Iterator19B <resetCheckItem>c__Iterator19B = new PlayerStats.<resetCheckItem>c__Iterator19B();
		<resetCheckItem>c__Iterator19B.equipmentSlot = equipmentSlot;
		<resetCheckItem>c__Iterator19B.<$>equipmentSlot = equipmentSlot;
		<resetCheckItem>c__Iterator19B.<>f__this = this;
		return <resetCheckItem>c__Iterator19B;
	}

	private void CheckBlood()
	{
		if (LocalPlayer.ScriptSetup.proxyAttackers.arrayList.Count < 1)
		{
			this.Tuts.BloodyTut();
			this.CheckingBlood = false;
			this.pm.SendEvent("toCheckArms");
		}
	}

	private void GotClean()
	{
		if (this.BuildingWarmth == 0)
		{
			if (!this.IsCold)
			{
				if (this.IsInNorthColdArea())
				{
					this.IsCold = true;
				}
				else
				{
					this.ShouldDoWetColdRoll = (Clock.InCave || Clock.Dark);
				}
			}
			this.ShouldDoGotCleanCheck = true;
			this.CaveStartSwimmingTime = Time.time;
		}
	}

	private void GotCleanReal()
	{
		if (this.BuildingWarmth == 0)
		{
			this.ShouldDoGotCleanCheck = false;
			if (this.IsLit && this.FireExtinguishEventInstance != null && this.FireExtinguishEventInstance.isValid())
			{
				this.UpdateExtinguishEvent();
				UnityUtil.ERRCHECK(this.FireExtinguishEventInstance.start());
			}
			this.resetSkinDamage();
			this.Player.CleanWeapon();
			this.StopBurning();
			this.Tuts.CloseBloodyTut();
			LocalPlayer.TargetFunctions.coveredInMud = false;
			this.IsBloody = false;
			if (this.BlackMan)
			{
				this.MyArms.sharedMaterial = this.BlackManClean;
			}
			else
			{
				this.MyArms.sharedMaterial = this.Clean;
			}
		}
	}

	public bool GoToSleep()
	{
		if (!BoltNetwork.isClient)
		{
			for (int i = 0; i < Scene.SceneTracker.allPlayers.Count; i++)
			{
				Transform transform = Scene.SceneTracker.allPlayers[i].transform;
				Transform transform2 = this.mutantControl.findClosestEnemy(transform);
				if (transform2 && (LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Count > 0 || Vector3.Distance(transform.position, transform2.transform.position) < 65f))
				{
					GraphNode node = AstarPath.active.GetNearest(transform2.transform.position, NNConstraint.Default).node;
					uint area = node.Area;
					NNConstraint nNConstraint = new NNConstraint();
					nNConstraint.constrainArea = true;
					int area2 = (int)area;
					nNConstraint.area = area2;
					GraphNode node2 = AstarPath.active.GetNearest(transform.position, nNConstraint).node;
					Vector3 a = new Vector3((float)(node2.position[0] / 1000), (float)(node2.position[1] / 1000), (float)(node2.position[2] / 1000));
					if (Vector3.Distance(a, LocalPlayer.Transform.position) < 6f)
					{
						base.StartCoroutine("setupSleepEncounter", transform2.gameObject);
						this.GoToSleepFake();
						return false;
					}
				}
			}
			Scene.MutantSpawnManager.offsetSleepAmounts();
			Scene.MutantControler.startSetupFamilies();
		}
		this.NextSleepTime = Scene.Clock.ElapsedGameTime;
		base.Invoke("TurnOffSleepCam", 3f);
		this.Tired = 0f;
		this.Atmos.TimeLapse();
		Scene.HudGui.GuiCam.SetActive(false);
		Scene.Cams.SleepCam.SetActive(true);
		this.Energy += 100f;
		return true;
	}

	public void JustSave()
	{
		if (!BoltNetwork.isRunning || BoltNetwork.isServer)
		{
			SaveSlotSelectionScreen.OnSlotSelected.AddListener(new UnityAction(this.OnSaveSlotSelected));
			SaveSlotSelectionScreen.OnSlotCanceled.AddListener(new UnityAction(this.OnSaveSlotSelectionCanceled));
			LocalPlayer.Inventory.TogglePauseMenu();
			LocalPlayer.Inventory.enabled = false;
			Scene.HudGui.SaveSlotSelectionScreen.SetActive(true);
		}
		else
		{
			Scene.Cams.SaveCam.SetActive(true);
			base.Invoke("TurnOffSaveCam", 1f);
			PlayerSpawn.SaveMpCharacter(base.gameObject);
		}
	}

	public void OnSaveSlotSelected()
	{
		if (!this.Dead)
		{
			base.StartCoroutine(this.OnSaveSlotSelectedRoutine());
		}
		else
		{
			Scene.Cams.SaveCam.SetActive(false);
		}
	}

	[DebuggerHidden]
	private IEnumerator OnSaveSlotSelectedRoutine()
	{
		PlayerStats.<OnSaveSlotSelectedRoutine>c__Iterator19C <OnSaveSlotSelectedRoutine>c__Iterator19C = new PlayerStats.<OnSaveSlotSelectedRoutine>c__Iterator19C();
		<OnSaveSlotSelectedRoutine>c__Iterator19C.<>f__this = this;
		return <OnSaveSlotSelectedRoutine>c__Iterator19C;
	}

	public void OnSaveSlotSelectionCanceled()
	{
		LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Pause;
		LocalPlayer.Inventory.TogglePauseMenu();
		LocalPlayer.Inventory.enabled = true;
		Scene.HudGui.SaveSlotSelectionScreen.SetActive(false);
		Scene.Cams.SaveCam.SetActive(false);
		SaveSlotSelectionScreen.OnSlotSelected.RemoveListener(new UnityAction(this.OnSaveSlotSelected));
		SaveSlotSelectionScreen.OnSlotSelected.RemoveListener(new UnityAction(this.OnSaveSlotSelectionCanceled));
	}

	private void TurnOffSleepCam()
	{
		Scene.HudGui.GuiCam.SetActive(true);
		Scene.Cams.SleepCam.SetActive(false);
	}

	private void TurnOffSaveCam()
	{
		Scene.Cams.SaveCam.SetActive(false);
	}

	[DebuggerHidden]
	private IEnumerator setupSleepEncounter(GameObject go)
	{
		PlayerStats.<setupSleepEncounter>c__Iterator19D <setupSleepEncounter>c__Iterator19D = new PlayerStats.<setupSleepEncounter>c__Iterator19D();
		<setupSleepEncounter>c__Iterator19D.go = go;
		<setupSleepEncounter>c__Iterator19D.<$>go = go;
		<setupSleepEncounter>c__Iterator19D.<>f__this = this;
		return <setupSleepEncounter>c__Iterator19D;
	}

	public void GoToSleepFake()
	{
		this.Tired -= 10f;
		Scene.HudGui.GuiCam.SetActive(false);
		Scene.Cams.SleepCam.SetActive(true);
		this.Energy += 10f;
		base.Invoke("WakeFake", (!BoltNetwork.isClient) ? 4f : 3.6f);
	}

	private void WakeFake()
	{
		Scene.HudGui.GuiCam.SetActive(true);
		Scene.Cams.SleepCam.SetActive(false);
		this.Sfx.PlayWokenByEnemies();
	}

	private void Wake()
	{
		if (this.Atmos.Sleeping)
		{
			this.Atmos.NoTimeLapse();
			float num = Scene.Clock.ElapsedGameTime - this.NextSleepTime;
			this.NextSleepTime = Scene.Clock.ElapsedGameTime + 0.95f - num;
			this.Sanity.OnSlept(num * 24f);
			this.FoodPoisoning.Cure();
		}
	}

	public void Heat()
	{
		this.FireWarmth = true;
		this.IsCold = false;
	}

	public void LeaveHeat()
	{
		this.FireWarmth = false;
	}

	private void HomeWarmth()
	{
		this.BuildingWarmth++;
		this.IsCold = false;
	}

	private void LeaveHomeWarmth()
	{
		this.BuildingWarmth--;
	}

	public void CheckStats()
	{
		if (this.IsHealthInGreyZone)
		{
			if (!this.DyingVision.enabled)
			{
				this.DyingVision.enabled = true;
			}
			if ((!this.Dead || !BoltNetwork.isRunning) && !this.Recharge && !this.Run)
			{
				this.Recharge = true;
				base.Invoke("RechargeHealth", 12f);
			}
		}
		else if (this.DyingVision.enabled)
		{
			this.DyingVision.enabled = false;
		}
	}

	private void RechargeHealth()
	{
		if (this.IsHealthInGreyZone && !this.Dead)
		{
			this.Health = (float)(this.GreyZoneThreshold + 1);
		}
		this.Recharge = false;
	}

	private void Fell()
	{
		this.Health -= 200f;
		if (this.Health <= 0f)
		{
			this.Dead = true;
			this.Player.enabled = false;
			this.KillPlayer();
		}
	}

	private void getHitDirection(Vector3 pos)
	{
		Vector3 a = new Vector3(pos.x, base.transform.position.y, pos.z);
		Vector3 normalized = (a - base.transform.position).normalized;
		LocalPlayer.ScriptSetup.pmDamage.FsmVariables.GetFsmVector3("hitDir").Value = normalized;
		this.hitReaction.hitDir = normalized;
	}

	public void setCurrentAttacker(enemyWeaponMelee attacker)
	{
		this.currentAttacker = attacker;
	}

	public void hitFromEnemy(int getDamage)
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).tagHash == this.enterCaveHash)
		{
			return;
		}
		if (this.currentAttacker)
		{
			EventRegistry.Enemy.Publish(TfEvent.EnemyContact, this.currentAttacker.GetComponentInParent<enemyType>().Type);
		}
		bool isHealthInGreyZone = this.IsHealthInGreyZone;
		if (!isHealthInGreyZone && (float)getDamage > this.Health)
		{
			getDamage = Mathf.Clamp(getDamage, 1, Mathf.FloorToInt(this.Health) - 1);
		}
		float num = 0f;
		if (this.currentAttacker)
		{
			Vector3 vector = base.transform.InverseTransformPoint(this.currentAttacker.transform.root.position);
			num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		}
		bool flag = this.animator.GetBool("stickBlock") && num < 60f && num > -60f;
		if (flag)
		{
			if (this.blockDamagePercent > 0f)
			{
				try
				{
					if (this.CurrentArmorTypes[this.ArmorVis] == PlayerStats.ArmorTypes.None)
					{
						int damage = Mathf.FloorToInt((float)getDamage * (this.blockDamagePercent / 6f));
						this.Hit(damage, true, PlayerStats.DamageType.Physical);
					}
				}
				catch
				{
				}
			}
			this.pm.SendEvent("blockReaction");
		}
		else
		{
			if (this.animator.GetBool("stickBlock"))
			{
				getDamage /= 2;
			}
			this.Hit(getDamage, false, PlayerStats.DamageType.Physical);
			this.setSkinDamage();
			this.pm.SendEvent("blockReaction");
		}
		if (this.currentAttacker)
		{
			if (flag)
			{
				if (this.animator.GetBool("shellHeld"))
				{
					FMODCommon.PlayOneshotNetworked(this.currentAttacker.shellBlockEvent, base.transform, FMODCommon.NetworkRole.Server);
				}
				else
				{
					FMODCommon.PlayOneshotNetworked(this.currentAttacker.blockEvent, base.transform, FMODCommon.NetworkRole.Server);
				}
			}
			else
			{
				FMODCommon.PlayOneshotNetworked(this.currentAttacker.weaponHitEvent, base.transform, FMODCommon.NetworkRole.Server);
			}
			this.currentAttacker = null;
		}
		if (LocalPlayer.AnimControl.currRaft)
		{
			RaftGrab raftGrab = RaftGrab.Create(GlobalTargets.OnlyServer);
			raftGrab.Raft = LocalPlayer.AnimControl.currRaft.GetComponentInParent<BoltEntity>();
			raftGrab.Player = null;
			raftGrab.Send();
		}
		if (!isHealthInGreyZone && this.IsHealthInGreyZone && !this.Dead)
		{
			base.StartCoroutine(this.AdrenalineRush());
		}
	}

	public void setSkinDamage()
	{
		this.MyBody.GetPropertyBlock(this.bloodPropertyBlock);
		float num = this.bloodPropertyBlock.GetFloat("_Damage1");
		if (num < 1f)
		{
			num += 0.2f;
			this.bloodPropertyBlock.SetFloat("_Damage1", num);
			this.bloodPropertyBlock.SetFloat("_Damage2", num);
			this.bloodPropertyBlock.SetFloat("_Damage3", num);
			this.bloodPropertyBlock.SetFloat("_Damage4", num);
			LocalPlayer.Animator.SetFloatReflected("skinDamage1", num);
			LocalPlayer.Animator.SetFloatReflected("skinDamage2", num);
			LocalPlayer.Animator.SetFloatReflected("skinDamage3", num);
			LocalPlayer.Animator.SetFloatReflected("skinDamage4", num);
			this.MyBody.SetPropertyBlock(this.bloodPropertyBlock);
		}
	}

	public void resetSkinDamage()
	{
		this.MyBody.GetPropertyBlock(this.bloodPropertyBlock);
		this.bloodPropertyBlock.SetFloat("_Damage1", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage2", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage3", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage4", 0f);
		LocalPlayer.Animator.SetFloatReflected("skinDamage1", 0f);
		LocalPlayer.Animator.SetFloatReflected("skinDamage2", 0f);
		LocalPlayer.Animator.SetFloatReflected("skinDamage3", 0f);
		LocalPlayer.Animator.SetFloatReflected("skinDamage4", 0f);
		this.MyBody.SetPropertyBlock(this.bloodPropertyBlock);
	}

	[DebuggerHidden]
	private IEnumerator AdrenalineRush()
	{
		PlayerStats.<AdrenalineRush>c__Iterator19E <AdrenalineRush>c__Iterator19E = new PlayerStats.<AdrenalineRush>c__Iterator19E();
		<AdrenalineRush>c__Iterator19E.<>f__this = this;
		return <AdrenalineRush>c__Iterator19E;
	}

	private GameObject GetArmorPiece(PlayerStats.ArmorTypes modelType, int index)
	{
		if (modelType != PlayerStats.ArmorTypes.Bone)
		{
			return this.ArmorModel[index];
		}
		return this.BoneArmorModel[index];
	}

	public void AddArmorVisible(PlayerStats.ArmorTypes type)
	{
		int armorSetIndex = this.GetArmorSetIndex(type);
		PlayerStats.ArmorSet armorSet = this.ArmorSets[armorSetIndex];
		for (int i = 0; i < this.ArmorVis; i++)
		{
			if (this.CurrentArmorTypes[i] == PlayerStats.ArmorTypes.None)
			{
				this.CurrentArmorTypes[i] = type;
				GameObject armorPiece = this.GetArmorPiece(armorSet.ModelType, i);
				armorPiece.SetActive(true);
				armorPiece.GetComponent<Renderer>().sharedMaterial = armorSet.Mat;
				this.LeafArmorModel[i].SetActive(type == PlayerStats.ArmorTypes.Leaves);
				this.BoneArmorModel[i].SetActive(type == PlayerStats.ArmorTypes.Bone);
				this.CurrentArmorHP[i] = armorSet.HP;
				ItemUtils.ApplyEffectsToStats(armorSet.Effects, true);
				return;
			}
		}
		if (this.ArmorVis == this.ArmorModel.Length)
		{
			this.ArmorVis = 0;
		}
		if (this.CurrentArmorTypes[this.ArmorVis] != PlayerStats.ArmorTypes.None)
		{
			PlayerStats.ArmorTypes type2 = this.CurrentArmorTypes[this.ArmorVis];
			int armorSetIndex2 = this.GetArmorSetIndex(type2);
			PlayerStats.ArmorSet armorSet2 = this.ArmorSets[armorSetIndex2];
			if (armorSet2.ModelType != armorSet.ModelType)
			{
				this.GetArmorPiece(armorSet2.ModelType, this.ArmorVis).SetActive(false);
			}
			ItemUtils.ApplyEffectsToStats(armorSet2.Effects, false);
			if (armorSet2.HP - this.CurrentArmorHP[this.ArmorVis] < 4)
			{
				this.Player.AddItem(armorSet2.ItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			}
		}
		this.CurrentArmorHP[this.ArmorVis] = armorSet.HP;
		this.CurrentArmorTypes[this.ArmorVis] = type;
		GameObject armorPiece2 = this.GetArmorPiece(armorSet.ModelType, this.ArmorVis);
		armorPiece2.SetActive(true);
		armorPiece2.GetComponent<Renderer>().sharedMaterial = armorSet.Mat;
		this.LeafArmorModel[this.ArmorVis].SetActive(type == PlayerStats.ArmorTypes.Leaves);
		this.BoneArmorModel[this.ArmorVis].SetActive(type == PlayerStats.ArmorTypes.Bone);
		ItemUtils.ApplyEffectsToStats(armorSet.Effects, true);
		this.ArmorVis++;
	}

	public int HitArmor(int damage)
	{
		PlayerStats.ArmorTypes armorTypes = PlayerStats.ArmorTypes.LizardSkin | PlayerStats.ArmorTypes.DeerSkin | PlayerStats.ArmorTypes.Leaves | PlayerStats.ArmorTypes.Bone;
		for (int i = this.CurrentArmorTypes.Length - 1; i >= 0; i--)
		{
			if ((this.CurrentArmorTypes[i] & armorTypes) != PlayerStats.ArmorTypes.None)
			{
				this.CurrentArmorHP[i] -= damage;
				if (this.CurrentArmorHP[i] > 0)
				{
					return 0;
				}
				ItemUtils.ApplyEffectsToStats(this.ArmorSets[this.GetArmorSetIndex(this.CurrentArmorTypes[i])].Effects, false);
				int armorSetIndex = this.GetArmorSetIndex(this.CurrentArmorTypes[i]);
				this.ArmorModel[i].SetActive(false);
				this.LeafArmorModel[i].SetActive(false);
				this.BoneArmorModel[i].SetActive(false);
				this.CurrentArmorTypes[i] = PlayerStats.ArmorTypes.None;
				if (this.CurrentArmorHP[i] == 0)
				{
					return 0;
				}
				damage = -this.CurrentArmorHP[i];
				this.CurrentArmorHP[i] = 0;
			}
		}
		return damage;
	}

	private void Explosion(float dist)
	{
		if (this.isExplode)
		{
			return;
		}
		if (LocalPlayer.Animator.GetCurrentAnimatorStateInfo(0).tagHash == this.enterCaveHash)
		{
			return;
		}
		if (LocalPlayer.Animator.GetCurrentAnimatorStateInfo(2).tagHash == this.explodeHash)
		{
			return;
		}
		if (dist < 15f)
		{
			this.isExplode = true;
			base.Invoke("resetExplosion", 2.2f);
			this.Health -= 25f;
			this.CheckDeath();
			if (this.Health < 1f)
			{
				return;
			}
			if (LocalPlayer.AnimControl.swimming)
			{
				LocalPlayer.HitReactions.enableHitState();
			}
			else
			{
				LocalPlayer.AnimControl.disconnectFromObject();
				this.pmDamage.SendEvent("toHitFall");
				this.pm.SendEvent("toHit");
				this.animator.SetIntegerReflected("knockBackInt", 1);
				this.animator.SetTriggerReflected("knockBackTrigger");
				base.Invoke("ResetHit", 1f);
			}
			this.Sfx.PlayHurtSound();
			base.Invoke("ShowFindHealth", 15f);
			this.CheckDeath();
		}
		else if (dist > 15f)
		{
			this.isExplode = true;
			base.Invoke("resetExplosion", 2.2f);
			this.Health -= 10f;
			this.CheckDeath();
			if (this.Health < 1f)
			{
				return;
			}
			if (LocalPlayer.AnimControl.swimming)
			{
				LocalPlayer.HitReactions.enableHitState();
			}
			else
			{
				this.pmDamage.SendEvent("toHit");
				this.pm.SendEvent("toHit");
				this.animator.SetIntegerReflected("knockBackInt", 0);
				this.animator.SetTriggerReflected("knockBackTrigger");
				base.Invoke("ResetHit", 1f);
			}
			this.Sfx.PlayHurtSound();
			base.Invoke("ShowFindHealth", 15f);
			this.CheckDeath();
		}
	}

	private void HitFromPlayMaker(int damage)
	{
		this.Hit(damage, false, PlayerStats.DamageType.Physical);
	}

	public void Hit(int damage, bool ignoreArmor, PlayerStats.DamageType type = PlayerStats.DamageType.Physical)
	{
		if (!this.Dead && this.animator.GetCurrentAnimatorStateInfo(0).tagHash != this.enterCaveHash)
		{
			if (UnityEngine.Random.Range(0, 4) == 0)
			{
				this.Player.SpecialItems.SendMessage("TurnLighterOff");
			}
			this.Player.SpecialActions.SendMessage("forceExitSled");
			damage = ((!ignoreArmor) ? this.HitArmor(damage) : damage);
			if (damage > 0)
			{
				BleedBehavior.BloodAmount += Mathf.Clamp01(3f * (float)damage / this.Health) * 0.9f;
				this.Health -= (float)damage;
				BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.75f);
			}
			if (!LocalPlayer.FpCharacter.jumpCoolDown && this.Health > 0f && (this.blockDamagePercent == 0f || !this.animator.GetBool("stickBlock")))
			{
				if (!LocalPlayer.FpCharacter.Sitting && !LocalPlayer.AnimControl.onRope && !LocalPlayer.AnimControl.cliffClimb && !LocalPlayer.AnimControl.onRaft)
				{
					this.pmDamage.SendEvent("toHit");
					this.pm.SendEvent("toHit");
					this.animator.SetIntegerReflected("knockBackInt", 0);
					this.animator.SetTriggerReflected("knockBackTrigger");
					base.Invoke("ResetHit", 1f);
				}
				if (LocalPlayer.FpCharacter.Sitting || LocalPlayer.AnimControl.onRope || LocalPlayer.AnimControl.cliffClimb || LocalPlayer.AnimControl.onRaft)
				{
					LocalPlayer.HitReactions.enableHitState();
				}
			}
			switch (type)
			{
			case PlayerStats.DamageType.Physical:
				this.Sfx.PlayHurtSound();
				if (this.IsBloody)
				{
					this.BloodInfection.TryGetInfected();
				}
				break;
			case PlayerStats.DamageType.Poison:
				this.Sfx.PlayEatPoison();
				this.FoodPoisoning.TryGetInfected();
				break;
			}
			if (Cheats.GodMode)
			{
				this.Health = 100f;
			}
			base.Invoke("ShowFindHealth", 15f);
			this.CheckDeath();
		}
		else if (BoltNetwork.isRunning && PlayerRespawnMP.IsKillable())
		{
			PlayerRespawnMP.KillPlayer();
		}
	}

	public void HitShark(int damage)
	{
		this.Health -= (float)damage;
		BleedBehavior.BloodAmount += Mathf.Clamp01(3f * (float)damage / this.Health) * 0.9f;
		BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.5f);
		if (this.Health > 0f)
		{
			this.pmDamage.SendEvent("toHit");
			this.pm.SendEvent("toHit");
			this.animator.SetIntegerReflected("knockBackInt", 0);
			this.animator.SetTriggerReflected("knockBackTrigger");
			base.Invoke("ResetHit", 1f);
			this.Sfx.PlayHurtSound();
		}
		if (BoltNetwork.isRunning)
		{
			this.CheckDeath();
		}
		else if (this.Health < 1f)
		{
			base.Invoke("KillMeFast", 7f);
			this.pm.SendEvent("toDeath");
			LocalPlayer.CamRotator.enabled = false;
			LocalPlayer.MainRotator.enabled = false;
			this.Player.Close();
			Scene.HudGui.DropButton.SetActive(false);
			LocalPlayer.Inventory.enabled = false;
			LocalPlayer.FpCharacter.enabled = false;
			LocalPlayer.MainRotator.enabled = false;
			LocalPlayer.FpCharacter.resetPhysicMaterial();
			LocalPlayer.AnimControl.controller.velocity = Vector3.zero;
			LocalPlayer.Create.Grabber.SendMessage("ExitMessage", SendMessageOptions.DontRequireReceiver);
			LocalPlayer.Create.Grabber.gameObject.SetActive(false);
			if (BoltNetwork.isRunning)
			{
				this.animator.SetBoolReflected("injuredBool", true);
				this.animator.SetBoolReflected("deathBool", true);
				base.Invoke("resetInjuredBool", 1f);
			}
			this.animator.SetBoolReflected("deathBool", true);
			this.camFollow.followAnim = true;
			BleedBehavior.BloodAmount += Mathf.Clamp01(3f * (float)damage / this.Health) * 0.9f;
			BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.5f);
			base.Invoke("ResetHit", 1f);
		}
	}

	private void CheckDeath()
	{
		if (Cheats.GodMode)
		{
			return;
		}
		if (this.Health <= 0f && !this.Dead)
		{
			this.Dead = true;
			this.Player.enabled = false;
			this.FallDownDead();
		}
	}

	private void FallDownDead()
	{
		float time = 4f;
		if (LocalPlayer.AnimControl.swimming)
		{
			time = 7f;
		}
		this.Player.Close();
		Scene.SceneTracker.DisableMusic();
		Scene.HudGui.DropButton.SetActive(false);
		LocalPlayer.Inventory.enabled = false;
		if (LocalPlayer.AnimControl.carry)
		{
			LocalPlayer.AnimControl.DropBody();
		}
		else if (LocalPlayer.Inventory.Logs.HasLogs)
		{
			LocalPlayer.Inventory.Logs.PutDown(false, true, false);
			LocalPlayer.Inventory.Logs.PutDown(false, true, false);
		}
		LocalPlayer.FpCharacter.enabled = false;
		LocalPlayer.MainRotator.enabled = false;
		LocalPlayer.FpCharacter.resetPhysicMaterial();
		LocalPlayer.AnimControl.controller.velocity = Vector3.zero;
		LocalPlayer.WaterViz.AudioOff();
		LocalPlayer.Create.Grabber.SendMessage("ExitMessage");
		LocalPlayer.Create.Grabber.gameObject.SetActive(false);
		this.pmDamage.SendEvent("toDeath");
		this.pm.SendEvent("toDeath");
		if (BoltNetwork.isRunning)
		{
			this.animator.SetBoolReflected("injuredBool", true);
			this.animator.SetBoolReflected("deathBool", true);
			base.Invoke("resetInjuredBool", 0.5f);
			base.Invoke("disablePlayerControl", 1f);
		}
		else
		{
			this.animator.SetBoolReflected("deathBool", true);
			this.animator.SetTriggerReflected("deathTrigger");
		}
		this.camFollow.followAnim = true;
		base.Invoke("BlackScreen", time);
		float num = 50f;
		BleedBehavior.BloodAmount += Mathf.Clamp01(3f * num / this.Health) * 0.9f;
		BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.5f);
		base.Invoke("ResetHit", 1f);
	}

	private void disablePlayerControl()
	{
		LocalPlayer.FpCharacter.enabled = false;
		LocalPlayer.MainRotator.enabled = false;
		LocalPlayer.CamRotator.enabled = false;
		LocalPlayer.CamFollowHead.followAnim = true;
	}

	private void resetInjuredBool()
	{
		this.animator.SetBoolReflected("deathBool", false);
	}

	private void KnockOut()
	{
		BleedBehavior.BloodAmount += 1f;
		BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.5f);
		this.pmDamage.SendEvent("toHit");
		this.pm.SendEvent("toHit");
		this.animator.SetTriggerReflected("knockBackTrigger");
		base.Invoke("BlackScreen", 1f);
	}

	private void CutSceneBlack()
	{
		Scene.Cams.SleepCam.SetActive(true);
		base.Invoke("CutSceneWake", 2f);
	}

	private void CutSceneWake()
	{
		Scene.Cams.SleepCam.SetActive(false);
		base.Invoke("CutSceneBlackToMorning", 8f);
	}

	private void CutSceneBlackToMorning()
	{
		Scene.Cams.SleepCam.SetActive(true);
		base.StartCoroutine(this.WakeFromKnockOut(false, YieldPresets.WaitFourSeconds));
	}

	private Transform getClosestDragMarker()
	{
		float num = float.PositiveInfinity;
		Transform result = null;
		foreach (Transform current in Scene.SceneTracker.dragMarkers)
		{
			float sqrMagnitude = (current.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = current;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	[DebuggerHidden]
	public IEnumerator dragAwayCutScene()
	{
		PlayerStats.<dragAwayCutScene>c__Iterator19F <dragAwayCutScene>c__Iterator19F = new PlayerStats.<dragAwayCutScene>c__Iterator19F();
		<dragAwayCutScene>c__Iterator19F.<>f__this = this;
		return <dragAwayCutScene>c__Iterator19F;
	}

	[DebuggerHidden]
	public IEnumerator hangingInCaveCutScene()
	{
		PlayerStats.<hangingInCaveCutScene>c__Iterator1A0 <hangingInCaveCutScene>c__Iterator1A = new PlayerStats.<hangingInCaveCutScene>c__Iterator1A0();
		<hangingInCaveCutScene>c__Iterator1A.<>f__this = this;
		return <hangingInCaveCutScene>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator releaseFromHanging()
	{
		PlayerStats.<releaseFromHanging>c__Iterator1A1 <releaseFromHanging>c__Iterator1A = new PlayerStats.<releaseFromHanging>c__Iterator1A1();
		<releaseFromHanging>c__Iterator1A.<>f__this = this;
		return <releaseFromHanging>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator forcePlayerPosToMarker(Transform m)
	{
		PlayerStats.<forcePlayerPosToMarker>c__Iterator1A2 <forcePlayerPosToMarker>c__Iterator1A = new PlayerStats.<forcePlayerPosToMarker>c__Iterator1A2();
		<forcePlayerPosToMarker>c__Iterator1A.<>f__this = this;
		return <forcePlayerPosToMarker>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator forcePlayerPosToMutant(Transform m)
	{
		PlayerStats.<forcePlayerPosToMutant>c__Iterator1A3 <forcePlayerPosToMutant>c__Iterator1A = new PlayerStats.<forcePlayerPosToMutant>c__Iterator1A3();
		<forcePlayerPosToMutant>c__Iterator1A.m = m;
		<forcePlayerPosToMutant>c__Iterator1A.<$>m = m;
		return <forcePlayerPosToMutant>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator lockLayersHanging()
	{
		return new PlayerStats.<lockLayersHanging>c__Iterator1A4();
	}

	public void setupFirstDayConditions()
	{
		this.Health = 28f;
		this.Energy = 11f;
		this.Fullness = 0f;
		this.Starvation = 0f;
		this.StarvationCurrentDuration = this.StarvationSettings.Duration * 2f;
		Scene.HudGui.StomachStarvation.gameObject.SetActive(true);
		this.Thirst = 0.35f;
		this.Atmos.FogMaxHeight = 400f;
		this.Atmos.TimeOfDay = 302f;
		Scene.Atmosphere.ForceSunRotationUpdate = true;
		if (this.BlackMan)
		{
			this.MyArms.sharedMaterial = this.BlackManBloody;
		}
		else
		{
			this.MyArms.sharedMaterial = this.Bloody;
		}
	}

	[DebuggerHidden]
	public IEnumerator WakeFromKnockOut(bool wasDead, WaitForSeconds timer)
	{
		PlayerStats.<WakeFromKnockOut>c__Iterator1A5 <WakeFromKnockOut>c__Iterator1A = new PlayerStats.<WakeFromKnockOut>c__Iterator1A5();
		<WakeFromKnockOut>c__Iterator1A.timer = timer;
		<WakeFromKnockOut>c__Iterator1A.wasDead = wasDead;
		<WakeFromKnockOut>c__Iterator1A.<$>timer = timer;
		<WakeFromKnockOut>c__Iterator1A.<$>wasDead = wasDead;
		<WakeFromKnockOut>c__Iterator1A.<>f__this = this;
		return <WakeFromKnockOut>c__Iterator1A;
	}

	public void CheckArmsStart()
	{
		this.pm.SendEvent("toCheckArms");
	}

	public void PlayWakeMusic()
	{
		this.WakeMusic.SetActive(true);
	}

	private void switchToLighter()
	{
		LocalPlayer.Inventory.Equip(LocalPlayer.AnimControl._lighterId, false);
		LocalPlayer.Inventory.StashLeftHand();
	}

	private void BlackScreen()
	{
		if (!BoltNetwork.isRunning)
		{
			this.camFollow.followAnim = false;
			Scene.HudGui.GuiCamC.enabled = false;
			Scene.Cams.SleepCam.SetActive(true);
			if (this.DeadTimes == 0 && !Clock.InCave && (!Scene.IsInSinkhole(LocalPlayer.Transform.position) || Terrain.activeTerrain.SampleHeight(LocalPlayer.Transform.position) < LocalPlayer.Transform.position.y))
			{
				base.StartCoroutine(this.dragAwayCutScene());
			}
			else
			{
				if (this.doneHangingScene)
				{
					base.StartCoroutine(this.WakeFromKnockOut(this.Dead, YieldPresets.WaitThreeSeconds));
				}
				base.Invoke("KillPlayer", (this.DeadTimes <= 0) ? 3f : 0.5f);
			}
		}
		else
		{
			base.Invoke("KillPlayer", 1f);
		}
	}

	public void CheckCollisionFromAbove(Collision coll)
	{
		FoundationHealth componentInParent = coll.collider.GetComponentInParent<FoundationHealth>();
		if (componentInParent && componentInParent.Collapsing)
		{
			this.Hit(1000, true, PlayerStats.DamageType.Physical);
		}
		else if (!coll.collider.GetComponentInParent<PrefabIdentifier>() && coll.collider.CompareTag("structure"))
		{
			this.Hit((int)Mathf.Clamp(coll.relativeVelocity.sqrMagnitude / 10f, 3f, 10f), false, PlayerStats.DamageType.Physical);
		}
	}

	private void hitFallDown()
	{
		this.pmDamage.SendEvent("toHit");
		this.pm.SendEvent("toDeath");
		LocalPlayer.CamRotator.enabled = false;
		this.animator.SetBoolReflected("deathBool", true);
		this.camFollow.followAnim = true;
		float num = 5f;
		BleedBehavior.BloodAmount += Mathf.Clamp01(3f * num / this.Health) * 0.9f;
		BleedBehavior.BloodReductionRatio = (Mathf.Clamp01(this.Health / 100f) + 0.1f) * ((!this.IsHealthInGreyZone) ? 1f : 0.5f);
		base.Invoke("ResetHit", 1f);
	}

	private void HitFire()
	{
		this.Hit(Mathf.RoundToInt(4f * this.Flammable), false, PlayerStats.DamageType.Fire);
	}

	private void ShowFindHealth()
	{
	}

	private void Infection()
	{
	}

	private void Bleed()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).tagHash == this.enterCaveHash)
		{
			return;
		}
		this.Health -= 1f;
		this.Hit(1, true, PlayerStats.DamageType.Physical);
	}

	private void HealedWounds()
	{
	}

	private void HealedInfections()
	{
		base.CancelInvoke("Infection");
	}

	private void HealedBleeding()
	{
		base.CancelInvoke("Bleed");
	}

	private void PoisonDamage()
	{
	}

	public void Poison()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		this.Hit(4, true, PlayerStats.DamageType.Physical);
		if (!this.Dead)
		{
			this.BloodInfection.GetInfected();
			base.InvokeRepeating("HitPoison", 4f, UnityEngine.Random.Range(6f, 8f));
			base.Invoke("disablePoison", UnityEngine.Random.Range(50f, 80f));
		}
	}

	private void disablePoison()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
	}

	private void HitPoison()
	{
		int min = 2;
		int max = 4;
		int damage = UnityEngine.Random.Range(min, max);
		this.Hit(damage, true, PlayerStats.DamageType.Physical);
	}

	private void Burn()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).tagHash == this.enterCaveHash)
		{
			return;
		}
		if (!this.IsLit && !this.Dead && LocalPlayer.WaterViz.ScreenCoverage < 0.5f)
		{
			LocalPlayer.AnimControl.onFire = true;
			this.IsLit = true;
			this.PlayerFlames.SetActive(true);
			base.InvokeRepeating("HitFire", 0f, 3f);
			base.Invoke("StopBurning", 10f);
		}
	}

	private void StopBurning()
	{
		LocalPlayer.AnimControl.onFire = false;
		this.PlayerFlames.SetActive(false);
		base.CancelInvoke("HitFire");
		this.IsLit = false;
		CapsuleCollider component = base.GetComponent<CapsuleCollider>();
		component.enabled = false;
		component.enabled = true;
	}

	private void ResetHit()
	{
	}

	private void resetExplosion()
	{
		this.isExplode = false;
	}

	public void KillMeFast()
	{
		if (!BoltNetwork.isRunning)
		{
			Scene.HudGui.GuiCamC.enabled = false;
			Scene.Cams.DeadCam.SetActive(true);
			LocalPlayer.PlayerDeadCam.SetActive(true);
			LocalPlayer.AnimControl.enabled = false;
			if (LocalPlayer.AnimControl.swimming)
			{
				LocalPlayer.Animator.CrossFade("fullBodyActions.swimDeath", 0f, 2, 1f);
			}
			else
			{
				LocalPlayer.Animator.CrossFade("fullBodyActions.deathFallForward", 0f, 2, 1f);
			}
			LocalPlayer.Animator.SetLayerWeightReflected(2, 1f);
			LocalPlayer.Animator.SetLayerWeightReflected(3, 0f);
			LocalPlayer.Animator.SetLayerWeightReflected(4, 0f);
			base.Invoke("GameOver", 6f);
		}
		else
		{
			this.KillPlayer();
		}
	}

	private void KillPlayer()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		base.CancelInvoke("HitFire");
		base.CancelInvoke("Bleed");
		base.CancelInvoke("Infection");
		base.CancelInvoke("CheckBlood");
		this.DeadTimes++;
		LocalPlayer.Animator.SetBoolReflected("deathBool", false);
		LocalPlayer.Create.CancelPlace();
		LocalPlayer.Tuts.CloseColdTut();
		LocalPlayer.Tuts.ColdDamageTutOff();
		LocalPlayer.Tuts.HideThirstyTut();
		this.OnSaveSlotSelectionCanceled();
		this.Player.TurnOffLastLight();
		this.Player.TurnOffLastUtility();
		if (this.Player.CurrentView == PlayerInventory.PlayerViews.Pause)
		{
			this.Player.TogglePauseMenu();
		}
		this.Player.Close();
		this.StopBurning();
		BleedBehavior.BloodReductionRatio = 3f;
		if (!BoltNetwork.isRunning)
		{
			if (this.DeadTimes > 1)
			{
				Scene.HudGui.GuiCamC.enabled = false;
				Scene.Cams.DeadCam.SetActive(true);
				LocalPlayer.PlayerDeadCam.SetActive(true);
				if (Cheats.PermaDeath)
				{
					PlayerPrefsFile.DeleteKey("__RESUME__", true);
					PlayerPrefsFile.DeleteKey("__RESUME__prev", true);
					PlayerPrefsFile.DeleteKey("info", true);
					PlayerPrefsFile.DeleteKey("thumb.png", true);
				}
				base.Invoke("GameOver", 6f);
			}
			else
			{
				TerrainCollider component = Terrain.activeTerrain.GetComponent<TerrainCollider>();
				SphereCollider component2 = base.transform.GetComponent<SphereCollider>();
				CapsuleCollider component3 = base.transform.GetComponent<CapsuleCollider>();
				if (component.enabled && component2.enabled)
				{
					Physics.IgnoreCollision(component, component2, true);
				}
				if (component.enabled && component3.enabled)
				{
					Physics.IgnoreCollision(component, component3, true);
				}
				Rigidbody arg_201_0 = LocalPlayer.GameObject.GetComponent<Rigidbody>();
				Vector3 position = this.DSpots.DeadSpots[UnityEngine.Random.Range(0, this.DSpots.DeadSpots.Length)].position;
				LocalPlayer.Transform.position = position;
				arg_201_0.position = position;
				UnityEngine.Debug.Log("dead spot: " + LocalPlayer.Transform.position);
				LocalPlayer.FpCharacter.enabled = true;
				LocalPlayer.Create.Grabber.gameObject.SetActive(true);
				Scene.Cams.CaveDeadCam.SetActive(true);
				this.camFollow.followAnim = false;
				this.hitReaction.Invoke("disableControllerFreeze", 4f);
				this.InACave();
				this.Player.StashLeftHand();
				this.Player.StashEquipedWeapon(false);
				base.StartCoroutine(this.WakeInCave());
				this.FrostScript.coverage = 0f;
				this.IsCold = false;
				this.Atmos.TimeOfDay = 1f;
				Scene.Atmosphere.ForceSunRotationUpdate = true;
				this.Starvation = 0f;
				this.StarvationCurrentDuration = this.StarvationSettings.Duration * 2f;
				Scene.HudGui.StomachStarvation.gameObject.SetActive(true);
				this.Thirst = 0.35f;
			}
		}
		else
		{
			PlayerRespawnMP.enableRespawnTimer();
			if (this.DeadTimes > 1)
			{
				PlayerRespawnMP.KillPlayer();
			}
		}
	}

	public void HealedMp()
	{
		UnityEngine.Debug.Log("HealedMp");
		this.Dead = false;
		this.DeadTimes = 0;
		this.Health = (float)(this.GreyZoneThreshold + 1);
		LocalPlayer.CamFollowHead.followAnim = true;
		this.animator.SetBoolReflected("injuredBool", false);
		LocalPlayer.Sfx.PlayStaminaBreath();
		LocalPlayer.Create.Grabber.gameObject.SetActive(true);
		LocalPlayer.CamFollowHead.followAnim = true;
		base.StartCoroutine("resetPlayerFromHeal");
	}

	[DebuggerHidden]
	private IEnumerator resetPlayerFromHeal()
	{
		PlayerStats.<resetPlayerFromHeal>c__Iterator1A6 <resetPlayerFromHeal>c__Iterator1A = new PlayerStats.<resetPlayerFromHeal>c__Iterator1A6();
		<resetPlayerFromHeal>c__Iterator1A.<>f__this = this;
		return <resetPlayerFromHeal>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator WakeInCave()
	{
		PlayerStats.<WakeInCave>c__Iterator1A7 <WakeInCave>c__Iterator1A = new PlayerStats.<WakeInCave>c__Iterator1A7();
		<WakeInCave>c__Iterator1A.<>f__this = this;
		return <WakeInCave>c__Iterator1A;
	}

	private void GameOver()
	{
		Scene.Cams.DeadCam.SetActive(false);
		LocalPlayer.PlayerDeadCam.SetActive(false);
		Application.LoadLevel("TitleScene");
	}

	private void GetTired()
	{
		if (this.Player.Logs.Amount == 1)
		{
			this.LogWeight = 0.15f;
		}
		else if (this.Player.Logs.Amount == 2)
		{
			this.LogWeight = 0.25f;
		}
		else
		{
			this.LogWeight = 0f;
		}
		this.Energy -= this.LogWeight;
		if (this.Fullness < 0.2f)
		{
			this.Energy -= 0.5f;
		}
		if (this.IsCold)
		{
			this.Energy -= 0.1f + 0.1f * (1f - this.ColdArmor);
		}
		else
		{
			this.Energy -= 0.1f;
		}
	}

	private void CallBird()
	{
		this.pm.SendEvent("toOnHand");
	}

	private void EnableMpCaveMutants()
	{
		this.mutantControl.SendMessage("enableMpCaveMutants");
	}

	public void IgnoreCollisionWithTerrain(bool onoff)
	{
		TerrainCollider terrainCollider = null;
		if (Terrain.activeTerrain)
		{
			terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
		}
		SphereCollider component = base.transform.GetComponent<SphereCollider>();
		CapsuleCollider component2 = base.transform.GetComponent<CapsuleCollider>();
		if (!terrainCollider || !component || !component2)
		{
			return;
		}
		if (component.enabled)
		{
			Physics.IgnoreCollision(terrainCollider, component, onoff);
		}
		if (component2.enabled)
		{
			Physics.IgnoreCollision(terrainCollider, component2, onoff);
		}
	}

	public void InACave()
	{
		Scene.Clock.IsCave();
		this.Atmos.InCave();
		if (this.Ocean)
		{
			this.Ocean.SetActive(false);
		}
		this.SetInCave(true);
		if (!this.sceneInfo.allPlayersInCave.Contains(base.gameObject))
		{
			this.sceneInfo.allPlayersInCave.Add(base.gameObject);
		}
		if (BoltNetwork.isServer)
		{
			if (this.mutantControl.gameObject.activeSelf)
			{
				this.EnableMpCaveMutants();
			}
			else
			{
				base.Invoke("EnableMpCaveMutants", 10f);
			}
		}
		else if (this.mutantControl)
		{
			this.mutantControl.StartCoroutine("updateCaveSpawns");
			base.Invoke("doRemoveWorldMutants", 30f);
			this.delayedMutantSpawnCheck = true;
		}
		this.IgnoreCollisionWithTerrain(true);
		CaveTriggers.CheckPlayersInCave();
	}

	private void doRemoveWorldMutants()
	{
		Scene.MutantControler.StartCoroutine("removeWorldMutants");
		this.delayedMutantSpawnCheck = false;
	}

	public void NotInACave()
	{
		Scene.Clock.IsNotCave();
		if (this.Ocean)
		{
			this.Ocean.SetActive(true);
		}
		this.Atmos.NotInCave();
		this.SetInCave(false);
		if (this.sceneInfo.allPlayersInCave.Contains(base.gameObject))
		{
			this.sceneInfo.allPlayersInCave.Remove(base.gameObject);
		}
		if (BoltNetwork.isServer)
		{
			if (this.sceneInfo.allPlayersInCave.Count == 0)
			{
				this.mutantControl.disableMpCaveMutants();
			}
		}
		else if (this.mutantControl)
		{
			if (this.delayedMutantSpawnCheck)
			{
				this.mutantControl.StartCoroutine("removeCaveMutants");
				base.CancelInvoke("doRemoveWorldMutants");
			}
			else
			{
				this.mutantControl.startSetupFamilies();
			}
		}
		this.IgnoreCollisionWithTerrain(false);
	}

	private void Life()
	{
		if (Scene.WeatherSystem.Raining && !this.Warm && Clock.Dark)
		{
			this.IsCold = true;
		}
		if (this.Warm)
		{
			this.Tuts.CloseColdTut();
			this.animator.SetFloatReflected("coldFloat", 0f);
			if (this.FrostScript.coverage <= 0f && this.IsCold)
			{
				this.IsCold = false;
			}
		}
		if (this.IsCold || this.IsInNorthColdArea())
		{
			if (this.IsCold && !this.Warm)
			{
				this.Tuts.ColdTut();
			}
			this.animator.SetFloatReflected("coldFloat", 1f);
		}
		if (!Clock.Dark && !Scene.WeatherSystem.Raining && !Clock.InCave && !this.IsInNorthColdArea())
		{
			if (!this.SunWarmth)
			{
				base.Invoke("WarmDay", 10f);
			}
		}
		else
		{
			this.SunWarmth = false;
		}
		this.Skills.CalcSkills();
		this.FoodPoisoning.TryAutoHeal();
		this.BloodInfection.TryAutoHeal();
	}

	private void WarmDay()
	{
		this.SunWarmth = true;
	}

	private void Infected()
	{
	}

	private void GetHealth()
	{
		base.SendMessage("PlayWhoosh");
		this.Health = 60f;
		this.CheckStats();
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
	}

	public void Drink()
	{
		this.Energy += 80f;
		this.Hunger -= 100;
		base.SendMessage("PlayDrink");
		this.CheckStats();
	}

	public void DrinkBooze()
	{
		this.Energy += 80f;
		base.SendMessage("PlayDrink");
		this.CheckStats();
	}

	private void DrinkLake()
	{
		base.SendMessage("PlayDrink");
		this.CheckStats();
	}

	public void AteChocBar()
	{
		this.Fullness += 0.2f;
		this.Health += 20f;
		this.Energy += 80f;
		base.SendMessage("PlayEat");
		BleedBehavior.BloodReductionRatio = this.Health / 100f * 1.25f;
	}

	public void SitDown()
	{
		this.Sitted = true;
	}

	public void StandUp()
	{
		this.Sitted = false;
	}

	private void AteMealRabbit()
	{
		this.Fullness += 0.9f;
		this.Health += 20f;
		this.Energy += 80f;
		base.SendMessage("PlayEatMeat");
		BleedBehavior.BloodReductionRatio = this.Health / 100f * 1.25f;
	}

	private void AteMushroomLibertyCap()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Health += 5f;
		this.Energy += 4f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
	}

	private void AteMushroomChant()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Health += 5f;
		this.Energy += 4f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
	}

	private void AteMushroomDeer()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Health += 4f;
		this.Energy += 3f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
	}

	private void AteMushroomAman()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Energy += 1f;
		base.SendMessage("PlayEat");
		this.PoisonMe();
	}

	private void AteMushroomPuffMush()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Health += 2f;
		this.Energy += 2f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
	}

	private void AteMushroomJack()
	{
		this.Tuts.ShowMushroomPage();
		this.Fullness += 0.02f;
		this.Health += 3f;
		this.Energy += 3f;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
	}

	private void AteBlueBerry()
	{
		this.Fullness += 0.02f;
		this.Health += 1f;
		this.Energy += 4f;
		base.SendMessage("PlayEat");
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
	}

	private void AteTwinBerry()
	{
		this.Fullness += 0.02f;
		base.Invoke("HitFood", 1.5f);
	}

	public void HitFood()
	{
		this.Hit(2, true, PlayerStats.DamageType.Poison);
	}

	public void HitFoodDelayed(int damage)
	{
		base.StartCoroutine(this.HitFoodRoutine(damage));
	}

	[DebuggerHidden]
	private IEnumerator HitFoodRoutine(int damage)
	{
		PlayerStats.<HitFoodRoutine>c__Iterator1A8 <HitFoodRoutine>c__Iterator1A = new PlayerStats.<HitFoodRoutine>c__Iterator1A8();
		<HitFoodRoutine>c__Iterator1A.damage = damage;
		<HitFoodRoutine>c__Iterator1A.<$>damage = damage;
		<HitFoodRoutine>c__Iterator1A.<>f__this = this;
		return <HitFoodRoutine>c__Iterator1A;
	}

	private void AtePlaneFood()
	{
		this.Fullness += 0.45f * this.FoodPoisoning.EffectRatio;
		this.Health += 5f * this.FoodPoisoning.EffectRatio;
		this.Energy += 30f * this.FoodPoisoning.EffectRatio;
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEat");
		this.Sanity.OnAteFreshFood();
	}

	public void AteFreshMeat(bool isLimb, float size)
	{
		if (!isLimb)
		{
			this.Fullness += 0.8f * this.FoodPoisoning.EffectRatio * size;
			this.Health += 20f * this.FoodPoisoning.EffectRatio * size;
			this.Energy += 80f * this.FoodPoisoning.EffectRatio * size;
			BleedBehavior.BloodReductionRatio = this.Health / 100f * 1.25f;
			this.Sanity.OnAteFreshFood();
		}
		else
		{
			this.Fullness += 0.4f * this.FoodPoisoning.EffectRatio;
			this.Health += 20f * this.FoodPoisoning.EffectRatio;
			this.Energy += 80f * this.FoodPoisoning.EffectRatio;
			base.Invoke("PoisonMe", 2f);
			BleedBehavior.BloodReductionRatio = this.Health / 100f;
			this.Sanity.OnCannibalism();
		}
		this.PhysicalStrength.OnAteFood();
		base.SendMessage("PlayEatMeat");
	}

	public void AteEdibleMeat(bool isLimb, float size)
	{
		if (!isLimb)
		{
			this.Fullness += 0.5f * this.FoodPoisoning.EffectRatio * size;
			this.Health = this.Health;
			this.Energy += 40f * this.FoodPoisoning.EffectRatio * size;
		}
		else
		{
			this.Fullness += 0.25f * this.FoodPoisoning.EffectRatio;
			this.Health += 0f * this.FoodPoisoning.EffectRatio;
			this.Energy += 40f * this.FoodPoisoning.EffectRatio;
			base.Invoke("PoisonMe", 2f);
			this.Sanity.OnCannibalism();
		}
		this.PhysicalStrength.OnAteFood();
		BleedBehavior.BloodReductionRatio = this.Health / 100f;
		base.SendMessage("PlayEatMeat");
	}

	public void AteSpoiltMeat(bool isLimb, float size)
	{
		if (!isLimb)
		{
			this.Fullness += 0.35f * this.FoodPoisoning.EffectRatio * size;
			this.Health += 0f * this.FoodPoisoning.EffectRatio * size;
			this.Energy += 20f * this.FoodPoisoning.EffectRatio * size;
		}
		else
		{
			this.Fullness += 0.175f * this.FoodPoisoning.EffectRatio;
			this.Health += 0f * this.FoodPoisoning.EffectRatio;
			this.Energy += 20f * this.FoodPoisoning.EffectRatio;
			this.Sanity.OnCannibalism();
		}
		this.PhysicalStrength.OnAteFood();
		BleedBehavior.BloodReductionRatio = this.Health / 100f * 0.75f;
		base.SendMessage("PlayEatMeat");
		base.Invoke("PoisonMe", 2f);
	}

	public void AteBurnt(bool isLimb, float size)
	{
		if (!isLimb)
		{
			this.Fullness += 0.15f * this.FoodPoisoning.EffectRatio * size;
			this.Health += 0f * this.FoodPoisoning.EffectRatio * size;
			this.Energy += 10f * this.FoodPoisoning.EffectRatio * size;
		}
		else
		{
			this.Fullness += 0.075f * this.FoodPoisoning.EffectRatio;
			this.Health += 0f * this.FoodPoisoning.EffectRatio;
			this.Energy += 10f * this.FoodPoisoning.EffectRatio;
			base.Invoke("PoisonMe", 2f);
			this.Sanity.OnCannibalism();
		}
		this.PhysicalStrength.OnAteFood();
		BleedBehavior.BloodReductionRatio = this.Health / 100f * 0.5f;
	}

	private void SpiderBite()
	{
		this.PoisonMe();
	}

	public void PoisonMe()
	{
		this.Hit(2, true, PlayerStats.DamageType.Physical);
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	[DebuggerHidden]
	private IEnumerator ResetStamina()
	{
		PlayerStats.<ResetStamina>c__Iterator1A9 <ResetStamina>c__Iterator1A = new PlayerStats.<ResetStamina>c__Iterator1A9();
		<ResetStamina>c__Iterator1A.<>f__this = this;
		return <ResetStamina>c__Iterator1A;
	}

	public void UseRebreather(bool onoff)
	{
		this.AirBreathing.UseRebreather = onoff;
		this.AirBreathing.RebreatherIsEquipped = onoff;
		if (this.AirBreathing.UseRebreather)
		{
			if (this.AirBreathing.CurrentRebreatherAir < this.AirBreathing.CurrentLungAir)
			{
				this.AirBreathing.CurrentRebreatherAir = this.AirBreathing.CurrentLungAir;
			}
		}
		else if (this.AirBreathing.CurrentLungAir < this.AirBreathing.CurrentRebreatherAir)
		{
			this.AirBreathing.CurrentLungAir = Mathf.Min(this.AirBreathing.CurrentRebreatherAir, this.AirBreathing.MaxLungAirCapacity);
		}
	}

	public void equipCutSceneAxe()
	{
		LocalPlayer.Inventory.AddItem(LocalPlayer.AnimControl._planeAxeId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
		LocalPlayer.Inventory.Equip(LocalPlayer.AnimControl._planeAxeId, false);
	}
}
