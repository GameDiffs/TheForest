using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TheForest.Utils;
using UnityEngine;

public class LoadSave : MonoBehaviour
{
	[Serializable]
	public class GameObjectList
	{
		public List<GameObject> _frameJobs;
	}

	public static bool ShouldLoad;

	public List<LoadSave.GameObjectList> _activateAfterLoading;

	private bool startedSequence;

	public static event Action OnGameStart
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LoadSave.OnGameStart = (Action)Delegate.Combine(LoadSave.OnGameStart, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LoadSave.OnGameStart = (Action)Delegate.Remove(LoadSave.OnGameStart, value);
		}
	}

	private void Awake()
	{
		UnityEngine.Random.seed = Convert.ToInt32(DateTime.UtcNow.ToUnixTimestamp());
		LevelSerializer.Progress += new Action<string, float>(this.LevelSerializer_Progress);
		if (Scene.PlaneCrash && Scene.PlaneCrash.gameObject.activeSelf && Scene.PlaneCrash.ShowCrash)
		{
			Scene.HudGui.Loading._cam.SetActive(true);
		}
		if (LevelLoader.Current)
		{
			Scene.HudGui.Loading._cam.SetActive(true);
			Scene.HudGui.Loading._message.SetActive(true);
			return;
		}
		if (LoadSave.ShouldLoad)
		{
			LoadSave.ShouldLoad = false;
			if (LevelSerializer.CanResume)
			{
				Scene.HudGui.Loading._cam.SetActive(true);
				LevelSerializer.Collect();
				LevelSerializer.Resume();
				return;
			}
		}
		if (Scene.HudGui.Loading._message)
		{
			Scene.HudGui.Loading._message.SetActive(false);
		}
		Time.timeScale = 1f;
		MainMenuAudio.FadeOut();
		UnityEngine.Debug.Log("****** Game Activation Sequence ******");
		base.StartCoroutine(this.Activation(true));
	}

	private void Start()
	{
		if (!LevelSerializer.IsDeserializing && !this.startedSequence)
		{
			base.StartCoroutine(this.Activation(true));
		}
	}

	private void OnDestroy()
	{
		LevelSerializer.Progress -= new Action<string, float>(this.LevelSerializer_Progress);
		LoadSave.OnGameStart = null;
	}

	private void LevelSerializer_Progress(string section, float alpha)
	{
		if (Mathf.Approximately(alpha, 1f) && section.Equals("Done"))
		{
			Time.timeScale = 0.1f;
			MainMenuAudio.FadeOut();
			base.StartCoroutine(this.Activation(true));
		}
	}

	[DebuggerHidden]
	public IEnumerator Activation(bool activate)
	{
		LoadSave.<Activation>c__Iterator1A6 <Activation>c__Iterator1A = new LoadSave.<Activation>c__Iterator1A6();
		<Activation>c__Iterator1A.activate = activate;
		<Activation>c__Iterator1A.<$>activate = activate;
		<Activation>c__Iterator1A.<>f__this = this;
		return <Activation>c__Iterator1A;
	}
}
