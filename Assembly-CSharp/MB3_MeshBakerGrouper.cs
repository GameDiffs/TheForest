using System;
using UnityEngine;

public class MB3_MeshBakerGrouper : MonoBehaviour
{
	public MB3_MeshBakerGrouperCore grouper;

	[HideInInspector]
	public Bounds sourceObjectBounds = new Bounds(Vector3.zero, Vector3.one);

	private void OnDrawGizmosSelected()
	{
		if (this.grouper == null)
		{
			return;
		}
		if (this.grouper.clusterGrouper == null)
		{
			return;
		}
		if (this.grouper.clusterGrouper.clusterType == MB3_MeshBakerGrouperCore.ClusterType.grid)
		{
			Vector3 cellSize = this.grouper.clusterGrouper.cellSize;
			if (cellSize.x <= 1E-05f || cellSize.y <= 1E-05f || cellSize.z <= 1E-05f)
			{
				return;
			}
			Vector3 vector = this.sourceObjectBounds.center - this.sourceObjectBounds.extents;
			Vector3 origin = this.grouper.clusterGrouper.origin;
			origin.x %= cellSize.x;
			origin.y %= cellSize.y;
			origin.z %= cellSize.z;
			vector.x = Mathf.Round(vector.x / cellSize.x) * cellSize.x + origin.x;
			vector.y = Mathf.Round(vector.y / cellSize.y) * cellSize.y + origin.y;
			vector.z = Mathf.Round(vector.z / cellSize.z) * cellSize.z + origin.z;
			if (vector.x > this.sourceObjectBounds.center.x - this.sourceObjectBounds.extents.x)
			{
				vector.x -= cellSize.x;
			}
			if (vector.y > this.sourceObjectBounds.center.y - this.sourceObjectBounds.extents.y)
			{
				vector.y -= cellSize.y;
			}
			if (vector.z > this.sourceObjectBounds.center.z - this.sourceObjectBounds.extents.z)
			{
				vector.z -= cellSize.z;
			}
			Vector3 vector2 = vector;
			int num = Mathf.CeilToInt(this.sourceObjectBounds.size.x / cellSize.x + this.sourceObjectBounds.size.y / cellSize.y + this.sourceObjectBounds.size.z / cellSize.z);
			if (num > 200)
			{
				Gizmos.DrawWireCube(this.grouper.clusterGrouper.origin + cellSize / 2f, cellSize);
			}
			else
			{
				while (vector.x < this.sourceObjectBounds.center.x + this.sourceObjectBounds.extents.x)
				{
					vector.y = vector2.y;
					while (vector.y < this.sourceObjectBounds.center.y + this.sourceObjectBounds.extents.y)
					{
						vector.z = vector2.z;
						while (vector.z < this.sourceObjectBounds.center.z + this.sourceObjectBounds.extents.z)
						{
							Gizmos.DrawWireCube(vector + cellSize / 2f, cellSize);
							vector.z += cellSize.z;
						}
						vector.y += cellSize.y;
					}
					vector.x += cellSize.x;
				}
			}
		}
		if (this.grouper.clusterGrouper.clusterType == MB3_MeshBakerGrouperCore.ClusterType.pie)
		{
			if (this.grouper.clusterGrouper.pieAxis.magnitude < 0.1f)
			{
				return;
			}
			if (this.grouper.clusterGrouper.pieNumSegments < 1)
			{
				return;
			}
			float magnitude = this.sourceObjectBounds.extents.magnitude;
			MB3_MeshBakerGrouper.DrawCircle(this.grouper.clusterGrouper.pieAxis, this.grouper.clusterGrouper.origin, magnitude, 24);
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, this.grouper.clusterGrouper.pieAxis);
			Quaternion rotation2 = Quaternion.AngleAxis(180f / (float)this.grouper.clusterGrouper.pieNumSegments, Vector3.up);
			Vector3 point = rotation2 * Vector3.forward;
			for (int i = 0; i < this.grouper.clusterGrouper.pieNumSegments; i++)
			{
				Vector3 a = rotation * point;
				Gizmos.DrawLine(this.grouper.clusterGrouper.origin, this.grouper.clusterGrouper.origin + a * magnitude);
				point = rotation2 * point;
				point = rotation2 * point;
			}
		}
	}

	public static void DrawCircle(Vector3 axis, Vector3 center, float radius, int subdiv)
	{
		Quaternion rotation = Quaternion.AngleAxis((float)(360 / subdiv), axis);
		Vector3 vector = new Vector3(axis.y, -axis.x, axis.z);
		vector.Normalize();
		vector *= radius;
		for (int i = 0; i < subdiv + 1; i++)
		{
			Vector3 vector2 = rotation * vector;
			Gizmos.DrawLine(center + vector, center + vector2);
			vector = vector2;
		}
	}
}
