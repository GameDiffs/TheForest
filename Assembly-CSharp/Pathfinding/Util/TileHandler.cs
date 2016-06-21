using Pathfinding.ClipperLib;
using Pathfinding.Poly2Tri;
using Pathfinding.Voxels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Util
{
	public class TileHandler
	{
		public class TileType
		{
			private Int3[] verts;

			private int[] tris;

			private Int3 offset;

			private int lastYOffset;

			private int lastRotation;

			private int width;

			private int depth;

			private static readonly int[] Rotations = new int[]
			{
				1,
				0,
				0,
				1,
				0,
				1,
				-1,
				0,
				-1,
				0,
				0,
				-1,
				0,
				-1,
				1,
				0
			};

			public int Width
			{
				get
				{
					return this.width;
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			public TileType(Int3[] sourceVerts, int[] sourceTris, Int3 tileSize, Int3 centerOffset, int width = 1, int depth = 1)
			{
				if (sourceVerts == null)
				{
					throw new ArgumentNullException("sourceVerts");
				}
				if (sourceTris == null)
				{
					throw new ArgumentNullException("sourceTris");
				}
				this.tris = new int[sourceTris.Length];
				for (int i = 0; i < this.tris.Length; i++)
				{
					this.tris[i] = sourceTris[i];
				}
				this.verts = new Int3[sourceVerts.Length];
				for (int j = 0; j < sourceVerts.Length; j++)
				{
					this.verts[j] = sourceVerts[j] + centerOffset;
				}
				this.offset = tileSize / 2f;
				this.offset.x = this.offset.x * width;
				this.offset.z = this.offset.z * depth;
				this.offset.y = 0;
				for (int k = 0; k < sourceVerts.Length; k++)
				{
					this.verts[k] = this.verts[k] + this.offset;
				}
				this.lastRotation = 0;
				this.lastYOffset = 0;
				this.width = width;
				this.depth = depth;
			}

			public TileType(Mesh source, Int3 tileSize, Int3 centerOffset, int width = 1, int depth = 1)
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				Vector3[] vertices = source.vertices;
				this.tris = source.triangles;
				this.verts = new Int3[vertices.Length];
				for (int i = 0; i < vertices.Length; i++)
				{
					this.verts[i] = (Int3)vertices[i] + centerOffset;
				}
				this.offset = tileSize / 2f;
				this.offset.x = this.offset.x * width;
				this.offset.z = this.offset.z * depth;
				this.offset.y = 0;
				for (int j = 0; j < vertices.Length; j++)
				{
					this.verts[j] = this.verts[j] + this.offset;
				}
				this.lastRotation = 0;
				this.lastYOffset = 0;
				this.width = width;
				this.depth = depth;
			}

			public void Load(out Int3[] verts, out int[] tris, int rotation, int yoffset)
			{
				rotation = (rotation % 4 + 4) % 4;
				int num = rotation;
				rotation = (rotation - this.lastRotation % 4 + 4) % 4;
				this.lastRotation = num;
				verts = this.verts;
				int num2 = yoffset - this.lastYOffset;
				this.lastYOffset = yoffset;
				if (rotation != 0 || num2 != 0)
				{
					for (int i = 0; i < verts.Length; i++)
					{
						Int3 @int = verts[i] - this.offset;
						Int3 lhs = @int;
						lhs.y += num2;
						lhs.x = @int.x * TileHandler.TileType.Rotations[rotation * 4] + @int.z * TileHandler.TileType.Rotations[rotation * 4 + 1];
						lhs.z = @int.x * TileHandler.TileType.Rotations[rotation * 4 + 2] + @int.z * TileHandler.TileType.Rotations[rotation * 4 + 3];
						verts[i] = lhs + this.offset;
					}
				}
				tris = this.tris;
			}
		}

		[Flags]
		public enum CutMode
		{
			CutAll = 1,
			CutDual = 2,
			CutExtra = 4
		}

		private const int CUT_ALL = 0;

		private const int CUT_DUAL = 1;

		private const int CUT_BREAK = 2;

		private RecastGraph _graph;

		private List<TileHandler.TileType> tileTypes = new List<TileHandler.TileType>();

		private Clipper clipper;

		private int[] cached_int_array = new int[32];

		private Dictionary<Int3, int> cached_Int3_int_dict = new Dictionary<Int3, int>();

		private Dictionary<Int2, int> cached_Int2_int_dict = new Dictionary<Int2, int>();

		private TileHandler.TileType[] activeTileTypes;

		private int[] activeTileRotations;

		private int[] activeTileOffsets;

		private bool[] reloadedInBatch;

		private bool isBatching;

		public RecastGraph graph
		{
			get
			{
				return this._graph;
			}
		}

		public TileHandler(RecastGraph graph)
		{
			if (graph == null)
			{
				throw new ArgumentNullException("graph");
			}
			if (graph.GetTiles() == null)
			{
				throw new ArgumentException("graph has no tiles. Please scan the graph before creating a TileHandler");
			}
			this.activeTileTypes = new TileHandler.TileType[graph.tileXCount * graph.tileZCount];
			this.activeTileRotations = new int[this.activeTileTypes.Length];
			this.activeTileOffsets = new int[this.activeTileTypes.Length];
			this.reloadedInBatch = new bool[this.activeTileTypes.Length];
			this._graph = graph;
		}

		public int GetActiveRotation(Int2 p)
		{
			return this.activeTileRotations[p.x + p.y * this._graph.tileXCount];
		}

		public TileHandler.TileType GetTileType(int index)
		{
			return this.tileTypes[index];
		}

		public int GetTileTypeCount()
		{
			return this.tileTypes.Count;
		}

		public TileHandler.TileType RegisterTileType(Mesh source, Int3 centerOffset, int width = 1, int depth = 1)
		{
			TileHandler.TileType tileType = new TileHandler.TileType(source, new Int3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize), centerOffset, width, depth);
			this.tileTypes.Add(tileType);
			return tileType;
		}

		public void CreateTileTypesFromGraph()
		{
			RecastGraph.NavmeshTile[] tiles = this.graph.GetTiles();
			if (tiles == null || tiles.Length != this.graph.tileXCount * this.graph.tileZCount)
			{
				throw new InvalidOperationException("Graph tiles are invalid (either null or number of tiles is not equal to width*depth of the graph");
			}
			for (int i = 0; i < this.graph.tileZCount; i++)
			{
				for (int j = 0; j < this.graph.tileXCount; j++)
				{
					RecastGraph.NavmeshTile navmeshTile = tiles[j + i * this.graph.tileXCount];
					Int3 @int = (Int3)this.graph.GetTileBounds(j, i, 1, 1).min;
					Int3 tileSize = new Int3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize);
					@int += new Int3(tileSize.x * navmeshTile.w / 2, 0, tileSize.z * navmeshTile.d / 2);
					@int = -@int;
					TileHandler.TileType tileType = new TileHandler.TileType(navmeshTile.verts, navmeshTile.tris, tileSize, @int, navmeshTile.w, navmeshTile.d);
					this.tileTypes.Add(tileType);
					int num = j + i * this.graph.tileXCount;
					this.activeTileTypes[num] = tileType;
					this.activeTileRotations[num] = 0;
					this.activeTileOffsets[num] = 0;
				}
			}
		}

		public bool StartBatchLoad()
		{
			if (this.isBatching)
			{
				return false;
			}
			this.isBatching = true;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.StartBatchTileUpdate();
				return true;
			}));
			return true;
		}

		public void EndBatchLoad()
		{
			if (!this.isBatching)
			{
				throw new Exception("Ending batching when batching has not been started");
			}
			for (int i = 0; i < this.reloadedInBatch.Length; i++)
			{
				this.reloadedInBatch[i] = false;
			}
			this.isBatching = false;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.EndBatchTileUpdate();
				return true;
			}));
		}

		private void CutPoly(Int3[] verts, int[] tris, ref Int3[] outVertsArr, ref int[] outTrisArr, out int outVCount, out int outTCount, Int3[] extraShape, Int3 cuttingOffset, Bounds realBounds, TileHandler.CutMode mode = TileHandler.CutMode.CutAll | TileHandler.CutMode.CutDual, int perturbate = 0)
		{
			if (verts.Length == 0 || tris.Length == 0)
			{
				outVCount = 0;
				outTCount = 0;
				outTrisArr = new int[0];
				outVertsArr = new Int3[0];
				return;
			}
			List<IntPoint> list = null;
			if (extraShape == null && (mode & TileHandler.CutMode.CutExtra) != (TileHandler.CutMode)0)
			{
				throw new Exception("extraShape is null and the CutMode specifies that it should be used. Cannot use null shape.");
			}
			if ((mode & TileHandler.CutMode.CutExtra) != (TileHandler.CutMode)0)
			{
				list = new List<IntPoint>(extraShape.Length);
				for (int i = 0; i < extraShape.Length; i++)
				{
					list.Add(new IntPoint((long)(extraShape[i].x + cuttingOffset.x), (long)(extraShape[i].z + cuttingOffset.z)));
				}
			}
			List<IntPoint> list2 = new List<IntPoint>(5);
			Dictionary<TriangulationPoint, int> dictionary = new Dictionary<TriangulationPoint, int>();
			List<PolygonPoint> list3 = new List<PolygonPoint>();
			Pathfinding.IntRect b = new Pathfinding.IntRect(verts[0].x, verts[0].z, verts[0].x, verts[0].z);
			for (int j = 0; j < verts.Length; j++)
			{
				b = b.ExpandToContain(verts[j].x, verts[j].z);
			}
			List<Int3> list4 = ListPool<Int3>.Claim(verts.Length * 2);
			List<int> list5 = ListPool<int>.Claim(tris.Length);
			PolyTree polyTree = new PolyTree();
			List<List<IntPoint>> list6 = new List<List<IntPoint>>();
			Stack<Pathfinding.Poly2Tri.Polygon> stack = new Stack<Pathfinding.Poly2Tri.Polygon>();
			this.clipper = (this.clipper ?? new Clipper(0));
			this.clipper.ReverseSolution = true;
			this.clipper.StrictlySimple = true;
			List<NavmeshCut> list7;
			if (mode == TileHandler.CutMode.CutExtra)
			{
				list7 = ListPool<NavmeshCut>.Claim();
			}
			else
			{
				list7 = NavmeshCut.GetAllInRange(realBounds);
			}
			List<int> list8 = ListPool<int>.Claim();
			List<Pathfinding.IntRect> list9 = ListPool<Pathfinding.IntRect>.Claim();
			List<Int2> list10 = ListPool<Int2>.Claim();
			List<List<IntPoint>> list11 = new List<List<IntPoint>>();
			List<bool> list12 = ListPool<bool>.Claim();
			List<bool> list13 = ListPool<bool>.Claim();
			if (perturbate > 10)
			{
				Debug.LogError("Too many perturbations aborting : " + mode);
				Debug.Break();
				outVCount = verts.Length;
				outTCount = tris.Length;
				outTrisArr = tris;
				outVertsArr = verts;
				return;
			}
			System.Random random = null;
			if (perturbate > 0)
			{
				random = new System.Random();
			}
			for (int k = 0; k < list7.Count; k++)
			{
				Bounds bounds = list7[k].GetBounds();
				Int3 @int = (Int3)bounds.min + cuttingOffset;
				Int3 int2 = (Int3)bounds.max + cuttingOffset;
				Pathfinding.IntRect a = new Pathfinding.IntRect(@int.x, @int.z, int2.x, int2.z);
				if (Pathfinding.IntRect.Intersects(a, b))
				{
					Int2 int3 = new Int2(0, 0);
					if (perturbate > 0)
					{
						int3.x = random.Next() % 6 * perturbate - 3 * perturbate;
						if (int3.x >= 0)
						{
							int3.x++;
						}
						int3.y = random.Next() % 6 * perturbate - 3 * perturbate;
						if (int3.y >= 0)
						{
							int3.y++;
						}
					}
					int count = list11.Count;
					list7[k].GetContour(list11);
					for (int l = count; l < list11.Count; l++)
					{
						List<IntPoint> list14 = list11[l];
						if (list14.Count == 0)
						{
							Debug.LogError("Zero Length Contour");
							list9.Add(default(Pathfinding.IntRect));
							list10.Add(new Int2(0, 0));
						}
						else
						{
							Pathfinding.IntRect item = new Pathfinding.IntRect((int)list14[0].X + cuttingOffset.x, (int)list14[0].Y + cuttingOffset.y, (int)list14[0].X + cuttingOffset.x, (int)list14[0].Y + cuttingOffset.y);
							for (int m = 0; m < list14.Count; m++)
							{
								IntPoint value = list14[m];
								value.X += (long)cuttingOffset.x;
								value.Y += (long)cuttingOffset.z;
								if (perturbate > 0)
								{
									value.X += (long)int3.x;
									value.Y += (long)int3.y;
								}
								list14[m] = value;
								item = item.ExpandToContain((int)value.X, (int)value.Y);
							}
							list10.Add(new Int2(@int.y, int2.y));
							list9.Add(item);
							list12.Add(list7[k].isDual);
							list13.Add(list7[k].cutsAddedGeom);
						}
					}
				}
			}
			List<NavmeshAdd> allInRange = NavmeshAdd.GetAllInRange(realBounds);
			Int3[] array = verts;
			int[] array2 = tris;
			int num = -1;
			int n = -3;
			Int3[] array3 = null;
			Int3[] array4 = null;
			Int3 int4 = Int3.zero;
			if (allInRange.Count > 0)
			{
				array3 = new Int3[7];
				array4 = new Int3[7];
				int4 = (Int3)realBounds.extents;
			}
			while (true)
			{
				n += 3;
				while (n >= array2.Length)
				{
					num++;
					n = 0;
					if (num >= allInRange.Count)
					{
						array = null;
						break;
					}
					if (array == verts)
					{
						array = null;
					}
					allInRange[num].GetMesh(cuttingOffset, ref array, out array2);
				}
				if (array == null)
				{
					break;
				}
				Int3 int5 = array[array2[n]];
				Int3 int6 = array[array2[n + 1]];
				Int3 int7 = array[array2[n + 2]];
				Pathfinding.IntRect a2 = new Pathfinding.IntRect(int5.x, int5.z, int5.x, int5.z);
				a2 = a2.ExpandToContain(int6.x, int6.z);
				a2 = a2.ExpandToContain(int7.x, int7.z);
				int num2 = Math.Min(int5.y, Math.Min(int6.y, int7.y));
				int num3 = Math.Max(int5.y, Math.Max(int6.y, int7.y));
				list8.Clear();
				bool flag = false;
				for (int num4 = 0; num4 < list11.Count; num4++)
				{
					int x = list10[num4].x;
					int y = list10[num4].y;
					if (Pathfinding.IntRect.Intersects(a2, list9[num4]) && y >= num2 && x <= num3 && (list13[num4] || num == -1))
					{
						Int3 int8 = int5;
						int8.y = x;
						Int3 int9 = int5;
						int9.y = y;
						list8.Add(num4);
						flag |= list12[num4];
					}
				}
				if (list8.Count == 0 && (mode & TileHandler.CutMode.CutExtra) == (TileHandler.CutMode)0 && (mode & TileHandler.CutMode.CutAll) != (TileHandler.CutMode)0 && num == -1)
				{
					list5.Add(list4.Count);
					list5.Add(list4.Count + 1);
					list5.Add(list4.Count + 2);
					list4.Add(int5);
					list4.Add(int6);
					list4.Add(int7);
				}
				else
				{
					list2.Clear();
					if (num == -1)
					{
						list2.Add(new IntPoint((long)int5.x, (long)int5.z));
						list2.Add(new IntPoint((long)int6.x, (long)int6.z));
						list2.Add(new IntPoint((long)int7.x, (long)int7.z));
					}
					else
					{
						array3[0] = int5;
						array3[1] = int6;
						array3[2] = int7;
						int num5 = Utility.ClipPolygon(array3, 3, array4, 1, 0, 0);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array4, num5, array3, -1, 2 * int4.x, 0);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array3, num5, array4, 1, 0, 2);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array4, num5, array3, -1, 2 * int4.z, 2);
						if (num5 == 0)
						{
							continue;
						}
						for (int num6 = 0; num6 < num5; num6++)
						{
							list2.Add(new IntPoint((long)array3[num6].x, (long)array3[num6].z));
						}
					}
					dictionary.Clear();
					Int3 int10 = int6 - int5;
					Int3 int11 = int7 - int5;
					Int3 int12 = int10;
					Int3 int13 = int11;
					int12.y = 0;
					int13.y = 0;
					for (int num7 = 0; num7 < 16; num7++)
					{
						if ((mode >> (num7 & 31) & TileHandler.CutMode.CutAll) != (TileHandler.CutMode)0)
						{
							if (1 << num7 == 1)
							{
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								for (int num8 = 0; num8 < list8.Count; num8++)
								{
									this.clipper.AddPolygon(list11[list8[num8]], PolyType.ptClip);
								}
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctDifference, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							else if (1 << num7 == 2)
							{
								if (!flag)
								{
									goto IL_1174;
								}
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								for (int num9 = 0; num9 < list8.Count; num9++)
								{
									if (list12[list8[num9]])
									{
										this.clipper.AddPolygon(list11[list8[num9]], PolyType.ptClip);
									}
								}
								list6.Clear();
								this.clipper.Execute(ClipType.ctIntersection, list6, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
								this.clipper.Clear();
								for (int num10 = 0; num10 < list6.Count; num10++)
								{
									this.clipper.AddPolygon(list6[num10], (!Clipper.Orientation(list6[num10])) ? PolyType.ptSubject : PolyType.ptClip);
								}
								for (int num11 = 0; num11 < list8.Count; num11++)
								{
									if (!list12[list8[num11]])
									{
										this.clipper.AddPolygon(list11[list8[num11]], PolyType.ptClip);
									}
								}
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctDifference, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							else if (1 << num7 == 4)
							{
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								this.clipper.AddPolygon(list, PolyType.ptClip);
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctIntersection, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							for (int num12 = 0; num12 < polyTree.ChildCount; num12++)
							{
								PolyNode polyNode = polyTree.Childs[num12];
								List<IntPoint> contour = polyNode.Contour;
								List<PolyNode> childs = polyNode.Childs;
								if (childs.Count == 0 && contour.Count == 3 && num == -1)
								{
									for (int num13 = 0; num13 < contour.Count; num13++)
									{
										Int3 item2 = new Int3((int)contour[num13].X, 0, (int)contour[num13].Y);
										double num14 = (double)(int6.z - int7.z) * (double)(int5.x - int7.x) + (double)(int7.x - int6.x) * (double)(int5.z - int7.z);
										if (num14 == 0.0)
										{
											Debug.LogWarning("Degenerate triangle");
										}
										else
										{
											double num15 = ((double)(int6.z - int7.z) * (double)(item2.x - int7.x) + (double)(int7.x - int6.x) * (double)(item2.z - int7.z)) / num14;
											double num16 = ((double)(int7.z - int5.z) * (double)(item2.x - int7.x) + (double)(int5.x - int7.x) * (double)(item2.z - int7.z)) / num14;
											item2.y = (int)Math.Round(num15 * (double)int5.y + num16 * (double)int6.y + (1.0 - num15 - num16) * (double)int7.y);
											list5.Add(list4.Count);
											list4.Add(item2);
										}
									}
								}
								else
								{
									Pathfinding.Poly2Tri.Polygon polygon = null;
									int num17 = -1;
									for (List<IntPoint> list15 = contour; list15 != null; list15 = ((num17 >= childs.Count) ? null : childs[num17].Contour))
									{
										list3.Clear();
										for (int num18 = 0; num18 < list15.Count; num18++)
										{
											PolygonPoint polygonPoint = new PolygonPoint((double)list15[num18].X, (double)list15[num18].Y);
											list3.Add(polygonPoint);
											Int3 item3 = new Int3((int)list15[num18].X, 0, (int)list15[num18].Y);
											double num19 = (double)(int6.z - int7.z) * (double)(int5.x - int7.x) + (double)(int7.x - int6.x) * (double)(int5.z - int7.z);
											if (num19 == 0.0)
											{
												Debug.LogWarning("Degenerate triangle");
											}
											else
											{
												double num20 = ((double)(int6.z - int7.z) * (double)(item3.x - int7.x) + (double)(int7.x - int6.x) * (double)(item3.z - int7.z)) / num19;
												double num21 = ((double)(int7.z - int5.z) * (double)(item3.x - int7.x) + (double)(int5.x - int7.x) * (double)(item3.z - int7.z)) / num19;
												item3.y = (int)Math.Round(num20 * (double)int5.y + num21 * (double)int6.y + (1.0 - num20 - num21) * (double)int7.y);
												dictionary[polygonPoint] = list4.Count;
												list4.Add(item3);
											}
										}
										Pathfinding.Poly2Tri.Polygon polygon2;
										if (stack.Count > 0)
										{
											polygon2 = stack.Pop();
											polygon2.AddPoints(list3);
										}
										else
										{
											polygon2 = new Pathfinding.Poly2Tri.Polygon(list3);
										}
										if (polygon == null)
										{
											polygon = polygon2;
										}
										else
										{
											polygon.AddHole(polygon2);
										}
										num17++;
									}
									try
									{
										P2T.Triangulate(polygon);
									}
									catch (PointOnEdgeException)
									{
										Debug.LogWarning(string.Concat(new object[]
										{
											"PointOnEdgeException, perturbating vertices slightly ( at ",
											num7,
											" in ",
											mode,
											")"
										}));
										this.CutPoly(verts, tris, ref outVertsArr, ref outTrisArr, out outVCount, out outTCount, extraShape, cuttingOffset, realBounds, mode, perturbate + 1);
										return;
									}
									for (int num22 = 0; num22 < polygon.Triangles.Count; num22++)
									{
										DelaunayTriangle delaunayTriangle = polygon.Triangles[num22];
										list5.Add(dictionary[delaunayTriangle.Points._0]);
										list5.Add(dictionary[delaunayTriangle.Points._1]);
										list5.Add(dictionary[delaunayTriangle.Points._2]);
									}
									if (polygon.Holes != null)
									{
										for (int num23 = 0; num23 < polygon.Holes.Count; num23++)
										{
											polygon.Holes[num23].Points.Clear();
											polygon.Holes[num23].ClearTriangles();
											if (polygon.Holes[num23].Holes != null)
											{
												polygon.Holes[num23].Holes.Clear();
											}
											stack.Push(polygon.Holes[num23]);
										}
									}
									polygon.ClearTriangles();
									if (polygon.Holes != null)
									{
										polygon.Holes.Clear();
									}
									polygon.Points.Clear();
									stack.Push(polygon);
								}
							}
						}
						IL_1174:;
					}
				}
			}
			Dictionary<Int3, int> dictionary2 = this.cached_Int3_int_dict;
			dictionary2.Clear();
			if (this.cached_int_array.Length < list4.Count)
			{
				this.cached_int_array = new int[Math.Max(this.cached_int_array.Length * 2, list4.Count)];
			}
			int[] array5 = this.cached_int_array;
			int num24 = 0;
			for (int num25 = 0; num25 < list4.Count; num25++)
			{
				int num26;
				if (!dictionary2.TryGetValue(list4[num25], out num26))
				{
					dictionary2.Add(list4[num25], num24);
					array5[num25] = num24;
					list4[num24] = list4[num25];
					num24++;
				}
				else
				{
					array5[num25] = num26;
				}
			}
			outTCount = list5.Count;
			if (outTrisArr == null || outTrisArr.Length < outTCount)
			{
				outTrisArr = new int[outTCount];
			}
			for (int num27 = 0; num27 < outTCount; num27++)
			{
				outTrisArr[num27] = array5[list5[num27]];
			}
			outVCount = num24;
			if (outVertsArr == null || outVertsArr.Length < outVCount)
			{
				outVertsArr = new Int3[outVCount];
			}
			for (int num28 = 0; num28 < outVCount; num28++)
			{
				outVertsArr[num28] = list4[num28];
			}
			for (int num29 = 0; num29 < list7.Count; num29++)
			{
				list7[num29].UsedForCut();
			}
			ListPool<Int3>.Release(list4);
			ListPool<int>.Release(list5);
			ListPool<int>.Release(list8);
			ListPool<Int2>.Release(list10);
			ListPool<bool>.Release(list12);
			ListPool<bool>.Release(list13);
			ListPool<Pathfinding.IntRect>.Release(list9);
			ListPool<NavmeshCut>.Release(list7);
		}

		private void DelaunayRefinement(Int3[] verts, int[] tris, ref int vCount, ref int tCount, bool delaunay, bool colinear, Int3 worldOffset)
		{
			if (tCount % 3 != 0)
			{
				throw new Exception("Triangle array length must be a multiple of 3");
			}
			Dictionary<Int2, int> dictionary = this.cached_Int2_int_dict;
			dictionary.Clear();
			for (int i = 0; i < tCount; i += 3)
			{
				if (!Pathfinding.Polygon.IsClockwise(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]))
				{
					int num = tris[i];
					tris[i] = tris[i + 2];
					tris[i + 2] = num;
				}
				dictionary[new Int2(tris[i], tris[i + 1])] = i + 2;
				dictionary[new Int2(tris[i + 1], tris[i + 2])] = i;
				dictionary[new Int2(tris[i + 2], tris[i])] = i + 1;
			}
			for (int j = 0; j < tCount; j += 3)
			{
				for (int k = 0; k < 3; k++)
				{
					int num2;
					if (dictionary.TryGetValue(new Int2(tris[j + (k + 1) % 3], tris[j + k % 3]), out num2))
					{
						Int3 @int = verts[tris[j + (k + 2) % 3]];
						Int3 int2 = verts[tris[j + (k + 1) % 3]];
						Int3 int3 = verts[tris[j + (k + 3) % 3]];
						Int3 int4 = verts[tris[num2]];
						@int.y = 0;
						int2.y = 0;
						int3.y = 0;
						int4.y = 0;
						bool flag = false;
						if (!Pathfinding.Polygon.Left(@int, int3, int4) || Pathfinding.Polygon.LeftNotColinear(@int, int2, int4))
						{
							if (!colinear)
							{
								goto IL_434;
							}
							flag = true;
						}
						if (colinear && AstarMath.DistancePointSegment(@int, int4, int2) < 9f && !dictionary.ContainsKey(new Int2(tris[j + (k + 2) % 3], tris[j + (k + 1) % 3])) && !dictionary.ContainsKey(new Int2(tris[j + (k + 1) % 3], tris[num2])))
						{
							tCount -= 3;
							int num3 = num2 / 3 * 3;
							tris[j + (k + 1) % 3] = tris[num2];
							if (num3 != tCount)
							{
								tris[num3] = tris[tCount];
								tris[num3 + 1] = tris[tCount + 1];
								tris[num3 + 2] = tris[tCount + 2];
								dictionary[new Int2(tris[num3], tris[num3 + 1])] = num3 + 2;
								dictionary[new Int2(tris[num3 + 1], tris[num3 + 2])] = num3;
								dictionary[new Int2(tris[num3 + 2], tris[num3])] = num3 + 1;
								tris[tCount] = 0;
								tris[tCount + 1] = 0;
								tris[tCount + 2] = 0;
							}
							else
							{
								tCount += 3;
							}
							dictionary[new Int2(tris[j], tris[j + 1])] = j + 2;
							dictionary[new Int2(tris[j + 1], tris[j + 2])] = j;
							dictionary[new Int2(tris[j + 2], tris[j])] = j + 1;
						}
						else if (delaunay && !flag)
						{
							float num4 = Int3.Angle(int2 - @int, int3 - @int);
							float num5 = Int3.Angle(int2 - int4, int3 - int4);
							if (num5 > 6.28318548f - 2f * num4)
							{
								tris[j + (k + 1) % 3] = tris[num2];
								int num6 = num2 / 3 * 3;
								int num7 = num2 - num6;
								tris[num6 + (num7 - 1 + 3) % 3] = tris[j + (k + 2) % 3];
								dictionary[new Int2(tris[j], tris[j + 1])] = j + 2;
								dictionary[new Int2(tris[j + 1], tris[j + 2])] = j;
								dictionary[new Int2(tris[j + 2], tris[j])] = j + 1;
								dictionary[new Int2(tris[num6], tris[num6 + 1])] = num6 + 2;
								dictionary[new Int2(tris[num6 + 1], tris[num6 + 2])] = num6;
								dictionary[new Int2(tris[num6 + 2], tris[num6])] = num6 + 1;
							}
						}
					}
					IL_434:;
				}
			}
		}

		private Vector3 Point2D2V3(TriangulationPoint p)
		{
			return new Vector3((float)p.X, 0f, (float)p.Y) * 0.001f;
		}

		private Int3 IntPoint2Int3(IntPoint p)
		{
			return new Int3((int)p.X, 0, (int)p.Y);
		}

		public void ClearTile(int x, int z)
		{
			if (AstarPath.active == null)
			{
				return;
			}
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount)
			{
				return;
			}
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.ReplaceTile(x, z, new Int3[0], new int[0], false);
				this.activeTileTypes[x + z * this.graph.tileXCount] = null;
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
				AstarPath.active.QueueWorkItemFloodFill();
				return true;
			}));
		}

		public void ReloadInBounds(Bounds b)
		{
			Int2 tileCoordinates = this.graph.GetTileCoordinates(b.min);
			Int2 tileCoordinates2 = this.graph.GetTileCoordinates(b.max);
			Pathfinding.IntRect a = new Pathfinding.IntRect(tileCoordinates.x, tileCoordinates.y, tileCoordinates2.x, tileCoordinates2.y);
			a = Pathfinding.IntRect.Intersection(a, new Pathfinding.IntRect(0, 0, this.graph.tileXCount - 1, this.graph.tileZCount - 1));
			if (!a.IsValid())
			{
				return;
			}
			for (int i = a.ymin; i <= a.ymax; i++)
			{
				for (int j = a.xmin; j <= a.xmax; j++)
				{
					this.ReloadTile(j, i);
				}
			}
		}

		public void ReloadTile(int x, int z)
		{
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount)
			{
				return;
			}
			int num = x + z * this.graph.tileXCount;
			if (this.activeTileTypes[num] != null)
			{
				this.LoadTile(this.activeTileTypes[num], x, z, this.activeTileRotations[num], this.activeTileOffsets[num]);
			}
		}

		public void CutShapeWithTile(int x, int z, Int3[] shape, ref Int3[] verts, ref int[] tris, out int vCount, out int tCount)
		{
			if (this.isBatching)
			{
				throw new Exception("Cannot cut with shape when batching. Please stop batching first.");
			}
			int num = x + z * this.graph.tileXCount;
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount || this.activeTileTypes[num] == null)
			{
				verts = new Int3[0];
				tris = new int[0];
				vCount = 0;
				tCount = 0;
				return;
			}
			Int3[] verts2;
			int[] tris2;
			this.activeTileTypes[num].Load(out verts2, out tris2, this.activeTileRotations[num], this.activeTileOffsets[num]);
			Bounds tileBounds = this.graph.GetTileBounds(x, z, 1, 1);
			Int3 @int = (Int3)tileBounds.min;
			@int = -@int;
			this.CutPoly(verts2, tris2, ref verts, ref tris, out vCount, out tCount, shape, @int, tileBounds, TileHandler.CutMode.CutExtra, 0);
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i] -= @int;
			}
		}

		protected static T[] ShrinkArray<T>(T[] arr, int newLength)
		{
			newLength = Math.Min(newLength, arr.Length);
			T[] array = new T[newLength];
			if (newLength % 4 == 0)
			{
				for (int i = 0; i < newLength; i += 4)
				{
					array[i] = arr[i];
					array[i + 1] = arr[i + 1];
					array[i + 2] = arr[i + 2];
					array[i + 3] = arr[i + 3];
				}
			}
			else if (newLength % 3 == 0)
			{
				for (int j = 0; j < newLength; j += 3)
				{
					array[j] = arr[j];
					array[j + 1] = arr[j + 1];
					array[j + 2] = arr[j + 2];
				}
			}
			else if (newLength % 2 == 0)
			{
				for (int k = 0; k < newLength; k += 2)
				{
					array[k] = arr[k];
					array[k + 1] = arr[k + 1];
				}
			}
			else
			{
				for (int l = 0; l < newLength; l++)
				{
					array[l] = arr[l];
				}
			}
			return array;
		}

		public void LoadTile(TileHandler.TileType tile, int x, int z, int rotation, int yoffset)
		{
			if (tile == null)
			{
				throw new ArgumentNullException("tile");
			}
			if (AstarPath.active == null)
			{
				return;
			}
			int index = x + z * this.graph.tileXCount;
			rotation %= 4;
			if (this.isBatching && this.reloadedInBatch[index] && this.activeTileOffsets[index] == yoffset && this.activeTileRotations[index] == rotation && this.activeTileTypes[index] == tile)
			{
				return;
			}
			this.reloadedInBatch[index] |= this.isBatching;
			this.activeTileOffsets[index] = yoffset;
			this.activeTileRotations[index] = rotation;
			this.activeTileTypes[index] = tile;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				if (this.activeTileOffsets[index] != yoffset || this.activeTileRotations[index] != rotation || this.activeTileTypes[index] != tile)
				{
					return true;
				}
				GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
				Int3[] verts;
				int[] tris;
				tile.Load(out verts, out tris, rotation, yoffset);
				Bounds tileBounds = this.graph.GetTileBounds(x, z, tile.Width, tile.Depth);
				Int3 @int = (Int3)tileBounds.min;
				@int = -@int;
				Int3[] array = null;
				int[] array2 = null;
				int num;
				int num2;
				this.CutPoly(verts, tris, ref array, ref array2, out num, out num2, null, @int, tileBounds, TileHandler.CutMode.CutAll | TileHandler.CutMode.CutDual, 0);
				this.DelaunayRefinement(array, array2, ref num, ref num2, true, false, -@int);
				if (num2 != array2.Length)
				{
					array2 = TileHandler.ShrinkArray<int>(array2, num2);
				}
				if (num != array.Length)
				{
					array = TileHandler.ShrinkArray<Int3>(array, num);
				}
				int w = (rotation % 2 != 0) ? tile.Depth : tile.Width;
				int d = (rotation % 2 != 0) ? tile.Width : tile.Depth;
				this.graph.ReplaceTile(x, z, w, d, array, array2, false);
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
				AstarPath.active.QueueWorkItemFloodFill();
				return true;
			}));
		}
	}
}
