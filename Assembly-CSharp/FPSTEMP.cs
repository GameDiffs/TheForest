using System;
using System.Collections;
using UnityEngine;

public class FPSTEMP : MonoBehaviour
{
	[Serializable]
	public class AdvancedSettings
	{
		public float gravityMultiplier = 1f;

		public PhysicMaterial zeroFrictionMaterial;

		public PhysicMaterial highFrictionMaterial;

		public float groundStickyEffect = 5f;
	}

	private class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	private const float jumpRayLength = 0.7f;

	[SerializeField]
	private float runSpeed = 8f;

	[SerializeField]
	private float strafeSpeed = 4f;

	[SerializeField]
	private float jumpPower = 5f;

	[SerializeField]
	private bool walkByDefault = true;

	[SerializeField]
	private float walkSpeed = 3f;

	[SerializeField]
	private FPSTEMP.AdvancedSettings advanced = new FPSTEMP.AdvancedSettings();

	[SerializeField]
	private bool lockCursor = true;

	private CapsuleCollider capsule;

	private Vector2 input;

	private IComparer rayHitComparer;

	public bool grounded
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.capsule = (base.GetComponent<Collider>() as CapsuleCollider);
		this.grounded = true;
		Screen.lockCursor = this.lockCursor;
		this.rayHitComparer = new FPSTEMP.RayHitComparer();
	}

	private void OnDisable()
	{
		Screen.lockCursor = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Screen.lockCursor = this.lockCursor;
		}
	}

	public void FixedUpdate()
	{
		float d = this.runSpeed;
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		bool button = Input.GetButton("Jump");
		bool key = Input.GetKey(KeyCode.LeftShift);
		d = ((!this.walkByDefault) ? ((!key) ? this.runSpeed : this.walkSpeed) : ((!key) ? this.walkSpeed : this.runSpeed));
		this.input = new Vector2(axis, axis2);
		if (this.input.sqrMagnitude > 1f)
		{
			this.input.Normalize();
		}
		Vector3 a = base.transform.forward * this.input.y * d + base.transform.right * this.input.x * this.strafeSpeed;
		float num = base.GetComponent<Rigidbody>().velocity.y;
		if (this.grounded && button)
		{
			num += this.jumpPower;
			this.grounded = false;
		}
		base.GetComponent<Rigidbody>().velocity = a + Vector3.up * num;
		if (a.magnitude > 0f || !this.grounded)
		{
			base.GetComponent<Collider>().material = this.advanced.zeroFrictionMaterial;
		}
		else
		{
			base.GetComponent<Collider>().material = this.advanced.highFrictionMaterial;
		}
		Ray ray = new Ray(base.transform.position, -base.transform.up);
		RaycastHit[] array = Physics.RaycastAll(ray, this.capsule.height * 0.7f);
		Array.Sort(array, this.rayHitComparer);
		if (this.grounded || base.GetComponent<Rigidbody>().velocity.y < this.jumpPower * 0.5f)
		{
			this.grounded = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].collider.isTrigger)
				{
					this.grounded = true;
					base.GetComponent<Rigidbody>().position = Vector3.MoveTowards(base.GetComponent<Rigidbody>().position, array[i].point + Vector3.up * this.capsule.height * 0.5f, Time.deltaTime * this.advanced.groundStickyEffect);
					base.GetComponent<Rigidbody>().velocity = new Vector3(base.GetComponent<Rigidbody>().velocity.x, 0f, base.GetComponent<Rigidbody>().velocity.z);
					break;
				}
			}
		}
		base.GetComponent<Rigidbody>().AddForce(Physics.gravity * (this.advanced.gravityMultiplier - 1f));
	}
}
