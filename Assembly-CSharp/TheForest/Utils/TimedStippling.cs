using PathologicalGames;
using System;
using UnityEngine;

namespace TheForest.Utils
{
	[RequireComponent(typeof(Renderer))]
	public class TimedStippling : MonoBehaviour
	{
		private bool _stipplingIn = true;

		private float _alpha;

		private Renderer _renderer;

		private MaterialPropertyBlock _block;

		private void Awake()
		{
			this._alpha = 0.001f;
			this._renderer = base.GetComponent<Renderer>();
			this._block = new MaterialPropertyBlock();
			base.enabled = false;
		}

		private void Update()
		{
			this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
			if (this._stipplingIn)
			{
				if (this._renderer.isVisible)
				{
					this._alpha += Time.deltaTime;
					if (this._alpha >= 0.9375f)
					{
						this.Stop();
					}
				}
			}
			else
			{
				this._alpha -= Time.deltaTime;
				if (this._alpha <= 0f)
				{
					this._alpha = 1E-05f;
				}
			}
		}

		private void OnEnable()
		{
			this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
		}

		private void ResetStippling()
		{
			this._alpha = 0.001f;
			this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
			this._stipplingIn = true;
			base.enabled = true;
		}

		private void OnDespawned(SpawnPool pool)
		{
			if (pool)
			{
				pool.DelayNextDisable = true;
			}
			this._alpha = 0.9374f;
			this._stipplingIn = false;
			base.enabled = true;
		}

		private void SkipStippling()
		{
			this.Stop();
		}

		private void SetStipplingAlpha(float alpha)
		{
			if (this._renderer)
			{
				this._renderer.GetPropertyBlock(this._block);
				this._block.SetVector("_StippleAlpha", new Vector4(alpha, Mathf.Ceil(alpha * 16f) * 0.0625f, 0f, 0f));
				this._renderer.SetPropertyBlock(this._block);
			}
		}

		private void Stop()
		{
			this._alpha = 0f;
			this.SetStipplingAlpha(this._alpha);
			base.enabled = false;
		}
	}
}
