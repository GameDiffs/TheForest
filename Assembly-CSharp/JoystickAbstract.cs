using System;
using UnityEngine;

public abstract class JoystickAbstract : MonoBehaviour
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

	public Vector2 deadZone = Vector2.zero;

	public bool normalize;

	public Vector2 autoReturnSpeed = new Vector2(4f, 4f);

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	public JoystickAbstract.AxisOption axesToUse;

	public bool invertX;

	public bool invertY;

	public GUITexture touchZone;

	public float touchZonePadding;

	public JoystickAbstract.ReturnStyleOption autoReturnStyle = JoystickAbstract.ReturnStyleOption.Curved;

	public float sensitivity = 1f;

	public float interpolateTime = 2f;

	public Vector2 startPosition = Vector2.zero;

	protected static JoystickAbstract[] joysticks;

	protected static bool enumeratedJoysticks;

	protected Rect touchZoneRect;

	protected Vector2 position;

	protected int lastFingerId = -1;

	protected GUITexture gui;

	protected Rect defaultRect;

	protected Rect guiBoundary = default(Rect);

	protected Vector2 guiTouchOffset;

	protected Vector2 guiCenter;

	protected bool moveStick;

	protected bool touchPad;

	protected CrossPlatformInput.VirtualAxis horizontalVirtualAxis;

	protected CrossPlatformInput.VirtualAxis verticalVirtualAxis;

	protected bool useX;

	protected bool useY;

	protected bool getTouchZoneRect;

	protected Vector2 lastTouchPos;

	protected Vector2 touchDelta;

	protected Vector2 touchStart;

	protected float swipeScale;

	protected virtual void TypeSpecificOnEnable()
	{
	}

	protected void OnEnable()
	{
		this.CreateVirtualAxes();
		this.gui = base.GetComponent<GUITexture>();
		if (this.gui != null)
		{
			this.defaultRect = this.gui.GetScreenRect();
			this.gui.pixelInset = this.defaultRect;
			base.transform.localScale = Vector3.zero;
		}
		base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
		this.moveStick = true;
		this.TypeSpecificOnEnable();
		if (JoystickAbstract.enumeratedJoysticks)
		{
			return;
		}
		JoystickAbstract.joysticks = UnityEngine.Object.FindObjectsOfType<JoystickAbstract>();
		JoystickAbstract.enumeratedJoysticks = true;
	}

	private void CreateVirtualAxes()
	{
		this.useX = (this.axesToUse == JoystickAbstract.AxisOption.Both || this.axesToUse == JoystickAbstract.AxisOption.OnlyHorizontal);
		this.useY = (this.axesToUse == JoystickAbstract.AxisOption.Both || this.axesToUse == JoystickAbstract.AxisOption.OnlyVertical);
		if (this.useX)
		{
			this.horizontalVirtualAxis = new CrossPlatformInput.VirtualAxis(this.horizontalAxisName);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis = new CrossPlatformInput.VirtualAxis(this.verticalAxisName);
		}
	}

	protected void OnDisable()
	{
		JoystickAbstract.enumeratedJoysticks = false;
		if (this.useX)
		{
			this.horizontalVirtualAxis.Remove();
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Remove();
		}
	}

	protected void ResetJoystick()
	{
		this.lastFingerId = -1;
	}

	protected internal virtual void LatchedFinger(int fingerId)
	{
		if (this.lastFingerId == fingerId)
		{
			this.ResetJoystick();
		}
	}

	protected virtual void TypeSpecificUpdate()
	{
	}

	protected virtual void ZeroWhenUnused()
	{
	}

	protected virtual void ForEachTouch(Touch touch, Vector2 guiTouchPos)
	{
	}

	protected virtual void MoveJoystickGraphic()
	{
	}

	public void Update()
	{
		this.ZeroWhenUnused();
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
				Vector2 guiTouchPos = touch.position - this.guiTouchOffset;
				this.ForEachTouch(touch, guiTouchPos);
			}
		}
		this.MoveJoystickGraphic();
		float x = this.position.x;
		float y = this.position.y;
		this.DeadZoneAndNormaliseAxes(ref x, ref y);
		this.AdjustAxesIfInverted(ref x, ref y);
		this.UpdateVirtualAxes(x, y);
	}

	private void DeadZoneAndNormaliseAxes(ref float modifiedX, ref float modifiedY)
	{
		float num = Mathf.Abs(modifiedX);
		float num2 = Mathf.Abs(modifiedY);
		if (num < this.deadZone.x)
		{
			modifiedX = 0f;
		}
		else if (this.normalize)
		{
			modifiedX = Mathf.Sign(modifiedX) * (num - this.deadZone.x) / (1f - this.deadZone.x);
		}
		if (num2 < this.deadZone.y)
		{
			modifiedY = 0f;
		}
		else if (this.normalize)
		{
			modifiedY = Mathf.Sign(modifiedY) * (num2 - this.deadZone.y) / (1f - this.deadZone.y);
		}
	}

	private void AdjustAxesIfInverted(ref float modifiedX, ref float modifiedY)
	{
		modifiedX *= (float)((!this.invertX) ? 1 : -1);
		modifiedY *= (float)((!this.invertY) ? 1 : -1);
	}

	private void UpdateVirtualAxes(float modifiedX, float modifiedY)
	{
		if (this.useX)
		{
			this.horizontalVirtualAxis.Update(modifiedX);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Update(modifiedY);
		}
	}
}
