using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), HutongGames.PlayMaker.Tooltip("Draw gizmos shape")]
	public class DebugDrawShape : FsmStateAction
	{
		public enum ShapeType
		{
			Sphere,
			Cube,
			WireSphere,
			WireCube
		}

		[RequiredField]
		public FsmOwnerDefault gameObject;

		public DebugDrawShape.ShapeType shape;

		public FsmColor color;

		[HutongGames.PlayMaker.Tooltip("Use this for sphere gizmos")]
		public FsmFloat radius;

		[HutongGames.PlayMaker.Tooltip("Use this for cube gizmos")]
		public FsmVector3 size;

		public override void Reset()
		{
			this.gameObject = null;
			this.shape = DebugDrawShape.ShapeType.Sphere;
			this.color = Color.grey;
			this.radius = 1f;
			this.size = new Vector3(1f, 1f, 1f);
		}

		public override void OnDrawGizmos()
		{
			Transform transform = base.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
			if (transform == null)
			{
				return;
			}
			Gizmos.color = this.color.Value;
			switch (this.shape)
			{
			case DebugDrawShape.ShapeType.Sphere:
				Gizmos.DrawSphere(transform.position, this.radius.Value);
				break;
			case DebugDrawShape.ShapeType.Cube:
				Gizmos.DrawCube(transform.position, this.size.Value);
				break;
			case DebugDrawShape.ShapeType.WireSphere:
				Gizmos.DrawWireSphere(transform.position, this.radius.Value);
				break;
			case DebugDrawShape.ShapeType.WireCube:
				Gizmos.DrawWireCube(transform.position, this.size.Value);
				break;
			}
		}
	}
}
