using System;
using System.Collections.Generic;
using UnityEngine;

public class MecanimEventEmitterWithData : MonoBehaviour
{
	public UnityEngine.Object animatorController;

	public Animator animator;

	public MecanimEventEmitTypes emitType;

	public MecanimEventData data;

	private Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> loadedData;

	private Dictionary<int, Dictionary<int, AnimatorStateInfo>> lastStates = new Dictionary<int, Dictionary<int, AnimatorStateInfo>>();

	private void Start()
	{
		if (this.animator == null)
		{
			Debug.LogWarning(string.Format("GameObject:{0} cannot find animator component.", base.transform.name));
			base.enabled = false;
			return;
		}
		if (this.animatorController == null)
		{
			Debug.LogWarning("Please assgin animator in editor. Add emitter at runtime is not currently supported.");
			base.enabled = false;
			return;
		}
		if (this.data == null)
		{
			base.enabled = false;
			return;
		}
		this.loadedData = MecanimEventManager.LoadData(this.data);
	}

	private void Update()
	{
		List<MecanimEvent> events = MecanimEventManager.GetEvents(this.loadedData, this.lastStates, this.animatorController.GetInstanceID(), this.animator);
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
		}
	}
}
