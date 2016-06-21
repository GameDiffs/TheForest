using System;
using UnityEngine;

namespace ScionEngine
{
	public struct CameraParameters
	{
		public CameraMode cameraMode;

		public float focalLength;

		public float apertureDiameter;

		public float fNumber;

		public float ISO;

		public float shutterSpeed;

		public Vector2 minMaxExposure;

		public float exposureCompensation;

		public float fieldOfView;

		public float adaptionSpeed;

		public float aspect;

		public float nearPlane;

		public float farPlane;
	}
}
