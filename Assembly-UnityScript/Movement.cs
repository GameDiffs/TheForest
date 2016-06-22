using System;
using UnityEngine;

[Serializable]
public class Movement : MonoBehaviour
{
	private CharacterController controller;

	public Texture2D texture;

	private Vector3 moveDirection;

	private Vector3 forward;

	private Vector3 right;

	public Movement()
	{
		this.moveDirection = Vector3.zero;
		this.forward = Vector3.zero;
		this.right = Vector3.zero;
	}

	public override void Start()
	{
		this.texture = null;
	}

	public override void Update()
	{
		this.forward = this.transform.forward;
		this.right = new Vector3(this.forward.z, (float)0, -this.forward.x);
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		Vector3 target = axisRaw * this.right + axisRaw2 * this.forward;
		this.moveDirection = Vector3.RotateTowards(this.moveDirection, target, 3.49065852f * Time.deltaTime, (float)1000);
		Vector3 motion = this.moveDirection * Time.deltaTime * (float)10;
		this.controller.Move(motion);
	}

	public override void Main()
	{
		this.controller = (CharacterController)this.gameObject.GetComponent(typeof(CharacterController));
	}
}
