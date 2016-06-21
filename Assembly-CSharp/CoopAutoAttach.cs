using Bolt;
using System;
using System.Collections;
using System.Diagnostics;

public class CoopAutoAttach : EntityBehaviour
{
	private void OnEnable()
	{
		if (BoltNetwork.isRunning && !base.transform.root.CompareTag("CaveProps"))
		{
			base.StartCoroutine(this.DelayedOnEnable());
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayedOnEnable()
	{
		CoopAutoAttach.<DelayedOnEnable>c__Iterator16 <DelayedOnEnable>c__Iterator = new CoopAutoAttach.<DelayedOnEnable>c__Iterator16();
		<DelayedOnEnable>c__Iterator.<>f__this = this;
		return <DelayedOnEnable>c__Iterator;
	}
}
