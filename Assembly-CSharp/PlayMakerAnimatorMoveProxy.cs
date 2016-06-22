using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(PlayMakerFSM))]
public class PlayMakerAnimatorMoveProxy : MonoBehaviour
{
	public event Action OnAnimatorMoveEvent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnAnimatorMoveEvent = (Action)Delegate.Combine(this.OnAnimatorMoveEvent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnAnimatorMoveEvent = (Action)Delegate.Remove(this.OnAnimatorMoveEvent, value);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnAnimatorMove()
	{
		if (this.OnAnimatorMoveEvent != null)
		{
			this.OnAnimatorMoveEvent();
		}
	}
}
