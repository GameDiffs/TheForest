using Serialization;
using System;
using UnityEngine;

[ComponentSerializerFor(typeof(Camera))]
public class SerializeCamera : IComponentSerializer
{
	public class CameraData
	{
		public float fieldOfView;

		public float nearClipPlane;

		public float farClipPlane;

		public float depth;
	}

	public byte[] Serialize(Component component)
	{
		Camera camera = (Camera)component;
		SerializeCamera.CameraData item = new SerializeCamera.CameraData
		{
			fieldOfView = camera.fieldOfView,
			depth = camera.depth,
			nearClipPlane = camera.nearClipPlane,
			farClipPlane = camera.farClipPlane
		};
		return UnitySerializer.Serialize(item);
	}

	public void Deserialize(byte[] data, Component instance)
	{
		SerializeCamera.CameraData cameraData = UnitySerializer.Deserialize<SerializeCamera.CameraData>(data);
		Camera camera = (Camera)instance;
		camera.fieldOfView = cameraData.fieldOfView;
		camera.nearClipPlane = cameraData.nearClipPlane;
		camera.farClipPlane = cameraData.farClipPlane;
		camera.depth = cameraData.depth;
	}
}
