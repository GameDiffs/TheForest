using Serialization;
using System;
using UnityEngine;

[ComponentSerializerFor(typeof(NavMeshAgent))]
public class SerializeNavMeshAgent : IComponentSerializer
{
	public class StoredInfo
	{
		public bool hasPath;

		public bool offMesh;

		public float x;

		public float y;

		public float z;

		public float speed;

		public float angularSpeed;

		public float height;

		public float offset;

		public float acceleration;

		public int passable = -1;
	}

	public byte[] Serialize(Component component)
	{
		NavMeshAgent navMeshAgent = (NavMeshAgent)component;
		return UnitySerializer.Serialize(new SerializeNavMeshAgent.StoredInfo
		{
			x = navMeshAgent.destination.x,
			y = navMeshAgent.destination.y,
			z = navMeshAgent.destination.z,
			speed = navMeshAgent.speed,
			acceleration = navMeshAgent.acceleration,
			angularSpeed = navMeshAgent.angularSpeed,
			height = navMeshAgent.height,
			offset = navMeshAgent.baseOffset,
			hasPath = navMeshAgent.hasPath,
			offMesh = navMeshAgent.isOnOffMeshLink,
			passable = navMeshAgent.walkableMask
		});
	}

	public void Deserialize(byte[] data, Component instance)
	{
		NavMeshPath path = new NavMeshPath();
		NavMeshAgent agent = (NavMeshAgent)instance;
		agent.enabled = false;
		Loom.QueueOnMainThread(delegate
		{
			SerializeNavMeshAgent.StoredInfo storedInfo = UnitySerializer.Deserialize<SerializeNavMeshAgent.StoredInfo>(data);
			agent.speed = storedInfo.speed;
			agent.angularSpeed = storedInfo.angularSpeed;
			agent.height = storedInfo.height;
			agent.baseOffset = storedInfo.offset;
			agent.walkableMask = storedInfo.passable;
			if (storedInfo.hasPath && !agent.isOnOffMeshLink)
			{
				agent.enabled = true;
				if (NavMesh.CalculatePath(agent.transform.position, new Vector3(storedInfo.x, storedInfo.y, storedInfo.z), storedInfo.passable, path))
				{
					agent.SetPath(path);
				}
			}
		}, 0.1f);
	}
}
