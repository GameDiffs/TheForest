using System;
using System.Collections;
using System.Diagnostics;

public class ColorMe : MonoBehaviourEx
{
	private void Start()
	{
		if (!base.GetComponent<UniqueIdentifier>().IsDeserializing)
		{
			base.StartCoroutine("DoColorMe");
		}
	}

	[DebuggerHidden]
	private IEnumerator DoColorMe()
	{
		ColorMe.<DoColorMe>c__Iterator20E <DoColorMe>c__Iterator20E = new ColorMe.<DoColorMe>c__Iterator20E();
		<DoColorMe>c__Iterator20E.<>f__this = this;
		return <DoColorMe>c__Iterator20E;
	}
}
