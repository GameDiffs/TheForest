using Rewired;
using System;
using TheForest.UI;
using UnityEngine;

namespace TheForest.Utils
{
	public class Input : MonoBehaviour
	{
		public bool isGamePad;

		public static Player player;

		public static float DelayedActionStartTime;

		public static float DelayedActionAlpha;

		public static string DelayedActionName = string.Empty;

		public static bool DelayedActionIsDown;

		public static bool DelayedActionWasUpdated;

		public static bool IsGamePad
		{
			get;
			private set;
		}

		public static bool IsMouseLocked
		{
			get;
			private set;
		}

		public static Vector3 mousePosition
		{
			get
			{
				if (!Input.IsGamePad)
				{
					return Input.player.controllers.Mouse.screenPosition;
				}
				return VirtualCursor.Instance.Position;
			}
		}

		private void Awake()
		{
			if (Input.player == null)
			{
				Input.player = ReInput.players.GetPlayer(0);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void LateUpdate()
		{
			if (ReInput.isReady)
			{
				Controller lastActiveController = Input.player.controllers.GetLastActiveController();
				if (lastActiveController != null)
				{
					bool flag = lastActiveController != Input.player.controllers.Keyboard && lastActiveController != Input.player.controllers.Mouse;
					Input.IsGamePad = flag;
					this.isGamePad = flag;
				}
				UICamera.disableController = !this.isGamePad;
			}
			if (Input.DelayedActionIsDown && !Input.DelayedActionWasUpdated)
			{
				Input.GetButtonAfterDelay(Input.DelayedActionName, 0.5f);
			}
		}

		private void OnDestroy()
		{
			Input.player = null;
		}

		public static bool GetButtonDown(string button)
		{
			return Input.player.GetButtonDown(button);
		}

		public static bool GetButtonUp(string button)
		{
			return Input.player.GetButtonUp(button);
		}

		public static bool GetKeyDown(KeyCode key)
		{
			return Input.player.controllers.Keyboard.GetKeyDown(key);
		}

		public static bool GetButton(string button)
		{
			return Input.player.GetButton(button);
		}

		public static bool GetButtonAfterDelay(string button, float delay)
		{
			if (!Input.DelayedActionIsDown)
			{
				if (Input.player.GetButtonDown(button))
				{
					Input.DelayedActionStartTime = Time.realtimeSinceStartup;
					Input.DelayedActionAlpha = 0f;
					Input.DelayedActionIsDown = true;
					Input.DelayedActionWasUpdated = true;
					Input.DelayedActionName = button;
				}
			}
			else
			{
				if (Input.DelayedActionName != button)
				{
					return false;
				}
				if (Input.player.GetButton(button) && Input.DelayedActionAlpha < 1f)
				{
					Input.DelayedActionAlpha = Mathf.Clamp01((Time.realtimeSinceStartup - Input.DelayedActionStartTime) / delay);
					if (Mathf.Approximately(Input.DelayedActionAlpha, 1f))
					{
						Input.DelayedActionAlpha = 0f;
						Input.DelayedActionIsDown = false;
						Input.DelayedActionWasUpdated = true;
						return true;
					}
					Input.DelayedActionWasUpdated = true;
				}
				else if (Input.DelayedActionWasUpdated)
				{
					Input.DelayedActionWasUpdated = false;
				}
				else
				{
					Input.DelayedActionAlpha = 0f;
					Input.DelayedActionIsDown = false;
					Input.DelayedActionWasUpdated = false;
				}
			}
			return false;
		}

		public static void ResetDelayedAction()
		{
			Input.DelayedActionAlpha = 0f;
			Input.DelayedActionIsDown = false;
			Input.DelayedActionWasUpdated = false;
			Input.DelayedActionName = string.Empty;
		}

		public static float GetAxis(string axis)
		{
			return Input.player.GetAxis(axis);
		}

		public static void LockMouse()
		{
			if (!CoopPeerStarter.DedicatedHost)
			{
				Input.IsMouseLocked = true;
			}
		}

		public static void UnLockMouse()
		{
			Input.IsMouseLocked = false;
		}
	}
}
