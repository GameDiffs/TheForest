using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Camera/ReflectionCameraCullingDistances"), RequireComponent(typeof(Camera))]
	public class ReflectionCameraCullingDistances : MonoBehaviour
	{
		public bool sphericalCulling = true;

		public float[] distances = new float[32];

		private Camera m_camera;

		private void Start()
		{
			this.m_camera = base.GetComponent<Camera>();
		}

		private void Update()
		{
			if (Ocean.Instance == null || this.distances.Length != 32)
			{
				return;
			}
			CameraData cameraData = Ocean.Instance.FindCameraData(this.m_camera);
			if (cameraData.reflection == null)
			{
				return;
			}
			Camera cam = cameraData.reflection.cam;
			cam.layerCullDistances = this.distances;
			cam.layerCullSpherical = this.sphericalCulling;
		}
	}
}
