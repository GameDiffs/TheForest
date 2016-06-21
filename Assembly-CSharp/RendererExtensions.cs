using System;
using UnityEngine;

public static class RendererExtensions
{
	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		Vector3 vector = camera.WorldToViewportPoint(renderer.transform.position);
		return vector.z > 0f && vector.z < camera.farClipPlane && vector.x > 0f && vector.y < 1f && vector.y > 0f && vector.y < 1f;
	}
}
