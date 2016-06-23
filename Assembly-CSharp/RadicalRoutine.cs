using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RadicalRoutine : IDeserialized
{
	public bool cancel;

	private IEnumerator extended;

	public IEnumerator enumerator;

	public object Notify;

	public string Method;

	public bool finished;

	public object Target;

	private bool isTracking;

	private MonoBehaviour _trackedObject;

	public event Action Cancelled
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Cancelled = (Action)Delegate.Combine(this.Cancelled, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Cancelled = (Action)Delegate.Remove(this.Cancelled, value);
		}
	}

	public event Action Finished
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Finished = (Action)Delegate.Combine(this.Finished, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Finished = (Action)Delegate.Remove(this.Finished, value);
		}
	}

	public MonoBehaviour trackedObject
	{
		get
		{
			return this._trackedObject;
		}
		set
		{
			this._trackedObject = value;
			this.isTracking = (this._trackedObject != null);
		}
	}

	public RadicalRoutine()
	{
		this.Cancelled = delegate
		{
		};
		this.Finished = delegate
		{
		};
		base..ctor();
	}

	public void Cancel()
	{
		this.cancel = true;
		if (this.extended is INotifyStartStop)
		{
			(this.extended as INotifyStartStop).Stop();
		}
	}

	public static RadicalRoutine Run(IEnumerator extendedCoRoutine)
	{
		return RadicalRoutine.Run(extendedCoRoutine, string.Empty, null);
	}

	public static RadicalRoutine Run(IEnumerator extendedCoRoutine, string methodName)
	{
		return RadicalRoutine.Run(extendedCoRoutine, methodName, null);
	}

	public static RadicalRoutine Run(IEnumerator extendedCoRoutine, string methodName, object target)
	{
		RadicalRoutine radicalRoutine = new RadicalRoutine();
		radicalRoutine.Method = methodName;
		radicalRoutine.Target = target;
		radicalRoutine.extended = extendedCoRoutine;
		if (radicalRoutine.extended is INotifyStartStop)
		{
			(radicalRoutine.extended as INotifyStartStop).Start();
		}
		radicalRoutine.enumerator = radicalRoutine.Execute(extendedCoRoutine);
		RadicalRoutineHelper.Current.Run(radicalRoutine);
		return radicalRoutine;
	}

	public static RadicalRoutine Create(IEnumerator extendedCoRoutine)
	{
		RadicalRoutine radicalRoutine = new RadicalRoutine();
		radicalRoutine.extended = extendedCoRoutine;
		if (radicalRoutine.extended is INotifyStartStop)
		{
			(radicalRoutine.extended as INotifyStartStop).Start();
		}
		radicalRoutine.enumerator = radicalRoutine.Execute(extendedCoRoutine);
		return radicalRoutine;
	}

	public void Run()
	{
		this.Run(string.Empty, null);
	}

	public void Run(string methodName)
	{
		this.Run(methodName, null);
	}

	public void Run(string methodName, object target)
	{
		this.Method = methodName;
		this.Target = target;
		if (this.trackedObject != null)
		{
			RadicalRoutineHelper radicalRoutineHelper = this.trackedObject.GetComponent<RadicalRoutineHelper>() ?? this.trackedObject.gameObject.AddComponent<RadicalRoutineHelper>();
			radicalRoutineHelper.Run(this);
		}
		else
		{
			RadicalRoutineHelper.Current.Run(this);
		}
	}

	private IEnumerator Execute(IEnumerator extendedCoRoutine)
	{
		return this.Execute(extendedCoRoutine, null);
	}

	[DebuggerHidden]
	private IEnumerator Execute(IEnumerator extendedCoRoutine, Action complete)
	{
		RadicalRoutine.<Execute>c__Iterator1CE <Execute>c__Iterator1CE = new RadicalRoutine.<Execute>c__Iterator1CE();
		<Execute>c__Iterator1CE.extendedCoRoutine = extendedCoRoutine;
		<Execute>c__Iterator1CE.complete = complete;
		<Execute>c__Iterator1CE.<$>extendedCoRoutine = extendedCoRoutine;
		<Execute>c__Iterator1CE.<$>complete = complete;
		<Execute>c__Iterator1CE.<>f__this = this;
		return <Execute>c__Iterator1CE;
	}

	public void Deserialized()
	{
	}
}
