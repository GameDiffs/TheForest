using System;
using UnityEngine;

public static class SunshineMath
{
	public struct BoundingSphere
	{
		public Vector3 origin;

		public float radius;
	}

	public struct ShadowCameraTemporalData
	{
		public float boundingRadius;

		public Vector3 lightWorldOrigin;
	}

	public static readonly Matrix4x4 ToTextureSpaceProjection = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(0.5f, 0.5f, 1f));

	public static readonly Rect[][] CascadeViewportArrangements = new Rect[][]
	{
		new Rect[]
		{
			new Rect(0f, 0f, 1f, 1f)
		},
		new Rect[]
		{
			new Rect(0f, 0f, 1f, 0.5f),
			new Rect(0f, 0.5f, 1f, 0.5f)
		},
		new Rect[]
		{
			new Rect(0f, 0f, 0.5f, 1f),
			new Rect(0.5f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0.5f, 0.5f, 0.5f)
		},
		new Rect[]
		{
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0f, 0.5f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0.5f, 0.5f, 0.5f)
		}
	};

	private static Vector3[] _frustumTestPoints = new Vector3[2];

	public static int UnityStyleLightmapResolution(SunshineLightResolutions resolution)
	{
		if (resolution == SunshineLightResolutions.Custom)
		{
			return 0;
		}
		int num = Mathf.Max(Screen.width, Screen.height);
		int num2 = Mathf.NextPowerOfTwo((int)((float)num * 1.9f));
		int a = 2048;
		if (SystemInfo.graphicsMemorySize >= 512)
		{
			a = 4096;
		}
		num2 = Mathf.Min(a, num2);
		switch (resolution)
		{
		case SunshineLightResolutions.LowResolution:
			num2 /= 4;
			break;
		case SunshineLightResolutions.MediumResolution:
			num2 /= 2;
			break;
		case SunshineLightResolutions.VeryHighResolution:
			num2 *= 2;
			break;
		}
		return Mathf.Min(a, num2);
	}

	public static float ShadowTexelWorldSize(float resolution, float orthographicSize)
	{
		return orthographicSize * 2f / resolution;
	}

	public static Matrix4x4 ToRectSpaceProjection(Rect rect)
	{
		return Matrix4x4.TRS(new Vector3(rect.width * 0.5f + rect.x, rect.height * 0.5f + rect.y, 0f), Quaternion.identity, new Vector3(rect.width * 0.5f, rect.height * 0.5f, 1f));
	}

	public static void SetLinearDepthProjection(ref Matrix4x4 projection, float farClip)
	{
		projection.SetRow(2, new Vector4(0f, 0f, -1f / farClip, 0f));
	}

	public static Matrix4x4 SetLinearDepthProjection(Matrix4x4 projection, float farClip)
	{
		SunshineMath.SetLinearDepthProjection(ref projection, farClip);
		return projection;
	}

	public static Vector2 xy(Vector3 self)
	{
		return new Vector2(self.x, self.y);
	}

	public static Vector2 xy(Vector4 self)
	{
		return new Vector2(self.x, self.y);
	}

	public static Vector3 xyz(Vector4 self)
	{
		return new Vector3(self.x, self.y, self.z);
	}

	public static Vector3 xy0(Vector2 self)
	{
		return new Vector3(self.x, self.y, 0f);
	}

	public static Vector3 xy0(Vector3 self)
	{
		return new Vector3(self.x, self.y, 0f);
	}

	public static Vector3 xy0(Vector4 self)
	{
		return new Vector3(self.x, self.y, 0f);
	}

	public static Vector4 xy00(Vector2 self)
	{
		return new Vector4(self.x, self.y, 0f, 0f);
	}

	public static Vector4 xy00(Vector3 self)
	{
		return new Vector4(self.x, self.y, 0f, 0f);
	}

	public static Vector4 xy00(Vector4 self)
	{
		return new Vector4(self.x, self.y, 0f, 0f);
	}

	public static Vector4 xyz0(Vector3 self)
	{
		return new Vector4(self.x, self.y, self.z, 0f);
	}

	public static Vector4 xyz0(Vector4 self)
	{
		return new Vector4(self.x, self.y, self.z, 0f);
	}

	public static int RelativeResolutionDivisor(SunshineRelativeResolutions resolution)
	{
		switch (resolution)
		{
		case SunshineRelativeResolutions.Full:
			return 1;
		case SunshineRelativeResolutions.Half:
			return 2;
		case SunshineRelativeResolutions.Third:
			return 3;
		case SunshineRelativeResolutions.Quarter:
			return 4;
		case SunshineRelativeResolutions.Fifth:
			return 5;
		case SunshineRelativeResolutions.Sixth:
			return 6;
		case SunshineRelativeResolutions.Seventh:
			return 7;
		case SunshineRelativeResolutions.Eighth:
			return 8;
		default:
			return 1;
		}
	}

	public static float ShadowKernelRadius(SunshineShadowFilters filter)
	{
		switch (filter)
		{
		case SunshineShadowFilters.PCF2x2:
			return 1.414214f;
		case SunshineShadowFilters.PCF3x3:
			return 2.12132f;
		case SunshineShadowFilters.PCF4x4:
			return 2.828427f;
		}
		return 0.7071068f;
	}

	public static float QuantizeValue(float number, float step)
	{
		return Mathf.Floor(number / step + 0.5f) * step;
	}

	public static float QuantizeValueWithoutFlicker(float number, float step, float lastResult)
	{
		float num = SunshineMath.QuantizeValue(number, step);
		if (Mathf.Abs(num - number) * 4f < Mathf.Abs(lastResult - number))
		{
			return num;
		}
		return lastResult;
	}

	public static float QuantizeValue(float number, int resolution)
	{
		return SunshineMath.QuantizeValue(number, 1f / (float)resolution);
	}

	public static float QuantizeValueWithoutFlicker(float number, int resolution, float lastResult)
	{
		return SunshineMath.QuantizeValueWithoutFlicker(number, 1f / (float)resolution, lastResult);
	}

	public static float RadialClipCornerRatio(Camera cam)
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0f, 0f, 0f));
		return cam.transform.InverseTransformDirection(ray.direction).z;
	}

	public static float MinRadiusSq(Vector3 origin, Vector3[] points)
	{
		float num = 0f;
		for (int i = 0; i < points.Length; i++)
		{
			float sqrMagnitude = (points[i] - origin).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				num = sqrMagnitude;
			}
		}
		return num;
	}

	public static SunshineMath.BoundingSphere FrustumBoundingSphereBinarySearch(Camera camera, float nearClip, float farClip, bool radial, float radialPadding, float maxError = 0.01f, int maxSteps = 100)
	{
		float num = SunshineMath.RadialClipCornerRatio(camera);
		float z = (!radial) ? nearClip : (nearClip * num);
		float z2 = (!radial) ? farClip : (farClip * num);
		Vector3 from = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, nearClip));
		Vector3 to = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, z2));
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0f, 0f, z));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, farClip));
		Vector3 vector3 = (!radial) ? vector2 : camera.ViewportToWorldPoint(new Vector3(1f, 1f, z2));
		SunshineMath._frustumTestPoints[0] = vector;
		SunshineMath._frustumTestPoints[1] = vector3;
		float num2 = 3.40282347E+38f;
		Vector3 origin = Vector3.zero;
		float num3 = 0f;
		float num4 = 0.2f;
		for (int i = 0; i < maxSteps; i++)
		{
			Vector3 vector4 = Vector3.Lerp(from, to, num3);
			float num5 = SunshineMath.MinRadiusSq(vector4, SunshineMath._frustumTestPoints);
			if (num5 < num2)
			{
				num2 = num5;
				origin = vector4;
			}
			else
			{
				num4 *= -0.5f;
				if (Mathf.Abs(num4) < maxError)
				{
					break;
				}
			}
			num3 += num4;
		}
		return new SunshineMath.BoundingSphere
		{
			origin = origin,
			radius = Mathf.Sqrt(num2) + radialPadding
		};
	}

	public static void SetupShadowCamera(Light light, Camera lightCamera, Camera eyeCamera, float eyeNearClip, float eyeFarClip, float paddingZ, float paddingRadius, int snapResolution, ref SunshineMath.BoundingSphere totalShadowBounds, ref SunshineMath.ShadowCameraTemporalData temporalData)
	{
		Transform transform = lightCamera.transform;
		SunshineMath.BoundingSphere boundingSphere = default(SunshineMath.BoundingSphere);
		if (Sunshine.Instance.UsingCustomBounds)
		{
			boundingSphere = Sunshine.Instance.CustomBounds;
		}
		else
		{
			boundingSphere = SunshineMath.FrustumBoundingSphereBinarySearch(eyeCamera, eyeNearClip, eyeFarClip, true, paddingRadius, 0.01f, 20);
		}
		float num = SunshineMath.QuantizeValueWithoutFlicker(boundingSphere.radius, 100, temporalData.boundingRadius);
		temporalData.boundingRadius = num;
		float num2 = num * 2f;
		lightCamera.aspect = 1f;
		lightCamera.orthographic = true;
		lightCamera.nearClipPlane = eyeCamera.nearClipPlane;
		lightCamera.farClipPlane = (totalShadowBounds.radius + paddingZ + lightCamera.nearClipPlane) * 2f;
		lightCamera.orthographicSize = num2 * 0.5f;
		transform.rotation = Quaternion.LookRotation(light.transform.forward);
		transform.position = boundingSphere.origin;
		Vector3 vector = transform.InverseTransformPoint(Vector3.zero);
		float step = num2 / (float)snapResolution;
		vector.x = SunshineMath.QuantizeValueWithoutFlicker(vector.x, step, temporalData.lightWorldOrigin.x);
		vector.y = SunshineMath.QuantizeValueWithoutFlicker(vector.y, step, temporalData.lightWorldOrigin.y);
		temporalData.lightWorldOrigin = vector;
		transform.position -= transform.TransformPoint(vector);
		Vector3 vector2 = transform.InverseTransformPoint(totalShadowBounds.origin);
		transform.position += transform.forward * (vector2.z - (totalShadowBounds.radius + lightCamera.nearClipPlane + paddingZ));
	}

	public static void ShadowCoordDataInRect(ref Vector4 shadowCoordData, ref Rect rect)
	{
		shadowCoordData.x = Mathf.Lerp(rect.xMin, rect.xMax, shadowCoordData.x);
		shadowCoordData.y = Mathf.Lerp(rect.yMin, rect.yMax, shadowCoordData.y);
	}

	public static void ShadowCoordDataRayInRect(ref Vector4 shadowCoordDataRay, ref Rect rect)
	{
		shadowCoordDataRay.x *= rect.width;
		shadowCoordDataRay.y *= rect.height;
	}

	public static LayerMask SubtractMask(LayerMask mask, LayerMask subtract)
	{
		return mask & ~subtract;
	}
}
