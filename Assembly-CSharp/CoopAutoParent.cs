using Bolt;
using System;
using System.Collections;
using System.Diagnostics;

public class CoopAutoParent : EntityBehaviour, IEntityReplicationFilter
{
	private bool _ready;

	bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
	{
		if (base.transform.parent)
		{
			BoltEntity component = base.transform.parent.GetComponent<BoltEntity>();
			return this._ready && connection.ExistsOnRemote(component) == ExistsResult.Yes;
		}
		return this._ready;
	}

	public override void Attached()
	{
		base.StartCoroutine(this.DelayedAttached());
	}

	[DebuggerHidden]
	private IEnumerator DelayedAttached()
	{
		CoopAutoParent.<DelayedAttached>c__Iterator17 <DelayedAttached>c__Iterator = new CoopAutoParent.<DelayedAttached>c__Iterator17();
		<DelayedAttached>c__Iterator.<>f__this = this;
		return <DelayedAttached>c__Iterator;
	}
}
