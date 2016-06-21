using System;
using UnityEngine;

[SerializeAll]
public class WaitForAnimation : CoroutineReturn
{
	private GameObject _go;

	private string _name;

	private float _time;

	private float _weight;

	[DoNotSerialize]
	private int startFrame;

	public string name
	{
		get
		{
			return this._name;
		}
	}

	[DoNotSerialize]
	public override bool finished
	{
		get
		{
			if (LevelSerializer.IsDeserializing)
			{
				return false;
			}
			if (Time.frameCount <= this.startFrame + 4)
			{
				return false;
			}
			AnimationState animationState = this._go.GetComponent<Animation>()[this._name];
			bool flag = true;
			if (animationState.enabled)
			{
				if (this._weight == -1f)
				{
					flag = (animationState.normalizedTime >= this._time);
				}
				else
				{
					if ((double)this._weight < 0.5)
					{
						flag = (animationState.weight <= Mathf.Clamp01(this._weight + 0.001f));
					}
					flag = (animationState.weight >= Mathf.Clamp01(this._weight - 0.001f));
				}
			}
			if (!this._go.GetComponent<Animation>().IsPlaying(this._name))
			{
				flag = true;
			}
			if (flag && (animationState.weight == 0f || animationState.normalizedTime == 1f))
			{
				animationState.enabled = false;
			}
			return flag;
		}
		set
		{
			base.finished = value;
		}
	}

	public WaitForAnimation()
	{
	}

	public WaitForAnimation(GameObject go, string name) : this(go, name, 1f, -1f)
	{
	}

	public WaitForAnimation(GameObject go, string name, float time) : this(go, name, time, -1f)
	{
	}

	public WaitForAnimation(GameObject go, string name, float time, float weight)
	{
		this.startFrame = Time.frameCount;
		this._go = go;
		this._name = name;
		this._time = Mathf.Clamp01(time);
		this._weight = weight;
	}
}
