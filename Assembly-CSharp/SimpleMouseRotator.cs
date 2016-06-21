using System;
using TheForest.Utils;
using UnityEngine;

public class SimpleMouseRotator : MonoBehaviour
{
	public bool useParent;

	public bool useRigidbody;

	public Vector2 rotationRange = new Vector3(70f, 70f);

	public float rotationSpeed = 10f;

	public float dampingTime = 0.2f;

	public bool autoZeroVerticalOnMobile = true;

	public bool autoZeroHorizontalOnMobile;

	public bool relative = true;

	public bool Xbox = true;

	public bool lockRotation;

	public bool resetOriginalRotation;

	public bool stopInput;

	private Vector3 targetAngles;

	public Vector3 followAngles;

	public float yOffset;

	public float xOffset;

	private Vector3 followVelocity;

	private Quaternion originalRotation;

	private Vector3 originalRotationEuler;

	public float prevSpeed;

	public Vector2 prevRange;

	public Rigidbody rb;

	private void Start()
	{
		this.originalRotation = ((!this.useRigidbody) ? base.transform.localRotation : this.rb.rotation);
		this.originalRotationEuler = this.originalRotation.eulerAngles;
		this.prevSpeed = this.rotationSpeed;
		this.prevRange = this.rotationRange;
	}

	private void OnEnable()
	{
	}

	public void disableMouse()
	{
	}

	public void enableMouse()
	{
	}

	public void updateOriginalRotation()
	{
		this.originalRotation = ((!this.useRigidbody) ? base.transform.localRotation : this.rb.rotation);
		this.originalRotationEuler = this.originalRotation.eulerAngles;
	}

	private void Update()
	{
		if (!LocalPlayer.FpCharacter.Locked && !this.lockRotation)
		{
			if (this.resetOriginalRotation)
			{
				this.originalRotation = ((!this.useRigidbody) ? base.transform.localRotation : this.rb.rotation);
				this.originalRotationEuler = this.originalRotation.eulerAngles;
				this.targetAngles.y = this.targetAngles.y - this.targetAngles.y;
				this.targetAngles.x = this.targetAngles.x - this.targetAngles.x;
				this.followAngles = Vector2.zero;
				this.resetOriginalRotation = false;
			}
			if (this.useParent)
			{
				base.transform.localRotation = this.originalRotation;
			}
			Vector2 zero = Vector2.zero;
			if (this.relative)
			{
				float num2;
				float num3;
				if (!this.Xbox)
				{
					float num = Mathf.Lerp(0.25f, 3f, PlayerPreferences.MouseSensitivity);
					num2 = TheForest.Utils.Input.GetAxis("Mouse X");
					num3 = TheForest.Utils.Input.GetAxis("Mouse Y");
					num2 *= num;
					num3 *= num;
					if (PlayerPreferences.MouseInvert)
					{
						num3 = -num3;
					}
				}
				else
				{
					if ((double)TheForest.Utils.Input.GetAxis("Joy X") > 0.1 || (double)TheForest.Utils.Input.GetAxis("Joy X") < -0.1)
					{
						num2 = TheForest.Utils.Input.GetAxis("Joy X");
					}
					else
					{
						num2 = 0f;
					}
					if ((double)TheForest.Utils.Input.GetAxis("Joy Y") > 0.1 || (double)TheForest.Utils.Input.GetAxis("Joy Y") < -0.1)
					{
						num3 = TheForest.Utils.Input.GetAxis("Joy Y");
					}
					else
					{
						num3 = 0f;
					}
				}
				if (this.targetAngles.y > 180f)
				{
					this.targetAngles.y = this.targetAngles.y - 360f;
					this.followAngles.y = this.followAngles.y - 360f;
				}
				if (this.targetAngles.x > 180f)
				{
					this.targetAngles.x = this.targetAngles.x - 360f;
					this.followAngles.x = this.followAngles.x - 360f;
				}
				if (this.targetAngles.y < -180f)
				{
					this.targetAngles.y = this.targetAngles.y + 360f;
					this.followAngles.y = this.followAngles.y + 360f;
				}
				if (this.targetAngles.x < -180f)
				{
					this.targetAngles.x = this.targetAngles.x + 360f;
					this.followAngles.x = this.followAngles.x + 360f;
				}
				this.targetAngles.y = this.targetAngles.y + num2 * this.rotationSpeed;
				this.targetAngles.x = this.targetAngles.x + num3 * this.rotationSpeed;
				this.targetAngles.y = Mathf.Clamp(this.targetAngles.y, -this.rotationRange.y * 0.5f, this.rotationRange.y * 0.5f);
				this.targetAngles.x = Mathf.Clamp(this.targetAngles.x, -this.rotationRange.x * 0.5f, this.rotationRange.x * 0.5f);
				zero.y = this.targetAngles.y + this.yOffset;
				zero.x = this.targetAngles.x + this.xOffset;
			}
			else
			{
				float num2 = TheForest.Utils.Input.mousePosition.x;
				float num3 = TheForest.Utils.Input.mousePosition.y;
				zero.y = Mathf.Lerp(-this.rotationRange.y * 0.5f, this.rotationRange.y * 0.5f, num2 / (float)Screen.width);
				zero.x = Mathf.Lerp(-this.rotationRange.x * 0.5f, this.rotationRange.x * 0.5f, num3 / (float)Screen.height);
			}
			this.followAngles = Vector3.SmoothDamp(this.followAngles, zero, ref this.followVelocity, this.dampingTime);
			if (this.stopInput)
			{
				return;
			}
			if (this.useParent)
			{
				base.transform.parent.localEulerAngles = base.transform.parent.localEulerAngles + new Vector3(-this.followAngles.x, this.followAngles.y, 0f);
			}
			else if (this.useRigidbody && PlayerPreferences.VSync)
			{
				this.rb.rotation = this.originalRotation * Quaternion.Euler(-this.followAngles.x, this.followAngles.y, 0f);
			}
			else
			{
				base.transform.localRotation = this.originalRotation * Quaternion.Euler(-this.followAngles.x, this.followAngles.y, 0f);
			}
		}
	}
}
