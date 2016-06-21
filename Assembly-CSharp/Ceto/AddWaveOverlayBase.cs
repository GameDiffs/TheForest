using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public abstract class AddWaveOverlayBase : MonoBehaviour
	{
		protected List<WaveOverlay> m_overlays = new List<WaveOverlay>();

		public IEnumerable<WaveOverlay> Overlays
		{
			get
			{
				return this.m_overlays;
			}
		}

		public WaveOverlay Overlay
		{
			get
			{
				return this.m_overlays[0];
			}
		}

		protected virtual void Start()
		{
		}

		public virtual void Translate(Vector3 amount)
		{
			if (this.m_overlays != null)
			{
				for (int i = 0; i < this.m_overlays.Count; i++)
				{
					this.m_overlays[i].Position = this.m_overlays[i].Position + amount;
				}
			}
		}

		protected virtual void Update()
		{
			if (this.m_overlays != null)
			{
				for (int i = 0; i < this.m_overlays.Count; i++)
				{
					this.m_overlays[i].UpdateOverlay();
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (this.m_overlays != null)
			{
				for (int i = 0; i < this.m_overlays.Count; i++)
				{
					this.m_overlays[i].Hide = false;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this.m_overlays != null)
			{
				for (int i = 0; i < this.m_overlays.Count; i++)
				{
					this.m_overlays[i].Hide = true;
				}
			}
		}

		protected virtual void OnDestroy()
		{
			if (this.m_overlays != null)
			{
				for (int i = 0; i < this.m_overlays.Count; i++)
				{
					this.m_overlays[i].Kill = true;
				}
			}
		}

		protected static AnimationCurve DefaultCurve()
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.012f, 0.98f),
				new Keyframe(0.026f, 1f),
				new Keyframe(1f, 0f)
			};
			return new AnimationCurve(keys);
		}

		protected void CheckCanSampleTex(Texture tex, string name)
		{
			if (tex == null)
			{
				return;
			}
			if (!(tex is Texture2D))
			{
				Ocean.LogWarning("Can not query overlays " + name + " if texture is not Texture2D");
				return;
			}
			Texture2D texture2D = tex as Texture2D;
			try
			{
				Color pixel = texture2D.GetPixel(0, 0);
			}
			catch
			{
				Ocean.LogWarning("Can not query overlays " + name + " if read/write is not enabled");
			}
		}
	}
}
