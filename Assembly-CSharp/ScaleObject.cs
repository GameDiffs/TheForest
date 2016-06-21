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
		ScaleObject.<ScaleMe>c__Iterator207 <ScaleMe>c__Iterator = new ScaleObject.<ScaleMe>c__Iterator207();
		<ScaleMe>c__Iterator.<>f__this = this;
		return <ScaleMe>c__Iterator;
	}
}
