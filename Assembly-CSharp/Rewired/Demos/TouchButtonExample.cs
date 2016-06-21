using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu(""), RequireComponent(typeof(GUITexture))]
	public class TouchButtonExample : MonoBehaviour
	{
		private class Boundary
		{
			public Vector2 min = Vector2.zero;

			public Vector2 max = Vector2.zero;
		}

		public bool allowMouseControl = true;

		public int tapCount = -1;

		private int lastFingerId = -1;

		private float tapTimeWindow;

		private float firstDeltaTime;

		private GUITexture gui;

		private Rect defaultRect;

		private TouchButtonExample.Boundary guiBoundary = new TouchButtonExample.Boundary();

		private Vector2 guiTouchOffset;

		private Vector2 guiCenter;

		private bool mouseActive;

		private int lastScreenWidth;

		private Rect origPixelInset;

		private Vector3 origTransformPosition;

		private static List<TouchButtonExample> buttons;

		private static float tapTimeDelta = 0.3f;

		public bool isFingerDown
		{
			get
			{
				return this.lastFingerId != -1;
			}
		}

		public bool isPressed
		{
			get;
			private set;
		}

		private void Reset()
		{
		}

		private void Awake()
		{
			if (SystemInfo.deviceType == DeviceType.Handheld)
			{
				this.allowMouseControl = false;
			}
			this.gui = base.GetComponent<GUITexture>();
			if (this.gui.texture == null)
			{
				Debug.LogError("TouchButton object requires a valid texture!");
				base.gameObject.SetActive(false);
				return;
			}
			this.origPixelInset = this.gui.pixelInset;
			this.origTransformPosition = base.transform.position;
			this.RefreshPosition();
		}

		private void RefreshPosition()
		{
			this.defaultRect = this.origPixelInset;
			this.defaultRect.x = this.defaultRect.x + this.origTransformPosition.x * (float)Screen.width;
			this.defaultRect.y = this.defaultRect.y + this.origTransformPosition.y * (float)Screen.height;
			this.gui.pixelInset = this.defaultRect;
			base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
			this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
			this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
			this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
			this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
			this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.lastScreenWidth = Screen.width;
			this.Restart();
		}

		public void Enable()
		{
			base.enabled = true;
		}

		public void Disable()
		{
			base.enabled = false;
		}

		public void Restart()
		{
			this.gui.pixelInset = this.defaultRect;
			this.lastFingerId = -1;
			this.isPressed = false;
			this.mouseActive = false;
		}

		private void Update()
		{
			if (this.lastScreenWidth != Screen.width)
			{
				this.RefreshPosition();
			}
			if (!ReInput.isReady)
			{
				return;
			}
			if (this.mouseActive && !ReInput.controllers.Mouse.GetButton(0))
			{
				this.mouseActive = false;
			}
			int num;
			if (this.allowMouseControl && (this.mouseActive || (ReInput.controllers.Mouse.GetButtonDown(0) && this.gui.HitTest(ReInput.controllers.Mouse.screenPosition))))
			{
				num = 1;
				this.mouseActive = true;
			}
			else
			{
				num = ReInput.touch.touchCount;
				if (this.mouseActive)
				{
					this.mouseActive = false;
				}
			}
			if (this.tapTimeWindow > 0f)
			{
				this.tapTimeWindow -= Time.deltaTime;
			}
			else
			{
				this.tapCount = 0;
			}
			if (num == 0)
			{
				this.Restart();
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					Vector2 v;
					int num2;
					int num3;
					TouchPhase touchPhase;
					if (this.mouseActive)
					{
						v = ReInput.controllers.Mouse.screenPosition;
						num2 = 0;
						num3 = 1;
						touchPhase = TouchPhase.Moved;
					}
					else
					{
						Touch touch = ReInput.touch.GetTouch(i);
						v = touch.position;
						num2 = touch.fingerId;
						num3 = touch.tapCount;
						touchPhase = touch.phase;
					}
					if (this.gui.HitTest(v))
					{
						if (this.lastFingerId == -1 || this.lastFingerId != num2)
						{
							this.lastFingerId = num2;
							if (this.tapTimeWindow > 0f)
							{
								this.tapCount++;
							}
							else
							{
								this.tapCount = 1;
								this.tapTimeWindow = TouchButtonExample.tapTimeDelta;
							}
						}
						if (this.lastFingerId == num2)
						{
							if (num3 > this.tapCount)
							{
								this.tapCount = num3;
							}
							this.isPressed = true;
							if (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled)
							{
								this.Restart();
							}
						}
					}
				}
			}
		}
	}
}
