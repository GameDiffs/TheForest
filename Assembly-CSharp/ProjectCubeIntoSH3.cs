using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ProjectCubeIntoSH3 : MonoBehaviour
{
	public Cubemap specCube;

	private Matrix4x4 QuaternionToMatrix(Quaternion quat)
	{
		Matrix4x4 result = default(Matrix4x4);
		float x = quat.x;
		float y = quat.y;
		float z = quat.z;
		float w = quat.w;
		result.m00 = 1f - 2f * Mathf.Pow(y, 2f) - 2f * Mathf.Pow(z, 2f);
		result.m01 = 2f * x * y + 2f * w * z;
		result.m02 = 2f * x * z - 2f * w * y;
		result.m03 = 0f;
		result.m10 = 2f * x * y - 2f * w * z;
		result.m11 = 1f - 2f * Mathf.Pow(x, 2f) - 2f * Mathf.Pow(z, 2f);
		result.m12 = 2f * y * z + 2f * w * x;
		result.m13 = 0f;
		result.m20 = 2f * x * z + 2f * w * y;
		result.m21 = 2f * y * z - 2f * w * x;
		result.m22 = 1f - 2f * Mathf.Pow(x, 2f) - 2f * Mathf.Pow(y, 2f);
		result.m23 = 0f;
		return result;
	}

	public SphericalHarmonicsL2 ProjectCubeIntoSH3Func(Cubemap src, int miplevel)
	{
		Vector4[] array = new Vector4[]
		{
			new Vector4(0f, 0f, -1f, 0f),
			new Vector4(0f, -1f, 0f, 0f),
			new Vector4(-1f, 0f, 0f, 0f),
			new Vector4(0f, 0f, 1f, 0f),
			new Vector4(0f, -1f, 0f, 0f),
			new Vector4(1f, 0f, 0f, 0f),
			new Vector4(1f, 0f, 0f, 0f),
			new Vector4(0f, 0f, 1f, 0f),
			new Vector4(0f, -1f, 0f, 0f),
			new Vector4(1f, 0f, 0f, 0f),
			new Vector4(0f, 0f, -1f, 0f),
			new Vector4(0f, 1f, 0f, 0f),
			new Vector4(1f, 0f, 0f, 0f),
			new Vector4(0f, -1f, 0f, 0f),
			new Vector4(0f, 0f, -1f, 0f),
			new Vector4(-1f, 0f, 0f, 0f),
			new Vector4(0f, -1f, 0f, 0f),
			new Vector4(0f, 0f, 1f, 0f)
		};
		Quaternion rotation = base.gameObject.transform.rotation;
		Matrix4x4 matrix4x = this.QuaternionToMatrix(rotation);
		for (int i = 0; i < 6; i++)
		{
			array[i] = matrix4x * array[i];
		}
		Shader.SetGlobalMatrix("_SkyRotation", matrix4x);
		float num = 0f;
		SphericalHarmonicsL2 sphericalHarmonicsL = default(SphericalHarmonicsL2);
		sphericalHarmonicsL.Clear();
		for (int j = 0; j < 6; j++)
		{
			Vector3 a = array[j * 3];
			Vector3 a2 = -array[j * 3 + 1];
			Vector3 a3 = -array[j * 3 + 2];
			Color[] pixels = src.GetPixels((CubemapFace)j, miplevel);
			int num2 = src.width >> miplevel;
			if (num2 < 1)
			{
				num2 = 1;
			}
			float num3 = -1f + 1f / (float)num2;
			float num4 = 2f * (1f - 1f / (float)num2) / ((float)num2 - 1f);
			for (int k = 0; k < num2; k++)
			{
				float num5 = (float)k * num4 + num3;
				for (int l = 0; l < num2; l++)
				{
					Color a4 = pixels[l + k * num2];
					float num6 = (float)l * num4 + num3;
					float num7 = 1f + num6 * num6 + num5 * num5;
					float num8 = 4f / (Mathf.Sqrt(num7) * num7);
					Vector3 a5 = a3 + a * num6 + a2 * num5;
					a5.Normalize();
					Color color = a4 * a4.a * 8f;
					sphericalHarmonicsL.AddDirectionalLight(-a5, (QualitySettings.activeColorSpace != ColorSpace.Linear) ? color : color.linear, num8 * 0.5f);
					num += num8;
				}
			}
		}
		float rhs = 4f / num;
		sphericalHarmonicsL *= rhs;
		return sphericalHarmonicsL;
	}
}
