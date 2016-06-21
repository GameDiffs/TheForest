using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	public class GameMode_Horde : EntityBehaviour<IGameModeState>
	{
		private float _startTime;

		private int _lastTime = -1;

		private void Awake()
		{
			Scene.WorkScheduler.gameObject.SetActive(true);
			Scene.PlaneCrash.ShowCrash = false;
			if (Scene.TriggerCutScene)
			{
				Scene.TriggerCutScene.StopSounds();
			}
			UnityEngine.Object.Destroy(Scene.TriggerCutScene.transform.parent.gameObject);
			Scene.MutantControler.hordeModeActive = true;
			Scene.MutantControler.hordeModePaused = true;
			this.StartingSoonMessage();
			if (!BoltNetwork.isRunning)
			{
				base.Invoke("Phase1", 2f);
			}
			base.InvokeRepeating("UpdateTimer", 1f, 1f);
		}

		private void StartingSoonMessage()
		{
			Scene.HudGui.ModTimer._title.text = "Game starting soon...";
			Scene.HudGui.ModTimer._root.SetActive(true);
			Scene.HudGui.ModTimer._title.gameObject.SetActive(true);
			Scene.HudGui.ModTimer._subtitle.gameObject.SetActive(false);
			Scene.HudGui.ModTimer._timer.gameObject.SetActive(false);
		}

		private void Phase1()
		{
			Debug.Log("HordeMode: Phase 1 (300s)");
			Scene.HudGui.GuiCam.SetActive(false);
			for (int i = 0; i < ItemDatabase.Items.Length; i++)
			{
				Item item = ItemDatabase.Items[i];
				try
				{
					if (item._maxAmount >= 0)
					{
						LocalPlayer.Inventory.AddItem(item._id, 100000, true, false, (WeaponStatUpgrade.Types)(-2));
					}
				}
				catch (Exception var_2_54)
				{
				}
			}
			Scene.HudGui.CheckHudState();
			Cheats.Creative = true;
			LocalPlayer.Stats.ThirstSettings.StartDay = 2147483647;
			LocalPlayer.Stats.StarvationSettings.StartDay = 2147483647;
			LocalPlayer.Stats.FrostDamageSettings.StartDay = 2147483647;
			Scene.HudGui.ModTimer._title.text = "Build defence";
			Scene.HudGui.ModTimer._subtitle.text = "Use infinite resources to prepare for coming attacks";
			Scene.HudGui.ModTimer._root.SetActive(true);
			Scene.HudGui.ModTimer._title.gameObject.SetActive(true);
			Scene.HudGui.ModTimer._subtitle.gameObject.SetActive(true);
			Scene.HudGui.ModTimer._timer.gameObject.SetActive(true);
			if (!BoltNetwork.isClient)
			{
				this._startTime = Time.time + 300f;
				if (BoltNetwork.isServer)
				{
					base.state.Timer = 300f;
					base.state.Phase = 1;
				}
				base.Invoke("Phase2", 300f);
			}
			else
			{
				this._startTime = base.state.Timer;
			}
		}

		private void Phase2()
		{
			Debug.Log("HordeMode: Phase 2 (10s)");
			Cheats.Creative = false;
			LocalPlayer.Stats.ThirstSettings.StartDay = 0;
			LocalPlayer.Stats.StarvationSettings.StartDay = 0;
			LocalPlayer.Stats.FrostDamageSettings.StartDay = 0;
			Scene.HudGui.ModTimer._title.text = "Prepare for battle";
			Scene.HudGui.ModTimer._root.SetActive(true);
			Scene.HudGui.ModTimer._timer.gameObject.SetActive(false);
			Scene.HudGui.ModTimer._subtitle.gameObject.SetActive(false);
			if (!BoltNetwork.isClient)
			{
				if (BoltNetwork.isServer)
				{
					base.state.Phase = 2;
				}
				base.Invoke("Phase3", 10f);
			}
		}

		private void Phase3()
		{
			Debug.Log("HordeMode: Phase 3");
			Scene.HudGui.ModTimer._root.SetActive(true);
			Scene.HudGui.ModTimer._title.text = "Horde is coming";
			Scene.HudGui.ModTimer._subtitle.text = "Survive as long as you can from enemy attacks";
			Scene.HudGui.ModTimer._root.SetActive(true);
			Scene.HudGui.ModTimer._title.gameObject.SetActive(true);
			Scene.HudGui.ModTimer._subtitle.gameObject.SetActive(true);
			Scene.HudGui.ModTimer._timer.gameObject.SetActive(true);
			if (!BoltNetwork.isClient)
			{
				this._startTime = Time.time;
				Scene.MutantControler.startHordeSpawnDelay = 0;
				Scene.MutantControler.nextWaveSpawnDelay = 30;
				Scene.MutantControler.hordeModePaused = false;
				if (BoltNetwork.isServer)
				{
					base.state.Timer = 0f;
					base.state.Phase = 3;
				}
			}
			else
			{
				this._startTime = base.state.Timer;
			}
		}

		private void UpdateTimer()
		{
			int num = BoltNetwork.isClient ? ((int)base.state.Timer) : Mathf.Abs((int)(Time.time - this._startTime));
			if (this._lastTime != num)
			{
				this._lastTime = num;
				Scene.HudGui.ModTimer._timer.text = this._lastTime.ToString();
				if (BoltNetwork.isServer)
				{
					base.state.Timer = (float)num;
				}
			}
		}

		public override void Attached()
		{
			if (BoltNetwork.isClient)
			{
				TitleScreen.StartGameSetup.Game = TitleScreen.GameSetup.GameModes.Horde;
				base.state.AddCallback("Phase", new PropertyCallbackSimple(this.SwitchPhase));
				base.state.AddCallback("Timer", new PropertyCallbackSimple(this.SetTime));
			}
			else
			{
				base.state.Game = 1;
				base.Invoke("Phase1", 2f);
			}
		}

		private void SwitchPhase()
		{
			switch (base.state.Phase)
			{
			case 1:
				this.Phase1();
				break;
			case 2:
				this.Phase2();
				break;
			case 3:
				this.Phase3();
				break;
			}
		}

		private void SetTime()
		{
			this._startTime = base.state.Timer;
		}
	}
}
