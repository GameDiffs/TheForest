using System;
using System.Collections.Generic;
using UnityEngine;

public static class MecanimEventManager
{
	private static List<MecanimEvent> allEvents = new List<MecanimEvent>(10);

	private static Stack<MecanimEvent> pooledEvents = new Stack<MecanimEvent>();

	private static MecanimEventData[] eventDataSources;

	private static Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> globalLoadedData;

	private static Dictionary<int, Dictionary<int, AnimatorStateInfo>> globalLastStates = new Dictionary<int, Dictionary<int, AnimatorStateInfo>>();

	public static void SetEventDataSource(MecanimEventData dataSource)
	{
		if (dataSource != null)
		{
			MecanimEventManager.eventDataSources = new MecanimEventData[1];
			MecanimEventManager.eventDataSources[0] = dataSource;
			MecanimEventManager.LoadGlobalData();
		}
	}

	public static void SetEventDataSource(MecanimEventData[] dataSources)
	{
		if (dataSources != null)
		{
			MecanimEventManager.eventDataSources = dataSources;
			MecanimEventManager.LoadGlobalData();
		}
	}

	public static void OnLevelLoaded()
	{
		MecanimEventManager.globalLastStates.Clear();
	}

	private static MecanimEvent GetPooledEvent()
	{
		MecanimEvent mecanimEvent;
		if (MecanimEventManager.pooledEvents.Count > 0)
		{
			mecanimEvent = MecanimEventManager.pooledEvents.Pop();
		}
		else
		{
			mecanimEvent = new MecanimEvent();
			mecanimEvent.SetContext(new EventContext());
		}
		return mecanimEvent;
	}

	public static void PoolEvent(MecanimEvent me)
	{
		me.Reset();
		me.GetContext().Reset();
		MecanimEventManager.pooledEvents.Push(me);
	}

	public static List<MecanimEvent> GetEvents(int animatorControllerId, Animator animator)
	{
		return MecanimEventManager.GetEvents(MecanimEventManager.globalLoadedData, MecanimEventManager.globalLastStates, animatorControllerId, animator);
	}

