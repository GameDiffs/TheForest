using System;
using UnityEngine;

public class AlphaCountdown : MonoBehaviour
{
	[HideInInspector]
	public TimeSpan TimeRemaining;

	[Range(2014f, 2020f)]
	public int Year;

	[Range(1f, 12f)]
	public int Month;

	[Range(1f, 31f)]
	public int Day;

	[Range(0f, 23f)]
	public int Hour = 9;

	public string TimeElapsedText = "NEW PATCH RELEASING TODAY             ";

	public Color TimeElapsedColor;

	private UILabel Ibl;

	private void Awake()
	{
		this.Ibl = base.GetComponent<UILabel>();
	}

	private void Update()
	{
		DateTime d = new DateTime(this.Year, this.Month, this.Day, this.Hour, 0, 0);
		this.TimeRemaining = d - DateTime.UtcNow + new TimeSpan(8, 0, 0);
		if (this.TimeRemaining.TotalSeconds > 0.0)
		{
			this.Ibl.text = string.Concat(new object[]
			{
				this.TimeRemaining.Days,
				" Days ",
				this.TimeRemaining.Hours,
				" Hours ",
				this.TimeRemaining.Minutes,
				" Minutes ",
				this.TimeRemaining.Seconds,
				" Seconds ".ToString()
			});
		}
		else
		{
			this.Ibl.text = this.TimeElapsedText;
			this.Ibl.color = this.TimeElapsedColor;
		}
	}
}
