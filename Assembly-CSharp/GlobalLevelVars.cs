using System;
using UnityEngine;

public class GlobalLevelVars : MonoBehaviour
{
	private new static string name = "Я не хочу писать имя";

	public static string stringToSendBN = string.Empty;

	public static string Name
	{
		get
		{
			return GlobalLevelVars.name;
		}
		set
		{
			GlobalLevelVars.name = value;
		}
	}

	private void Start()
	{
		GlobalLevelVars.name = PlayerPrefs.GetString("playerName");
	}

	private void Update()
	{
	}
}
