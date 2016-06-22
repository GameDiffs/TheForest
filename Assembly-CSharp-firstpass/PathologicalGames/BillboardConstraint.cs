using System;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Billboard")]
	public class BillboardConstraint : LookAtBaseClass
	{
		public bool vertical = true;

		protected override void Awake()
		{
			base.Awake();
			Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
			Camera[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Camera camera = array2[i];
				if ((camera.cullingMask & 1 << base.gameObject.layer) > 0)
				{
					base.target = camera.transform;
					break;
				}
			}
		}

		protected override void OnConstraintUpdate()
		{
			Vector3 a = this.xform.position + base.target.rotation * Vector3.back;
			Vector3 upwards = Vector3.up;
			if (this.vertical)
			{
				upwards = base.target.rotation * Vector3.up;
			}
			else
			{
				a.y = this.xform.position.y;
			}
			Vector3 forward = a - this.xform.position;
			Quaternion lookRot = Quaternion.LookRotation(forward, upwards);
			this.xform.rotation = base.GetUserLookRotation(lookRot);
		}
	}
}
