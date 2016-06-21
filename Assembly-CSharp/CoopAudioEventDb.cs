using System;
using System.Collections.Generic;
using UnityEngine;

public class CoopAudioEventDb : ScriptableObject
{
	private static CoopAudioEventDb _instance;

	[SerializeField]
	public string[] EventList = new string[0];

	[HideInInspector]
	private Dictionary<string, int> EventToId = new Dictionary<string, int>();

	[HideInInspector]
	private Dictionary<int, string> IdToEvent = new Dictionary<int, string>();

	public static CoopAudioEventDb Instance
	{
		get
		{
			if (!CoopAudioEventDb._instance)
			{
				CoopAudioEventDb._instance = (Resources.Load("CoopAudioEventDb", typeof(CoopAudioEventDb)) as CoopAudioEventDb);
				if (CoopAudioEventDb._instance)
				{
					CoopAudioEventDb._instance.EventToId = new Dictionary<string, int>();
					CoopAudioEventDb._instance.IdToEvent = new Dictionary<int, string>();
					for (int i = 0; i < CoopAudioEventDb._instance.EventList.Length; i++)
					{
						string text = CoopAudioEventDb._instance.EventList[i].ToLowerInvariant();
						if (!CoopAudioEventDb._instance.EventToId.ContainsKey(text))
						{
							CoopAudioEventDb._instance.EventToId.Add(text, i);
							CoopAudioEventDb._instance.IdToEvent.Add(i, text);
						}
					}
				}
				else
				{
					Debug.LogError("Could not load 'CoopAudioEventDb'");
				}
			}
			return CoopAudioEventDb._instance;
		}
	}

	public static string FindEvent(int id)
	{
		if (id == -1)
		{
			return null;
		}
		string result;
		if (CoopAudioEventDb.Instance.IdToEvent.TryGetValue(id, out result))
		{
			return result;
		}
		return null;
	}

	public static int FindId(string eventPath)
	{
		int result;
		if (CoopAudioEventDb.Instance.EventToId.TryGetValue(eventPath.ToLowerInvariant(), out result))
		{
			return result;
		}
		return -1;
	}
}
