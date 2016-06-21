using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeOut : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Color to fade to. E.g., Fade to black.")]
		public FsmColor color;

		[HasFloatSlider(0f, 10f), RequiredField, HutongGames.PlayMaker.Tooltip("Fade out time in seconds.")]
		public FsmFloat time;

		[HutongGames.PlayMaker.Tooltip("Event to send when finished.")]
		public FsmEvent finishEvent;

		[HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		private Color colorLerp;

		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.finishEvent = null;
		}

		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = Color.clear;
		}

		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			this.colorLerp = Color.Lerp(Color.clear, this.color.Value, this.currentTime / this.time.Value);
			if (this.currentTime > this.time.Value && this.finishEvent != null)
			{
				base.Fsm.Event(this.finishEvent);
			}
		}

		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}
	}
}
