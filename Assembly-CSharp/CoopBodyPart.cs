using Bolt;
using System;
using TheForest.Buildings.World;
using TheForest.Items;
using UnityEngine;

public class CoopBodyPart : EntityBehaviour<IPartState>, IPriorityCalculator
{
	[ItemIdPicker]
	public int itemid;

	public bool isTorso;

	bool IPriorityCalculator.Always
	{
		get
		{
			return true;
		}
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, 1);
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, skipped + 1);
	}

	public override void Attached()
	{
		if (BoltNetwork.isClient)
		{
			base.state.AddCallback("Effigy", new PropertyCallback(this.EffigyChanged));
		}
		base.state.Transform.SetTransforms(base.transform);
	}

	private void EffigyChanged(IState _, string propertyPath, ArrayIndices arrayIndices)
	{
		if (base.state.Effigy)
		{
			Component[] componentsInChildren = base.state.Effigy.GetComponentsInChildren(typeof(EffigyArchitect), true);
			if (componentsInChildren.Length > 0)
			{
				EffigyArchitect effigyArchitect = componentsInChildren[0] as EffigyArchitect;
				if (effigyArchitect)
				{
					effigyArchitect._parts.Add(new EffigyArchitect.Part
					{
						_itemId = (!this.isTorso) ? this.itemid : -2,
						_position = base.transform.position,
						_rotation = base.transform.rotation.eulerAngles
					});
					base.transform.parent = effigyArchitect.transform;
				}
			}
		}
	}
}
