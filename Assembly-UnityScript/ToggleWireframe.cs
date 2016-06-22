using System;
using UnityEngine;

[Serializable]
public class ToggleWireframe : MonoBehaviour
{
	public GameObject wireframeCamera;

	public bool isOn;

	public override void Start()
	{
		((Camera)this.wireframeCamera.GetComponent(typeof(Camera))).GetComponent<Camera>().enabled = this.isOn;
	}

	public override void Update()
	{
		bool enabled = ((Camera)this.wireframeCamera.GetComponent(typeof(Camera))).GetComponent<Camera>().enabled;
		if (Input.GetKeyDown(KeyCode.F2))
		{
			((Camera)this.wireframeCamera.GetComponent(typeof(Camera))).GetComponent<Camera>().enabled = !enabled;
		}
	}

	public override void Main()
	{
	}
}
