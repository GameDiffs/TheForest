using System;
using UnityEngine;

namespace RadicalLibrary
{
	[SerializeAll]
	public class SmoothQuaternion
	{
		public SmoothingMode Mode = SmoothingMode.slerp;

		public EasingType Ease = EasingType.Linear;

		private Quaternion _target;

		private Quaternion _start;

		private Vector3 _velocity;

		private float _startTime;

		public float Duration = 0.2f;

		public Quaternion Current;

		public Quaternion Value
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
				float t = Easing.EaseInOut((double)((Time.time - this._startTime) / this.Duration), this.Ease, this.Ease);
				switch (this.Mode)
				{
				case SmoothingMode.smooth:
					this.Current = Quaternion.Euler(new Vector3(Mathf.SmoothStep(this._start.x, this._target.x, t), Mathf.SmoothStep(this._start.y, this._target.y, t), Mathf.SmoothStep(this._start.z, this._target.z, t)));
					break;
				case SmoothingMode.damp:
					this.Current = Quaternion.Euler(Vector3.SmoothDamp(this.Current.eulerAngles, this._target.eulerAngles, ref this._velocity, this.Duration, float.PositiveInfinity, Time.time - this._startTime));
					this._startTime = Time.time;
					break;
				case SmoothingMode.lerp:
					this.Current = Quaternion.Lerp(this._start, this._target, t);
					break;
				case SmoothingMode.slerp:
					this.Current = Quaternion.Slerp(this._start, this._target, t);
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

		public float x
		{
			get
			{
				return this.Current.x;
			}
			set
			{
				this.Value = new Quaternion(value, this._target.y, this._target.z, this._target.w);
			}
		}

		public float y
		{
			get
			{
				return this.Current.y;
			}
			set
			{
				this.Value = new Quaternion(this._target.x, value, this._target.y, this._target.w);
			}
		}

		public float z
		{
			get
			{
				return this.Current.z;
			}
			set
			{
				this.Value = new Quaternion(this._target.x, this._target.y, value, this._target.w);
			}
		}

		public float w
		{
			get
			{
				return this.Current.w;
			}
			set
			{
				this.Value = new Quaternion(this._target.x, this._target.y, this._target.z, value);
			}
		}

		public bool IsComplete
		{
			get
			{
				return (Time.time - this._startTime) / this.Duration >= 1f;
			}
		}

		public SmoothQuaternion()
		{
		}

		public SmoothQuaternion(Quaternion q)
		{
			this.Current = q;
			this._start = q;
			this._target = q;
			this._startTime = Time.time;
		}

		public static implicit operator Quaternion(SmoothQuaternion obj)
		{
			return obj.Value;
		}

		public static implicit operator SmoothQuaternion(Quaternion q)
		{
			return new SmoothQuaternion(q);
		}
	}
}
