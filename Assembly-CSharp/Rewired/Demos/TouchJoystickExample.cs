using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu(""), RequireComponent(typeof(GUITexture))]
	public class TouchJoystickExample : MonoBehaviour
	{
		private class Boundary
		{
			public Vector2 min = Vector2.zero;

			public Vector2 max = Vector2.zero;
		}

		public bool allowMouseControl = true;

		public bool touchPad;

		public bool fadeGUI;

		public Vector2 deadZone = Vector2.zero;

		public bool normalize;

		public int tapCount = -1;

		private Rect touchZone;

		private int lastFingerId = -1;

		private float tapTimeWindow;

		private Vector2 fingerDownPos;

		private float firstDeltaTime;

		private GUITexture gui;

		private Rect defaultRect;

		private TouchJoystickExample.Boundary guiBoundary = new TouchJoystickExample.Boundary();

		private Vector2 guiTouchOffset;

		private Vector2 guiCenter;

		[NonSerialized]
		private bool initialized;

		private bool mouseActive;

		private int lastScreenWidth;

		private Rect origPixelInset;

		private Vector3 origTransformPosition;

		private static List<TouchJoystickExample> joysticks;

		private static bool enumeratedTouchJoysticks;

		private static float tapTimeDelta = 0.3f;

		public bool isFingerDown
		{
			get
			{
				return this.lastFingerId != -1;
			}
		}

		public int latchedFinger
		{
			set
			{
				if (this.lastFingerId == value)
				{
					this.Restart();
				}
			}
		}

		public Vector2 position
		{
			get;
			private set;
		}

		private void Awake()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			ReInput.EditorRecompileEvent += new Action(this.OnEditorRecompile);
			if (SystemInfo.deviceType == DeviceType.Handheld)
			{
				this.allowMouseControl = false;
			}
			this.gui = base.GetComponent<GUITexture>();
			if (this.gui.texture == null)
			{
				Debug.LogError("TouchJoystick object requires a valid texture!");
				base.gameObject.SetActive(false);
				return;
			}
			if (!TouchJoystickExample.enumeratedTouchJoysticks)
			{
				try
				{
					TouchJoystickExample[] array = (TouchJoystickExample[])UnityEngine.Object.FindObjectsOfType(typeof(TouchJoystickExample));
					TouchJoystickExample.joysticks = new List<TouchJoystickExample>(array.Length);
					TouchJoystickExample[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						TouchJoystickExample item = array2[i];
						TouchJoystickExample.joysticks.Add(item);
					}
					TouchJoystickExample.enumeratedTouchJoysticks = true;
				}
				catch (Exception ex)
				{
					Debug.LogError("Error collecting TouchJoystick objects: " + ex.Message);
					throw;
				}
			}
			this.origPixelInset = this.gui.pixelInset;
			this.origTransformPosition = base.transform.position;
			this.RefreshPosition();
			this.initialized = true;
		}

		private void RefreshPosition()
		{
			this.defaultRect = this.origPixelInset;
			this.defaultRect.x = this.defaultRect.x + this.origTransformPosition.x * (float)Screen.width;
			this.defaultRect.y = this.defaultRect.y + this.origTransformPosition.y * (float)Screen.height;
			this.gui.pixelInset = this.defaultRect;
			base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
			if (this.touchPad)
			{
				this.touchZone = this.defaultRect;
			}
			else
			{
				this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
				this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
				this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
				this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
				this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
				this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
				this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
				this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
			}
			this.lastScreenWidth = Screen.width;
			this.Restart();
		}

		private void OnEditorRecompile()
		{
			this.initialized = false;
			TouchJoystickExample.enumeratedTouchJoysticks = false;
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
			this.position = Vector2.zero;
			this.fingerDownPos = Vector2.zero;
			if (this.touchPad && this.fadeGUI)
			{
				this.gui.color = new Color(this.gui.color.r, this.gui.color.g, this.gui.color.b, 0.025f);
			}
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
			if (!this.initialized)
			{
				this.Initialize();
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
					Vector2 vector;
					int num2;
					int num3;
					TouchPhase touchPhase;
					if (this.mouseActive)
					{
						vector = ReInput.controllers.Mouse.screenPosition;
						num2 = 0;
						num3 = 1;
						touchPhase = TouchPhase.Moved;
					}
					else
					{
						Touch touch = ReInput.touch.GetTouch(i);
						vector = touch.position;
						num2 = touch.fingerId;
						num3 = touch.tapCount;
						touchPhase = touch.phase;
					}
					Vector2 vector2 = vector - this.guiTouchOffset;
					bool flag = false;
					if (this.touchPad && this.touchZone.Contains(vector))
					{
						flag = true;
					}
					else if (this.gui.HitTest(vector))
					{
						flag = true;
					}
					if (flag && (this.lastFingerId == -1 || this.lastFingerId != num2))
					{
						if (this.touchPad)
						{
							if (this.fadeGUI)
							{
								this.gui.color = new Color(this.gui.color.r, this.gui.color.g, this.gui.color.b, 0.15f);
							}
							this.lastFingerId = num2;
							this.fingerDownPos = vector;
						}
						this.lastFingerId = num2;
						if (this.tapTimeWindow > 0f)
						{
							this.tapCount++;
						}
						else
						{
							this.tapCount = 1;
							this.tapTimeWindow = TouchJoystickExample.tapTimeDelta;
						}
						foreach (TouchJoystickExample current in TouchJoystickExample.joysticks)
						{
							if (!(current == this))
							{
								current.latchedFinger = num2;
							}
						}
					}
					if (this.lastFingerId == num2)
					{
						if (num3 > this.tapCount)
						{
							this.tapCount = num3;
						}
						if (this.touchPad)
						{
							this.position = new Vector2(Mathf.Clamp((vector.x - this.fingerDownPos.x) / (this.touchZone.width / 2f), -1f, 1f), Mathf.Clamp((vector.y - this.fingerDownPos.y) / (this.touchZone.height / 2f), -1f, 1f));
						}
						else
						{
							this.gui.pixelInset = new Rect(Mathf.Clamp(vector2.x, this.guiBoundary.min.x, this.guiBoundary.max.x), Mathf.Clamp(vector2.y, this.guiBoundary.min.y, this.guiBoundary.max.y), this.gui.pixelInset.width, this.gui.pixelInset.height);
						}
						if (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled)
						{
							this.Restart();
						}
					}
				}
			}
			if (!this.touchPad)
			{
				this.position = new Vector2((this.gui.pixelInset.x + this.guiTouchOffset.x - this.guiCenter.x) / this.guiTouchOffset.x, (this.gui.pixelInset.y + this.guiTouchOffset.y - this.guiCenter.y) / this.guiTouchOffset.y);
			}
			float num4 = Mathf.Abs(this.position.x);
			float num5 = Mathf.Abs(this.position.y);
			if (num4 < this.deadZone.x)
			{
				this.position = new Vector2(0f, this.position.y);
			}
			else if (this.normalize)
			{
				this.position = new Vector2(Mathf.Sign(this.position.x) * (num4 - this.deadZone.x) / (1f - this.deadZone.x), this.position.y);
			}
			if (num5 < this.deadZone.y)
			{
				this.position = new Vector2(this.position.x, 0f);
			}
			else if (this.normalize)
			{
				this.position = new Vector2(this.position.x, Mathf.Sign(this.position.y) * (num5 - this.deadZone.y) / (1f - this.deadZone.y));
			}
		}
	}
}
