using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Overlays/AddFoamThruster")]
	public class AddFoamThruster : AddWaveOverlayBase
	{
		public enum ROTATION
		{
			NONE,
			RANDOM,
			RELATIVE
		}

		public Texture foamTexture;

		public bool textureFoam = true;

		public AddFoamThruster.ROTATION rotation = AddFoamThruster.ROTATION.RANDOM;

		public AnimationCurve timeLine = AddWaveOverlayBase.DefaultCurve();

		public float duration = 4f;

		public float size = 2f;

		[Range(16f, 1000f)]
		public float rate = 128f;

		public float expansion = 4f;

		public float momentum = 10f;

		public float spin = 10f;

		public bool mustBeBelowWater = true;

		[Range(0f, 2f)]
		public float alpha = 0.8f;

		[Range(0f, 1f)]
		public float jitter = 0.2f;

		private float m_lastTime;

		private float m_remainingTime;

		private List<FoamOverlay> m_remove = new List<FoamOverlay>();

		private LinkedList<FoamOverlay> m_pool = new LinkedList<FoamOverlay>();

		protected override void Start()
		{
			this.m_lastTime = Time.time;
		}

		protected override void Update()
		{
			this.UpdateOverlays();
			this.AddFoam();
			this.RemoveOverlays();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_lastTime = Time.time;
		}

		public override void Translate(Vector3 amount)
		{
			base.Translate(amount);
		}

		private float Rotation()
		{
			switch (this.rotation)
			{
			case AddFoamThruster.ROTATION.NONE:
				return 0f;
			case AddFoamThruster.ROTATION.RANDOM:
				return UnityEngine.Random.Range(0f, 360f);
			case AddFoamThruster.ROTATION.RELATIVE:
				return base.transform.eulerAngles.y;
			default:
				return 0f;
			}
		}

		private FoamOverlay NewFoamOverlay(Vector3 pos, float rotation, float size, float duration, Texture texture)
		{
			FoamOverlay foamOverlay;
			if (this.m_pool.Count > 0)
			{
				foamOverlay = this.m_pool.First.Value;
				foamOverlay.Reset(pos, this.Rotation(), size, duration, this.foamTexture);
				this.m_pool.RemoveFirst();
			}
			else
			{
				foamOverlay = new FoamOverlay(pos, this.Rotation(), size, duration, this.foamTexture);
			}
			return foamOverlay;
		}

		private void AddFoam()
		{
			if (this.duration <= 0f || Ocean.Instance == null)
			{
				this.m_lastTime = Time.time;
				return;
			}
			this.size = Mathf.Max(1f, this.size);
			float num = base.transform.position.y;
			Vector3 position = base.transform.position;
			Vector3 a = base.transform.forward;
			if (this.mustBeBelowWater)
			{
				num = Ocean.Instance.QueryWaves(position.x, position.z);
			}
			if (num < position.y || (a.x == 0f && a.z == 0f))
			{
				this.m_lastTime = Time.time;
				return;
			}
			float num2 = Time.time - this.m_lastTime;
			a = a.normalized;
			position.y = 0f;
			Vector3 vector = a * this.momentum;
			this.m_remainingTime += num2;
			float num3 = this.rate / 1000f;
			float num4 = 0f;
			while (this.m_remainingTime > num3)
			{
				Vector3 pos = position + a * num4;
				FoamOverlay foamOverlay = this.NewFoamOverlay(pos, this.Rotation(), this.size, this.duration, this.foamTexture);
				foamOverlay.FoamTex.alpha = 0f;
				foamOverlay.FoamTex.textureFoam = this.textureFoam;
				foamOverlay.Momentum = vector;
				foamOverlay.Spin = ((UnityEngine.Random.value <= 0.5f) ? this.spin : (-this.spin));
				foamOverlay.Expansion = this.expansion;
				if (this.jitter > 0f)
				{
					foamOverlay.Spin *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
					foamOverlay.Expansion *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
				}
				this.m_overlays.Add(foamOverlay);
				Ocean.Instance.OverlayManager.Add(foamOverlay);
				this.m_remainingTime -= num3;
				num4 += num3;
			}
			this.m_lastTime = Time.time;
		}

		private void UpdateOverlays()
		{
			for (int i = 0; i < this.m_overlays.Count; i++)
			{
				WaveOverlay waveOverlay = this.m_overlays[i];
				float normalizedAge = waveOverlay.NormalizedAge;
				waveOverlay.FoamTex.alpha = this.timeLine.Evaluate(normalizedAge) * this.alpha;
				waveOverlay.FoamTex.textureFoam = this.textureFoam;
				waveOverlay.UpdateOverlay();
			}
		}

		private void RemoveOverlays()
		{
			this.m_remove.Clear();
			for (int i = 0; i < this.m_overlays.Count; i++)
			{
				FoamOverlay foamOverlay = this.m_overlays[i] as FoamOverlay;
				if (foamOverlay.Age >= foamOverlay.Duration)
				{
					this.m_remove.Add(foamOverlay);
					foamOverlay.Kill = true;
				}
			}
			for (int j = 0; j < this.m_remove.Count; j++)
			{
				this.m_overlays.Remove(this.m_remove[j]);
				this.m_pool.AddLast(this.m_remove[j]);
			}
		}

		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, 2f);
		}
	}
}
