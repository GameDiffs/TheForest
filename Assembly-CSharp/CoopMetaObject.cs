using Bolt;
using System;

public class CoopMetaObject : EntityBehaviour, IPriorityCalculator
{
	bool IPriorityCalculator.Always
	{
		get
		{
			return true;
		}
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Event evnt)
	{
		return 256f;
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return 256f;
	}
}
