using System;
using System.Collections.Generic;
using UnityEngine;

public class LOD_Stats
{
	public bool StopPooling;

	private static LOD_Stats DefaultNonLod = new LOD_Stats();

	public static LOD_Stats Current;

	public Dictionary<Component, float> Settings = new Dictionary<Component, float>();

	private static LOD_Stats Instance
	{
		get
		{
			return (LOD_Stats.Current == null) ? LOD_Stats.DefaultNonLod : LOD_Stats.Current;
		}
	}

	public static void SetInt(Component component, int value)
	{
		LOD_Stats.Instance.setInt(component, value);
	}

	private void setInt(Component component, int value)
	{
		if (!this.Settings.ContainsKey(component))
		{
			this.Settings.Add(component, 0f);
		}
		this.Settings[component] = (float)value + 0.01f;
	}

	public static void SetFloat(Component component, float value)
	{
		LOD_Stats.Instance.setFloat(component, value);
	}

	private void setFloat(Component component, float value)
	{
		if (!this.Settings.ContainsKey(component))
		{
			this.Settings.Add(component, 0f);
		}
		this.Settings[component] = value;
	}

	public static void SetBool(Component component, bool value)
	{
		LOD_Stats.Instance.setBool(component, value);
	}

	private void setBool(Component component, bool value)
	{
		if (!this.Settings.ContainsKey(component))
		{
			this.Settings.Add(component, 0f);
		}
		this.Settings[component] = ((!value) ? 0f : 1f);
	}

	public static int GetInt(Component component, int value)
	{
		return LOD_Stats.Instance.getInt(component, value);
	}

	private int getInt(Component component, int defaultValue)
	{
		if (!this.Settings.ContainsKey(component))
		{
			return defaultValue;
		}
		return (int)this.Settings[component];
	}

	public static float GetFloat(Component component, float value)
	{
		return LOD_Stats.Instance.getFloat(component, value);
	}

	private float getFloat(Component component, float defaultValue)
	{
		if (!this.Settings.ContainsKey(component))
		{
			return defaultValue;
		}
		return this.Settings[component];
	}

	public static bool GetBool(Component component, bool value)
	{
		return LOD_Stats.Instance.getBool(component, value);
	}

	private bool getBool(Component component, bool defaultValue)
	{
		if (!this.Settings.ContainsKey(component))
		{
			return defaultValue;
		}
		return this.Settings[component] > 0.5f;
	}
}
