using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PlayMakerFSM)), RequireComponent(typeof(Animator))]
public class PlayMakerAnimatorIKProxy : MonoBehaviour
{
	private Animator _animator;

	public event Action<int> OnAnimatorIKEvent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnAnimatorIKEvent = (Action<int>)Delegate.Combine(this.OnAnimatorIKEvent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnAnimatorIKEvent = (Action<int>)Delegate.Remove(this.OnAnimatorIKEvent, value);
		}
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (this.OnAnimatorIKEvent != null)
		{
			this.OnAnimatorIKEvent(layerIndex);
		}
	}
}
