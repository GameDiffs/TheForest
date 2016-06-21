using PathologicalGames;
using System;
using UnityEngine;

namespace TheForest.Utils
{
	[RequireComponent(typeof(Renderer))]
	public class TimedStippling_Pool : MonoBehaviour
	{
		private bool _stipplingIn = true;

		private float _alpha;

		private Renderer _renderer;

		private MaterialPropertyBlock _block;

		private int _wsToken = -1;

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
					this.SetStipplingAlpha(this._alpha);
					base.enabled = false;
				}
			}
		}

		private void OnEnable()
		{
			this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
		}

		private void OnDestroy()
		{
			this.WSUnregister();
		}

		public void OnSpawned()
		{
			this.WSUnregister();
			if (!this._renderer.isVisible)
			{
				this._alpha = 0.001f;
				this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
				this.WSRegister();
			}
			else
			{
				this.BeginStipplingIn();
			}
		}

		public void OnDespawned(SpawnPool pool)
		{
			this.WSUnregister();
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
			this.WSUnregister();
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

		private void VisibilityCheck()
		{
			if (this._renderer)
			{
				if (this._renderer.isVisible)
				{
					this.WSUnregister();
					this.BeginStipplingIn();
				}
			}
			else
			{
				this.WSUnregister();
				UnityEngine.Object.Destroy(this);
			}
		}

		private void BeginStipplingIn()
		{
			this._alpha = 0.001f;
			this.SetStipplingAlpha(Mathf.Clamp01(this._alpha));
			this._stipplingIn = true;
			base.enabled = true;
		}

		private void WSRegister()
		{
			if (this._wsToken == -1)
			{
				this._wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.VisibilityCheck), base.transform.position, false);
			}
		}

		private void WSUnregister()
		{
			if (this._wsToken != -1)
			{
				WorkScheduler.Unregister(new WorkScheduler.Task(this.VisibilityCheck), this._wsToken);
				this._wsToken = -1;
			}
		}
	}
}
