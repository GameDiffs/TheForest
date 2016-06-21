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
			LightFadeIn.<FadeInRoutine>c__Iterator1BF <FadeInRoutine>c__Iterator1BF = new LightFadeIn.<FadeInRoutine>c__Iterator1BF();
			<FadeInRoutine>c__Iterator1BF.<>f__this = this;
			return <FadeInRoutine>c__Iterator1BF;
		}
	}
}
