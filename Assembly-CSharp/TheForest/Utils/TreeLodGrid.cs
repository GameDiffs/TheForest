using System;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Utils
{
	[ExecuteInEditMode]
	public class TreeLodGrid : MonoBehaviour
	{
		public int _gridSize = 100;

		public int _gridWorldSize = 35;

		public int _maxDistance = 300;

		public int _maxTreeDensity = 20;

		public float _occlusionRayNormalizeRatio = 6f;

		public float _treeCountOffset = 5f;

		public float _openRayThreshold = 0.265f;

		[Range(1f, 5f)]
		public float _openDirsOutputContribution = 2f;

		[Range(0.25f, 1f)]
		public float _minOutput = 0.5f;

		[Range(1f, 3f)]
		public float _maxOutput = 2.5f;

		public bool _updateTreeOcclusionBonusRatio;

		[Header("Commands")]
		public bool _refreshGrid;

		[Header("Gizmos"), Range(0f, 1f)]
		public float _gizmosAlphaOffset;

		[Range(1f, 10f)]
		public int _cellValueGroups = 1;

		private Vector3[] _rayDirections;

		private Vector3 _offset;

		private int[,] _treeGrid;

		private int[,] _lodGrid;

		private int _highestCellTreeCount;

		private float _rayOcclusionStrength;

		private bool _isGizmo;

		public float TreeDensityUnscaled;

		private void Awake()
		{
			this.InitGrid();
			TreeHealth.OnTreeCutDown.AddListener(new UnityAction<Vector3>(this.RegisterCutDownTree));
		}

		private void Update()
		{
			if (LocalPlayer.Transform)
			{
				float num = this.CalcCellLodRatio(this.WorldToGridX(LocalPlayer.Transform.position.x), this.WorldToGridY(LocalPlayer.Transform.position.z));
				this.TreeDensityUnscaled = 1f - (num - this._minOutput) / this._maxOutput;
				if (this._updateTreeOcclusionBonusRatio)
				{
					switch (TheForestQualitySettings.UserSettings.DrawDistance)
					{
					case TheForestQualitySettings.DrawDistances.Medium:
						num -= (num - this._minOutput) * 0.2f;
						break;
					case TheForestQualitySettings.DrawDistances.Low:
						num -= (num - this._minOutput) * 0.4f;
						break;
					case TheForestQualitySettings.DrawDistances.UltraLow:
						num -= (num - this._minOutput) * 0.6f;
						break;
					}
					LOD_Manager.TreeOcclusionBonusRatio = Mathf.Lerp(LOD_Manager.TreeOcclusionBonusRatio, num, (LOD_Manager.TreeOcclusionBonusRatio >= num) ? 0.03f : 0.012f);
				}
			}
			else
			{
				LOD_Manager.TreeOcclusionBonusRatio = 1f;
			}
		}

		private void OnDisable()
		{
			if (this._updateTreeOcclusionBonusRatio)
			{
				LOD_Manager.TreeOcclusionBonusRatio = 1f;
			}
		}

		private void OnDrawGizmosSelected()
		{
			this._isGizmo = true;
			if (this._refreshGrid || this._treeGrid == null || this._treeGrid.GetLength(0) != this._gridSize)
			{
				this._refreshGrid = false;
				this.InitGrid();
			}
			Color a = new Color(0f, 1f, 0f, 0.01f + this._gizmosAlphaOffset);
			Color b = new Color(1f, 0f, 0f, 0.35f + this._gizmosAlphaOffset);
			Vector3 size = new Vector3((float)this._gridWorldSize, 500f, (float)this._gridWorldSize);
			for (int i = 0; i < this._gridSize; i++)
			{
				for (int j = 0; j < this._gridSize; j++)
				{
					float num = (float)this._treeGrid[i, j];
					if (num > 0f)
					{
						if (this._cellValueGroups == 1)
						{
							Gizmos.color = Color.Lerp(a, b, num / (float)this._highestCellTreeCount);
						}
						else
						{
							Gizmos.color = Color.Lerp(a, b, Mathf.Round(num * (float)this._cellValueGroups / (float)this._highestCellTreeCount) / (float)this._cellValueGroups);
						}
						Vector3 center = new Vector3((float)(i * this._gridWorldSize) + this._offset.x, base.transform.position.y, (float)(j * this._gridWorldSize) + this._offset.z);
						Gizmos.DrawCube(center, size);
					}
				}
			}
			Gizmos.color = Color.white;
			if (LocalPlayer.Transform)
			{
				Vector3 position = LocalPlayer.Transform.position;
				int x = this.WorldToGridX(position.x);
				int y = this.WorldToGridX(position.z);
				this.CalcCellLodRatio(x, y);
				Gizmos.DrawWireSphere(LocalPlayer.Transform.position, (float)this._maxDistance);
				Gizmos.DrawWireSphere(LocalPlayer.Transform.position, (float)this._maxDistance * (1f - this._openRayThreshold));
			}
			else
			{
				Vector3 position2 = GameObject.Find("/PlayerSpawner/player").transform.position;
				int x2 = this.WorldToGridX(position2.x);
				int y2 = this.WorldToGridX(position2.z);
				this.CalcCellLodRatio(x2, y2);
				Gizmos.DrawWireSphere(position2, (float)this._maxDistance);
				Gizmos.DrawWireSphere(position2, (float)this._maxDistance * (1f - this._openRayThreshold));
			}
			this._isGizmo = false;
		}

		public void RegisterCutDownTree(Vector3 treePos)
		{
			int num = this.WorldToGridX(treePos.x);
			int num2 = this.WorldToGridX(treePos.z);
			if (num >= 0 && num < this._gridSize && num2 >= 0 && num2 < this._gridSize)
			{
				this._treeGrid[num, num2] = Mathf.Max(this._treeGrid[num, num2] - 1, 0);
			}
		}

		private void InitGrid()
		{
			this._offset = new Vector3((float)(-(float)this._gridSize / 2 * this._gridWorldSize), 0f, (float)(-(float)this._gridSize / 2 * this._gridWorldSize));
			this._treeGrid = new int[this._gridSize, this._gridSize];
			this._lodGrid = new int[this._gridSize, this._gridSize];
			int num = 32;
			float num2 = 360f / (float)num;
			this._rayDirections = new Vector3[32];
			for (int i = 0; i < num; i++)
			{
				this._rayDirections[i] = Quaternion.Euler(0f, (float)i * num2, 0f) * Vector3.forward * (float)(this._maxDistance / this._gridWorldSize);
			}
			this._highestCellTreeCount = 1;
			LOD_Trees[] array = UnityEngine.Object.FindObjectsOfType<LOD_Trees>();
			LOD_Trees[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				LOD_Trees lOD_Trees = array2[j];
				Vector3 position = lOD_Trees.transform.position;
				int num3 = this.WorldToGridX(position.x);
				int num4 = this.WorldToGridX(position.z);
				if (num3 >= 0 && num3 < this._gridSize && num4 >= 0 && num4 < this._gridSize)
				{
					int num5 = ++this._treeGrid[num3, num4];
					if (num5 > this._highestCellTreeCount)
					{
						this._highestCellTreeCount = num5;
					}
				}
			}
			this._highestCellTreeCount = Mathf.RoundToInt((float)this._highestCellTreeCount * 0.9f);
		}

		private void SumTreeOcclusion(int treeCount)
		{
			this._rayOcclusionStrength += Mathf.Clamp01(((float)treeCount + this._treeCountOffset) / (float)this._maxTreeDensity);
		}

		private float CalcCellLodRatio(int x, int y)
		{
			int num = 32;
			float num2 = 360f / (float)num;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = this._rayDirections[i];
				this._rayOcclusionStrength = 0f;
				this.RayCast2DCells(x, y, x + Mathf.FloorToInt(vector.x), y + Mathf.FloorToInt(vector.z));
				this._rayOcclusionStrength /= this._occlusionRayNormalizeRatio;
				if (this._rayOcclusionStrength < this._openRayThreshold)
				{
					num3++;
				}
				if (this._isGizmo)
				{
					Gizmos.DrawLine(new Vector3(this.GridToWorldX((float)x), base.transform.position.y, this.GridToWorldY((float)y)), new Vector3(this.GridToWorldX((float)x), base.transform.position.y, this.GridToWorldY((float)y)) + vector.normalized * ((float)this._maxDistance * (1f - Mathf.Clamp01(this._rayOcclusionStrength))));
				}
			}
			return Mathf.Clamp(this._minOutput + (float)num3 * this._openDirsOutputContribution / (float)num, this._minOutput, this._maxOutput);
		}

		private void RayCast2DCells(int x1, int y1, int x2, int y2)
		{
			int num = x2 - x1;
			int num2 = y2 - y1;
			int num3 = Mathf.Abs(num);
			int num4 = Mathf.Abs(num2);
			int num5 = (int)Mathf.Sign((float)num);
			int num6 = (int)Mathf.Sign((float)num2);
			int num7 = num4 >> 1;
			int num8 = num3 >> 1;
			int num9 = x1;
			int num10 = y1;
			if (num3 >= num4)
			{
				for (int i = 0; i < num3; i++)
				{
					num8 += num4;
					if (num8 >= num3)
					{
						num8 -= num3;
						num10 += num6;
					}
					num9 += num5;
					if (num9 >= 0 && num9 < this._gridSize && num10 >= 0 && num10 < this._gridSize)
					{
						this.SumTreeOcclusion(this._treeGrid[num9, num10]);
					}
				}
			}
			else
			{
				for (int i = 0; i < num4; i++)
				{
					num7 += num3;
					if (num7 >= num4)
					{
						num7 -= num4;
						num9 += num5;
					}
					num10 += num6;
					if (num9 >= 0 && num9 < this._gridSize && num10 >= 0 && num10 < this._gridSize)
					{
						this.SumTreeOcclusion(this._treeGrid[num9, num10]);
					}
				}
			}
		}

		private int WorldToGridX(float xPosition)
		{
			return Mathf.FloorToInt((xPosition - this._offset.x) / (float)this._gridWorldSize);
		}

		private int WorldToGridY(float zPosition)
		{
			return Mathf.FloorToInt((zPosition - this._offset.z) / (float)this._gridWorldSize);
		}

		private int WorldToGridXRounded(float xPosition)
		{
			return Mathf.RoundToInt((xPosition - this._offset.x) / (float)this._gridWorldSize);
		}

		private int WorldToGridYRounded(float zPosition)
		{
			return Mathf.RoundToInt((zPosition - this._offset.z) / (float)this._gridWorldSize);
		}

		private float GridToWorldX(float xPosition)
		{
			return xPosition * (float)this._gridWorldSize + this._offset.x;
		}

		private float GridToWorldY(float yPosition)
		{
			return yPosition * (float)this._gridWorldSize + this._offset.z;
		}
	}
}
