using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class Clock : MonoBehaviour
{
	[SerializeThis]
	public static int Day;

	public static int Temp = 30;

	public static bool Dark;

	[SerializeThis]
	public static bool InCave;

	public static bool planecrash;

	public bool dayOverride;

	[SerializeThis]
	public float ElapsedGameTime;

	public Transform Ocean;

	public Light Sun;

	public Light Moon;

	private bool updateMutantsCheck;

	public GameObject RainbowA;

	public GameObject LightningFlashGroup;

	public GameObject NightTimeSfx;

	public GameObject DayTimeSfx;

	public bool TurnRainBowOff;

	public Color RainbowIntensity;

	private bool DaySwitch;

	public TheForestAtmosphere Atmos;

	public string ThunderEventPath;

	private void Awake()
	{
		if (BoltNetwork.isClient)
		{
			this.Atmos.TimeOfDay = 302f;
		}
		Clock.Temp = 30;
		Clock.Dark = false;
		Clock.InCave = false;
		Clock.Temp = UnityEngine.Random.Range(-10, 1);
		Clock.Dark = false;
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		Clock.<Start>c__Iterator15D <Start>c__Iterator15D = new Clock.<Start>c__Iterator15D();
		<Start>c__Iterator15D.<>f__this = this;
		return <Start>c__Iterator15D;
	}

	[DebuggerHidden]
	private IEnumerator OnDeserialized()
	{
		Clock.<OnDeserialized>c__Iterator15E <OnDeserialized>c__Iterator15E = new Clock.<OnDeserialized>c__Iterator15E();
		<OnDeserialized>c__Iterator15E.<>f__this = this;
		return <OnDeserialized>c__Iterator15E;
	}

	private void debugDay()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
	}

	private void Update()
	{
		if (this.Atmos.TimeOfDay > 88f && this.Atmos.TimeOfDay < 270f)
		{
			this.GoDark();
		}
		else
		{
			this.GoLight();
		}
		if (this.Atmos.Sleeping && !this.Atmos.SleepUntilMorning && Scene.Clock.ElapsedGameTime - LocalPlayer.Stats.NextSleepTime > 0.3f)
		{
			this.ClockWakeUp();
		}
		if (this.Atmos.TimeOfDay > 266f && this.Atmos.TimeOfDay < 270f && !this.DaySwitch)
		{
			this.DaySwitch = true;
			this.ChangeDay();
		}
		if (this.Atmos.TimeOfDay > 270f)
		{
			this.DaySwitch = false;
		}
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.ElapsedGameTime += Scene.Atmosphere.DeltaTimeOfDay;
		if (this.RainbowA.activeSelf && Clock.Dark)
		{
			this.RainbowA.SetActive(false);
		}
		if (this.RainbowA.activeSelf && this.TurnRainBowOff)
		{
			this.RainbowIntensity.a = this.RainbowIntensity.a - 0.1f * Time.deltaTime;
			this.RainbowA.GetComponent<Renderer>().material.SetColor("_TintColor", this.RainbowIntensity);
			if (this.RainbowIntensity.a <= 0f)
			{
				this.RainbowA.SetActive(false);
				this.TurnRainBowOff = false;
			}
		}
		if (this.RainbowA.activeSelf && !this.TurnRainBowOff && this.RainbowIntensity.a < this.Sun.intensity)
		{
			this.RainbowIntensity.a = this.RainbowIntensity.a + 0.1f * Time.deltaTime;
			this.RainbowA.GetComponent<Renderer>().material.SetColor("_TintColor", this.RainbowIntensity);
		}
		this.CopyPropertiesToNetwork();
	}

	private void resetUpdateMutantCheck()
	{
		this.updateMutantsCheck = false;
	}

	private void ClockWakeUp()
	{
		this.Atmos.SleepUntilMorning = false;
		LocalPlayer.GameObject.SendMessage("Wake");
	}

	private void GoDark()
	{
		if (!Clock.Dark)
		{
			Clock.Dark = true;
			Clock.Temp = UnityEngine.Random.Range(-20, 1);
			Scene.SceneTracker.Invoke("WentDark", 4f);
			this.NightTimeSfx.SetActive(true);
			this.DayTimeSfx.SetActive(false);
		}
	}

	private void GoLight()
	{
		if (Clock.Dark)
		{
			Clock.Dark = false;
			if (this.Atmos.SleepUntilMorning)
			{
				base.Invoke("ClockWakeUp", 0.15f);
			}
			Clock.Temp = UnityEngine.Random.Range(5, 30);
			Scene.SceneTracker.Invoke("WentLight", 4f);
			this.NightTimeSfx.SetActive(false);
			this.DayTimeSfx.SetActive(true);
		}
	}

	public void IsCave()
	{
		Clock.InCave = true;
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (Clock.Dark)
		{
			Clock.Dark = false;
		}
		else
		{
			Clock.Dark = true;
		}
	}

	public void IsNotCave()
	{
		Clock.InCave = false;
	}

	private void UpdatePlaneMoss()
	{
		Prefabs.Instance.PlaneMossMaterial.SetFloat("_MossSpread", Mathf.Clamp01((float)Clock.Day / 10f) * 0.7f + 0.3f);
	}

	private void ChangeDay()
	{
		this.UpdatePlaneMoss();
		if (BoltNetwork.isClient)
		{
			return;
		}
		Clock.Day++;
	}

	private void ShowDay()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
	}

	private void TurnDayTextOff()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
	}

	public void CopyPropertiesToNetwork()
	{
		if (BoltNetwork.isServer && CoopWeatherProxy.Instance)
		{
			IWeatherState state = CoopWeatherProxy.Instance.state;
			state.Temp = Clock.Temp;
			state.Day = Clock.Day;
			if (LocalPlayer.Inventory && LocalPlayer.Inventory.CurrentView >= PlayerInventory.PlayerViews.World)
			{
				state.TimeOfDay = this.Atmos.TimeOfDay;
			}
			state.ElapsedGameTime = this.ElapsedGameTime;
			state.Raining = Scene.WeatherSystem.Raining;
			state.RainLight = Scene.RainTypes.RainLight.activeInHierarchy;
			state.RainMedium = Scene.RainTypes.RainMedium.activeInHierarchy;
			state.RainHeavy = Scene.RainTypes.RainHeavy.activeInHierarchy;
			state.Rainbow = this.RainbowA.activeInHierarchy;
			state.RainbowIntensity = this.RainbowIntensity;
			state.NightTimeSfx = this.NightTimeSfx.activeInHierarchy;
			state.DayTimeSfx = this.DayTimeSfx.activeInHierarchy;
			state.Lightning = Scene.WeatherSystem.ShouldDoLighting;
		}
	}
}
