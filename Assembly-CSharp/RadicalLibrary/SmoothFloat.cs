using System;
using UnityEngine;

namespace RadicalLibrary
{
	[SerializeAll]
	public class SmoothFloat
	{
		public SmoothingMode Mode = SmoothingMode.lerp;

		public EasingType Ease = EasingType.Linear;

		private float _target;

		private float _start;

		private float _velocity;

		private float _startTime;

		public float Duration = 0.5f;

		private float _current;

		[DoNotSerialize]
		public float Current
		{
			get
			{
				return this._current;
			}
			set
			{
				this._current = value;
				this._target = value;
			}
		}

		public float Target
		{
			get
			{
				return this._target;
			}
		}

		public float Value
		{
			get
			{
				if (this.Duration == 0f)
				{
					this.Duration = 0.5f;
				}
				if (this._startTime == 0f)
				{
					this._startTime = Time.time;
				}
				float t = Mathf.Clamp01(Easing.EaseInOut((double)Mathf.Clamp01((Time.time - this._startTime) / this.Duration), this.Ease, this.Ease));
				switch (this.Mode)
				{
				case SmoothingMode.smooth:
					this._current = Mathf.SmoothStep(this._start, this._target, t);
					break;
				case SmoothingMode.damp:
					this._current = Mathf.SmoothDamp(this.Current, this._target, ref this._velocity, this.Duration, float.PositiveInfinity, Time.time - this._startTime);
					this._startTime = Time.time;
					break;
				case SmoothingMode.lerp:
					this._current = Mathf.Lerp(this._start, this._target, t);
					break;
				case SmoothingMode.slerp:
					this._current = Mathf.Lerp(this._start, this._target, t);
					break;
				}
				return this.Current;
			}
			set
			{
				if (value.Equals(this._target))
				{
					return;
				}
				this._start = this.Value;
				this._startTime = Time.time;
				this._target = value;
			}
		}

		public bool IsComplete
		{
			get
			{
				return (Time.time - this._startTime) / this.Duration >= 1f;
			}
		}

		public SmoothFloat()
		{
		}

		public SmoothFloat(float f)
		{
			this.Current = f;
			this._start = f;
			this._velocity = 0f;
			this._target = f;
			this._startTime = Time.time;
		}

		public override string ToString()
		{
			return string.Format("[SmoothFloat: Value={0}]", this.Current);
		}

		public static implicit operator float(SmoothFloat obj)
		{
			return obj.Value;
		}

		public static implicit operator SmoothFloat(float f)
		{
			return new SmoothFloat(f);
		}
	}
}
