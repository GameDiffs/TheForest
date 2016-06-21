using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class TimedObjectActivator : MonoBehaviour
{
	public enum Action
	{
		Activate,
		Deactivate,
		Destroy,
		ReloadLevel,
		Call
	}

	[Serializable]
	public class Entry
	{
		public GameObject target;

		public TimedObjectActivator.Action action;

		public float delay;
	}

	[Serializable]
	public class Entries
	{
		public TimedObjectActivator.Entry[] entries;
	}

	public TimedObjectActivator.Entries entries = new TimedObjectActivator.Entries();

	private void Awake()
	{
		TimedObjectActivator.Entry[] array = this.entries.entries;
		for (int i = 0; i < array.Length; i++)
		{
			TimedObjectActivator.Entry entry = array[i];
			switch (entry.action)
			{
			case TimedObjectActivator.Action.Activate:
				base.StartCoroutine(this.Activate(entry));
				break;
			case TimedObjectActivator.Action.Deactivate:
				base.StartCoroutine(this.Deactivate(entry));
				break;
			case TimedObjectActivator.Action.Destroy:
				UnityEngine.Object.Destroy(entry.target, entry.delay);
				break;
			case TimedObjectActivator.Action.ReloadLevel:
				base.StartCoroutine(this.ReloadLevel(entry));
				break;
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator Activate(TimedObjectActivator.Entry entry)
	{
		TimedObjectActivator.<Activate>c__Iterator122 <Activate>c__Iterator = new TimedObjectActivator.<Activate>c__Iterator122();
		<Activate>c__Iterator.entry = entry;
		<Activate>c__Iterator.<$>entry = entry;
		return <Activate>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator Deactivate(TimedObjectActivator.Entry entry)
	{
		TimedObjectActivator.<Deactivate>c__Iterator123 <Deactivate>c__Iterator = new TimedObjectActivator.<Deactivate>c__Iterator123();
		<Deactivate>c__Iterator.entry = entry;
		<Deactivate>c__Iterator.<$>entry = entry;
		return <Deactivate>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ReloadLevel(TimedObjectActivator.Entry entry)
	{
		TimedObjectActivator.<ReloadLevel>c__Iterator124 <ReloadLevel>c__Iterator = new TimedObjectActivator.<ReloadLevel>c__Iterator124();
		<ReloadLevel>c__Iterator.entry = entry;
		<ReloadLevel>c__Iterator.<$>entry = entry;
		return <ReloadLevel>c__Iterator;
	}
}
