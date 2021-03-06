using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.World
{
	public class LightFadeIn : MonoBehaviour
	{
		public Light _targetLight;

		public float _fadeInDuration = 1f;

		public float _targetIntensity;

		private void Awake()
		{
			this._targetIntensity = this._targetLight.intensity;
			this._targetLight.intensity = 0f;
		}

		private void OnEnable()
		{
			this._targetLight.intensity = 0f;
			base.StartCoroutine(this.FadeInRoutine());
		}

		[DebuggerHidden]
		private IEnumerator FadeInRoutine()
		{
			LightFadeIn.<FadeInRoutine>c__Iterator1C8 <FadeInRoutine>c__Iterator1C = new LightFadeIn.<FadeInRoutine>c__Iterator1C8();
			<FadeInRoutine>c__Iterator1C.<>f__this = this;
			return <FadeInRoutine>c__Iterator1C;
		}
	}
}
