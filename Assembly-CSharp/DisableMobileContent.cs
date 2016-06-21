using System;
using UnityEngine;

[ExecuteInEditMode]
public class DisableMobileContent : MonoBehaviour
{
	public bool enableMobileControls;

	private bool mobileControlsPreviousState;

	private void Awake()
	{
		this.enableMobileControls = false;
		this.SetMobileControlsStatus(this.enableMobileControls);
		this.mobileControlsPreviousState = this.enableMobileControls;
	}

	private void UpdateControlStatus()
	{
		if (this.mobileControlsPreviousState != this.enableMobileControls)
		{
			this.SetMobileControlsStatus(this.enableMobileControls);
			this.mobileControlsPreviousState = this.enableMobileControls;
		}
	}

	private void SetMobileControlsStatus(bool activeStatus)
	{
		foreach (Transform transform in base.transform)
		{
			transform.transform.gameObject.SetActive(activeStatus);
		}
	}
}
