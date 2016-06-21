using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.World
{
	[DoNotSerializePublic]
	public class WeatherSystem : MonoBehaviour
	{
		public enum States
		{
			Idle,
			GrowingClouds,
			ReducingClouds,
			Raining
		}

		public enum RainTypes
		{
			None,
			Light,
			Medium,
			Heavy
		}

		public Material CloudOvercastMat;

		public string ThunderEventPath;

		[SerializeThis]
		public float LastRainTime;

		public float CloudSmoothTime = 20f;

		public int RainRollBase = 30;

		public int MaxRainDuration = 9;

		private int RainDice;

		private int RainDiceStop;

		private int RainStopRolls;

		private float CloudOvercastCurrentValue;

		private float CloudOvercastTargetValue;

		private float CloudOvercastVelocity;

		private float CloudOpacityScaleCurrentValue = 1f;

		private float CloudOpacityScaleTargetValue = 1f;

		private float CloudOpacityScaleVelocity;

		private WeatherSystem.States State;

		private WeatherSystem.RainTypes CurrentType;

		public bool Raining
		{
			get
			{
				return this.State == WeatherSystem.States.Raining;
			}
			set
			{
				this.State = ((!value) ? WeatherSystem.States.Idle : WeatherSystem.States.Raining);
			}
		}

		public float CloudCovergage
		{
			get
			{
				return this.CloudOvercastCurrentValue;
			}
		}

		public bool UsingSnow
		{
			get;
			set;
		}

		public bool ShouldDoLighting
		{
			get;
			private set;
		}

		private void Awake()
		{
			base.InvokeRepeating("RainChance", 150f, 60f);
			this.ResetClouds();
			if (BoltNetwork.isClient)
			{
				base.enabled = false;
			}
		}

		private void Update()
		{
			this.CheckInCave();
			if (this.State == WeatherSystem.States.GrowingClouds)
			{
				this.CloudOpacityScaleCurrentValue = Mathf.SmoothDamp(this.CloudOpacityScaleCurrentValue, this.CloudOpacityScaleTargetValue * 1.1f, ref this.CloudOpacityScaleVelocity, this.CloudSmoothTime * 3f / 4f);
				if (this.CloudOpacityScaleCurrentValue < this.CloudOpacityScaleTargetValue)
				{
					this.CloudOvercastMat.SetFloat("CloudOpacityScale", this.CloudOpacityScaleCurrentValue);
				}
				this.CloudOvercastCurrentValue = Mathf.SmoothDamp(this.CloudOvercastCurrentValue, this.CloudOvercastTargetValue * 1.1f, ref this.CloudOvercastVelocity, this.CloudSmoothTime);
				if (this.CloudOvercastCurrentValue < this.CloudOvercastTargetValue)
				{
					this.CloudOvercastMat.SetFloat("OvercastAmount", this.CloudOvercastCurrentValue);
				}
				else
				{
					this.CloudOpacityScaleVelocity = 0f;
					this.CloudOvercastVelocity = 0f;
					this.CloudOvercastMat.SetFloat("OvercastAmount", this.CloudOvercastTargetValue);
					this.DoRain(true);
				}
			}
			else if (this.State == WeatherSystem.States.ReducingClouds)
			{
				this.CloudOpacityScaleCurrentValue = Mathf.SmoothDamp(this.CloudOpacityScaleCurrentValue, this.CloudOpacityScaleTargetValue * 0.9f, ref this.CloudOpacityScaleVelocity, this.CloudSmoothTime * 3f / 4f);
				if (this.CloudOpacityScaleCurrentValue > this.CloudOpacityScaleTargetValue)
				{
					this.CloudOvercastMat.SetFloat("CloudOpacityScale", this.CloudOpacityScaleCurrentValue);
				}
				this.CloudOvercastCurrentValue = Mathf.SmoothDamp(this.CloudOvercastCurrentValue, (this.CloudOvercastTargetValue != 0f) ? (this.CloudOvercastTargetValue * 0.8f) : -0.2f, ref this.CloudOvercastVelocity, this.CloudSmoothTime);
				if (this.CloudOvercastCurrentValue > this.CloudOvercastTargetValue)
				{
					this.CloudOvercastMat.SetFloat("OvercastAmount", this.CloudOvercastCurrentValue);
				}
				else
				{
					this.CloudOpacityScaleVelocity = 0f;
					this.CloudOvercastVelocity = 0f;
					this.CloudOvercastMat.SetFloat("OvercastAmount", this.CloudOvercastTargetValue);
					this.State = WeatherSystem.States.Idle;
				}
			}
			else
			{
				this.CheckSnowArea();
			}
		}

		public void ForceRain(int rainDice)
		{
			if (BoltNetwork.isClient)
			{
				return;
			}
			this.RainDice = rainDice;
			this.RainDiceStop = ((rainDice >= 6) ? 2 : 1);
			this.TryRain();
		}

		private void RainChance()
		{
			if (BoltNetwork.isClient)
			{
				return;
			}
			if (this.State != WeatherSystem.States.GrowingClouds)
			{
				if (RainEffigy.RainAdd == 0)
				{
					this.RainDice = UnityEngine.Random.Range(1, this.RainRollBase * 4);
				}
				else if (RainEffigy.RainAdd == 1)
				{
					this.RainDice = UnityEngine.Random.Range(1, this.RainRollBase * 3);
				}
				else if (RainEffigy.RainAdd == 2)
				{
					this.RainDice = UnityEngine.Random.Range(1, this.RainRollBase * 2);
				}
				else if (RainEffigy.RainAdd == 3)
				{
					this.RainDice = UnityEngine.Random.Range(1, this.RainRollBase);
				}
				else
				{
					this.RainDice = UnityEngine.Random.Range(1, 16);
				}
				this.RainDiceStop = UnityEngine.Random.Range(1, 9);
				base.Invoke("TryRain", 5f);
			}
		}

		private void TryRain()
		{
			if (!this.Raining)
			{
				if (this.RainDice >= 2 && this.RainDice <= 6)
				{
					this.RainStopRolls = 0;
					this.State = WeatherSystem.States.GrowingClouds;
					if (this.RainDice == 2)
					{
						this.CloudOvercastTargetValue = UnityEngine.Random.Range(0.55f, 0.7f);
						this.CloudOpacityScaleTargetValue = UnityEngine.Random.Range(1f, 1.05f);
					}
					else if (this.RainDice == 3)
					{
						this.CloudOvercastTargetValue = UnityEngine.Random.Range(0.7f, 0.85f);
						this.CloudOpacityScaleTargetValue = UnityEngine.Random.Range(1.05f, 1.12f);
					}
					else if (this.RainDice == 4)
					{
						this.CloudOvercastTargetValue = UnityEngine.Random.Range(0.85f, 1f);
						this.CloudOpacityScaleTargetValue = UnityEngine.Random.Range(1.12f, 1.2f);
					}
					else
					{
						this.CloudOvercastTargetValue = UnityEngine.Random.Range(0.55f, 1f);
						this.CloudOpacityScaleTargetValue = UnityEngine.Random.Range(1f, 1.2f);
					}
				}
			}
			else if (this.RainDiceStop == 2 || this.RainStopRolls >= this.MaxRainDuration)
			{
				this.RainStopRolls = 0;
				if (!this.UsingSnow)
				{
					LocalPlayer.MudGreeble.gameObject.SetActive(true);
				}
				this.RainDiceStop = 1;
				this.State = WeatherSystem.States.ReducingClouds;
				LocalPlayer.Sfx.PlayAfterStorm();
				this.CloudOvercastTargetValue = 0f;
				this.CloudOpacityScaleTargetValue = UnityEngine.Random.Range(0.8f, 1f);
				this.AllOff();
				if (!Clock.Dark)
				{
					Scene.Clock.RainbowA.SetActive(true);
					base.Invoke("RainBowOff", 60f);
				}
				this.LastRainTime = Scene.Clock.ElapsedGameTime;
			}
			else
			{
				this.RainStopRolls++;
			}
		}

		private void DoRain(bool doLighting)
		{
			if (BoltNetwork.isClient)
			{
				return;
			}
			if (this.RainDice == 2)
			{
				LocalPlayer.GameObject.SendMessage("GotClean");
				LocalPlayer.GameObject.SendMessage("GotCleanReal");
				this.TurnOn(WeatherSystem.RainTypes.Light);
				this.LastRainTime = Scene.Clock.ElapsedGameTime;
				if (doLighting)
				{
					this.Lightning();
				}
			}
			else if (this.RainDice == 3)
			{
				LocalPlayer.GameObject.SendMessage("GotClean");
				LocalPlayer.GameObject.SendMessage("GotCleanReal");
				this.TurnOn(WeatherSystem.RainTypes.Medium);
				this.LastRainTime = Scene.Clock.ElapsedGameTime;
				if (doLighting)
				{
					this.Lightning();
				}
			}
			else if (this.RainDice == 4)
			{
				LocalPlayer.GameObject.SendMessage("GotClean");
				LocalPlayer.GameObject.SendMessage("GotCleanReal");
				this.TurnOn(WeatherSystem.RainTypes.Heavy);
				this.LastRainTime = Scene.Clock.ElapsedGameTime;
				if (doLighting)
				{
					this.Lightning();
				}
			}
			else
			{
				this.State = WeatherSystem.States.Idle;
			}
		}

		public void CheckSnowArea()
		{
			if (this.State == WeatherSystem.States.Raining)
			{
				if (this.UsingSnow != LocalPlayer.Stats.IsInNorthColdArea())
				{
					this.AllOff();
					this.TurnOn(this.CurrentType);
				}
			}
			else if (LocalPlayer.Transform && this.UsingSnow != LocalPlayer.Stats.IsInNorthColdArea())
			{
				this.AllOff();
			}
		}

		public void CheckInCave()
		{
			if (Clock.InCave)
			{
				if (Scene.RainTypes.CaveFilter.activeSelf)
				{
					Scene.RainTypes.CaveFilter.SetActive(false);
				}
			}
			else if (!Scene.RainTypes.CaveFilter.activeSelf)
			{
				Scene.RainTypes.CaveFilter.SetActive(true);
			}
		}

		private void Lightning()
		{
			this.ShouldDoLighting = true;
			if (BoltNetwork.isClient || this.UsingSnow)
			{
				return;
			}
			FMOD_StudioSystem.instance.PlayOneShot(this.ThunderEventPath, base.transform.position, null);
			if (!Clock.InCave)
			{
				Scene.Clock.LightningFlashGroup.SetActive(true);
			}
			base.Invoke("LightningOff", 0.5f);
		}

		public void AllOff()
		{
			this.CurrentType = WeatherSystem.RainTypes.None;
			Scene.RainTypes.RainLight.SetActive(false);
			Scene.RainTypes.RainMedium.SetActive(false);
			Scene.RainTypes.RainHeavy.SetActive(false);
			Scene.RainTypes.SnowLight.SetActive(false);
			Scene.RainTypes.SnowMedium.SetActive(false);
			Scene.RainTypes.SnowHeavy.SetActive(false);
			this.UsingSnow = (LocalPlayer.Stats && LocalPlayer.Stats.IsInNorthColdArea());
			if (this.UsingSnow)
			{
				Scene.RainFollowGO.SetActive(true);
				Scene.RainTypes.SnowConstant.SetActive(true);
			}
			else
			{
				Scene.RainFollowGO.SetActive(false);
				Scene.RainTypes.SnowConstant.SetActive(false);
			}
		}

		public void TurnOn(WeatherSystem.RainTypes type)
		{
			this.CurrentType = type;
			this.State = WeatherSystem.States.Raining;
			this.UsingSnow = (LocalPlayer.Stats && LocalPlayer.Stats.IsInNorthColdArea());
			Scene.RainFollowGO.SetActive(true);
			Scene.RainTypes.SnowConstant.SetActive(false);
			switch (type)
			{
			case WeatherSystem.RainTypes.Light:
				if (this.UsingSnow)
				{
					Scene.RainTypes.SnowLight.SetActive(true);
				}
				else
				{
					Scene.RainTypes.RainLight.SetActive(true);
				}
				break;
			case WeatherSystem.RainTypes.Medium:
				if (this.UsingSnow)
				{
					Scene.RainTypes.SnowMedium.SetActive(true);
				}
				else
				{
					Scene.RainTypes.RainMedium.SetActive(true);
				}
				break;
			case WeatherSystem.RainTypes.Heavy:
				if (this.UsingSnow)
				{
					Scene.RainTypes.SnowHeavy.SetActive(true);
				}
				else
				{
					Scene.RainTypes.RainHeavy.SetActive(true);
				}
				break;
			default:
				this.AllOff();
				break;
			}
		}

		private void LightningOff()
		{
			if (BoltNetwork.isClient)
			{
				return;
			}
			this.ShouldDoLighting = false;
			Scene.Clock.LightningFlashGroup.SetActive(false);
		}

		private void RainBowOff()
		{
			if (BoltNetwork.isClient)
			{
				return;
			}
			Scene.Clock.TurnRainBowOff = true;
		}

		private void ResetClouds()
		{
			if (this.CloudOvercastMat)
			{
				this.CloudOvercastMat.SetFloat("CloudOpacityScale", 1f);
				this.CloudOvercastMat.SetFloat("OvercastAmount", 0f);
			}
		}
	}
}
