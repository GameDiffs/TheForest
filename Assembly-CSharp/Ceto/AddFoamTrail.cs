using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Overlays/AddFoamTrail")]
	public class AddFoamTrail : AddWaveOverlayBase
	{
		public enum ROTATION
		{
			NONE,
			RANDOM,
			RELATIVE
		}

		private readonly float MIN_MOVEMENT = 0.1f;

		private readonly float MAX_MOVEMENT = 100f;

		public Texture foamTexture;

		public bool textureFoam = true;

		public AddFoamTrail.ROTATION rotation = AddFoamTrail.ROTATION.RANDOM;

		public AnimationCurve timeLine = AddWaveOverlayBase.DefaultCurve();

		public float duration = 10f;

		public float size = 10f;

		public float spacing = 4f;

		public float expansion = 1f;

		public float momentum = 1f;

		public float spin = 10f;

		public bool mustBeBelowWater = true;

		[Range(0f, 2f)]
		public float alpha = 0.8f;

		[Range(0f, 1f)]
		public float jitter = 0.2f;

		private Vector3 m_lastPosition;

		private float m_remainingDistance;

		private List<FoamOverlay> m_remove = new List<FoamOverlay>();

		private LinkedList<FoamOverlay> m_pool = new LinkedList<FoamOverlay>();

		protected override void Start()
		{
			this.m_lastPosition = base.transform.position;
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
			this.m_lastPosition = base.transform.position;
		}

		public override void Translate(Vector3 amount)
		{
			base.Translate(amount);
			this.m_lastPosition += amount;
		}

		private float Rotation()
		{
			switch (this.rotation)
			{
			case AddFoamTrail.ROTATION.NONE:
				return 0f;
			case AddFoamTrail.ROTATION.RANDOM:
				return UnityEngine.Random.Range(0f, 360f);
			case AddFoamTrail.ROTATION.RELATIVE:
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
				this.m_lastPosition = base.transform.position;
				return;
			}
			this.spacing = Mathf.Max(1f, this.spacing);
			this.size = Mathf.Max(1f, this.size);
			Vector3 position = base.transform.position;
			float num = position.y;
			if (this.mustBeBelowWater)
			{
				num = Ocean.Instance.QueryWaves(position.x, position.z);
			}
			if (num < position.y)
			{
				this.m_lastPosition = position;
				return;
			}
			position.y = 0f;
			this.m_lastPosition.y = 0f;
			Vector3 vector = this.m_lastPosition - position;
			Vector3 normalized = vector.normalized;
			float num2 = vector.magnitude;
			if (num2 < this.MIN_MOVEMENT)
			{
				return;
			}
			num2 = Mathf.Min(this.MAX_MOVEMENT, num2);
			Vector3 vector2 = normalized * this.momentum;
			this.m_remainingDistance += num2;
			float num3 = 0f;
			while (this.m_remainingDistance > this.spacing)
			{
				Vector3 pos = position + normalized * num3;
				FoamOverlay foamOverlay = this.NewFoamOverlay(pos, this.Rotation(), this.size, this.duration, this.foamTexture);
				foamOverlay.FoamTex.alpha = 0f;
				foamOverlay.FoamTex.textureFoam = this.textureFoam;
				foamOverlay.Momentum = vector2;
				foamOverlay.Spin = ((UnityEngine.Random.value <= 0.5f) ? this.spin : (-this.spin));
				foamOverlay.Expansion = this.expansion;
				if (this.jitter > 0f)
				{
					foamOverlay.Spin *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
					foamOverlay.Expansion *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
				}
				this.m_overlays.Add(foamOverlay);
				Ocean.Instance.OverlayManager.Add(foamOverlay);
				this.m_remainingDistance -= this.spacing;
				num3 += this.spacing;
			}
			this.m_lastPosition = position;
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
