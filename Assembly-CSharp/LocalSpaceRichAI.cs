using Pathfinding;
using System;
using UnityEngine;

public class LocalSpaceRichAI : RichAI
{
	public LocalSpaceGraph graph;

	public override void UpdatePath()
	{
		this.canSearchPath = true;
		this.waitingForPathCalc = false;
		Path currentPath = this.seeker.GetCurrentPath();
		if (currentPath != null && !this.seeker.IsDone())
		{
			currentPath.Error();
			currentPath.Claim(this);
			currentPath.Release(this);
		}
		this.waitingForPathCalc = true;
		this.lastRepath = Time.time;
		Matrix4x4 matrix = this.graph.GetMatrix();
		this.seeker.StartPath(matrix.MultiplyPoint3x4(this.tr.position), matrix.MultiplyPoint3x4(this.target.position));
	}

	protected override Vector3 UpdateTarget(RichFunnel fn)
	{
		Matrix4x4 matrix = this.graph.GetMatrix();
		Matrix4x4 inverse = matrix.inverse;
		Debug.DrawRay(matrix.MultiplyPoint3x4(this.tr.position), Vector3.up * 2f, Color.red);
		Debug.DrawRay(inverse.MultiplyPoint3x4(this.tr.position), Vector3.up * 2f, Color.green);
		this.buffer.Clear();
		Vector3 vector = this.tr.position;
		Vector3 vector2 = matrix.MultiplyPoint3x4(vector);
		bool flag;
		vector2 = fn.Update(vector2, this.buffer, 2, out this.lastCorner, out flag);
		vector = inverse.MultiplyPoint3x4(vector2);
		Debug.DrawRay(vector, Vector3.up * 3f, Color.black);
		for (int i = 0; i < this.buffer.Count; i++)
		{
			this.buffer[i] = inverse.MultiplyPoint3x4(this.buffer[i]);
			Debug.DrawRay(this.buffer[i], Vector3.up * 3f, Color.yellow);
		}
		if (flag && !this.waitingForPathCalc)
		{
			this.UpdatePath();
		}
		return vector;
	}
}
