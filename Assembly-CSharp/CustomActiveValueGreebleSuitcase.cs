using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CustomActiveValueGreebleSuitcase : CustomActiveValueGreeble
{
	public GameObject _interior;

	public GameObject _trigger;

	public GameObject _pickup;

	public Animation _lid;

	private Quaternion LidDefaultRotation;

	private void Awake()
	{
		this.LidDefaultRotation = this._lid.transform.localRotation;
	}

	private void OnDisable()
	{
		this.UpdateGreebleData();
		this._lid.transform.localRotation = this.LidDefaultRotation;
		this._interior.SetActive(false);
		this._trigger.SetActive(true);
	}

	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		base.StartCoroutine(this.OnEnableRoutine());
	}

	[DebuggerHidden]
	private IEnumerator OnEnableRoutine()
	{
		CustomActiveValueGreebleSuitcase.<OnEnableRoutine>c__Iterator109 <OnEnableRoutine>c__Iterator = new CustomActiveValueGreebleSuitcase.<OnEnableRoutine>c__Iterator109();
		<OnEnableRoutine>c__Iterator.<>f__this = this;
		return <OnEnableRoutine>c__Iterator;
	}

	private void UpdateGreebleData()
	{
		if (base.Data != null)
		{
			if (!this._interior.activeSelf)
			{
				if (this._trigger.activeSelf)
				{
					base.Data._instancesState[base.Index] = 0;
				}
				else
				{
					base.Data._instancesState[base.Index] = 2;
				}
			}
			else if (!this._pickup.activeSelf)
			{
				base.Data._instancesState[base.Index] = 2;
				this._pickup.SetActive(true);
			}
			else
			{
				base.Data._instancesState[base.Index] = 1;
			}
		}
	}
}
