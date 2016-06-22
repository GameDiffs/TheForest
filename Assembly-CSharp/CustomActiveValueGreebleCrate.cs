using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CustomActiveValueGreebleCrate : CustomActiveValueGreeble
{
	public GameObject _interior;

	public GameObject _trigger;

	public GameObject[] _pickups;

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
		CustomActiveValueGreebleCrate.<OnEnableRoutine>c__Iterator108 <OnEnableRoutine>c__Iterator = new CustomActiveValueGreebleCrate.<OnEnableRoutine>c__Iterator108();
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
			else
			{
				byte b = 0;
				for (int i = 0; i < this._pickups.Length; i++)
				{
					GameObject gameObject = this._pickups[i];
					if (!gameObject)
					{
						b |= (byte)(1 << i);
					}
					else if (!gameObject.activeSelf)
					{
						b |= (byte)(1 << i);
						gameObject.SetActive(true);
					}
				}
				base.Data._instancesState[base.Index] = (byte)(((int)b << 1) + 1);
				base.Data = null;
			}
		}
	}
}
