using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class Camera2World : MonoBehaviour
{
	private void OnPreCull()
	{
		Shader.SetGlobalMatrix("_Camera2World", base.GetComponent<Camera>().cameraToWorldMatrix);
		Shader.SetGlobalVector("_CameraLookDirection", base.transform.forward);
	}
}
