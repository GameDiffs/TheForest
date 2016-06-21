using System;
using UnityEngine;

namespace TheForest.Items.Utils
{
	[Serializable]
	public class RandomRange
	{
		public int _min;

		public int _max;

		public int Average
		{
			get
			{
				return (int)Mathf.Lerp((float)this._min, (float)this._max, 0.5f);
			}
		}

		public override string ToString()
		{
			return (this._min == this._max) ? this._max.ToString() : (this._min + " to " + this._max);
		}

		public static implicit operator int(RandomRange rr)
		{
			return (rr._min >= rr._max) ? rr._min : UnityEngine.Random.Range(rr._min, rr._max);
		}
	}
}
