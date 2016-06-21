using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MBVersion
	{
		private static MBVersionInterface _MBVersion;

		private static MBVersionInterface _CreateMBVersionConcrete()
		{
			Type typeFromHandle = typeof(MBVersionConcrete);
			return (MBVersionInterface)Activator.CreateInstance(typeFromHandle);
		}

		public static string version()
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.version();
		}

		public static int GetMajorVersion()
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetMajorVersion();
		}

		public static int GetMinorVersion()
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetMinorVersion();
		}

		public static bool GetActive(GameObject go)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetActive(go);
		}

		public static void SetActive(GameObject go, bool isActive)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			MBVersion._MBVersion.SetActive(go, isActive);
		}

		public static void SetActiveRecursively(GameObject go, bool isActive)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			MBVersion._MBVersion.SetActiveRecursively(go, isActive);
		}

		public static UnityEngine.Object[] FindSceneObjectsOfType(Type t)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.FindSceneObjectsOfType(t);
		}

		public static bool IsRunningAndMeshNotReadWriteable(Mesh m)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.IsRunningAndMeshNotReadWriteable(m);
		}

		public static Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetMeshUV3orUV4(m, get3, LOG_LEVEL);
		}

		public static void MeshClear(Mesh m, bool t)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			MBVersion._MBVersion.MeshClear(m, t);
		}

		public static void MeshAssignUV3(Mesh m, Vector2[] uv3s)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			MBVersion._MBVersion.MeshAssignUV3(m, uv3s);
		}

		public static void MeshAssignUV4(Mesh m, Vector2[] uv4s)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			MBVersion._MBVersion.MeshAssignUV4(m, uv4s);
		}

		public static Vector4 GetLightmapTilingOffset(Renderer r)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetLightmapTilingOffset(r);
		}

		public static Transform[] GetBones(Renderer r)
		{
			if (MBVersion._MBVersion == null)
			{
				MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
			}
			return MBVersion._MBVersion.GetBones(r);
		}
	}
}
