using System;
using UnityEngine;

[RequireComponent(typeof(GUIElement))]
public class AxisTouchButton : MonoBehaviour
{
	public string axisName = "Horizontal";

	public float axisValue = 1f;

	public float responseSpeed = 3f;

	public float returnToCentreSpeed = 3f;

	private AxisTouchButton pairedWith;

	private Rect rect;

	private CrossPlatformInput.VirtualAxis axis;

	private bool pressedThisFrame;

	private float axisCentre;

	private void OnEnable()
	{
		this.axis = (CrossPlatformInput.VirtualAxisReference(this.axisName) ?? new CrossPlatformInput.VirtualAxis(this.axisName));
		this.rect = base.GetComponent<GUIElement>().GetScreenRect();
		this.FindPairedButton();
	}

	private void FindPairedButton()
	{
		AxisTouchButton[] array = UnityEngine.Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].axisName == this.axisName && array[i] != this)
				{
					this.pairedWith = array[i];
					this.axisCentre = (this.axisValue + array[i].axisValue) / 2f;
				}
			}
		}
	}

	private void OnDisable()
	{
		this.axis.Remove();
	}

	private void Update()
	{
		if (this.pairedWith == null)
		{
			this.FindPairedButton();
		}
		this.pressedThisFrame = false;
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (this.rect.Contains(Input.GetTouch(i).position))
			{
				this.axis.Update(Mathf.MoveTowards(this.axis.GetValue, this.axisValue, this.responseSpeed * Time.deltaTime));
				this.pressedThisFrame = true;
			}
		}
	}

	private void LateUpdate()
	{
		if (this.pairedWith != null && !this.pressedThisFrame && !this.pairedWith.pressedThisFrame)
		{
			this.axis.Update(Mathf.MoveTowards(this.axis.GetValue, this.axisCentre, this.returnToCentreSpeed * Time.deltaTime * 0.5f));
		}
	}
}
