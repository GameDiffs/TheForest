using System;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
	private static MainMenuAudio Instance;

	private void Start()
	{
		MainMenuAudio.Instance = this;
	}

	private void OnDestroy()
	{
		MainMenuAudio.FadeOut();
		MainMenuAudio.Instance = null;
	}

	public static void FadeOut()
	{
		if (MainMenuAudio.Instance)
		{
			MainMenuAudio.Instance.GetComponent<FMOD_StudioEventEmitter>().Stop();
		}
	}

	public static void FadeIn()
	{
		if (MainMenuAudio.Instance)
		{
			MainMenuAudio.Instance.GetComponent<FMOD_StudioEventEmitter>().Play();
		}
	}
}
