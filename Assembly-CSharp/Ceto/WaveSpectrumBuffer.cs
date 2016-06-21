using System;
using UnityEngine;

namespace Ceto
{
	public abstract class WaveSpectrumBuffer
	{
		public abstract bool Done
		{
			get;
		}

		public abstract int Size
		{
			get;
		}

		public abstract bool IsGPU
		{
			get;
		}

		public float TimeValue
		{
			get;
			protected set;
		}

		public bool HasRun
		{
			get;
			protected set;
		}

		public bool BeenSampled
		{
			get;
			set;
		}

		public Material InitMaterial
		{
			get;
			set;
		}

		public int InitPass
		{
			get;
			set;
		}

		public WaveSpectrumBuffer()
		{
		}

		public abstract Texture GetTexture(int idx);

		protected abstract void Initilize(WaveSpectrumCondition condition, float time);

		public abstract void Run(WaveSpectrumCondition condition, float time);

		public abstract void EnableBuffer(int idx);

		public abstract void DisableBuffer(int idx);

		public abstract int EnabledBuffers();

		public abstract bool IsEnabledBuffer(int i);

		public virtual void Release()
		{
		}

		public virtual void EnableSampling()
		{
		}

		public virtual void DisableSampling()
		{
		}
	}
}
