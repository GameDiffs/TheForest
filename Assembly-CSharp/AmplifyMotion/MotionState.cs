using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmplifyMotion
{
	[Serializable]
	internal abstract class MotionState
	{
		protected struct MaterialDesc
		{
			public Material material;

			public MaterialPropertyBlock propertyBlock;

			public bool coverage;

			public bool cutoff;
		}

		public const int AsyncUpdateTimeout = 100;

		protected bool m_error;

		protected bool m_initialized;

		protected Transform m_transform;

		protected AmplifyMotionCamera m_owner;

		protected AmplifyMotionObjectBase m_obj;

		private static HashSet<Material> m_materialWarnings = new HashSet<Material>();

		public AmplifyMotionCamera Owner
		{
			get
			{
				return this.m_owner;
			}
		}

		public bool Initialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		public bool Error
		{
			get
			{
				return this.m_error;
			}
		}

		public MotionState(AmplifyMotionCamera owner, AmplifyMotionObjectBase obj)
		{
			this.m_error = false;
			this.m_initialized = false;
			this.m_owner = owner;
			this.m_obj = obj;
			this.m_transform = obj.transform;
		}

		internal virtual void Initialize()
		{
			this.m_initialized = true;
		}

		internal virtual void Shutdown()
		{
		}

		internal virtual void AsyncUpdate()
		{
		}

		internal abstract void UpdateTransform(CommandBuffer updateCB, bool starting);

		internal virtual void RenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
		{
		}

		internal virtual void RenderDebugHUD()
		{
		}

		protected MotionState.MaterialDesc[] ProcessSharedMaterials(Material[] mats)
		{
			MotionState.MaterialDesc[] array = new MotionState.MaterialDesc[mats.Length];
			for (int i = 0; i < mats.Length; i++)
			{
				array[i].material = mats[i];
				bool flag = mats[i].GetTag("RenderType", false) == "TransparentCutout" || mats[i].IsKeywordEnabled("_ALPHATEST_ON");
				array[i].propertyBlock = new MaterialPropertyBlock();
				array[i].coverage = (mats[i].HasProperty("_MainTex") && flag);
				array[i].cutoff = mats[i].HasProperty("_Cutoff");
				if (flag && !array[i].coverage && !MotionState.m_materialWarnings.Contains(array[i].material))
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"[AmplifyMotion] TransparentCutout material \"",
						array[i].material.name,
						"\" {",
						array[i].material.shader.name,
						"} not using _MainTex standard property."
					}));
					MotionState.m_materialWarnings.Add(array[i].material);
				}
			}
			return array;
		}

		internal static bool VectorChanged(Vector3 a, Vector3 b)
		{
			return Vector3.SqrMagnitude(a - b) > 0f;
		}

		internal static bool RotationChanged(Quaternion a, Quaternion b)
		{
			Vector4 a2 = new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
			return Vector4.SqrMagnitude(a2) > 0f;
		}

		internal static void MulPoint4x4_XYZW(ref Vector4 result, ref Matrix4x4 mat, Vector4 vec)
		{
			result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
			result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
			result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
			result.w = mat.m30 * vec.x + mat.m31 * vec.y + mat.m32 * vec.z + mat.m33 * vec.w;
		}

		internal static void MulPoint3x4_XYZ(ref Vector3 result, ref Matrix4x4 mat, Vector4 vec)
		{
			result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03;
			result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13;
			result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23;
		}

		internal static void MulPoint3x4_XYZW(ref Vector3 result, ref Matrix4x4 mat, Vector4 vec)
		{
			result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
			result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
			result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
		}

		internal static void MulAddPoint3x4_XYZW(ref Vector3 result, ref Matrix4x4 mat, Vector4 vec)
		{
			result.x += mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
			result.y += mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
			result.z += mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
		}
	}
}
