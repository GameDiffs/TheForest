using Bolt;
using System;
using UnityEngine;

public class CoopScriptsRemover : EntityBehaviour
{
	public Component[] RemoveInCoop;

	public override void Attached()
	{
		Component[] removeInCoop = this.RemoveInCoop;
		for (int i = 0; i < removeInCoop.Length; i++)
		{
			Component obj = removeInCoop[i];
			UnityEngine.Object.Destroy(obj);
		}
	}
}
