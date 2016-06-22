using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SkidTrail : MonoBehaviour
{
	[SerializeField]
	private float persistTime;

	[SerializeField]
	private float fadeDuration;

	private float startAlpha;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		SkidTrail.<Start>c__Iterator116 <Start>c__Iterator = new SkidTrail.<Start>c__Iterator116();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}
}
