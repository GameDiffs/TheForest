using System;
using UnityEngine;

namespace RadicalLibrary
{
	[SerializeAll]
	public class SmoothVector3
	{
		public EasingType Ease = EasingType.Linear;

		public SmoothingMode Mode = SmoothingMode.lerp;

		private Vector3 _target;

		private Vector3 _start;

		private Vector3 _velocity;

		private float _startTime;

		private Vector3 _current;

		public float Duration = 0.1f;

		public bool Lock;

		public bool IsComplete
		{
			get
			{
				return (Time.time - this._startTime) / this.Duration >= 1f;
			}
		}

		public Vector3 Target
		{
			get
			{
				return this._target;
			}
		}

		public Vector3 Current
		{
			get
			{
				return this._current;
			}
			set
			{
				this._current = value;
				this._start = value;
			}
		}

		public float x
		{
			get
			{
				Vector3 current = this.Current;
				return current.x;
			}
			set
			{
				this.Value = new Vector3(value, this._target.y, this._target.z);
			}
		}

		public float y
		{
			get
			{
				Vector3 current = this.Current;
				return current.y;
			}
			set
			{
				this.Value = new Vector3(this._target.x, value, this._target.y);
			}
		}

		public float z
		{
			get
			{
				Vector3 current = this.Current;
				return current.z;
			}
			set
			{
				this.Value = new Vector3(this._target.x, this._target.y, value);
			}
		}

		public Vector3 Value
		{
			get
			{
				if (this.Duration == 0f)
				{
					this.Duration = 0.1f;
				}
				if (this._startTime == 0f)
				{
					this._startTime = Time.time;
				}
				float t = Easing.EaseInOut((double)(Time.time - this._startTime) / (double)this.Duration, this.Ease, this.Ease);
				switch (this.Mode)
				{
				case SmoothingMode.smooth:
					this._current = new Vector3(Mathf.SmoothStep(this._start.x, this._target.x, t), Mathf.SmoothStep(this._start.y, this._target.y, t), Mathf.SmoothStep(this._start.z, this._target.z, t));
					break;
				case SmoothingMode.damp:
					this._current = Vector3.SmoothDamp(this._current, this._target, ref this._velocity, this.Duration, float.PositiveInfinity, Time.time - this._startTime);
					this._startTime = Time.time;
					break;
				case SmoothingMode.lerp:
					this._current = Vector3.Lerp(this._start, this._target, t);
					break;
				case SmoothingMode.slerp:
					this._current = Vector3.Slerp(this._start, this._target, t);
					break;
				}
				if (this.Lock)
				{
					if (this._target.x == this._start.x)
					{
						this._current.x = this._target.x;
					}
					if (this._target.y == this._start.y)
					{
						this._current.y = this._target.y;
					}
					if (this._target.x == this._start.z)
					{
						this._current.z = this._target.z;
					}
				}
				return this._current;
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

		public SmoothVector3()
		{
		}

		public SmoothVector3(float x, float y, float z) : this(new Vector3(x, y, z))
		{
		}

		public SmoothVector3(Vector3 value)
		{
			this._current = value;
			this._target = value;
			this._current = value;
			this._start = value;
			this._startTime = Time.time;
		}

		public override string ToString()
		{
			return string.Format("[SmoothVector3: IsComplete={0}, Target={1}, Current={2}, x={3}, y={4}, z={5}, Value={6}]", new object[]
			{
				this.IsComplete,
				this.Target,
				this.Current,
				this.x,
				this.y,
				this.z,
				this.Value
			});
		}

		public static implicit operator Vector3(SmoothVector3 obj)
		{
			return obj.Value;
		}

		public static implicit operator SmoothVector3(Vector3 obj)
		{
			return new SmoothVector3(obj);
		}
	}
}
