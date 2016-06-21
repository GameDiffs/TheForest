using Bolt;
using System;
using UnityEngine;

public abstract class CoopBase<T> : EntityEventListener<T>, IPriorityCalculator
{
	[SerializeField]
	public float MultiplayerPriority = 0.5f;

	bool IPriorityCalculator.Always
	{
		get
		{
			return false;
		}
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, this.MultiplayerPriority, 1);
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, this.MultiplayerPriority, skipped);
	}

	public override void OnEvent(SendMessageEvent evnt)
	{
		this.entity.SendMessage(evnt.Message);
	}
}
