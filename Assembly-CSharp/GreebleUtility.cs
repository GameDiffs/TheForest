using System;
using UnityEngine;

public static class GreebleUtility
{
	public static int ProceduralValue(int minValue, int maxValue)
	{
		return UnityEngine.Random.Range(minValue, maxValue);
	}

	public static float ProceduralValue(float minValue, float maxValue)
	{
		return UnityEngine.Random.Range(minValue, maxValue);
	}

	public static float ProceduralAngle()
	{
		return GreebleUtility.ProceduralValue(0f, 360f);
	}

	public static Vector3 ProceduralDirectionFast()
	{
		return new Vector3(GreebleUtility.ProceduralValue(-1f, 1f), GreebleUtility.ProceduralValue(-1f, 1f), GreebleUtility.ProceduralValue(-1f, 1f));
	}

	public static Vector3 ProceduralDirection()
	{
		return Vector3.Normalize(GreebleUtility.ProceduralDirectionFast());
	}

	public static Quaternion ProceduralRotation()
	{
		return Quaternion.Euler(GreebleUtility.ProceduralAngle(), GreebleUtility.ProceduralAngle(), GreebleUtility.ProceduralAngle());
	}

	public static GreebleDefinition ProceduralGreebleType(GreebleDefinition[] greebleDefinitions, float ageInMinutes = 0f)
	{
		if (greebleDefinitions == null)
		{
			return null;
		}
		int num = 0;
		for (int i = 0; i < greebleDefinitions.Length; i++)
		{
			GreebleDefinition greebleDefinition = greebleDefinitions[i];
			if (greebleDefinition.Prefab && greebleDefinition.IsAgeAppropriate(ageInMinutes))
			{
				num += greebleDefinition.Chance;
			}
		}
		int num2 = GreebleUtility.ProceduralValue(0, num);
		num = 0;
		for (int j = 0; j < greebleDefinitions.Length; j++)
		{
			GreebleDefinition greebleDefinition2 = greebleDefinitions[j];
			if (greebleDefinition2.Prefab && greebleDefinition2.IsAgeAppropriate(ageInMinutes))
			{
				num += greebleDefinition2.Chance;
				if (num2 < num)
				{
					return greebleDefinition2;
				}
			}
		}
		return null;
	}

	public static GameObject Spawn(GreebleDefinition greebleDefinition, Ray ray, float distance, Quaternion rotation, float maxSlope = 0.5f)
	{
		if (distance <= 0f)
		{
			return null;
		}
		Vector3 vector = ray.origin;
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, out raycastHit, distance, greebleDefinition.SurfaceMask | greebleDefinition.KillMask))
		{
			return null;
		}
		if ((greebleDefinition.KillMask.value & 1 << raycastHit.collider.gameObject.layer) > 0)
		{
			return null;
		}
		if (Vector3.Dot(ray.direction, raycastHit.normal) > -maxSlope)
		{
			return null;
		}
		bool flag = greebleDefinition.TerrainTextureMask != null && greebleDefinition.TerrainTextureMask.Length > 0;
		if (flag)
		{
			Terrain component = raycastHit.collider.GetComponent<Terrain>();
			if (component)
			{
				Vector3 position = component.GetPosition();
				TerrainData terrainData = component.terrainData;
				int alphamapResolution = terrainData.alphamapResolution;
				int num = (int)((raycastHit.point.x - position.x) / terrainData.size.x * (float)alphamapResolution);
				int num2 = (int)((raycastHit.point.z - position.z) / terrainData.size.z * (float)alphamapResolution);
				bool flag2 = false;
				if (num >= 0 && num2 >= 0 && num < alphamapResolution && num2 < alphamapResolution)
				{
					float[,,] alphamaps = component.terrainData.GetAlphamaps(num, num2, 1, 1);
					float num3 = 0f;
					int num4 = -1;
					for (int i = terrainData.alphamapLayers - 1; i >= 0; i--)
					{
						if (alphamaps[0, 0, i] > num3)
						{
							num3 = alphamaps[0, 0, i];
							num4 = i;
						}
					}
					if (num4 >= 0)
					{
						for (int j = 0; j < greebleDefinition.TerrainTextureMask.Length; j++)
						{
							if (greebleDefinition.TerrainTextureMask[j] == num4)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if (!flag2)
				{
					return null;
				}
			}
		}
		vector = raycastHit.point;
		if (greebleDefinition.MatchSurfaceNormal)
		{
			Vector3 lhs = Vector3.Cross(Vector3.forward, raycastHit.normal);
			Vector3 forward = Vector3.Cross(lhs, raycastHit.normal);
			Quaternion quaternion = Quaternion.LookRotation(forward, raycastHit.normal);
			if (greebleDefinition.RandomizeRotation)
			{
				rotation = quaternion * rotation;
			}
			else
			{
				rotation = quaternion;
			}
		}
		vector += rotation * greebleDefinition.Prefab.transform.position;
		rotation *= greebleDefinition.Prefab.transform.rotation;
		GameObject result;
		if (Application.isPlaying)
		{
			result = GreeblePlugin.Instantiate(greebleDefinition.Prefab, vector, rotation);
		}
		else
		{
			result = (GameObject)UnityEngine.Object.Instantiate(greebleDefinition.Prefab, vector, rotation);
		}
		return result;
	}
}
