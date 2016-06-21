using Pathfinding;
using System;
using UnityEngine;

public class BezierMover : MonoBehaviour
{
	public Transform[] points;

	public float tangentLengths = 5f;

	public float speed = 1f;

	private float time;

	private void Update()
	{
		this.Move(true);
	}

	private Vector3 Plot(float t)
	{
		int num = this.points.Length;
		int num2 = Mathf.FloorToInt(t);
		Vector3 normalized = ((this.points[(num2 + 1) % num].position - this.points[num2 % num].position).normalized - (this.points[(num2 - 1 + num) % num].position - this.points[num2 % num].position).normalized).normalized;
		Vector3 normalized2 = ((this.points[(num2 + 2) % num].position - this.points[(num2 + 1) % num].position).normalized - (this.points[(num2 + num) % num].position - this.points[(num2 + 1) % num].position).normalized).normalized;
		Debug.DrawLine(this.points[num2 % num].position, this.points[num2 % num].position + normalized * this.tangentLengths, Color.red);
		Debug.DrawLine(this.points[(num2 + 1) % num].position - normalized2 * this.tangentLengths, this.points[(num2 + 1) % num].position, Color.green);
		return AstarMath.CubicBezier(this.points[num2 % num].position, this.points[num2 % num].position + normalized * this.tangentLengths, this.points[(num2 + 1) % num].position - normalized2 * this.tangentLengths, this.points[(num2 + 1) % num].position, t - (float)num2);
	}

	private void Move(bool progress)
	{
		float num = this.time;
		float num2 = this.time + 1f;
		while (num2 - num > 0.0001f)
		{
			float num3 = (num + num2) / 2f;
			Vector3 a = this.Plot(num3);
			if ((a - base.transform.position).sqrMagnitude > this.speed * Time.deltaTime * (this.speed * Time.deltaTime))
			{
				num2 = num3;
			}
			else
			{
				num = num3;
			}
		}
		this.time = (num + num2) / 2f;
		Vector3 vector = this.Plot(this.time);
		Vector3 a2 = this.Plot(this.time + 0.001f);
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(a2 - vector);
	}

	public void OnDrawGizmos()
	{
		if (this.points.Length >= 3)
		{
			for (int i = 0; i < this.points.Length; i++)
			{
				if (this.points[i] == null)
				{
					return;
				}
			}
			for (int j = 0; j < this.points.Length; j++)
			{
				int num = this.points.Length;
				Vector3 normalized = ((this.points[(j + 1) % num].position - this.points[j].position).normalized - (this.points[(j - 1 + num) % num].position - this.points[j].position).normalized).normalized;
				Vector3 normalized2 = ((this.points[(j + 2) % num].position - this.points[(j + 1) % num].position).normalized - (this.points[(j + num) % num].position - this.points[(j + 1) % num].position).normalized).normalized;
				Vector3 from = this.points[j].position;
				for (int k = 1; k <= 100; k++)
				{
					Vector3 vector = AstarMath.CubicBezier(this.points[j].position, this.points[j].position + normalized * this.tangentLengths, this.points[(j + 1) % num].position - normalized2 * this.tangentLengths, this.points[(j + 1) % num].position, (float)k / 100f);
					Gizmos.DrawLine(from, vector);
					from = vector;
				}
			}
		}
	}
}
