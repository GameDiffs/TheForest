using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public interface MBVersionInterface
	{
		string version();

		int GetMajorVersion();

		int GetMinorVersion();

		bool GetActive(GameObject go);

		void SetActive(GameObject go, bool isActive);

		void SetActiveRecursively(GameObject go, bool isActive);

		UnityEngine.Object[] FindSceneObjectsOfType(Type t);

		bool IsRunningAndMeshNotReadWriteable(Mesh m);

		Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL);

		void MeshClear(Mesh m, bool t);

		void MeshAssignUV3(Mesh m, Vector2[] uv3s);

		void MeshAssignUV4(Mesh m, Vector2[] uv4s);

		Vector4 GetLightmapTilingOffset(Renderer r);

		Transform[] GetBones(Renderer r);
	}
}
