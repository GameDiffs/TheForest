using System;
using UnityEngine;

[RequireComponent(typeof(GUITexture)), RequireComponent(typeof(Button))]
public class ButtonDownTextureChange : MonoBehaviour
{
	private Button m_Button;

	private GUITexture guiTexture;

	public Texture idleTexture;

	public Texture activeTexture;

	private bool down;

	private void OnEnable()
	{
		this.m_Button = base.GetComponent<Button>();
		this.guiTexture = base.GetComponent<GUITexture>();
	}

	private void Update()
	{
		if (CrossPlatformInput.GetButtonDown(this.m_Button.buttonName) && !this.down)
		{
			this.guiTexture.texture = this.activeTexture;
			this.down = true;
		}
		if (CrossPlatformInput.GetButtonUp("NextCamera") && this.down)
		{
			this.guiTexture.texture = this.idleTexture;
			this.down = false;
		}
	}
}
