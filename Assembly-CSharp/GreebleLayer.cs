using System;
using UnityEngine;

public class GreebleLayer : MonoBehaviour
{
	public struct GreebleReference
	{
		public bool IsSpawning;

		public GameObject Instance;
	}

	public static bool HasBeenRebuilt;

	[Range(1f, 100f)]
	public int CountX = 10;

	private static int CountY = 1;

	[Range(1f, 100f)]
	public int CountZ = 10;

	public bool AlwaysGenerateIdentically = true;

	[Range(0f, 100000f)]
	public int RandomSeed;

	public Vector3 Dimensions = new Vector3(100f, 1000f, 100f);

	public GreebleDefinition[] GreebleDefinitions;

	private Vector3 previousDimensions = Vector3.zero;

	private Vector3 greebleUnit = Vector3.one;

	private Vector3 greebleOffset = Vector3.zero;

	private int worldOffsetX;

	private int worldOffsetY;

	private int worldOffsetZ;

	private GreebleLayer.GreebleReference[,,] greebles;

	public void OnDrawGizmosSelected()
	{
		if (!base.enabled || !Application.isPlaying)
		{
			return;
		}
		Color color = Gizmos.color;
		Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.25f);
		Vector3 world = this.GetWorld(this.worldOffsetX + this.CountX / 2, this.worldOffsetY + GreebleLayer.CountY / 2, this.worldOffsetZ + this.CountZ / 2);
		if (this.CountX % 2 == 0)
		{
			world.x -= this.greebleUnit.x * 0.5f;
		}
		if (GreebleLayer.CountY % 2 == 0)
		{
			world.y -= this.greebleUnit.y * 0.5f;
		}
		if (this.CountZ % 2 == 0)
		{
			world.z -= this.greebleUnit.z * 0.5f;
		}
		Gizmos.DrawCube(world, this.Dimensions);
		Gizmos.color = color;
	}

	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			this.Dimensions.y = 10000f;
			if (this.CountX != this.CountZ)
			{
				this.CountX = (this.CountZ = (this.CountX + this.CountZ) / 2);
			}
		}
		if (this.greebles == null)
		{
			this.RebuildGreebles();
		}
		if (this.greebles.GetLength(0) != this.CountX || this.greebles.GetLength(1) != GreebleLayer.CountY || this.greebles.GetLength(2) != this.CountZ || this.previousDimensions != this.Dimensions)
		{
			this.RebuildGreebles();
		}
		if (this.ShiftGreebles())
		{
			this.PerformScheduledSpawning();
		}
	}

	private void PerformScheduledSpawning()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < this.CountX; i++)
		{
			for (int j = 0; j < GreebleLayer.CountY; j++)
			{
				for (int k = 0; k < this.CountZ; k++)
				{
					if (this.greebles[i, j, k].IsSpawning)
					{
						this.InstantiateGreeble(i, j, k);
						num++;
						if (num >= 5)
						{
							WorkScheduler.RegisterOneShot(new WorkScheduler.Task(this.PerformScheduledSpawning));
							return;
						}
					}
				}
			}
		}
	}

	private void OnEnable()
	{
		if (GreebleLayer.HasBeenRebuilt)
		{
			return;
		}
		GreebleLayer.HasBeenRebuilt = true;
		this.RebuildGreebles();
	}

	private void OnDisable()
	{
		GreebleLayer.HasBeenRebuilt = false;
		this.DestroyGreebles();
	}

	private void OnDestroy()
	{
		GreebleLayer.HasBeenRebuilt = false;
		this.DestroyGreebles();
	}

	private void DestroyGreeble(int x, int y, int z)
	{
		GreeblePlugin.Destroy(this.greebles[x, y, z].Instance);
		this.greebles[x, y, z].Instance = null;
	}

	private void DestroyGreebles()
	{
		if (this.greebles == null)
		{
			return;
		}
		for (int i = this.greebles.GetLength(0) - 1; i >= 0; i--)
		{
			for (int j = this.greebles.GetLength(1) - 1; j >= 0; j--)
			{
				for (int k = this.greebles.GetLength(2) - 1; k >= 0; k--)
				{
					this.DestroyGreeble(i, j, k);
				}
			}
		}
	}

	public void RebuildGreebles()
	{
		this.DestroyGreebles();
		if (base.gameObject.activeInHierarchy)
		{
			this.CreateGreeblesArray();
			this.SetupSpacialData();
			this.InstantiateGreebles();
		}
	}

	private void CreateGreeblesArray()
	{
		this.DestroyGreebles();
		this.CountX = Mathf.Max(1, this.CountX);
		GreebleLayer.CountY = Mathf.Max(1, GreebleLayer.CountY);
		this.CountZ = Mathf.Max(1, this.CountZ);
		this.greebles = new GreebleLayer.GreebleReference[this.CountX, GreebleLayer.CountY, this.CountZ];
	}

	private void SetupSpacialData()
	{
		this.previousDimensions = this.Dimensions;
		this.greebleUnit = this.CalculateGreebleUnit();
		this.greebleOffset = (this.greebleUnit - this.Dimensions) * 0.5f;
		this.ShiftGreebles();
	}

	private int GetWorldX(int localX = 0)
	{
		return Mathf.RoundToInt(base.transform.position.x / this.greebleUnit.x + (float)localX);
	}

	private int GetWorldY(int localY = 0)
	{
		return Mathf.RoundToInt(base.transform.position.y / this.greebleUnit.y + (float)localY);
	}

	private int GetWorldZ(int localZ = 0)
	{
		return Mathf.RoundToInt(base.transform.position.z / this.greebleUnit.z + (float)localZ);
	}

	private Vector3 GetWorld(int worldX, int worldY, int worldZ)
	{
		return new Vector3((float)worldX * this.greebleUnit.x + this.greebleOffset.x, (float)worldY * this.greebleUnit.y + this.greebleOffset.y, (float)worldZ * this.greebleUnit.z + this.greebleOffset.z);
	}

	private bool ShiftGreebles()
	{
		bool flag = false;
		int worldX = this.GetWorldX(0);
		if (worldX != this.worldOffsetX)
		{
			flag = true;
		}
		int worldY = this.GetWorldY(0);
		if (worldY != this.worldOffsetY)
		{
			flag = true;
		}
		int worldZ = this.GetWorldZ(0);
		if (worldZ != this.worldOffsetZ)
		{
			flag = true;
		}
		if (flag)
		{
			int num = worldX - this.worldOffsetX;
			int num2 = worldY - this.worldOffsetY;
			int num3 = worldZ - this.worldOffsetZ;
			this.worldOffsetX = worldX;
			this.worldOffsetY = worldY;
			this.worldOffsetZ = worldZ;
			int num4 = this.CountX - 1;
			int num5 = GreebleLayer.CountY - 1;
			int num6 = this.CountZ - 1;
			bool flag2 = num > 0;
			bool flag3 = num2 > 0;
			bool flag4 = num3 > 0;
			int num7 = (!flag2) ? num4 : 0;
			int num8 = (!flag2) ? -1 : 1;
			int num9 = (!flag3) ? num5 : 0;
			int num10 = (!flag3) ? -1 : 1;
			int num11 = (!flag4) ? num6 : 0;
			int num12 = (!flag4) ? -1 : 1;
			int num13 = 0;
			int num14 = num7;
			while (num14 >= 0 && num14 <= num4)
			{
				bool flag5 = num14 >= -num && num14 <= num4 - num;
				bool flag6 = num14 < num || num14 > num4 + num;
				int num15 = num9;
				while (num15 >= 0 && num15 <= num5)
				{
					bool flag7 = num15 >= -num2 && num15 <= num5 - num2;
					bool flag8 = num15 < num2 || num15 > num5 + num2;
					int num16 = num11;
					while (num16 >= 0 && num16 <= num6)
					{
						bool flag9 = num16 >= -num3 && num16 <= num6 - num3;
						bool flag10 = num16 < num3 || num16 > num6 + num3;
						if (flag6 || flag8 || flag10)
						{
							this.DestroyGreeble(num14, num15, num16);
							num13++;
						}
						if (flag5 && flag7 && flag9)
						{
							this.greebles[num14, num15, num16] = this.greebles[num14 + num, num15 + num2, num16 + num3];
							this.greebles[num14 + num, num15 + num2, num16 + num3].Instance = null;
						}
						else
						{
							this.InstantiateGreebleLater(num14, num15, num16);
						}
						num16 += num12;
					}
					num15 += num10;
				}
				num14 += num8;
			}
		}
		return flag;
	}

	private void ProceduralSeed(int worldX, int worldY, int worldZ)
	{
	}

	private Vector3 ProceduralPositionOffset()
	{
		return new Vector3(GreebleUtility.ProceduralValue(-0.5f, 0.5f) * this.greebleUnit.x, GreebleUtility.ProceduralValue(-0.5f, 0.5f) * this.greebleUnit.y, GreebleUtility.ProceduralValue(-0.5f, 0.5f) * this.greebleUnit.z);
	}

	private void InstantiateGreebleLater(int localX, int localY, int localZ)
	{
		this.greebles[localX, localY, localZ].IsSpawning = true;
	}

	private void InstantiateGreeble(int localX, int localY, int localZ)
	{
		int seed = UnityEngine.Random.Range(0, 6060842);
		int worldX = localX + this.worldOffsetX;
		int worldY = localY + this.worldOffsetY;
		int worldZ = localZ + this.worldOffsetZ;
		this.ProceduralSeed(worldX, worldY, worldZ);
		GreebleDefinition greebleDefinition = GreebleUtility.ProceduralGreebleType(this.GreebleDefinitions, 0f);
		if (greebleDefinition == null)
		{
			UnityEngine.Random.seed = seed;
			return;
		}
		Vector3 world = this.GetWorld(worldX, worldY, worldZ);
		Vector3 b = this.ProceduralPositionOffset();
		b.y = 0f;
		Quaternion rotation = (!greebleDefinition.RandomizeRotation) ? Quaternion.identity : GreebleUtility.ProceduralRotation();
		this.greebles[localX, localY, localZ].Instance = GreebleUtility.Spawn(greebleDefinition, new Ray(world + b + new Vector3(0f, this.Dimensions.y * 0.5f, 0f), Vector3.down), this.Dimensions.y, rotation, 0.5f);
		this.greebles[localX, localY, localZ].IsSpawning = false;
		UnityEngine.Random.seed = seed;
	}

	private void InstantiateGreebles()
	{
		this.DestroyGreebles();
		for (int i = 0; i < this.CountX; i++)
		{
			for (int j = 0; j < GreebleLayer.CountY; j++)
			{
				for (int k = 0; k < this.CountZ; k++)
				{
					if (!this.greebles[i, j, k].Instance)
					{
						this.InstantiateGreeble(i, j, k);
					}
				}
			}
		}
	}

	private Vector3 CalculateGreebleUnit()
	{
		return new Vector3(this.Dimensions.x / (float)this.CountX, this.Dimensions.y / (float)GreebleLayer.CountY, this.Dimensions.z / (float)this.CountZ);
	}

	public void RemoveGreeble(GameObject greebleInstance)
	{
		if (greebleInstance == null)
		{
			return;
		}
		for (int i = 0; i < this.CountX; i++)
		{
			for (int j = 0; j < GreebleLayer.CountY; j++)
			{
				for (int k = 0; k < this.CountZ; k++)
				{
					if (this.greebles[i, j, k].Instance == greebleInstance)
					{
						GreeblePlugin.Remove(greebleInstance);
						this.greebles[i, j, k].Instance = null;
					}
				}
			}
		}
	}
}
