using System;
using UnityEngine;

[Serializable]
public class CameraGUIOff : MonoBehaviour
{
	public Camera GuiCam;

	public override void Awake()
	{
		this.GuiCam = GameObject.FindGameObjectWithTag("GuiCam").GetComponent<Camera>();
	}

	public override void GuiOff()
	{
		((GUILayer)this.gameObject.GetComponent(typeof(GUILayer))).enabled = false;
		this.GuiCam.enabled = false;
	}

	public override void GuiOn()
	{
		this.GuiCam.enabled = true;
		((GUILayer)this.gameObject.GetComponent(typeof(GUILayer))).enabled = true;
	}

	public override void Main()
	{
	}
}
