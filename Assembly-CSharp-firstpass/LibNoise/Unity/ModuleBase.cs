using System;
using System.Xml.Serialization;
using UnityEngine;

namespace LibNoise.Unity
{
	public abstract class ModuleBase : IDisposable
	{
		protected ModuleBase[] m_modules;

		[XmlIgnore]
		[NonSerialized]
		private bool m_disposed;

		public virtual ModuleBase this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_modules.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (this.m_modules[index] == null)
				{
					throw new ArgumentNullException();
				}
				return this.m_modules[index];
			}
			set
			{
				if (index < 0 || index >= this.m_modules.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.m_modules[index] = value;
			}
		}

		public int SourceModuleCount
		{
			get
			{
				return (this.m_modules != null) ? this.m_modules.Length : 0;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.m_disposed;
			}
		}

		protected ModuleBase(int count)
		{
			if (count > 0)
			{
				this.m_modules = new ModuleBase[count];
			}
		}

		public abstract double GetValue(double x, double y, double z);

		public double GetValue(Vector3 coordinate)
		{
			return this.GetValue((double)coordinate.x, (double)coordinate.y, (double)coordinate.z);
		}

		public double GetValue(ref Vector3 coordinate)
		{
			return this.GetValue((double)coordinate.x, (double)coordinate.y, (double)coordinate.z);
		}

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.m_disposed = this.Disposing();
			}
			GC.SuppressFinalize(this);
		}

		protected virtual bool Disposing()
		{
			if (this.m_modules != null)
			{
				for (int i = 0; i < this.m_modules.Length; i++)
				{
					this.m_modules[i].Dispose();
					this.m_modules[i] = null;
				}
				this.m_modules = null;
			}
			return true;
		}
	}
}
