using System;
using UnityEngine;

[RequireComponent(typeof(GUIElement))]
public class Button : MonoBehaviour
{
	public string buttonName = "Fire1";

	public bool pairedWithInputManager;

	private AbstractButton m_Button;

	private void OnEnable()
	{
		this.m_Button = ButtonFactory.GetPlatformSpecificButtonImplementation();
		this.m_Button.Enable(this.buttonName, this.pairedWithInputManager, base.GetComponent<GUIElement>().GetScreenRect());
	}

	private void OnDisable()
	{
		this.m_Button.Disable();
	}

	private void Update()
	{
		this.m_Button.Update();
	}
}