	public static List<MecanimEvent> GetEvents(Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> contextLoadedData, Dictionary<int, Dictionary<int, AnimatorStateInfo>> contextLastStates, int animatorControllerId, Animator animator)
	{
		int hashCode = animator.GetHashCode();
		if (!contextLastStates.ContainsKey(hashCode))
		{
			contextLastStates[hashCode] = new Dictionary<int, AnimatorStateInfo>();
		}
		int layerCount = animator.layerCount;
		Dictionary<int, AnimatorStateInfo> dictionary = contextLastStates[hashCode];
		for (int i = 0; i < layerCount; i++)
		{
			if (!dictionary.ContainsKey(i))
			{
				dictionary[i] = default(AnimatorStateInfo);
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
			int num = (int)dictionary[i].normalizedTime;
			int num2 = (int)currentAnimatorStateInfo.normalizedTime;
			float normalizedTimeStart = dictionary[i].normalizedTime - (float)num;
			float normalizedTimeEnd = currentAnimatorStateInfo.normalizedTime - (float)num2;
			if (dictionary[i].fullPathHash == currentAnimatorStateInfo.fullPathHash)
			{
				if (currentAnimatorStateInfo.loop)
				{
					if (num == num2)
					{
						MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, currentAnimatorStateInfo.fullPathHash, currentAnimatorStateInfo.tagHash, normalizedTimeStart, normalizedTimeEnd, false);
					}
					else
					{
						MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, currentAnimatorStateInfo.fullPathHash, currentAnimatorStateInfo.tagHash, normalizedTimeStart, 1.00001f, false);
						MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, currentAnimatorStateInfo.fullPathHash, currentAnimatorStateInfo.tagHash, 0f, normalizedTimeEnd, false);
					}
				}
				else
				{
					float num3 = Mathf.Clamp01(dictionary[i].normalizedTime);
					float num4 = Mathf.Clamp01(currentAnimatorStateInfo.normalizedTime);
					if (num == 0 && num2 == 0)
					{
						if (num3 != num4)
						{
							MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, currentAnimatorStateInfo.fullPathHash, currentAnimatorStateInfo.tagHash, num3, num4, false);
						}
					}
					else if (num == 0 && num2 > 0)
					{
						MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, dictionary[i].fullPathHash, dictionary[i].tagHash, num3, 1.00001f, false);
					}
				}
			}
			else
			{
				MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, currentAnimatorStateInfo.fullPathHash, currentAnimatorStateInfo.tagHash, 0f, normalizedTimeEnd, false);
				if (!dictionary[i].loop)
				{
					MecanimEventManager.CollectEvents(MecanimEventManager.allEvents, contextLoadedData, animator, animatorControllerId, i, dictionary[i].fullPathHash, dictionary[i].tagHash, normalizedTimeStart, 1.00001f, true);
				}
			}
			dictionary[i] = currentAnimatorStateInfo;
		}
		return MecanimEventManager.allEvents;
	}

	private static void CollectEvents(List<MecanimEvent> allEvents, Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> contextLoadedData, Animator animator, int animatorControllerId, int layer, int nameHash, int tagHash, float normalizedTimeStart, float normalizedTimeEnd, bool onlyCritical = false)
	{
		if (contextLoadedData.ContainsKey(animatorControllerId) && contextLoadedData[animatorControllerId].ContainsKey(layer) && contextLoadedData[animatorControllerId][layer].ContainsKey(nameHash))
		{
			List<MecanimEvent> list = contextLoadedData[animatorControllerId][layer][nameHash];
			for (int i = 0; i < list.Count; i++)
			{
				MecanimEvent mecanimEvent = list[i];
				if (mecanimEvent.isEnable)
				{
					if (mecanimEvent.normalizedTime >= normalizedTimeStart && mecanimEvent.normalizedTime < normalizedTimeEnd && mecanimEvent.condition.Test(animator))
					{
						if (!onlyCritical || mecanimEvent.critical)
						{
							MecanimEvent pooledEvent = MecanimEventManager.GetPooledEvent();
							pooledEvent.Copy(mecanimEvent);
							EventContext context = pooledEvent.GetContext();
							context.controllerId = animatorControllerId;
							context.layer = layer;
							context.stateHash = nameHash;
							context.tagHash = tagHash;
							pooledEvent.SetContext(context);
							allEvents.Add(pooledEvent);
						}
					}
				}
			}
			return;
		}
	}

	private static void LoadGlobalData()
	{
		if (MecanimEventManager.eventDataSources == null)
		{
			return;
		}
		MecanimEventManager.globalLoadedData = new Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>>();
		MecanimEventData[] array = MecanimEventManager.eventDataSources;
		for (int i = 0; i < array.Length; i++)
		{
			MecanimEventData mecanimEventData = array[i];
			if (!(mecanimEventData == null))
			{
				MecanimEventDataEntry[] data = mecanimEventData.data;
				MecanimEventDataEntry[] array2 = data;
				for (int j = 0; j < array2.Length; j++)
				{
					MecanimEventDataEntry mecanimEventDataEntry = array2[j];
					int instanceID = mecanimEventDataEntry.animatorController.GetInstanceID();
					if (!MecanimEventManager.globalLoadedData.ContainsKey(instanceID))
					{
						MecanimEventManager.globalLoadedData[instanceID] = new Dictionary<int, Dictionary<int, List<MecanimEvent>>>();
					}
					if (!MecanimEventManager.globalLoadedData[instanceID].ContainsKey(mecanimEventDataEntry.layer))
					{
						MecanimEventManager.globalLoadedData[instanceID][mecanimEventDataEntry.layer] = new Dictionary<int, List<MecanimEvent>>();
					}
					MecanimEventManager.globalLoadedData[instanceID][mecanimEventDataEntry.layer][mecanimEventDataEntry.stateNameHash] = new List<MecanimEvent>(mecanimEventDataEntry.events);
				}
			}
		}
	}

	public static Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> LoadData(MecanimEventData data)
	{
		Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> dictionary = new Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>>();
		MecanimEventDataEntry[] data2 = data.data;
		MecanimEventDataEntry[] array = data2;
		for (int i = 0; i < array.Length; i++)
		{
			MecanimEventDataEntry mecanimEventDataEntry = array[i];
			int instanceID = mecanimEventDataEntry.animatorController.GetInstanceID();
			if (!dictionary.ContainsKey(instanceID))
			{
				dictionary[instanceID] = new Dictionary<int, Dictionary<int, List<MecanimEvent>>>();
			}
			if (!dictionary[instanceID].ContainsKey(mecanimEventDataEntry.layer))
			{
				dictionary[instanceID][mecanimEventDataEntry.layer] = new Dictionary<int, List<MecanimEvent>>();
			}
			dictionary[instanceID][mecanimEventDataEntry.layer][mecanimEventDataEntry.stateNameHash] = new List<MecanimEvent>(mecanimEventDataEntry.events);
		}
		return dictionary;
	}
}
