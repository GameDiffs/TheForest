using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
	private void Start()
	{
		if (!base.GetComponent<UniqueIdentifier>().IsDeserializing)
		{
			this.StartExtendedCoroutine(this.ScaleMe());
		}
	}

	[DebuggerHidden]
	private IEnumerator ScaleMe()
	{
		ScaleObject.<ScaleMe>c__Iterator20F <ScaleMe>c__Iterator20F = new ScaleObject.<ScaleMe>c__Iterator20F();
		<ScaleMe>c__Iterator20F.<>f__this = this;
		return <ScaleMe>c__Iterator20F;
	}
}
