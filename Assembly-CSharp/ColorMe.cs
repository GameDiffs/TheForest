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
		ColorMe.<DoColorMe>c__Iterator206 <DoColorMe>c__Iterator = new ColorMe.<DoColorMe>c__Iterator206();
		<DoColorMe>c__Iterator.<>f__this = this;
		return <DoColorMe>c__Iterator;
	}
}
