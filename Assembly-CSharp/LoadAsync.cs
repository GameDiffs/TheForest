using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class LoadAsync : MonoBehaviour
{
	public GameObject Menu;

	public bool SavedGameLoader;

	public GameObject LoadSavedGameObject;

	public AsyncOperation async;

	public string _levelName = "ForestMain_v07";

	internal bool showGUI = true;

	public float progress
	{
		get
		{
			if (TitleScreen.StartGameSetup.Type != TitleScreen.GameSetup.InitTypes.Continue)
			{
				return (this.async == null) ? 0f : this.async.progress;
			}
			if (LevelSerializer.IsDeserializing)
			{
				return 0.5f;
			}
			return 1f;
		}
	}

	public bool isDone
	{
		get
		{
			if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue)
			{
				return !LevelSerializer.IsDeserializing;
			}
			return this.async != null && this.async.isDone;
		}
	}

	private void Start()
	{
		TheForest.Utils.Input.LockMouse();
		base.transform.parent = null;
		if (this.Menu)
		{
			UnityEngine.Object.Destroy(this.Menu);
		}
		base.StartCoroutine(this.LoadLevelWithProgress(this._levelName));
	}

	[DebuggerHidden]
	private IEnumerator LoadLevelWithProgress(string levelToLoad)
	{
		LoadAsync.<LoadLevelWithProgress>c__Iterator1AD <LoadLevelWithProgress>c__Iterator1AD = new LoadAsync.<LoadLevelWithProgress>c__Iterator1AD();
		<LoadLevelWithProgress>c__Iterator1AD.levelToLoad = levelToLoad;
		<LoadLevelWithProgress>c__Iterator1AD.<$>levelToLoad = levelToLoad;
		<LoadLevelWithProgress>c__Iterator1AD.<>f__this = this;
		return <LoadLevelWithProgress>c__Iterator1AD;
	}
}
