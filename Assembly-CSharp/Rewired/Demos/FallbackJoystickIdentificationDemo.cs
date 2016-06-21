using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class FallbackJoystickIdentificationDemo : MonoBehaviour
	{
		private const float windowWidth = 250f;

		private const float windowHeight = 250f;

		private const float inputDelay = 1f;

		private bool identifyRequired;

		private Queue<Rewired.Joystick> joysticksToIdentify;

		private float nextInputAllowedTime;

		private GUIStyle style;

		private void Awake()
		{
			if (!ReInput.unityJoystickIdentificationRequired)
			{
				return;
			}
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickConnected);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickDisconnected);
			this.IdentifyAllJoysticks();
		}

		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			this.IdentifyAllJoysticks();
		}

		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			this.IdentifyAllJoysticks();
		}

		public void IdentifyAllJoysticks()
		{
			this.Reset();
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Rewired.Joystick[] joysticks = ReInput.controllers.GetJoysticks();
			if (joysticks == null)
			{
				return;
			}
			this.identifyRequired = true;
			this.joysticksToIdentify = new Queue<Rewired.Joystick>(joysticks);
			this.SetInputDelay();
		}

		private void SetInputDelay()
		{
			this.nextInputAllowedTime = Time.time + 1f;
		}

		private void OnGUI()
		{
			if (!this.identifyRequired)
			{
				return;
			}
			if (this.joysticksToIdentify == null || this.joysticksToIdentify.Count == 0)
			{
				this.Reset();
				return;
			}
			Rect screenRect = new Rect((float)Screen.width * 0.5f - 125f, (float)Screen.height * 0.5f - 125f, 250f, 250f);
			GUILayout.Window(0, screenRect, new GUI.WindowFunction(this.DrawDialogWindow), "Joystick Identification Required", new GUILayoutOption[0]);
			GUI.FocusWindow(0);
			if (Time.time < this.nextInputAllowedTime)
			{
				return;
			}
			if (!ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(this.joysticksToIdentify.Peek().id, 0.8f, false))
			{
				return;
			}
			this.joysticksToIdentify.Dequeue();
			this.SetInputDelay();
			if (this.joysticksToIdentify.Count == 0)
			{
				this.Reset();
			}
		}

		private void DrawDialogWindow(int windowId)
		{
			if (!this.identifyRequired)
			{
				return;
			}
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label);
				this.style.wordWrap = true;
			}
			GUILayout.Space(15f);
			GUILayout.Label("A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:", this.style, new GUILayoutOption[0]);
			Rewired.Joystick joystick = this.joysticksToIdentify.Peek();
			GUILayout.Label("Press any button on \"" + joystick.name + "\" now.", this.style, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Skip", new GUILayoutOption[0]))
			{
				this.joysticksToIdentify.Dequeue();
				return;
			}
		}

		private void Reset()
		{
			this.joysticksToIdentify = null;
			this.identifyRequired = false;
		}
	}
}
