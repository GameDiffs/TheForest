using System;
using UnityEngine;

namespace Ceto
{
	public abstract class WaveSpectrumCondition
	{
		public bool Done
		{
			get;
			set;
		}

		public Vector4 GridSizes
		{
			get;
			protected set;
		}

		public Vector4 Choppyness
		{
			get;
			protected set;
		}

		public Vector4 WaveAmps
		{
			get;
			protected set;
		}

		public WaveSpectrumConditionKey Key
		{
			get;
			protected set;
		}

		public Texture2D Spectrum01
		{
			get;
			private set;
		}

		public Color[] SpectrumData01
		{
			get;
			private set;
		}

		public Texture2D Spectrum23
		{
			get;
			private set;
		}

		public Color[] SpectrumData23
		{
			get;
			private set;
		}

		public Texture2D WTable
		{
			get;
			private set;
		}

		public Color[] WTableData
		{
			get;
			private set;
		}

		public int LastUpdated
		{
			get;
			set;
		}

		public bool SupportsJacobians
		{
			get;
			protected set;
		}

		public WaveSpectrumCondition(int size, int numGrids)
		{
			this.GridSizes = Vector4.one;
			this.Choppyness = Vector4.one;
			this.WaveAmps = Vector4.one;
			this.SpectrumData01 = new Color[size * size];
			if (numGrids > 2)
			{
				this.SpectrumData23 = new Color[size * size];
			}
			this.WTableData = new Color[size * size];
			this.LastUpdated = -1;
			this.SupportsJacobians = true;
		}

		public abstract SpectrumTask GetCreateSpectrumConditionTask();

		public InitSpectrumDisplacementsTask GetInitSpectrumDisplacementsTask(DisplacementBufferCPU buffer, float time)
		{
			return new InitSpectrumDisplacementsTask(buffer, this, time);
		}

		public Vector4 InverseGridSizes()
		{
			float num = 6.28318548f * (float)this.Key.Size;
			return new Vector4(num / this.GridSizes.x, num / this.GridSizes.y, num / this.GridSizes.z, num / this.GridSizes.w);
		}

		public void Release()
		{
			if (this.Spectrum01 != null)
			{
				UnityEngine.Object.Destroy(this.Spectrum01);
				this.Spectrum01 = null;
			}
			if (this.Spectrum23 != null)
			{
				UnityEngine.Object.Destroy(this.Spectrum23);
				this.Spectrum23 = null;
			}
			if (this.WTable != null)
			{
				UnityEngine.Object.Destroy(this.WTable);
				this.WTable = null;
			}
		}

		public void CreateTextures()
		{
			if (this.Spectrum01 == null)
			{
				this.Spectrum01 = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.Spectrum01.filterMode = FilterMode.Point;
				this.Spectrum01.wrapMode = TextureWrapMode.Repeat;
				this.Spectrum01.hideFlags = HideFlags.HideAndDontSave;
				this.Spectrum01.name = "Ceto Spectrum01 Condition";
			}
			if (this.Spectrum23 == null && this.Key.NumGrids > 2)
			{
				this.Spectrum23 = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.Spectrum23.filterMode = FilterMode.Point;
				this.Spectrum23.wrapMode = TextureWrapMode.Repeat;
				this.Spectrum23.hideFlags = HideFlags.HideAndDontSave;
				this.Spectrum23.name = "Ceto Spectrum23 Condition";
			}
			if (this.WTable == null)
			{
				this.WTable = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.WTable.filterMode = FilterMode.Point;
				this.WTable.wrapMode = TextureWrapMode.Clamp;
				this.WTable.hideFlags = HideFlags.HideAndDontSave;
				this.WTable.name = "Ceto Wave Spectrum GPU WTable";
			}
		}

		public void Apply(Color[] spectrum01, Color[] spectrum23, Color[] wtable)
		{
			if (this.Spectrum01 != null && spectrum01 != null)
			{
				this.Spectrum01.SetPixels(spectrum01);
				this.Spectrum01.Apply();
				Array.Copy(spectrum01, this.SpectrumData01, spectrum01.Length);
			}
			if (this.Spectrum23 != null && spectrum23 != null)
			{
				this.Spectrum23.SetPixels(spectrum23);
				this.Spectrum23.Apply();
				Array.Copy(spectrum23, this.SpectrumData23, spectrum23.Length);
			}
			if (this.WTable != null && wtable != null)
			{
				this.WTable.SetPixels(wtable);
				this.WTable.Apply();
				Array.Copy(wtable, this.WTableData, wtable.Length);
			}
		}
	}
}
