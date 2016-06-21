using System;
using System.Collections.Generic;
using UnityEngine;

public class MecanimEventEmitter : MonoBehaviour
{
	public UnityEngine.Object animatorController;

	public Animator animator;

	public MecanimEventEmitTypes emitType;

	private void Start()
	{
		if (this.animator == null)
		{
			base.enabled = false;
			return;
		}
		if (this.animatorController == null)
		{
			base.enabled = false;
			return;
		}
	}

	private void Update()
	{
		List<MecanimEvent> events = MecanimEventManager.GetEvents(this.animatorController.GetInstanceID(), this.animator);
		for (int i = 0; i < events.Count; i++)
		{
			MecanimEvent mecanimEvent = events[i];
			MecanimEvent.SetCurrentContext(mecanimEvent);
			MecanimEventEmitTypes mecanimEventEmitTypes = this.emitType;
			if (mecanimEventEmitTypes != MecanimEventEmitTypes.Upwards)
			{
				if (mecanimEventEmitTypes != MecanimEventEmitTypes.Broadcast)
				{
					if (mecanimEvent.paramType != MecanimEventParamTypes.None)
					{
						base.SendMessage(mecanimEvent.functionName, mecanimEvent.parameter, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						base.SendMessage(mecanimEvent.functionName, SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (mecanimEvent.paramType != MecanimEventParamTypes.None)
				{
					base.BroadcastMessage(mecanimEvent.functionName, mecanimEvent.parameter, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					base.BroadcastMessage(mecanimEvent.functionName, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (mecanimEvent.paramType != MecanimEventParamTypes.None)
			{
				base.SendMessageUpwards(mecanimEvent.functionName, mecanimEvent.parameter, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				base.SendMessageUpwards(mecanimEvent.functionName, SendMessageOptions.DontRequireReceiver);
			}
			MecanimEventManager.PoolEvent(mecanimEvent);
		}
		events.Clear();
	}
}
