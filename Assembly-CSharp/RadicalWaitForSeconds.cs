using System;
using UnityEngine;

public class RadicalWaitForSeconds : IYieldInstruction
{
	private float _time;

	private float _seconds;

	public float TimeRemaining
	{
		get
		{
			return Mathf.Clamp(this._time + this._seconds - Time.time, 0f, 1E+07f);
		}
		set
		{
			this._time = Time.time;
			this._seconds = value;
		}
	}

	public YieldInstruction Instruction
	{
		get
		{
			return new WaitForSeconds(this.TimeRemaining);
		}
	}

	public RadicalWaitForSeconds()
	{
	}

	public RadicalWaitForSeconds(float seconds)
	{
		this._time = Time.time;
		this._seconds = seconds;
	}
}
