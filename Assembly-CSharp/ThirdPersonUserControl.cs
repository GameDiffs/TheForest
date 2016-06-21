using System;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
	public bool walkByDefault;

	public bool lookInCameraDirection = true;

	private Transform cam;

	private Vector3 lookPos;

	private ThirdPersonCharacter character;

	private void Start()
	{
		this.cam = Camera.main.transform;
		this.character = base.GetComponent<ThirdPersonCharacter>();
	}

	private void FixedUpdate()
	{
		bool key = Input.GetKey(KeyCode.C);
		bool button = CrossPlatformInput.GetButton("Jump");
		float axis = CrossPlatformInput.GetAxis("Horizontal");
		float axis2 = CrossPlatformInput.GetAxis("Vertical");
		Vector3 normalized = Vector3.Scale(this.cam.forward, new Vector3(1f, 0f, 1f)).normalized;
		Vector3 vector = axis2 * normalized + axis * this.cam.right;
		bool key2 = Input.GetKey(KeyCode.LeftShift);
		float d = (!this.walkByDefault) ? ((!key2) ? 1f : 0.5f) : ((!key2) ? 0.5f : 1f);
		vector *= d;
		this.lookPos = ((!this.lookInCameraDirection || !(this.cam != null)) ? (base.transform.position + base.transform.forward * 100f) : (base.transform.position + this.cam.forward * 100f));
		this.character.Move(vector, key, button, this.lookPos);
	}
}
