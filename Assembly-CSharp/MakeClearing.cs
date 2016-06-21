using System;
using System.Collections.Generic;
using UnityEngine;

public class MakeClearing : MonoBehaviour
{
	public bool ShouldCutGrass;

	[Range(0.5f, 40f)]
	public float Radius = 5f;

	[Range(1f, 200f)]
	public float Length = 1f;

	private int GetStepCount()
	{
		return (int)(this.Length / this.Radius) + 1;
	}

	private Vector3 GetPosition(float progress)
	{
		progress = Mathf.Clamp01(progress);
		Vector3 vector = base.transform.position - base.transform.forward * (progress * this.Length);
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain)
		{
			vector.y = activeTerrain.transform.position.y + activeTerrain.SampleHeight(vector);
		}
		return vector;
	}

	public void Start()
	{
		int stepCount = this.GetStepCount();
		LOD_Base[] array = UnityEngine.Object.FindObjectsOfType<LOD_Base>();
		List<LOD_Base> list = new List<LOD_Base>();
		Vector3 position = base.transform.position;
		float num = this.Radius + this.Length;
		num *= num;
		float num2 = this.Radius * this.Radius;
		LOD_Base[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			LOD_Base lOD_Base = array2[i];
			if ((lOD_Base.transform.position - position).sqrMagnitude < num)
			{
				list.Add(lOD_Base);
			}
		}
		for (int j = 0; j < stepCount; j++)
		{
			float progress = (float)j / (float)stepCount;
			Vector3 position2 = this.GetPosition(progress);
			foreach (LOD_Base current in list)
			{
				if ((current.transform.position - position2).sqrMagnitude < num2)
				{
					CoopTreeId coopTreeId = null;
					if (BoltNetwork.isServer)
					{
						coopTreeId = current.GetComponent<CoopTreeId>();
					}
					UnityEngine.Object.Destroy(current);
					if (coopTreeId)
					{
						coopTreeId.Goto_Removed();
					}
				}
			}
			if (this.ShouldCutGrass)
			{
				NeoGrassCutter.Cut(position2, this.Radius, false);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		int stepCount = this.GetStepCount();
		Color color = Gizmos.color;
		Gizmos.color = Color.red;
		for (int i = 0; i < stepCount; i++)
		{
			float progress = (float)i / (float)stepCount;
			Vector3 position = this.GetPosition(progress);
			Gizmos.DrawWireSphere(position, this.Radius);
		}
		Gizmos.color = color;
	}
}
