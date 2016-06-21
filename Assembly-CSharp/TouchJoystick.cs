using System;
using UnityEngine;

public class TouchJoystick : MonoBehaviour
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public enum ReturnStyleOption
	{
		Linear,
		Curved
	}

	public enum InputMode
	{
		Joystick,
		TouchPadPositional,
		TouchPadRelativePositional,
		TouchPadSwipe
	}

	public enum SensitivityRelativeTo
	{
		ZoneSize,
		Resolution
	}

	private class Boundary
	{
		public Vector2 min = Vector2.zero;

		public Vector2 max = Vector2.zero;
	}

	public Vector2 deadZone = Vector2.zero;

	public bool normalize;

	public Vector2 autoReturnSpeed = new Vector2(4f, 4f);

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	public TouchJoystick.AxisOption axesToUse;

	public bool invertX;

	public bool invertY;

	public TouchJoystick.InputMode inputMode;

	public GUITexture touchZone;

	public float touchZonePadding;

	public TouchJoystick.ReturnStyleOption autoReturnStyle = TouchJoystick.ReturnStyleOption.Curved;

	public float sensitivity = 1f;

	public float interpolateTime = 2f;

	public Vector2 startPosition = Vector2.zero;

	public bool relativeSensitivity;

	public TouchJoystick.SensitivityRelativeTo sensitivityRelativeTo;

	private static TouchJoystick[] joysticks;

	private static bool enumeratedJoysticks;

	private Rect touchZoneRect;

	private Vector2 position;

	private int lastFingerId = -1;

	private GUITexture gui;

	private Rect defaultRect;

	private TouchJoystick.Boundary guiBoundary = new TouchJoystick.Boundary();

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	private bool moveStick;

	private bool touchPad;

	private CrossPlatformInput.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInput.VirtualAxis verticalVirtualAxis;

	private bool useX;

	private bool useY;

	private bool getTouchZoneRect;

	private Vector2 lastTouchPos;

	private Vector2 touchDelta;

	private Vector2 touchStart;

	private float swipeScale;

	private float sensitivityRelativeX;

	private float sensitivityRelativeY;

	private void OnEnable()
	{
		this.useX = (this.axesToUse == TouchJoystick.AxisOption.Both || this.axesToUse == TouchJoystick.AxisOption.OnlyHorizontal);
		this.useY = (this.axesToUse == TouchJoystick.AxisOption.Both || this.axesToUse == TouchJoystick.AxisOption.OnlyVertical);
		if (this.useX)
		{
			this.horizontalVirtualAxis = new CrossPlatformInput.VirtualAxis(this.horizontalAxisName);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis = new CrossPlatformInput.VirtualAxis(this.verticalAxisName);
		}
		this.gui = base.GetComponent<GUITexture>();
		if (this.gui != null)
		{
			this.defaultRect = this.gui.GetScreenRect();
		}
		base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
		this.moveStick = true;
		if (this.inputMode == TouchJoystick.InputMode.TouchPadPositional || this.inputMode == TouchJoystick.InputMode.TouchPadRelativePositional || this.inputMode == TouchJoystick.InputMode.TouchPadSwipe)
		{
			this.touchPad = true;
			this.getTouchZoneRect = true;
			if (this.gui == null)
			{
				this.moveStick = false;
			}
			else if (this.touchZone == null)
			{
				this.touchZone = this.gui;
				this.moveStick = false;
			}
			else
			{
				this.moveStick = true;
			}
		}
		else
		{
			this.touchPad = false;
			this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
			this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
			this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
			this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
			this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.moveStick = true;
		}
		if (this.gui != null)
		{
			this.gui.pixelInset = this.defaultRect;
			base.transform.localScale = Vector3.zero;
		}
	}

	private void OnDisable()
	{
		TouchJoystick.enumeratedJoysticks = false;
		if (this.useX)
		{
			this.horizontalVirtualAxis.Remove();
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Remove();
		}
	}

	private void ResetJoystick()
	{
		this.lastFingerId = -1;
	}

	private void LatchedFinger(int fingerId)
	{
		if (this.lastFingerId == fingerId)
		{
			this.ResetJoystick();
		}
	}

	public void Update()
	{
		if (this.touchPad && this.getTouchZoneRect)
		{
			this.getTouchZoneRect = false;
			this.touchZoneRect = this.touchZone.GetScreenRect();
			Vector2 center = this.touchZoneRect.center;
			this.touchZoneRect.width = this.touchZoneRect.width * (1f - this.touchZonePadding);
			this.touchZoneRect.height = this.touchZoneRect.height * (1f - this.touchZonePadding);
			this.touchZoneRect.center = center;
			this.position = this.startPosition;
			Vector2 vector = new Vector2(this.touchZoneRect.width, this.touchZoneRect.height);
			this.swipeScale = vector.magnitude * 0.01f;
			TouchJoystick.SensitivityRelativeTo sensitivityRelativeTo = this.sensitivityRelativeTo;
			if (sensitivityRelativeTo != TouchJoystick.SensitivityRelativeTo.ZoneSize)
			{
				if (sensitivityRelativeTo == TouchJoystick.SensitivityRelativeTo.Resolution)
				{
					float num = (Screen.dpi <= 0f) ? 100f : Screen.dpi;
					this.sensitivityRelativeX = num;
					this.sensitivityRelativeY = num;
				}
			}
			else
			{
				this.sensitivityRelativeX = this.touchZoneRect.width;
				this.sensitivityRelativeY = this.touchZoneRect.height;
			}
		}
		if (this.lastFingerId == -1 || this.inputMode == TouchJoystick.InputMode.TouchPadSwipe)
		{
			if (this.touchPad)
			{
				if (this.autoReturnStyle == TouchJoystick.ReturnStyleOption.Curved)
				{
					this.position.x = Mathf.Lerp(this.position.x, 0f, Time.deltaTime * this.autoReturnSpeed.x);
					this.position.y = Mathf.Lerp(this.position.y, 0f, Time.deltaTime * this.autoReturnSpeed.y);
				}
				else
				{
					this.position.x = Mathf.MoveTowards(this.position.x, 0f, Time.deltaTime * this.autoReturnSpeed.x);
					this.position.y = Mathf.MoveTowards(this.position.y, 0f, Time.deltaTime * this.autoReturnSpeed.y);
				}
			}
			else
			{
				Rect pixelInset = this.gui.pixelInset;
				if (this.autoReturnStyle == TouchJoystick.ReturnStyleOption.Curved)
				{
					pixelInset.x = Mathf.Lerp(pixelInset.x, this.defaultRect.x, Time.deltaTime * this.autoReturnSpeed.x * this.guiTouchOffset.x);
					pixelInset.y = Mathf.Lerp(pixelInset.y, this.defaultRect.y, Time.deltaTime * this.autoReturnSpeed.y * this.guiTouchOffset.y);
				}
				else
				{
					pixelInset.x = Mathf.MoveTowards(pixelInset.x, this.defaultRect.x, Time.deltaTime * this.autoReturnSpeed.x * this.guiTouchOffset.x);
					pixelInset.y = Mathf.MoveTowards(pixelInset.y, this.defaultRect.y, Time.deltaTime * this.autoReturnSpeed.y * this.guiTouchOffset.y);
				}
				this.gui.pixelInset = pixelInset;
			}
		}
		if (!TouchJoystick.enumeratedJoysticks)
		{
			TouchJoystick.joysticks = UnityEngine.Object.FindObjectsOfType<TouchJoystick>();
			TouchJoystick.enumeratedJoysticks = true;
		}
		int touchCount = Input.touchCount;
		if (touchCount == 0)
		{
			this.ResetJoystick();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 vector2 = touch.position - this.guiTouchOffset;
				bool flag = false;
				if (this.touchPad)
				{
					if (this.touchZoneRect.Contains(touch.position))
					{
						flag = true;
					}
				}
				else if (this.gui.HitTest(touch.position))
				{
					flag = true;
				}
				if (flag && (this.lastFingerId == -1 || this.lastFingerId != touch.fingerId))
				{
					if (this.touchPad)
					{
						this.lastFingerId = touch.fingerId;
					}
					this.lastFingerId = touch.fingerId;
					for (int j = 0; j < TouchJoystick.joysticks.Length; j++)
					{
						if (TouchJoystick.joysticks[j] != this)
						{
							TouchJoystick.joysticks[j].LatchedFinger(touch.fingerId);
						}
					}
				}
				if (this.lastFingerId == touch.fingerId)
				{
					if (this.touchPad)
					{
						switch (this.inputMode)
						{
						case TouchJoystick.InputMode.TouchPadPositional:
						{
							Vector2 a = new Vector2((touch.position.x - this.touchZoneRect.center.x) / this.sensitivityRelativeX, (touch.position.y - this.touchZoneRect.center.y) / this.sensitivityRelativeY) * 2f;
							Vector2 vector3 = Vector2.Lerp(this.position, a * this.sensitivity, Time.deltaTime * this.interpolateTime);
							if (this.useX)
							{
								this.position.x = Mathf.Clamp(vector3.x, -1f, 1f);
							}
							if (this.useY)
							{
								this.position.y = Mathf.Clamp(vector3.y, -1f, 1f);
							}
							break;
						}
						case TouchJoystick.InputMode.TouchPadRelativePositional:
						{
							if (touch.phase == TouchPhase.Began)
							{
								this.touchStart = touch.position;
							}
							Vector2 a2 = new Vector2((touch.position.x - this.touchStart.x) / this.sensitivityRelativeX, (touch.position.y - this.touchStart.y) / this.sensitivityRelativeY);
							Vector2 vector4 = Vector2.Lerp(this.position, a2 * this.sensitivity * 2f, Time.deltaTime * this.interpolateTime);
							if (this.useX)
							{
								this.position.x = Mathf.Clamp(vector4.x, -1f, 1f);
							}
							if (this.useY)
							{
								this.position.y = Mathf.Clamp(vector4.y, -1f, 1f);
							}
							break;
						}
						case TouchJoystick.InputMode.TouchPadSwipe:
							if (touch.phase == TouchPhase.Began)
							{
								this.lastTouchPos = touch.position;
								this.touchDelta = Vector2.zero;
							}
							this.touchDelta = Vector2.Lerp(this.touchDelta, (this.lastTouchPos - touch.position) / this.swipeScale, Time.deltaTime * this.interpolateTime);
							if (touch.deltaTime > 0f)
							{
								if (this.useX)
								{
									float x = this.touchDelta.x * this.sensitivity;
									this.position.x = x;
								}
								if (this.useY)
								{
									float y = this.touchDelta.y * this.sensitivity;
									this.position.y = y;
								}
							}
							this.lastTouchPos = touch.position;
							break;
						}
					}
					else
					{
						this.gui.pixelInset = new Rect(Mathf.Clamp(vector2.x, this.guiBoundary.min.x, this.guiBoundary.max.x), Mathf.Clamp(vector2.y, this.guiBoundary.min.y, this.guiBoundary.max.y), this.gui.pixelInset.width, this.gui.pixelInset.height);
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						this.ResetJoystick();
					}
				}
			}
		}
		if (this.touchPad && this.moveStick)
		{
			this.gui.pixelInset = new Rect(Mathf.Lerp(this.touchZoneRect.x, this.touchZoneRect.x + this.touchZoneRect.width, this.position.x * 0.5f + 0.5f) - this.defaultRect.width * 0.5f, Mathf.Lerp(this.touchZoneRect.y, this.touchZoneRect.y + this.touchZoneRect.height, this.position.y * 0.5f + 0.5f) - this.defaultRect.height * 0.5f, this.defaultRect.width, this.defaultRect.height);
		}
		if (!this.touchPad)
		{
			if (this.useX)
			{
				this.position.x = (this.gui.pixelInset.x + this.guiTouchOffset.x - this.guiCenter.x) / this.guiTouchOffset.x;
			}
			if (this.useY)
			{
				this.position.y = (this.gui.pixelInset.y + this.guiTouchOffset.y - this.guiCenter.y) / this.guiTouchOffset.y;
			}
		}
		float num2 = this.position.x;
		float num3 = this.position.y;
		float num4 = Mathf.Abs(num2);
		float num5 = Mathf.Abs(num3);
		if (num4 < this.deadZone.x)
		{
			num2 = 0f;
		}
		else if (this.normalize)
		{
			num2 = Mathf.Sign(num2) * (num4 - this.deadZone.x) / (1f - this.deadZone.x);
		}
		if (num5 < this.deadZone.y)
		{
			num3 = 0f;
		}
		else if (this.normalize)
		{
			num3 = Mathf.Sign(num3) * (num5 - this.deadZone.y) / (1f - this.deadZone.y);
		}
		num2 *= (float)((!this.invertX) ? 1 : -1);
		num3 *= (float)((!this.invertY) ? 1 : -1);
		if (this.useX)
		{
			this.horizontalVirtualAxis.Update(num2);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Update(num3);
		}
	}
}
