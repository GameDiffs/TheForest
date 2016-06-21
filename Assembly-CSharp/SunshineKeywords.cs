using System;
using UnityEngine;

public static class SunshineKeywords
{
	private class ChangeTracker
	{
		private int lastValue = -1;

		public int Value
		{
			get
			{
				return this.lastValue;
			}
		}

		public bool ValueBool
		{
			get
			{
				return this.lastValue > 0;
			}
		}

		public bool Change(int newValue)
		{
			if (newValue != this.lastValue)
			{
				this.lastValue = newValue;
				return true;
			}
			return false;
		}

		public bool Change(bool newValue)
		{
			return this.Change((!newValue) ? 0 : 1);
		}
	}

	private const string FILTER_DISABLED = "SUNSHINE_DISABLED";

	private const string FILTER_HARD = "SUNSHINE_FILTER_HARD";

	private const string FILTER_PCF_2x2 = "SUNSHINE_FILTER_PCF_2x2";

	private const string FILTER_PCF_3x3 = "SUNSHINE_FILTER_PCF_3x3";

	private const string FILTER_PCF_4x4 = "SUNSHINE_FILTER_PCF_4x4";

	private const string SCATTER_QUALITY_LOW = "SUNSHINE_FILTER_HARD";

	private const string SCATTER_QUALITY_MEDIUM = "SUNSHINE_FILTER_PCF_2x2";

	private const string SCATTER_QUALITY_HIGH = "SUNSHINE_FILTER_PCF_3x3";

	private const string SCATTER_QUALITY_VERYHIGH = "SUNSHINE_FILTER_PCF_4x4";

	private const string SUNSHINE_SCATTER_BLENDNOW_ON = "SUNSHINE_SCATTER_BLENDNOW_ON";

	private const string SUNSHINE_SCATTER_BLENDNOW_OFF = "SUNSHINE_SCATTER_BLENDNOW_OFF";

	private static SunshineKeywords.ChangeTracker cascadeCount = new SunshineKeywords.ChangeTracker();

	private static readonly string[] FILTER_STYLES = new string[]
	{
		"SUNSHINE_DISABLED",
		"SUNSHINE_FILTER_HARD",
		"SUNSHINE_FILTER_PCF_2x2",
		"SUNSHINE_FILTER_PCF_3x3",
		"SUNSHINE_FILTER_PCF_4x4"
	};

	private static readonly string[] SCATTER_QUALITIES = new string[]
	{
		"SUNSHINE_FILTER_HARD",
		"SUNSHINE_FILTER_PCF_2x2",
		"SUNSHINE_FILTER_PCF_3x3",
		"SUNSHINE_FILTER_PCF_4x4"
	};

	private static SunshineKeywords.ChangeTracker scatterBlendNow = new SunshineKeywords.ChangeTracker();

	private static void SetKeyword(int index, string[] keywords)
	{
		for (int i = 0; i < keywords.Length; i++)
		{
			if (i == index)
			{
				Shader.EnableKeyword(keywords[i]);
			}
			else
			{
				Shader.DisableKeyword(keywords[i]);
			}
		}
	}

	private static void SetKeywordWithFallbacks(int index, string[] keywords, int minimumFallback)
	{
		for (int i = 0; i < keywords.Length; i++)
		{
			if (i == index || (i < index && i >= minimumFallback))
			{
				Shader.EnableKeyword(keywords[i]);
			}
			else
			{
				Shader.DisableKeyword(keywords[i]);
			}
		}
	}

	private static void ToggleKeyword(bool toggle, string keywordON, string keywordOFF)
	{
		Shader.DisableKeyword((!toggle) ? keywordON : keywordOFF);
		Shader.EnableKeyword((!toggle) ? keywordOFF : keywordON);
	}

	private static void ToggleKeyword(bool toggle, string keyword)
	{
		if (toggle)
		{
			Shader.EnableKeyword(keyword);
		}
		else
		{
			Shader.DisableKeyword(keyword);
		}
	}

	public static void SetFilterStyle(int style)
	{
		SunshineKeywords.SetKeywordWithFallbacks(style, SunshineKeywords.FILTER_STYLES, 1);
	}

	public static void SetFilterStyle(SunshineShadowFilters style)
	{
		SunshineKeywords.SetFilterStyle((int)(style + 1));
	}

	public static void DisableShadows()
	{
		SunshineKeywords.SetFilterStyle(0);
	}

	public static void SetScatterQuality(SunshineScatterSamplingQualities quality)
	{
		SunshineKeywords.SetKeyword((int)quality, SunshineKeywords.SCATTER_QUALITIES);
	}

	public static void ToggleScatterBlendNow(bool enabled)
	{
		if (SunshineKeywords.scatterBlendNow.Change(enabled))
		{
			SunshineKeywords.ToggleKeyword(enabled, "SUNSHINE_SCATTER_BLENDNOW_ON");
		}
	}
}
