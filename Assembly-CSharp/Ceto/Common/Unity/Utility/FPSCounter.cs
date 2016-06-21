using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class FPSCounter : MonoBehaviour
	{
		private float updateInterval = 0.5f;

		private float accum;

		private float frames;

		private float timeleft;

		public float FrameRate
		{
			get;
			set;
		}

		private void Start()
		{
			this.timeleft = this.updateInterval;
		}

		private void Update()
		{
			this.timeleft -= Time.deltaTime;
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames += 1f;
			if (this.timeleft <= 0f)
			{
				this.FrameRate = this.accum / this.frames;
				this.timeleft = this.updateInterval;
				this.accum = 0f;
				this.frames = 0f;
			}
		}
	}
}
