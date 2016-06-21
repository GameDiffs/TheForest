using System;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{
	private PlatformerCharacter2D character;

	private bool jump;

	private void Awake()
	{
		this.character = base.GetComponent<PlatformerCharacter2D>();
	}

	private void Update()
	{
		if (CrossPlatformInput.GetButtonDown("Jump"))
		{
			this.jump = true;
		}
	}

	private void FixedUpdate()
	{
		bool key = Input.GetKey(KeyCode.LeftControl);
		float axis = CrossPlatformInput.GetAxis("Horizontal");
		this.character.Move(axis, key, this.jump);
		this.jump = false;
	}
}
