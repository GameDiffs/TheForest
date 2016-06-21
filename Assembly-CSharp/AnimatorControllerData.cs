using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerData : ScriptableObject
{
	public enum ParamType
	{
		Bool,
		Int,
		Float,
		Trigger
	}

	[Serializable]
	public class Parameter
	{
		public string Name = string.Empty;

		public AnimatorControllerData.ParamType ParamType = AnimatorControllerData.ParamType.Float;
	}

	[Serializable]
	public class ControllerData
	{
		public string Name = string.Empty;

		public int ControllerHash;

		public AnimatorControllerData.Parameter[] Parameters = new AnimatorControllerData.Parameter[0];

		public ControllerData(int hash)
		{
			this.ControllerHash = hash;
		}
	}

	private static string AssetName = "AnimatorControllerData";

	private static AnimatorControllerData instance;

	public List<AnimatorControllerData.ControllerData> Controllers = new List<AnimatorControllerData.ControllerData>();

	private static string FullPath
	{
		get
		{
			return string.Format("Assets/Resources/{0}.asset", AnimatorControllerData.AssetName);
		}
	}

	public static AnimatorControllerData Instance
	{
		get
		{
			if (!AnimatorControllerData.instance)
			{
				AnimatorControllerData.instance = Resources.Load<AnimatorControllerData>(AnimatorControllerData.AssetName);
			}
			return AnimatorControllerData.instance;
		}
	}

	private int ControllerIndex(int hash)
	{
		int num = 0;
		foreach (AnimatorControllerData.ControllerData current in this.Controllers)
		{
			if (current.ControllerHash == hash)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public AnimatorControllerData.ControllerData GetData(int hash)
	{
		int num = this.ControllerIndex(hash);
		if (num < 0)
		{
			num = this.Controllers.Count;
			this.Controllers.Add(new AnimatorControllerData.ControllerData(hash));
		}
		return this.Controllers[num];
	}
}
