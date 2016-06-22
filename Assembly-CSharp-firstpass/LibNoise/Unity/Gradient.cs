using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibNoise.Unity
{
	public struct Gradient
	{
		private List<KeyValuePair<double, Color>> m_data;

		private bool m_inverted;

		private static Gradient _empty;

		private static Gradient _terrain;

		private static Gradient _grayscale;

		public Color this[double position]
		{
			get
			{
				int i;
				for (i = 0; i < this.m_data.Count; i++)
				{
					if (position < this.m_data[i].Key)
					{
						break;
					}
				}
				int num = Mathf.Clamp(i - 1, 0, this.m_data.Count - 1);
				int num2 = Mathf.Clamp(i, 0, this.m_data.Count - 1);
				if (num == num2)
				{
					return this.m_data[num2].Value;
				}
				double key = this.m_data[num].Key;
				double key2 = this.m_data[num2].Key;
				double num3 = (position - key) / (key2 - key);
				if (this.m_inverted)
				{
					num3 = 1.0 - num3;
				}
				return Color.Lerp(this.m_data[num].Value, this.m_data[num2].Value, (float)num3);
			}
			set
			{
				for (int i = 0; i < this.m_data.Count; i++)
				{
					if (this.m_data[i].Key == position)
					{
						this.m_data.RemoveAt(i);
						break;
					}
				}
				this.m_data.Add(new KeyValuePair<double, Color>(position, value));
				this.m_data.Sort((KeyValuePair<double, Color> lhs, KeyValuePair<double, Color> rhs) => lhs.Key.CompareTo(rhs.Key));
			}
		}

		public bool IsInverted
		{
			get
			{
				return this.m_inverted;
			}
			set
			{
				this.m_inverted = value;
			}
		}

		public static Gradient Empty
		{
			get
			{
				return Gradient._empty;
			}
		}

		public static Gradient Grayscale
		{
			get
			{
				return Gradient._grayscale;
			}
		}

		public static Gradient Terrain
		{
			get
			{
				return Gradient._terrain;
			}
		}

		public Gradient(Color color)
		{
			this.m_data = new List<KeyValuePair<double, Color>>();
			this.m_data.Add(new KeyValuePair<double, Color>(-1.0, color));
			this.m_data.Add(new KeyValuePair<double, Color>(1.0, color));
			this.m_inverted = false;
		}

		public Gradient(Color start, Color end)
		{
			this.m_data = new List<KeyValuePair<double, Color>>();
			this.m_data.Add(new KeyValuePair<double, Color>(-1.0, start));
			this.m_data.Add(new KeyValuePair<double, Color>(1.0, end));
			this.m_inverted = false;
		}

		static Gradient()
		{
			Gradient._terrain.m_data = new List<KeyValuePair<double, Color>>();
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(-1.0, new Color(0f, 0f, 128f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(-0.2, new Color(32f, 64f, 128f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(-0.04, new Color(64f, 96f, 192f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(-0.02, new Color(192f, 192f, 128f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(0.0, new Color(0f, 192f, 0f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(0.25, new Color(192f, 192f, 0f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(0.5, new Color(160f, 96f, 64f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(0.75, new Color(128f, 255f, 255f)));
			Gradient._terrain.m_data.Add(new KeyValuePair<double, Color>(1.0, new Color(255f, 255f, 255f)));
			Gradient._terrain.m_inverted = false;
			Gradient._grayscale.m_data = new List<KeyValuePair<double, Color>>();
			Gradient._grayscale.m_data.Add(new KeyValuePair<double, Color>(-1.0, Color.black));
			Gradient._grayscale.m_data.Add(new KeyValuePair<double, Color>(1.0, Color.white));
			Gradient._grayscale.m_inverted = false;
			Gradient._empty.m_data = new List<KeyValuePair<double, Color>>();
			Gradient._empty.m_data.Add(new KeyValuePair<double, Color>(-1.0, Color.clear));
			Gradient._empty.m_data.Add(new KeyValuePair<double, Color>(1.0, Color.clear));
			Gradient._empty.m_inverted = false;
		}

		public void Clear()
		{
			this.m_data.Clear();
			this.m_data.Add(new KeyValuePair<double, Color>(0.0, Color.clear));
			this.m_data.Add(new KeyValuePair<double, Color>(1.0, Color.clear));
		}

		public void Invert()
		{
			throw new NotImplementedException();
		}
	}
}
