using System;
using UnityEngine;

[DoNotSerializePublic]
public class CutGrass : MonoBehaviour
{
	[Range(0.5f, 40f)]
	public float Radius = 5f;

	[Range(1f, 2f)]
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

	private void Start()
	{
		this.CutteyCut();
	}

	public void CutteyCut()
	{
		int stepCount = this.GetStepCount();
		float num = this.Radius + this.Length;
		num *= num;
		for (int i = 0; i < stepCount; i++)
		{
			float progress = (float)i / (float)stepCount;
			Vector3 position = this.GetPosition(progress);
			NeoGrassCutter.Cut(position, this.Radius, false);
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
