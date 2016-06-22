using System;
using UnityEngine;

[Serializable]
public class GenerateRainbow : MonoBehaviour
{
	public int segments;

	public float zSpacing;

	public float ySpacing;

	public bool curveTypeA;

	public override void New()
	{
		LineRenderer lineRenderer = (LineRenderer)this.GetComponent(typeof(LineRenderer));
		lineRenderer.SetVertexCount(this.segments);
		for (float num = (float)0; num < (float)this.segments; num += (float)1)
		{
			if (this.curveTypeA)
			{
				lineRenderer.SetPosition((int)num, new Vector3((float)0, num * this.ySpacing, num * num * this.zSpacing));
			}
			else
			{
				lineRenderer.SetPosition((int)num, new Vector3((float)0, -num * (num - (float)this.segments) / (float)(3 * this.segments) * this.ySpacing, num * this.zSpacing));
			}
		}
	}

	public override void Main()
	{
	}
}
