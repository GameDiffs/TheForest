using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class effigyRangeDetect : MonoBehaviour
{
	public float destroyTime;

	private void Start()
	{
		if (this.destroyTime > 0f)
		{
			base.Invoke("destroyMe", this.destroyTime);
		}
	}

	private void doPulse()
	{
	}

	[DebuggerHidden]
	private IEnumerator pulse()
	{
		effigyRangeDetect.<pulse>c__Iterator56 <pulse>c__Iterator = new effigyRangeDetect.<pulse>c__Iterator56();
		<pulse>c__Iterator.<>f__this = this;
		return <pulse>c__Iterator;
	}

	private void OnDisable()
	{
	}

	private void destroyMe()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
