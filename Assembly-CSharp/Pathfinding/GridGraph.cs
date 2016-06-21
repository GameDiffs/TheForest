using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	public class GridGraph : NavGraph, IUpdatableGraph, IRaycastableGraph
	{
		public class TextureData
		{
			public enum ChannelUse
			{
				None,
				Penalty,
				Position,
				WalkablePenalty
			}

			public bool enabled;

			public Texture2D source;

			public float[] factors = new float[3];

			public GridGraph.TextureData.ChannelUse[] channels = new GridGraph.TextureData.ChannelUse[3];

			private Color32[] data;

			public void Initialize()
			{
				if (this.enabled && this.source != null)
				{
					for (int i = 0; i < this.channels.Length; i++)
					{
						if (this.channels[i] != GridGraph.TextureData.ChannelUse.None)
						{
							try
							{
								this.data = this.source.GetPixels32();
							}
							catch (UnityException ex)
							{
								Debug.LogWarning(ex.ToString());
								this.data = null;
							}
							break;
						}
					}
				}
			}

			public void Apply(GridNode node, int x, int z)
			{
				if (this.enabled && this.data != null && x < this.source.width && z < this.source.height)
				{
					Color32 color = this.data[z * this.source.width + x];
					if (this.channels[0] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.r, this.channels[0], this.factors[0]);
					}
					if (this.channels[1] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.g, this.channels[1], this.factors[1]);
					}
					if (this.channels[2] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.b, this.channels[2], this.factors[2]);
					}
				}
			}

			private void ApplyChannel(GridNode node, int x, int z, int value, GridGraph.TextureData.ChannelUse channelUse, float factor)
			{
				switch (channelUse)
				{
				case GridGraph.TextureData.ChannelUse.Penalty:
					node.Penalty += (uint)Mathf.RoundToInt((float)value * factor);
					break;
				case GridGraph.TextureData.ChannelUse.Position:
					node.position = GridNode.GetGridGraph(node.GraphIndex).GraphPointToWorld(x, z, (float)value);
					break;
				case GridGraph.TextureData.ChannelUse.WalkablePenalty:
					if (value == 0)
					{
						node.Walkable = false;
					}
					else
					{
						node.Penalty += (uint)Mathf.RoundToInt((float)(value - 1) * factor);
					}
					break;
				}
			}
		}

		public const int getNearestForceOverlap = 2;

		public int width;

		public int depth;

		[JsonMember]
		public float aspectRatio = 1f;

		[JsonMember]
		public float isometricAngle;

		[JsonMember]
		public bool uniformEdgeCosts;

		[JsonMember]
		public Vector3 rotation;

		public Bounds bounds;

		[JsonMember]
		public Vector3 center;

		[JsonMember]
		public Vector2 unclampedSize;

		[JsonMember]
		public float nodeSize = 1f;

		[JsonMember]
		public GraphCollision collision;

		[JsonMember]
		public float maxClimb = 0.4f;

		[JsonMember]
		public int maxClimbAxis = 1;

		[JsonMember]
		public float maxSlope = 90f;

		[JsonMember]
		public int erodeIterations;

		[JsonMember]
		public bool erosionUseTags;

		[JsonMember]
		public int erosionFirstTag = 1;

		[JsonMember]
		public bool autoLinkGrids;

		[JsonMember]
		public float autoLinkDistLimit = 10f;

		[JsonMember]
		public NumNeighbours neighbours = NumNeighbours.Eight;

		[JsonMember]
		public bool cutCorners = true;

		[JsonMember]
		public float penaltyPositionOffset;

		[JsonMember]
		public bool penaltyPosition;

		[JsonMember]
		public float penaltyPositionFactor = 1f;

		[JsonMember]
		public bool penaltyAngle;

		[JsonMember]
		public float penaltyAngleFactor = 100f;

		[JsonMember]
		public float penaltyAnglePower = 1f;

		[JsonMember]
		public bool useJumpPointSearch;

		[JsonMember]
		public GridGraph.TextureData textureData = new GridGraph.TextureData();

		[NonSerialized]
		public readonly int[] neighbourOffsets = new int[8];

		[NonSerialized]
		public readonly uint[] neighbourCosts = new uint[8];

		[NonSerialized]
		public readonly int[] neighbourXOffsets = new int[8];

		[NonSerialized]
		public readonly int[] neighbourZOffsets = new int[8];

		private static readonly int[] hexagonNeighbourIndices = new int[]
		{
			0,
			1,
			2,
			3,
			5,
			7
		};

		public GridNode[] nodes;

		[NonSerialized]
		protected int[] corners;

		public virtual bool uniformWidthDepthGrid
		{
			get
			{
				return true;
			}
		}

		public bool useRaycastNormal
		{
			get
			{
				return Math.Abs(90f - this.maxSlope) > 1.401298E-45f;
			}
		}

		public Vector2 size
		{
			get;
			protected set;
		}

		public Matrix4x4 boundsMatrix
		{
			get;
			protected set;
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		public GridGraph()
		{
			this.unclampedSize = new Vector2(10f, 10f);
			this.nodeSize = 1f;
			this.collision = new GraphCollision();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			this.RemoveGridGraphFromStatic();
		}

		private void RemoveGridGraphFromStatic()
		{
			GridNode.SetGridGraph(AstarPath.active.astarData.GetGraphIndex(this), null);
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.nodes == null)
			{
				return;
			}
			int num = 0;
			while (num < this.nodes.Length && del(this.nodes[num]))
			{
				num++;
			}
		}

		public void RelocateNodes(Vector3 center, Quaternion rotation, float nodeSize, float aspectRatio = 1f, float isometricAngle = 0f)
		{
			Matrix4x4 matrix = this.matrix;
			this.center = center;
			this.rotation = rotation.eulerAngles;
			this.nodeSize = nodeSize;
			this.aspectRatio = aspectRatio;
			this.isometricAngle = isometricAngle;
			this.UpdateSizeFromWidthDepth();
			this.RelocateNodes(matrix, this.matrix);
		}

		public Int3 GraphPointToWorld(int x, int z, float height)
		{
			return (Int3)this.matrix.MultiplyPoint3x4(new Vector3((float)x + 0.5f, height, (float)z + 0.5f));
		}

		public uint GetConnectionCost(int dir)
		{
			return this.neighbourCosts[dir];
		}

		public GridNode GetNodeConnection(GridNode node, int dir)
		{
			if (!node.GetConnectionInternal(dir))
			{
				return null;
			}
			if (!node.EdgeNode)
			{
				return this.nodes[node.NodeInGridIndex + this.neighbourOffsets[dir]];
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.GetNodeConnection(nodeInGridIndex, x, num, dir);
		}

		public bool HasNodeConnection(GridNode node, int dir)
		{
			if (!node.GetConnectionInternal(dir))
			{
				return false;
			}
			if (!node.EdgeNode)
			{
				return true;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.HasNodeConnection(nodeInGridIndex, x, num, dir);
		}

		public void SetNodeConnection(GridNode node, int dir, bool value)
		{
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			this.SetNodeConnection(nodeInGridIndex, x, num, dir, value);
		}

		private GridNode GetNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].GetConnectionInternal(dir))
			{
				return null;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return null;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			if (num2 < 0 || num2 >= this.Depth)
			{
				return null;
			}
			int num3 = index + this.neighbourOffsets[dir];
			return this.nodes[num3];
		}

		public void SetNodeConnection(int index, int x, int z, int dir, bool value)
		{
			this.nodes[index].SetConnectionInternal(dir, value);
		}

		public bool HasNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].GetConnectionInternal(dir))
			{
				return false;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return false;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			return num2 >= 0 && num2 < this.Depth;
		}

		public void UpdateSizeFromWidthDepth()
		{
			this.unclampedSize = new Vector2((float)this.width, (float)this.depth) * this.nodeSize;
			this.GenerateMatrix();
		}

		public void GenerateMatrix()
		{
			Vector2 size = this.unclampedSize;
			size.x *= Mathf.Sign(size.x);
			size.y *= Mathf.Sign(size.y);
			this.nodeSize = Mathf.Clamp(this.nodeSize, size.x / 1024f, float.PositiveInfinity);
			this.nodeSize = Mathf.Clamp(this.nodeSize, size.y / 1024f, float.PositiveInfinity);
			size.x = ((size.x >= this.nodeSize) ? size.x : this.nodeSize);
			size.y = ((size.y >= this.nodeSize) ? size.y : this.nodeSize);
			this.size = size;
			Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 45f, 0f), Vector3.one);
			rhs = Matrix4x4.Scale(new Vector3(Mathf.Cos(0.0174532924f * this.isometricAngle), 1f, 1f)) * rhs;
			rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, -45f, 0f), Vector3.one) * rhs;
			this.boundsMatrix = Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), new Vector3(this.aspectRatio, 1f, 1f)) * rhs;
			this.width = Mathf.FloorToInt(this.size.x / this.nodeSize);
			this.depth = Mathf.FloorToInt(this.size.y / this.nodeSize);
			if (Mathf.Approximately(this.size.x / this.nodeSize, (float)Mathf.CeilToInt(this.size.x / this.nodeSize)))
			{
				this.width = Mathf.CeilToInt(this.size.x / this.nodeSize);
			}
			if (Mathf.Approximately(this.size.y / this.nodeSize, (float)Mathf.CeilToInt(this.size.y / this.nodeSize)))
			{
				this.depth = Mathf.CeilToInt(this.size.y / this.nodeSize);
			}
			Matrix4x4 matrix = Matrix4x4.TRS(this.boundsMatrix.MultiplyPoint3x4(-new Vector3(this.size.x, 0f, this.size.y) * 0.5f), Quaternion.Euler(this.rotation), new Vector3(this.nodeSize * this.aspectRatio, 1f, this.nodeSize)) * rhs;
			base.SetMatrix(matrix);
		}

		public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfo);
			}
			position = this.inverseMatrix.MultiplyPoint3x4(position);
			float num = position.x - 0.5f;
			float num2 = position.z - 0.5f;
			int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 0, this.width - 1);
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, this.depth - 1);
			NNInfo result = new NNInfo(this.nodes[num4 * this.width + num3]);
			float y = this.inverseMatrix.MultiplyPoint3x4((Vector3)this.nodes[num4 * this.width + num3].position).y;
			result.clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)num3 - 0.5f, (float)num3 + 0.5f) + 0.5f, y, Mathf.Clamp(num2, (float)num4 - 0.5f, (float)num4 + 0.5f) + 0.5f));
			return result;
		}

		public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfo);
			}
			Vector3 b = position;
			position = this.inverseMatrix.MultiplyPoint3x4(position);
			float num = position.x - 0.5f;
			float num2 = position.z - 0.5f;
			int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 0, this.width - 1);
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, this.depth - 1);
			GridNode gridNode = this.nodes[num3 + num4 * this.width];
			GridNode gridNode2 = null;
			float num5 = float.PositiveInfinity;
			int num6 = 2;
			Vector3 clampedPosition = Vector3.zero;
			NNInfo result = new NNInfo(null);
			if (constraint.Suitable(gridNode))
			{
				gridNode2 = gridNode;
				num5 = ((Vector3)gridNode2.position - b).sqrMagnitude;
				float y = this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode.position).y;
				clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)num3 - 0.5f, (float)num3 + 0.5f) + 0.5f, y, Mathf.Clamp(num2, (float)num4 - 0.5f, (float)num4 + 0.5f) + 0.5f));
			}
			if (gridNode2 != null)
			{
				result.node = gridNode2;
				result.clampedPosition = clampedPosition;
				if (num6 == 0)
				{
					return result;
				}
				num6--;
			}
			float num7 = (!constraint.constrainDistance) ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistance;
			float num8 = num7 * num7;
			int num9 = 1;
			while (this.nodeSize * (float)num9 <= num7)
			{
				bool flag = false;
				int i = num4 + num9;
				int num10 = i * this.width;
				int j;
				for (j = num3 - num9; j <= num3 + num9; j++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + num10]))
						{
							float sqrMagnitude = ((Vector3)this.nodes[j + num10].position - b).sqrMagnitude;
							if (sqrMagnitude < num5 && sqrMagnitude < num8)
							{
								num5 = sqrMagnitude;
								gridNode2 = this.nodes[j + num10];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				i = num4 - num9;
				num10 = i * this.width;
				for (j = num3 - num9; j <= num3 + num9; j++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + num10]))
						{
							float sqrMagnitude2 = ((Vector3)this.nodes[j + num10].position - b).sqrMagnitude;
							if (sqrMagnitude2 < num5 && sqrMagnitude2 < num8)
							{
								num5 = sqrMagnitude2;
								gridNode2 = this.nodes[j + num10];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				j = num3 - num9;
				for (i = num4 - num9 + 1; i <= num4 + num9 - 1; i++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + i * this.width]))
						{
							float sqrMagnitude3 = ((Vector3)this.nodes[j + i * this.width].position - b).sqrMagnitude;
							if (sqrMagnitude3 < num5 && sqrMagnitude3 < num8)
							{
								num5 = sqrMagnitude3;
								gridNode2 = this.nodes[j + i * this.width];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				j = num3 + num9;
				for (i = num4 - num9 + 1; i <= num4 + num9 - 1; i++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + i * this.width]))
						{
							float sqrMagnitude4 = ((Vector3)this.nodes[j + i * this.width].position - b).sqrMagnitude;
							if (sqrMagnitude4 < num5 && sqrMagnitude4 < num8)
							{
								num5 = sqrMagnitude4;
								gridNode2 = this.nodes[j + i * this.width];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				if (gridNode2 != null)
				{
					if (num6 == 0)
					{
						result.node = gridNode2;
						result.clampedPosition = clampedPosition;
						return result;
					}
					num6--;
				}
				if (!flag)
				{
					result.node = gridNode2;
					result.clampedPosition = clampedPosition;
					return result;
				}
				num9++;
			}
			result.node = gridNode2;
			result.clampedPosition = clampedPosition;
			return result;
		}

		public virtual void SetUpOffsetsAndCosts()
		{
			this.neighbourOffsets[0] = -this.width;
			this.neighbourOffsets[1] = 1;
			this.neighbourOffsets[2] = this.width;
			this.neighbourOffsets[3] = -1;
			this.neighbourOffsets[4] = -this.width + 1;
			this.neighbourOffsets[5] = this.width + 1;
			this.neighbourOffsets[6] = this.width - 1;
			this.neighbourOffsets[7] = -this.width - 1;
			uint num = (uint)Mathf.RoundToInt(this.nodeSize * 1000f);
			uint num2 = (uint)((!this.uniformEdgeCosts) ? Mathf.RoundToInt(this.nodeSize * Mathf.Sqrt(2f) * 1000f) : ((int)num));
			this.neighbourCosts[0] = num;
			this.neighbourCosts[1] = num;
			this.neighbourCosts[2] = num;
			this.neighbourCosts[3] = num;
			this.neighbourCosts[4] = num2;
			this.neighbourCosts[5] = num2;
			this.neighbourCosts[6] = num2;
			this.neighbourCosts[7] = num2;
			this.neighbourXOffsets[0] = 0;
			this.neighbourXOffsets[1] = 1;
			this.neighbourXOffsets[2] = 0;
			this.neighbourXOffsets[3] = -1;
			this.neighbourXOffsets[4] = 1;
			this.neighbourXOffsets[5] = 1;
			this.neighbourXOffsets[6] = -1;
			this.neighbourXOffsets[7] = -1;
			this.neighbourZOffsets[0] = -1;
			this.neighbourZOffsets[1] = 0;
			this.neighbourZOffsets[2] = 1;
			this.neighbourZOffsets[3] = 0;
			this.neighbourZOffsets[4] = -1;
			this.neighbourZOffsets[5] = 1;
			this.neighbourZOffsets[6] = 1;
			this.neighbourZOffsets[7] = -1;
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			AstarPath.OnPostScan = (OnScanDelegate)Delegate.Combine(AstarPath.OnPostScan, new OnScanDelegate(this.OnPostScan));
			if (this.nodeSize <= 0f)
			{
				return;
			}
			this.GenerateMatrix();
			if (this.width > 1024 || this.depth > 1024)
			{
				Debug.LogError("One of the grid's sides is longer than 1024 nodes");
				return;
			}
			if (this.useJumpPointSearch)
			{
				Debug.LogError("Trying to use Jump Point Search, but support for it is not enabled. Please enable it in the inspector (Grid Graph settings).");
			}
			this.SetUpOffsetsAndCosts();
			int graphIndex = AstarPath.active.astarData.GetGraphIndex(this);
			GridNode.SetGridGraph(graphIndex, this);
			this.nodes = new GridNode[this.width * this.depth];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i] = new GridNode(this.active);
				this.nodes[i].GraphIndex = (uint)graphIndex;
			}
			if (this.collision == null)
			{
				this.collision = new GraphCollision();
			}
			this.collision.Initialize(this.matrix, this.nodeSize);
			this.textureData.Initialize();
			for (int j = 0; j < this.depth; j++)
			{
				for (int k = 0; k < this.width; k++)
				{
					GridNode gridNode = this.nodes[j * this.width + k];
					gridNode.NodeInGridIndex = j * this.width + k;
					this.UpdateNodePositionCollision(gridNode, k, j, true);
					this.textureData.Apply(gridNode, k, j);
				}
			}
			for (int l = 0; l < this.depth; l++)
			{
				for (int m = 0; m < this.width; m++)
				{
					GridNode node = this.nodes[l * this.width + m];
					this.CalculateConnections(this.nodes, m, l, node);
				}
			}
			this.ErodeWalkableArea();
		}

		public virtual void UpdateNodePositionCollision(GridNode node, int x, int z, bool resetPenalty = true)
		{
			node.position = this.GraphPointToWorld(x, z, 0f);
			RaycastHit raycastHit;
			bool flag;
			Vector3 ob = this.collision.CheckHeight((Vector3)node.position, out raycastHit, out flag);
			node.position = (Int3)ob;
			if (resetPenalty)
			{
				node.Penalty = this.initialPenalty;
				if (this.penaltyPosition)
				{
					node.Penalty += (uint)Mathf.RoundToInt(((float)node.position.y - this.penaltyPositionOffset) * this.penaltyPositionFactor);
				}
			}
			if (flag && this.useRaycastNormal && this.collision.heightCheck && raycastHit.normal != Vector3.zero)
			{
				float num = Vector3.Dot(raycastHit.normal.normalized, this.collision.up);
				if (this.penaltyAngle && resetPenalty)
				{
					node.Penalty += (uint)Mathf.RoundToInt((1f - Mathf.Pow(num, this.penaltyAnglePower)) * this.penaltyAngleFactor);
				}
				float num2 = Mathf.Cos(this.maxSlope * 0.0174532924f);
				if (num < num2)
				{
					flag = false;
				}
			}
			node.Walkable = (flag && this.collision.Check((Vector3)node.position));
			node.WalkableErosion = node.Walkable;
		}

		public virtual void ErodeWalkableArea()
		{
			this.ErodeWalkableArea(0, 0, this.Width, this.Depth);
		}

		private bool ErosionAnyFalseConnections(GridNode node)
		{
			if (this.neighbours == NumNeighbours.Six)
			{
				for (int i = 0; i < 6; i++)
				{
					if (!this.HasNodeConnection(node, GridGraph.hexagonNeighbourIndices[i]))
					{
						return true;
					}
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if (!this.HasNodeConnection(node, j))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void ErodeWalkableArea(int xmin, int zmin, int xmax, int zmax)
		{
			xmin = Mathf.Clamp(xmin, 0, this.Width);
			xmax = Mathf.Clamp(xmax, 0, this.Width);
			zmin = Mathf.Clamp(zmin, 0, this.Depth);
			zmax = Mathf.Clamp(zmax, 0, this.Depth);
			if (!this.erosionUseTags)
			{
				for (int i = 0; i < this.erodeIterations; i++)
				{
					for (int j = zmin; j < zmax; j++)
					{
						for (int k = xmin; k < xmax; k++)
						{
							GridNode gridNode = this.nodes[j * this.Width + k];
							if (gridNode.Walkable && this.ErosionAnyFalseConnections(gridNode))
							{
								gridNode.Walkable = false;
							}
						}
					}
					for (int l = zmin; l < zmax; l++)
					{
						for (int m = xmin; m < xmax; m++)
						{
							GridNode node = this.nodes[l * this.Width + m];
							this.CalculateConnections(this.nodes, m, l, node);
						}
					}
				}
			}
			else
			{
				if (this.erodeIterations + this.erosionFirstTag > 31)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Too few tags available for ",
						this.erodeIterations,
						" erode iterations and starting with tag ",
						this.erosionFirstTag,
						" (erodeIterations+erosionFirstTag > 31)"
					}));
					return;
				}
				if (this.erosionFirstTag <= 0)
				{
					Debug.LogError("First erosion tag must be greater or equal to 1");
					return;
				}
				for (int n = 0; n < this.erodeIterations; n++)
				{
					for (int num = zmin; num < zmax; num++)
					{
						for (int num2 = xmin; num2 < xmax; num2++)
						{
							GridNode gridNode2 = this.nodes[num * this.width + num2];
							if (gridNode2.Walkable && (ulong)gridNode2.Tag >= (ulong)((long)this.erosionFirstTag) && (ulong)gridNode2.Tag < (ulong)((long)(this.erosionFirstTag + n)))
							{
								if (this.neighbours == NumNeighbours.Six)
								{
									for (int num3 = 0; num3 < 6; num3++)
									{
										GridNode nodeConnection = this.GetNodeConnection(gridNode2, GridGraph.hexagonNeighbourIndices[num3]);
										if (nodeConnection != null)
										{
											uint tag = nodeConnection.Tag;
											if ((ulong)tag > (ulong)((long)(this.erosionFirstTag + n)) || (ulong)tag < (ulong)((long)this.erosionFirstTag))
											{
												nodeConnection.Tag = (uint)(this.erosionFirstTag + n);
											}
										}
									}
								}
								else
								{
									for (int num4 = 0; num4 < 4; num4++)
									{
										GridNode nodeConnection2 = this.GetNodeConnection(gridNode2, num4);
										if (nodeConnection2 != null)
										{
											uint tag2 = nodeConnection2.Tag;
											if ((ulong)tag2 > (ulong)((long)(this.erosionFirstTag + n)) || (ulong)tag2 < (ulong)((long)this.erosionFirstTag))
											{
												nodeConnection2.Tag = (uint)(this.erosionFirstTag + n);
											}
										}
									}
								}
							}
							else if (gridNode2.Walkable && n == 0 && this.ErosionAnyFalseConnections(gridNode2))
							{
								gridNode2.Tag = (uint)(this.erosionFirstTag + n);
							}
						}
					}
				}
			}
		}

		public virtual bool IsValidConnection(GridNode n1, GridNode n2)
		{
			return n1.Walkable && n2.Walkable && (this.maxClimb <= 0f || (float)Mathf.Abs(n1.position[this.maxClimbAxis] - n2.position[this.maxClimbAxis]) <= this.maxClimb * 1000f);
		}

		public static void CalculateConnections(GridNode node)
		{
			GridGraph gridGraph = AstarData.GetGraph(node) as GridGraph;
			if (gridGraph != null)
			{
				int nodeInGridIndex = node.NodeInGridIndex;
				int x = nodeInGridIndex % gridGraph.width;
				int z = nodeInGridIndex / gridGraph.width;
				gridGraph.CalculateConnections(gridGraph.nodes, x, z, node);
			}
		}

		public virtual void CalculateConnections(GridNode[] nodes, int x, int z, GridNode node)
		{
			node.ResetConnectionsInternal();
			if (!node.Walkable)
			{
				return;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			if (this.neighbours == NumNeighbours.Four || this.neighbours == NumNeighbours.Eight)
			{
				if (this.corners == null)
				{
					this.corners = new int[4];
				}
				else
				{
					for (int i = 0; i < 4; i++)
					{
						this.corners[i] = 0;
					}
				}
				int j = 0;
				int num = 3;
				while (j < 4)
				{
					int num2 = x + this.neighbourXOffsets[j];
					int num3 = z + this.neighbourZOffsets[j];
					if (num2 >= 0 && num3 >= 0 && num2 < this.width && num3 < this.depth)
					{
						GridNode n = nodes[nodeInGridIndex + this.neighbourOffsets[j]];
						if (this.IsValidConnection(node, n))
						{
							node.SetConnectionInternal(j, true);
							this.corners[j]++;
							this.corners[num]++;
						}
						else
						{
							node.SetConnectionInternal(j, false);
						}
					}
					num = j;
					j++;
				}
				if (this.neighbours == NumNeighbours.Eight)
				{
					if (this.cutCorners)
					{
						for (int k = 0; k < 4; k++)
						{
							if (this.corners[k] >= 1)
							{
								int num4 = x + this.neighbourXOffsets[k + 4];
								int num5 = z + this.neighbourZOffsets[k + 4];
								if (num4 >= 0 && num5 >= 0 && num4 < this.width && num5 < this.depth)
								{
									GridNode n2 = nodes[nodeInGridIndex + this.neighbourOffsets[k + 4]];
									node.SetConnectionInternal(k + 4, this.IsValidConnection(node, n2));
								}
							}
						}
					}
					else
					{
						for (int l = 0; l < 4; l++)
						{
							if (this.corners[l] == 2)
							{
								GridNode n3 = nodes[nodeInGridIndex + this.neighbourOffsets[l + 4]];
								node.SetConnectionInternal(l + 4, this.IsValidConnection(node, n3));
							}
						}
					}
				}
			}
			else
			{
				for (int m = 0; m < GridGraph.hexagonNeighbourIndices.Length; m++)
				{
					int num6 = GridGraph.hexagonNeighbourIndices[m];
					int num7 = x + this.neighbourXOffsets[num6];
					int num8 = z + this.neighbourZOffsets[num6];
					if (num7 >= 0 && num8 >= 0 && num7 < this.width && num8 < this.depth)
					{
						GridNode n4 = nodes[nodeInGridIndex + this.neighbourOffsets[num6]];
						node.SetConnectionInternal(num6, this.IsValidConnection(node, n4));
					}
				}
			}
		}

		public void OnPostScan(AstarPath script)
		{
			AstarPath.OnPostScan = (OnScanDelegate)Delegate.Remove(AstarPath.OnPostScan, new OnScanDelegate(this.OnPostScan));
			if (!this.autoLinkGrids || this.autoLinkDistLimit <= 0f)
			{
				return;
			}
			throw new NotSupportedException();
		}

		public override void OnDrawGizmos(bool drawNodes)
		{
			Gizmos.matrix = this.boundsMatrix;
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.size.x, 0f, this.size.y));
			Gizmos.matrix = Matrix4x4.identity;
			if (!drawNodes || this.nodes == null || this.nodes.Length != this.width * this.depth)
			{
				return;
			}
			PathHandler debugPathData = AstarPath.active.debugPathData;
			bool flag = AstarPath.active.showSearchTree && debugPathData != null;
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					GridNode gridNode = this.nodes[i * this.width + j];
					if (gridNode.Walkable)
					{
						Gizmos.color = this.NodeColor(gridNode, debugPathData);
						Vector3 from = (Vector3)gridNode.position;
						if (flag)
						{
							if (NavGraph.InSearchTree(gridNode, AstarPath.active.debugPath))
							{
								PathNode pathNode = debugPathData.GetPathNode(gridNode);
								if (pathNode != null && pathNode.parent != null)
								{
									Gizmos.DrawLine(from, (Vector3)pathNode.parent.node.position);
								}
							}
						}
						else
						{
							for (int k = 0; k < 8; k++)
							{
								if (gridNode.GetConnectionInternal(k))
								{
									GridNode gridNode2 = this.nodes[gridNode.NodeInGridIndex + this.neighbourOffsets[k]];
									Gizmos.DrawLine(from, (Vector3)gridNode2.position);
								}
							}
							if (gridNode.connections != null)
							{
								for (int l = 0; l < gridNode.connections.Length; l++)
								{
									GraphNode graphNode = gridNode.connections[l];
									Gizmos.DrawLine(from, (Vector3)graphNode.position);
								}
							}
						}
					}
				}
			}
		}

		protected static void GetBoundsMinMax(Bounds b, Matrix4x4 matrix, out Vector3 min, out Vector3 max)
		{
			Vector3[] array = new Vector3[]
			{
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, -b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, -b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, -b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, -b.extents.y, -b.extents.z))
			};
			min = array[0];
			max = array[0];
			for (int i = 1; i < 8; i++)
			{
				min = Vector3.Min(min, array[i]);
				max = Vector3.Max(max, array[i]);
			}
		}

		public List<GraphNode> GetNodesInArea(Bounds b)
		{
			return this.GetNodesInArea(b, null);
		}

		public List<GraphNode> GetNodesInArea(GraphUpdateShape shape)
		{
			return this.GetNodesInArea(shape.GetBounds(), shape);
		}

		private List<GraphNode> GetNodesInArea(Bounds b, GraphUpdateShape shape)
		{
			if (this.nodes == null || this.width * this.depth != this.nodes.Length)
			{
				return null;
			}
			List<GraphNode> list = ListPool<GraphNode>.Claim();
			Vector3 vector;
			Vector3 vector2;
			GridGraph.GetBoundsMinMax(b, this.inverseMatrix, out vector, out vector2);
			int xmin = Mathf.RoundToInt(vector.x - 0.5f);
			int xmax = Mathf.RoundToInt(vector2.x - 0.5f);
			int ymin = Mathf.RoundToInt(vector.z - 0.5f);
			int ymax = Mathf.RoundToInt(vector2.z - 0.5f);
			IntRect a = new IntRect(xmin, ymin, xmax, ymax);
			IntRect b2 = new IntRect(0, 0, this.width - 1, this.depth - 1);
			IntRect intRect = IntRect.Intersection(a, b2);
			for (int i = intRect.xmin; i <= intRect.xmax; i++)
			{
				for (int j = intRect.ymin; j <= intRect.ymax; j++)
				{
					int num = j * this.width + i;
					GraphNode graphNode = this.nodes[num];
					if (b.Contains((Vector3)graphNode.position) && (shape == null || shape.Contains((Vector3)graphNode.position)))
					{
						list.Add(graphNode);
					}
				}
			}
			return list;
		}

		public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		public void UpdateAreaInit(GraphUpdateObject o)
		{
		}

		public void UpdateArea(GraphUpdateObject o)
		{
			if (this.nodes == null || this.nodes.Length != this.width * this.depth)
			{
				Debug.LogWarning("The Grid Graph is not scanned, cannot update area ");
				return;
			}
			Bounds b = o.bounds;
			Vector3 a;
			Vector3 a2;
			GridGraph.GetBoundsMinMax(b, this.inverseMatrix, out a, out a2);
			int xmin = Mathf.RoundToInt(a.x - 0.5f);
			int xmax = Mathf.RoundToInt(a2.x - 0.5f);
			int ymin = Mathf.RoundToInt(a.z - 0.5f);
			int ymax = Mathf.RoundToInt(a2.z - 0.5f);
			IntRect intRect = new IntRect(xmin, ymin, xmax, ymax);
			IntRect intRect2 = intRect;
			IntRect b2 = new IntRect(0, 0, this.width - 1, this.depth - 1);
			IntRect intRect3 = intRect;
			int num = (!o.updateErosion) ? 0 : this.erodeIterations;
			bool flag = o.updatePhysics || o.modifyWalkability;
			if (o.updatePhysics && !o.modifyWalkability && this.collision.collisionCheck)
			{
				Vector3 a3 = new Vector3(this.collision.diameter, 0f, this.collision.diameter) * 0.5f;
				a -= a3 * 1.02f;
				a2 += a3 * 1.02f;
				intRect3 = new IntRect(Mathf.RoundToInt(a.x - 0.5f), Mathf.RoundToInt(a.z - 0.5f), Mathf.RoundToInt(a2.x - 0.5f), Mathf.RoundToInt(a2.z - 0.5f));
				intRect2 = IntRect.Union(intRect3, intRect2);
			}
			if (flag || num > 0)
			{
				intRect2 = intRect2.Expand(num + 1);
			}
			IntRect intRect4 = IntRect.Intersection(intRect2, b2);
			for (int i = intRect4.xmin; i <= intRect4.xmax; i++)
			{
				for (int j = intRect4.ymin; j <= intRect4.ymax; j++)
				{
					o.WillUpdateNode(this.nodes[j * this.width + i]);
				}
			}
			if (o.updatePhysics && !o.modifyWalkability)
			{
				this.collision.Initialize(this.matrix, this.nodeSize);
				intRect4 = IntRect.Intersection(intRect3, b2);
				for (int k = intRect4.xmin; k <= intRect4.xmax; k++)
				{
					for (int l = intRect4.ymin; l <= intRect4.ymax; l++)
					{
						int num2 = l * this.width + k;
						GridNode node = this.nodes[num2];
						this.UpdateNodePositionCollision(node, k, l, o.resetPenaltyOnPhysics);
					}
				}
			}
			intRect4 = IntRect.Intersection(intRect, b2);
			for (int m = intRect4.xmin; m <= intRect4.xmax; m++)
			{
				for (int n = intRect4.ymin; n <= intRect4.ymax; n++)
				{
					int num3 = n * this.width + m;
					GridNode gridNode = this.nodes[num3];
					if (flag)
					{
						gridNode.Walkable = gridNode.WalkableErosion;
						if (o.bounds.Contains((Vector3)gridNode.position))
						{
							o.Apply(gridNode);
						}
						gridNode.WalkableErosion = gridNode.Walkable;
					}
					else if (o.bounds.Contains((Vector3)gridNode.position))
					{
						o.Apply(gridNode);
					}
				}
			}
			if (flag && num == 0)
			{
				intRect4 = IntRect.Intersection(intRect2, b2);
				for (int num4 = intRect4.xmin; num4 <= intRect4.xmax; num4++)
				{
					for (int num5 = intRect4.ymin; num5 <= intRect4.ymax; num5++)
					{
						int num6 = num5 * this.width + num4;
						GridNode node2 = this.nodes[num6];
						this.CalculateConnections(this.nodes, num4, num5, node2);
					}
				}
			}
			else if (flag && num > 0)
			{
				IntRect a4 = IntRect.Union(intRect, intRect3).Expand(num);
				IntRect a5 = a4.Expand(num);
				a4 = IntRect.Intersection(a4, b2);
				a5 = IntRect.Intersection(a5, b2);
				for (int num7 = a5.xmin; num7 <= a5.xmax; num7++)
				{
					for (int num8 = a5.ymin; num8 <= a5.ymax; num8++)
					{
						int num9 = num8 * this.width + num7;
						GridNode gridNode2 = this.nodes[num9];
						bool walkable = gridNode2.Walkable;
						gridNode2.Walkable = gridNode2.WalkableErosion;
						if (!a4.Contains(num7, num8))
						{
							gridNode2.TmpWalkable = walkable;
						}
					}
				}
				for (int num10 = a5.xmin; num10 <= a5.xmax; num10++)
				{
					for (int num11 = a5.ymin; num11 <= a5.ymax; num11++)
					{
						int num12 = num11 * this.width + num10;
						GridNode node3 = this.nodes[num12];
						this.CalculateConnections(this.nodes, num10, num11, node3);
					}
				}
				this.ErodeWalkableArea(a5.xmin, a5.ymin, a5.xmax + 1, a5.ymax + 1);
				for (int num13 = a5.xmin; num13 <= a5.xmax; num13++)
				{
					for (int num14 = a5.ymin; num14 <= a5.ymax; num14++)
					{
						if (!a4.Contains(num13, num14))
						{
							int num15 = num14 * this.width + num13;
							GridNode gridNode3 = this.nodes[num15];
							gridNode3.Walkable = gridNode3.TmpWalkable;
						}
					}
				}
				for (int num16 = a5.xmin; num16 <= a5.xmax; num16++)
				{
					for (int num17 = a5.ymin; num17 <= a5.ymax; num17++)
					{
						int num18 = num17 * this.width + num16;
						GridNode node4 = this.nodes[num18];
						this.CalculateConnections(this.nodes, num16, num17, node4);
					}
				}
			}
		}

		public bool Linecast(Vector3 _a, Vector3 _b)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(_a, _b, null, out graphHitInfo);
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(_a, _b, hint, out graphHitInfo);
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint, out GraphHitInfo hit)
		{
			return this.Linecast(_a, _b, hint, out hit, null);
		}

		protected static float CrossMagnitude(Vector2 a, Vector2 b)
		{
			return a.x * b.y - b.x * a.y;
		}

		protected virtual GridNodeBase GetNeighbourAlongDirection(GridNodeBase node, int direction)
		{
			GridNode gridNode = node as GridNode;
			if (gridNode.GetConnectionInternal(direction))
			{
				return this.nodes[gridNode.NodeInGridIndex + this.neighbourOffsets[direction]];
			}
			return null;
		}

		protected bool ClipLineSegmentToBounds(Vector3 a, Vector3 b, out Vector3 outA, out Vector3 outB)
		{
			if (a.x < 0f || a.z < 0f || a.x > (float)this.width || a.z > (float)this.depth || b.x < 0f || b.z < 0f || b.x > (float)this.width || b.z > (float)this.depth)
			{
				Vector3 vector = new Vector3(0f, 0f, 0f);
				Vector3 vector2 = new Vector3(0f, 0f, (float)this.depth);
				Vector3 vector3 = new Vector3((float)this.width, 0f, (float)this.depth);
				Vector3 vector4 = new Vector3((float)this.width, 0f, 0f);
				int num = 0;
				bool flag;
				Vector3 vector5 = Polygon.SegmentIntersectionPoint(a, b, vector, vector2, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector, vector2, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(a, b, vector2, vector3, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector2, vector3, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(a, b, vector3, vector4, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector3, vector4, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(a, b, vector4, vector, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector4, vector, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				if (num == 0)
				{
					outA = Vector3.zero;
					outB = Vector3.zero;
					return false;
				}
			}
			outA = a;
			outB = b;
			return true;
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			hit = default(GraphHitInfo);
			hit.origin = _a;
			Vector3 vector = this.inverseMatrix.MultiplyPoint3x4(_a);
			Vector3 vector2 = this.inverseMatrix.MultiplyPoint3x4(_b);
			if (!this.ClipLineSegmentToBounds(vector, vector2, out vector, out vector2))
			{
				return false;
			}
			GridNodeBase gridNodeBase = base.GetNearest(this.matrix.MultiplyPoint3x4(vector), NNConstraint.None).node as GridNodeBase;
			GridNodeBase gridNodeBase2 = base.GetNearest(this.matrix.MultiplyPoint3x4(vector2), NNConstraint.None).node as GridNodeBase;
			if (!gridNodeBase.Walkable)
			{
				hit.node = gridNodeBase;
				hit.point = this.matrix.MultiplyPoint3x4(vector);
				hit.tangentOrigin = hit.point;
				return true;
			}
			Vector2 vector3 = new Vector2(vector.x, vector.z);
			Vector2 vector4 = new Vector2(vector2.x, vector2.z);
			vector3 -= Vector2.one * 0.5f;
			vector4 -= Vector2.one * 0.5f;
			if (gridNodeBase == null || gridNodeBase2 == null)
			{
				hit.node = null;
				hit.point = _a;
				return true;
			}
			Vector2 a = vector4 - vector3;
			Int2 @int = new Int2((int)Mathf.Sign(a.x), (int)Mathf.Sign(a.y));
			float num = GridGraph.CrossMagnitude(a, new Vector2((float)@int.x, (float)@int.y)) * 0.5f;
			int num2;
			int num3;
			if (a.y >= 0f)
			{
				if (a.x >= 0f)
				{
					num2 = 1;
					num3 = 2;
				}
				else
				{
					num2 = 2;
					num3 = 3;
				}
			}
			else if (a.x < 0f)
			{
				num2 = 3;
				num3 = 0;
			}
			else
			{
				num2 = 0;
				num3 = 1;
			}
			GridNodeBase gridNodeBase3 = gridNodeBase;
			while (gridNodeBase3.NodeInGridIndex != gridNodeBase2.NodeInGridIndex)
			{
				if (trace != null)
				{
					trace.Add(gridNodeBase3);
				}
				Vector2 a2 = new Vector2((float)(gridNodeBase3.NodeInGridIndex % this.width), (float)(gridNodeBase3.NodeInGridIndex / this.width));
				float num4 = GridGraph.CrossMagnitude(a, a2 - vector3);
				float num5 = num4 + num;
				int num6 = (num5 >= 0f) ? num2 : num3;
				GridNodeBase neighbourAlongDirection = this.GetNeighbourAlongDirection(gridNodeBase3, num6);
				if (neighbourAlongDirection == null)
				{
					Vector2 vector5 = a2 + new Vector2((float)this.neighbourXOffsets[num6], (float)this.neighbourZOffsets[num6]) * 0.5f;
					Vector2 b;
					if (this.neighbourXOffsets[num6] == 0)
					{
						b = new Vector2(1f, 0f);
					}
					else
					{
						b = new Vector2(0f, 1f);
					}
					Vector2 vector6 = Polygon.IntersectionPoint(vector5, vector5 + b, vector3, vector4);
					Vector3 vector7 = this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNodeBase3.position);
					Vector3 v = new Vector3(vector6.x + 0.5f, vector7.y, vector6.y + 0.5f);
					Vector3 v2 = new Vector3(vector5.x + 0.5f, vector7.y, vector5.y + 0.5f);
					hit.point = this.matrix.MultiplyPoint3x4(v);
					hit.tangentOrigin = this.matrix.MultiplyPoint3x4(v2);
					hit.tangent = this.matrix.MultiplyVector(new Vector3(b.x, 0f, b.y));
					hit.node = gridNodeBase3;
					return true;
				}
				gridNodeBase3 = neighbourAlongDirection;
			}
			if (trace != null)
			{
				trace.Add(gridNodeBase3);
			}
			if (gridNodeBase3 == gridNodeBase2)
			{
				return false;
			}
			hit.point = (Vector3)gridNodeBase3.position;
			hit.tangentOrigin = hit.point;
			return true;
		}

		public bool SnappedLinecast(Vector3 a, Vector3 b, GraphNode hint, out GraphHitInfo hit)
		{
			return this.Linecast((Vector3)base.GetNearest(a, NNConstraint.None).node.position, (Vector3)base.GetNearest(b, NNConstraint.None).node.position, hint, out hit);
		}

		public bool CheckConnection(GridNode node, int dir)
		{
			if (this.neighbours == NumNeighbours.Eight || this.neighbours == NumNeighbours.Six || dir < 4)
			{
				return this.HasNodeConnection(node, dir);
			}
			int num = dir - 4 - 1 & 3;
			int num2 = dir - 4 + 1 & 3;
			if (!this.HasNodeConnection(node, num) || !this.HasNodeConnection(node, num2))
			{
				return false;
			}
			GridNode gridNode = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num]];
			GridNode gridNode2 = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num2]];
			return gridNode.Walkable && gridNode2.Walkable && this.HasNodeConnection(gridNode2, num) && this.HasNodeConnection(gridNode, num2);
		}

		public override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null)
			{
				ctx.writer.Write(-1);
				return;
			}
			ctx.writer.Write(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].SerializeNode(ctx);
			}
		}

		public override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = null;
				return;
			}
			this.nodes = new GridNode[num];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i] = new GridNode(this.active);
				this.nodes[i].DeserializeNode(ctx);
			}
		}

		public override void PostDeserialization()
		{
			this.GenerateMatrix();
			this.SetUpOffsetsAndCosts();
			if (this.nodes == null || this.nodes.Length == 0)
			{
				return;
			}
			if (this.width * this.depth != this.nodes.Length)
			{
				Debug.LogError("Node data did not match with bounds data. Probably a change to the bounds/width/depth data was made after scanning the graph just prior to saving it. Nodes will be discarded");
				this.nodes = new GridNode[0];
				return;
			}
			GridNode.SetGridGraph(AstarPath.active.astarData.GetGraphIndex(this), this);
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					GridNode gridNode = this.nodes[i * this.width + j];
					if (gridNode == null)
					{
						Debug.LogError("Deserialization Error : Couldn't cast the node to the appropriate type - GridGenerator");
						return;
					}
					gridNode.NodeInGridIndex = i * this.width + j;
				}
			}
		}
	}
}
