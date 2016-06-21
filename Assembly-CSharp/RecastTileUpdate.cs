using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Pathfinding/Navmesh/RecastTileUpdate")]
public class RecastTileUpdate : MonoBehaviour
{
	public static event Action<Bounds> OnNeedUpdates
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			RecastTileUpdate.OnNeedUpdates = (Action<Bounds>)Delegate.Combine(RecastTileUpdate.OnNeedUpdates, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			RecastTileUpdate.OnNeedUpdates = (Action<Bounds>)Delegate.Remove(RecastTileUpdate.OnNeedUpdates, value);
		}
	}

	private void Start()
	{
		this.ScheduleUpdate();
	}

	private void OnDestroy()
	{
		this.ScheduleUpdate();
	}

	public void ScheduleUpdate()
	{
		Collider component = base.GetComponent<Collider>();
		if (component != null)
		{
			if (RecastTileUpdate.OnNeedUpdates != null)
			{
				RecastTileUpdate.OnNeedUpdates(component.bounds);
			}
		}
		else if (RecastTileUpdate.OnNeedUpdates != null)
		{
			RecastTileUpdate.OnNeedUpdates(new Bounds(base.transform.position, Vector3.zero));
		}
	}
}
