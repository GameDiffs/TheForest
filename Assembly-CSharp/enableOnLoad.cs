using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class enableOnLoad : MonoBehaviour
{
	public bool enableOnStart;

	private bool doingEnable;

	public GameObject go;

	private void Start()
	{
		if (this.enableOnStart)
		{
			base.StartCoroutine("doEnable");
		}
	}

	private void OnDeserialized()
	{
		base.StartCoroutine("doEnable");
	}

	[DebuggerHidden]
	private IEnumerable doEnable()
	{
		enableOnLoad.<doEnable>c__Iterator58 <doEnable>c__Iterator = new enableOnLoad.<doEnable>c__Iterator58();
		<doEnable>c__Iterator.<>f__this = this;
		enableOnLoad.<doEnable>c__Iterator58 expr_0E = <doEnable>c__Iterator;
		expr_0E.$PC = -2;
		return expr_0E;
	}
}
