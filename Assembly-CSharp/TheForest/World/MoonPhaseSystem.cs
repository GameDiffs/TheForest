using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.World
{
	public class MoonPhaseSystem : MonoBehaviour
	{
		public Material _moonMat;

		public Renderer _moonRenderer;

		public int _dayOffset;

		private int _lastUpdateDay = -1;

		private MaterialPropertyBlock _moonPropertyBlock;

		private void Start()
		{
			this._moonPropertyBlock = new MaterialPropertyBlock();
			this.RefreshMoonPhase();
		}

		private void Update()
		{
			if (Clock.Day != this._lastUpdateDay && (Scene.Atmosphere.TimeOfDay > 270f || Scene.Atmosphere.TimeOfDay < 88f))
			{
				this.RefreshMoonPhase();
			}
		}

		private void RefreshMoonPhase()
		{
			this._lastUpdateDay = Clock.Day;
			float value = Mathf.Lerp(0.28f, 0.72f, (float)(this._lastUpdateDay + this._dayOffset) % 30f / 30f);
			if (this._moonRenderer)
			{
				this._moonRenderer.GetPropertyBlock(this._moonPropertyBlock);
				this._moonPropertyBlock.SetFloat("_MoonPhase", value);
				this._moonRenderer.SetPropertyBlock(this._moonPropertyBlock);
			}
			else
			{
				this._moonMat.SetFloat("_MoonPhase", value);
			}
		}
	}
}
