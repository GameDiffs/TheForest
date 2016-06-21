using System;
using UnityEngine;

namespace Pathfinding.RVO
{
	[AddComponentMenu("Pathfinding/Local Avoidance/Square Obstacle")]
	public class RVOSquareObstacle : RVOObstacle
	{
		public float height = 1f;

		public Vector2 size = Vector3.one;

		public Vector2 center = Vector3.one;

		protected override bool StaticObstacle
		{
			get
			{
				return false;
			}
		}

		protected override bool ExecuteInEditor
		{
			get
			{
				return true;
			}
		}

		protected override bool LocalCoordinates
		{
			get
			{
				return true;
			}
		}

		protected override float Height
		{
			get
			{
				return this.height;
			}
		}

		protected override bool AreGizmosDirty()
		{
			return false;
		}

		protected override void CreateObstacles()
		{
			this.size.x = Mathf.Abs(this.size.x);
			this.size.y = Mathf.Abs(this.size.y);
			this.height = Mathf.Abs(this.height);
			Vector3[] array = new Vector3[]
			{
				new Vector3(1f, 0f, -1f),
				new Vector3(1f, 0f, 1f),
				new Vector3(-1f, 0f, 1f),
				new Vector3(-1f, 0f, -1f)
			};
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale(new Vector3(this.size.x * 0.5f, 0f, this.size.y * 0.5f));
				array[i] += new Vector3(this.center.x, 0f, this.center.y);
			}
			base.AddObstacle(array, this.height);
		}
	}
}
