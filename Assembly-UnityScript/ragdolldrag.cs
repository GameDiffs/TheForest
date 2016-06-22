using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class ragdolldrag : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $DragObject$754 : GenericGenerator<object>
	{
		internal float $distance$761;

		internal ragdolldrag $self_$762;

		public $DragObject$754(float distance, ragdolldrag self_)
		{
			this.$distance$761 = distance;
			this.$self_$762 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new ragdolldrag.$DragObject$754.$(this.$distance$761, this.$self_$762);
		}
	}

	public float spring;

	public float damper;

	public float drag;

	public float angularDrag;

	public float distance;

	public bool attachToCenterOfMass;

	private SpringJoint springJoint;

	public ragdolldrag()
	{
		this.spring = 50f;
		this.damper = 5f;
		this.drag = 10f;
		this.angularDrag = 5f;
		this.distance = 0.2f;
	}

	public override void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Camera camera = this.FindCamera();
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out raycastHit, (float)100))
			{
				if (raycastHit.rigidbody && !raycastHit.rigidbody.isKinematic)
				{
					if (!this.springJoint)
					{
						GameObject gameObject = new GameObject("Rigidbody dragger");
						Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>() as Rigidbody;
						this.springJoint = (gameObject.AddComponent<SpringJoint>() as SpringJoint);
						rigidbody.isKinematic = true;
					}
					this.springJoint.transform.position = raycastHit.point;
					if (this.attachToCenterOfMass)
					{
						Vector3 vector = this.transform.TransformDirection(raycastHit.rigidbody.centerOfMass) + raycastHit.rigidbody.transform.position;
						vector = this.springJoint.transform.InverseTransformPoint(vector);
						this.springJoint.anchor = vector;
					}
					else
					{
						this.springJoint.anchor = Vector3.zero;
					}
					this.springJoint.spring = this.spring;
					this.springJoint.damper = this.damper;
					this.springJoint.maxDistance = this.distance;
					this.springJoint.connectedBody = raycastHit.rigidbody;
					this.StartCoroutine("DragObject", raycastHit.distance);
				}
			}
		}
	}

	public override IEnumerator DragObject(float distance)
	{
		return new ragdolldrag.$DragObject$754(distance, this).GetEnumerator();
	}

	public override Camera FindCamera()
	{
		return (!this.GetComponent<Camera>()) ? Camera.main : this.GetComponent<Camera>();
	}

	public override void Main()
	{
	}
}
