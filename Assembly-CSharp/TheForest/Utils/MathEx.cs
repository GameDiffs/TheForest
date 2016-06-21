using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public static class MathEx
	{
		private struct Edge
		{
			public int v1;

			public int v2;

			public Edge(int v1, int v2)
			{
				this.v1 = Mathf.Min(v1, v2);
				this.v2 = Mathf.Max(v1, v2);
			}
		}

		private class EdgeTable
		{
			public Dictionary<MathEx.Edge, int> faceCounts;

			public EdgeTable()
			{
				this.faceCounts = new Dictionary<MathEx.Edge, int>();
			}

			public void AddFaceToEdge(int v1, int v2)
			{
				MathEx.Edge edge = new MathEx.Edge(v1, v2);
				if (this.faceCounts.ContainsKey(edge))
				{
					Dictionary<MathEx.Edge, int> dictionary;
					Dictionary<MathEx.Edge, int> expr_20 = dictionary = this.faceCounts;
					MathEx.Edge key;
					MathEx.Edge expr_23 = key = edge;
					int num = dictionary[key];
					expr_20[expr_23] = num + 1;
				}
				else
				{
					this.faceCounts.Add(edge, 1);
				}
			}
		}

		private struct IntPair
		{
			public int a;

			public int b;
		}

		private class VertexTable
		{
			public Dictionary<int, MathEx.IntPair> neighbours;

			public VertexTable()
			{
				this.neighbours = new Dictionary<int, MathEx.IntPair>();
			}

			public void AddEdge(MathEx.Edge edge)
			{
				this.AddVertexNeighbour(edge.v1, edge.v2);
				this.AddVertexNeighbour(edge.v2, edge.v1);
			}

			private void AddVertexNeighbour(int v1, int v2)
			{
				MathEx.IntPair value;
				if (this.neighbours.ContainsKey(v1))
				{
					value = this.neighbours[v1];
					if (value.b != -1)
					{
						Debug.LogErrorFormat("Vertex {0} has more than two neighbours", new object[]
						{
							v1
						});
					}
					value.b = v2;
				}
				else
				{
					value.a = v2;
					value.b = -1;
				}
				this.neighbours[v1] = value;
			}
		}

		private class ContourState
		{
			private enum PointStatus : byte
			{
				Above,
				Below,
				Outside
			}

			[Flags]
			private enum CellType : byte
			{
				None = 0,
				BottomLeft = 1,
				BottomRight = 2,
				TopRight = 4,
				TopLeft = 8,
				InverseBottomLeft = 14,
				InverseBottomRight = 13,
				InverseTopRight = 11,
				InverseTopLeft = 7,
				Left = 9,
				Right = 6,
				Top = 12,
				Bottom = 3,
				SaddleUpRight = 5,
				SaddleUpLeft = 10,
				All = 15
			}

			private MathEx.ContourState.PointStatus[,] points;

			private MathEx.ContourState.CellType[,] cells;

			private byte[,] visited;

			private int[,] verticesRight;

			private int[,] verticesUp;

			private List<Vector2> vertices;

			private List<Vector2> perimeter;

			private Rect bounds;

			private int pointGridSize;

			private int cellGridSize;

			private float threshold;

			private Func<Vector2, float> Sample;

			public ContourState(int cellGridSize, List<Vector2> perimeter, float threshold, Func<Vector2, float> sampleFunction)
			{
				this.Sample = sampleFunction;
				this.perimeter = perimeter;
				this.bounds = new Rect(perimeter[0], Vector2.zero);
				for (int i = 1; i < perimeter.Count; i++)
				{
					this.bounds.min = Vector2.Min(this.bounds.min, perimeter[i]);
					this.bounds.max = Vector2.Max(this.bounds.max, perimeter[i]);
				}
				Vector2 b = new Vector2(this.bounds.width / (float)cellGridSize, this.bounds.height / (float)cellGridSize);
				this.bounds.min = this.bounds.min - b;
				this.bounds.max = this.bounds.max + b;
				cellGridSize += 2;
				this.cellGridSize = cellGridSize;
				this.pointGridSize = cellGridSize + 1;
				this.threshold = threshold;
				this.points = new MathEx.ContourState.PointStatus[this.pointGridSize, this.pointGridSize];
				this.cells = new MathEx.ContourState.CellType[cellGridSize, cellGridSize];
				this.visited = new byte[cellGridSize, cellGridSize];
				this.verticesRight = new int[cellGridSize, cellGridSize];
				this.verticesUp = new int[cellGridSize, cellGridSize];
				this.vertices = new List<Vector2>();
			}

			public List<List<Vector2>> CalculateContours()
			{
				for (int i = 0; i < this.pointGridSize; i++)
				{
					for (int j = 0; j < this.pointGridSize; j++)
					{
						Vector2 vector = this.CalculateGridPoint((float)i, (float)j);
						if (MathEx.IsPointInPolygon(vector, this.perimeter, this.bounds))
						{
							float num = this.Sample(vector);
							this.points[i, j] = ((num >= this.threshold) ? MathEx.ContourState.PointStatus.Above : MathEx.ContourState.PointStatus.Below);
						}
						else
						{
							this.points[i, j] = MathEx.ContourState.PointStatus.Outside;
						}
					}
				}
				for (int k = 0; k < this.cellGridSize; k++)
				{
					for (int l = 0; l < this.cellGridSize; l++)
					{
						if (this.points[k, l] != MathEx.ContourState.PointStatus.Outside || this.points[k + 1, l] != MathEx.ContourState.PointStatus.Outside || this.points[k + 1, l + 1] != MathEx.ContourState.PointStatus.Outside || this.points[k, l + 1] != MathEx.ContourState.PointStatus.Outside)
						{
							MathEx.ContourState.CellType cellType = MathEx.ContourState.CellType.None;
							if (this.points[k, l] == MathEx.ContourState.PointStatus.Above || this.points[k, l] == MathEx.ContourState.PointStatus.Outside)
							{
								cellType |= MathEx.ContourState.CellType.BottomLeft;
							}
							if (this.points[k + 1, l] == MathEx.ContourState.PointStatus.Above || this.points[k + 1, l] == MathEx.ContourState.PointStatus.Outside)
							{
								cellType |= MathEx.ContourState.CellType.BottomRight;
							}
							if (this.points[k + 1, l + 1] == MathEx.ContourState.PointStatus.Above || this.points[k + 1, l + 1] == MathEx.ContourState.PointStatus.Outside)
							{
								cellType |= MathEx.ContourState.CellType.TopRight;
							}
							if (this.points[k, l + 1] == MathEx.ContourState.PointStatus.Above || this.points[k, l + 1] == MathEx.ContourState.PointStatus.Outside)
							{
								cellType |= MathEx.ContourState.CellType.TopLeft;
							}
							if (cellType == MathEx.ContourState.CellType.SaddleUpRight || cellType == MathEx.ContourState.CellType.SaddleUpLeft)
							{
								Vector2 arg = this.CalculateGridPoint((float)k + 0.5f, (float)l + 0.5f);
								float num2 = this.Sample(arg);
								if (num2 < this.threshold)
								{
									cellType = (MathEx.ContourState.CellType)((byte)(~(byte)cellType) & 15);
								}
							}
							this.cells[k, l] = cellType;
						}
					}
				}
				for (int m = 0; m < this.cellGridSize; m++)
				{
					for (int n = 0; n < this.cellGridSize; n++)
					{
						switch (this.cells[m, n])
						{
						case MathEx.ContourState.CellType.BottomRight:
						case MathEx.ContourState.CellType.Bottom:
						case MathEx.ContourState.CellType.Top:
						case MathEx.ContourState.CellType.InverseBottomRight:
							this.AddVertexRight(m, n);
							break;
						case MathEx.ContourState.CellType.TopRight:
						case MathEx.ContourState.CellType.SaddleUpRight:
						case MathEx.ContourState.CellType.SaddleUpLeft:
						case MathEx.ContourState.CellType.InverseTopRight:
							this.AddVertexUp(m, n);
							this.AddVertexRight(m, n);
							break;
						case MathEx.ContourState.CellType.Right:
						case MathEx.ContourState.CellType.InverseTopLeft:
						case MathEx.ContourState.CellType.TopLeft:
						case MathEx.ContourState.CellType.Left:
							this.AddVertexUp(m, n);
							break;
						}
					}
				}
				List<List<Vector2>> list = new List<List<Vector2>>();
				while (true)
				{
					List<Vector2> list2 = this.ExtractNextContour();
					if (list2 == null)
					{
						break;
					}
					list.Add(list2);
				}
				return list;
			}

			private unsafe List<Vector2> ExtractNextContour()
			{
				int num;
				int num2;
				int lastColumn;
				int lastRow;
				if (!this.FindStartCell(out num, out num2, out lastColumn, out lastRow))
				{
					return null;
				}
				List<Vector2> list = new List<Vector2>();
				int num3 = num;
				int num4 = num2;
				int num5;
				int num6;
				int index;
				while (this.CalculateNextCell(lastColumn, lastRow, num3, num4, out num5, out num6, out index))
				{
					lastColumn = num3;
					lastRow = num4;
					num3 = num5;
					num4 = num6;
					byte* expr_5B = ref this.visited[num3, num4];
					*expr_5B += 1;
					list.Add(this.vertices[index]);
					if (num3 == num && num4 == num2)
					{
						return list;
					}
				}
				return list;
			}

			private bool FindStartCell(out int startColumn, out int startRow, out int lastColumn, out int lastRow)
			{
				startColumn = -1;
				startRow = -1;
				lastColumn = -1;
				lastRow = -1;
				bool flag = false;
				int num = 0;
				while (num < this.cellGridSize && !flag)
				{
					int num2 = 0;
					while (num2 < this.cellGridSize && !flag)
					{
						bool flag2 = this.cells[num, num2] != MathEx.ContourState.CellType.None && this.cells[num, num2] != MathEx.ContourState.CellType.All;
						bool flag3;
						if (this.cells[num, num2] == MathEx.ContourState.CellType.SaddleUpRight || this.cells[num, num2] == MathEx.ContourState.CellType.SaddleUpLeft)
						{
							flag3 = (this.visited[num, num2] >= 2);
						}
						else
						{
							flag3 = (this.visited[num, num2] >= 1);
						}
						if (flag2 && !flag3)
						{
							startColumn = num;
							startRow = num2;
							flag = true;
						}
						num2++;
					}
					num++;
				}
				if (!flag)
				{
					return false;
				}
				switch (this.cells[startColumn, startRow])
				{
				case MathEx.ContourState.CellType.BottomLeft:
				case MathEx.ContourState.CellType.InverseBottomLeft:
					if (startColumn > 0)
					{
						lastColumn = startColumn - 1;
						lastRow = startRow;
					}
					else
					{
						lastColumn = startColumn;
						lastRow = startRow - 1;
					}
					break;
				case MathEx.ContourState.CellType.BottomRight:
				case MathEx.ContourState.CellType.SaddleUpRight:
				case MathEx.ContourState.CellType.InverseBottomRight:
					if (startColumn < this.cellGridSize - 1)
					{
						lastColumn = startColumn + 1;
						lastRow = startRow;
					}
					else
					{
						lastColumn = startColumn;
						lastRow = startRow - 1;
					}
					break;
				case MathEx.ContourState.CellType.Bottom:
				case MathEx.ContourState.CellType.Top:
					if (startColumn > 0)
					{
						lastColumn = startColumn - 1;
					}
					else
					{
						lastColumn = startColumn + 1;
					}
					lastRow = startRow;
					break;
				case MathEx.ContourState.CellType.TopRight:
				case MathEx.ContourState.CellType.SaddleUpLeft:
				case MathEx.ContourState.CellType.InverseTopRight:
					if (startRow < this.cellGridSize - 1)
					{
						lastColumn = startColumn;
						lastRow = startRow + 1;
					}
					else
					{
						lastColumn = startColumn + 1;
						lastRow = startRow;
					}
					break;
				case MathEx.ContourState.CellType.Right:
				case MathEx.ContourState.CellType.Left:
					if (startRow > 0)
					{
						lastRow = startRow - 1;
					}
					else
					{
						lastRow = startRow + 1;
					}
					lastColumn = startColumn;
					break;
				case MathEx.ContourState.CellType.InverseTopLeft:
				case MathEx.ContourState.CellType.TopLeft:
					if (startColumn > 0)
					{
						lastColumn = startColumn - 1;
						lastRow = startRow;
					}
					else
					{
						lastColumn = startColumn;
						lastRow = startRow + 1;
					}
					break;
				}
				return true;
			}

			private bool CalculateNextCell(int lastColumn, int lastRow, int column, int row, out int nextColumn, out int nextRow, out int vertex)
			{
				nextColumn = -1;
				nextRow = -1;
				vertex = -1;
				if (!(lastColumn == column ^ lastRow == row))
				{
					Debug.LogError("Invalid last position");
					return false;
				}
				if (lastColumn < column - 1 || column + 1 < lastColumn)
				{
					Debug.LogError("Invalid last column");
					return false;
				}
				if (lastRow < row - 1 || row + 1 < lastRow)
				{
					Debug.LogError("Invalid last row");
					return false;
				}
				switch (this.cells[column, row])
				{
				case MathEx.ContourState.CellType.None:
				case MathEx.ContourState.CellType.All:
					return false;
				case MathEx.ContourState.CellType.BottomLeft:
				case MathEx.ContourState.CellType.InverseBottomLeft:
					if (column < lastColumn || row < lastRow)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastColumn == column - 1)
					{
						this.GoDown(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoLeft(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.BottomRight:
				case MathEx.ContourState.CellType.InverseBottomRight:
					if (lastColumn < column || row < lastRow)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastColumn == column)
					{
						this.GoRight(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoDown(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.Bottom:
				case MathEx.ContourState.CellType.Top:
					if (row != lastRow)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastColumn == column - 1)
					{
						this.GoRight(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoLeft(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.TopRight:
				case MathEx.ContourState.CellType.InverseTopRight:
					if (lastColumn < column || lastRow < row)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastColumn == column)
					{
						this.GoRight(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoUp(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.SaddleUpRight:
					if (lastColumn == column)
					{
						if (lastRow < row)
						{
							this.GoRight(column, row, out nextColumn, out nextRow, out vertex);
						}
						else
						{
							this.GoLeft(column, row, out nextColumn, out nextRow, out vertex);
						}
					}
					else if (lastColumn < column)
					{
						this.GoUp(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoDown(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.Right:
				case MathEx.ContourState.CellType.Left:
					if (column != lastColumn)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastRow == row - 1)
					{
						this.GoUp(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoDown(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.InverseTopLeft:
				case MathEx.ContourState.CellType.TopLeft:
					if (column < lastColumn || lastRow < row)
					{
						Debug.LogError("Invalid last position");
						return false;
					}
					if (lastColumn == column)
					{
						this.GoLeft(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoUp(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				case MathEx.ContourState.CellType.SaddleUpLeft:
					if (lastColumn == column)
					{
						if (lastRow < row)
						{
							this.GoLeft(column, row, out nextColumn, out nextRow, out vertex);
						}
						else
						{
							this.GoRight(column, row, out nextColumn, out nextRow, out vertex);
						}
					}
					else if (lastColumn < column)
					{
						this.GoDown(column, row, out nextColumn, out nextRow, out vertex);
					}
					else
					{
						this.GoUp(column, row, out nextColumn, out nextRow, out vertex);
					}
					return true;
				default:
					Debug.LogError("Invalid cell type");
					return false;
				}
			}

			private void GoLeft(int column, int row, out int nextColumn, out int nextRow, out int vertex)
			{
				nextColumn = column - 1;
				nextRow = row;
				vertex = this.GetVertexRight(nextColumn, nextRow);
			}

			private void GoRight(int column, int row, out int nextColumn, out int nextRow, out int vertex)
			{
				nextColumn = column + 1;
				nextRow = row;
				vertex = this.GetVertexRight(column, row);
			}

			private void GoUp(int column, int row, out int nextColumn, out int nextRow, out int vertex)
			{
				nextColumn = column;
				nextRow = row + 1;
				vertex = this.GetVertexUp(column, row);
			}

			private void GoDown(int column, int row, out int nextColumn, out int nextRow, out int vertex)
			{
				nextColumn = column;
				nextRow = row - 1;
				vertex = this.GetVertexUp(nextColumn, nextRow);
			}

			private Vector2 CalculateGridPoint(float column, float row)
			{
				return new Vector2(this.bounds.min.x + this.bounds.width * column / (float)this.pointGridSize, this.bounds.min.y + this.bounds.height * row / (float)this.pointGridSize);
			}

			private int AddVertex(float x, float y)
			{
				this.vertices.Add(new Vector2(x, y));
				return this.vertices.Count - 1;
			}

			private void AddVertexRight(int column, int row)
			{
				Vector2 arg = this.CalculateGridPoint((float)(column + 1), (float)row);
				Vector2 arg2 = this.CalculateGridPoint((float)(column + 1), (float)(row + 1));
				float y;
				if (this.points[column + 1, row] == MathEx.ContourState.PointStatus.Outside)
				{
					y = arg2.y;
				}
				else if (this.points[column + 1, row + 1] == MathEx.ContourState.PointStatus.Outside)
				{
					y = arg.y;
				}
				else
				{
					float num = this.Sample(arg);
					float num2 = this.Sample(arg2);
					if (num2 == num)
					{
						y = (arg.y + arg2.y) / 2f;
					}
					else
					{
						float num3 = (arg2.y - arg.y) / (num2 - num);
						y = arg.y + (this.threshold - num) * num3;
					}
				}
				this.verticesRight[column, row] = this.AddVertex(arg.x, y);
			}

			private void AddVertexUp(int column, int row)
			{
				Vector2 arg = this.CalculateGridPoint((float)column, (float)(row + 1));
				Vector2 arg2 = this.CalculateGridPoint((float)(column + 1), (float)(row + 1));
				float x;
				if (this.points[column, row + 1] == MathEx.ContourState.PointStatus.Outside)
				{
					x = arg.x;
				}
				else if (this.points[column + 1, row + 1] == MathEx.ContourState.PointStatus.Outside)
				{
					x = arg2.x;
				}
				else
				{
					float num = this.Sample(arg);
					float num2 = this.Sample(arg2);
					if (num2 == num)
					{
						x = (arg.x + arg2.x) / 2f;
					}
					else
					{
						float num3 = (arg2.x - arg.x) / (num2 - num);
						x = arg.x + (this.threshold - num) * num3;
					}
				}
				this.verticesUp[column, row] = this.AddVertex(x, arg.y);
			}

			private int GetVertexLeft(int column, int row)
			{
				return this.verticesRight[column - 1, row];
			}

			private int GetVertexRight(int column, int row)
			{
				return this.verticesRight[column, row];
			}

			private int GetVertexUp(int column, int row)
			{
				return this.verticesUp[column, row];
			}

			private int GetVertexDown(int column, int row)
			{
				return this.verticesUp[column, row - 1];
			}
		}

		private class ErasureSpan
		{
			public int start
			{
				get;
				private set;
			}

			public int end
			{
				get;
				private set;
			}

			public float maximumError
			{
				get;
				private set;
			}

			public ErasureSpan(int start, IList<Vector2> polygon)
			{
				this.start = start;
				this.end = (start + 2) % polygon.Count;
				this.CalculateMaximumError(polygon);
			}

			public void SetStart(int start, IList<Vector2> polygon)
			{
				this.start = start;
				this.CalculateMaximumError(polygon);
			}

			public void SetEnd(int end, IList<Vector2> polygon)
			{
				this.end = end;
				this.CalculateMaximumError(polygon);
			}

			private void CalculateMaximumError(IList<Vector2> polygon)
			{
				this.maximumError = 0f;
				Vector2 normalized = (polygon[this.end] - polygon[this.start]).normalized;
				for (int num = (this.start + 1) % polygon.Count; num != this.end; num = (num + 1) % polygon.Count)
				{
					if (MathEx.IsPointLeftOfLine(polygon[this.start], polygon[this.end], polygon[num]))
					{
						this.maximumError = 3.40282347E+38f;
						break;
					}
					float distPointToLine = MathEx.GetDistPointToLine(polygon[this.start], normalized, polygon[num]);
					this.maximumError = Mathf.Max(this.maximumError, distPointToLine);
				}
			}
		}

		private class MergeSpan
		{
			public int start
			{
				get;
				private set;
			}

			public int mergeStart
			{
				get;
				private set;
			}

			public int mergeEnd
			{
				get;
				private set;
			}

			public int end
			{
				get;
				private set;
			}

			public Vector2 mergePoint
			{
				get;
				private set;
			}

			public float error
			{
				get;
				private set;
			}

			public static MathEx.MergeSpan Create(int start, int end, List<Vector2> polygon, IList<Vector2> originalPolygon, List<int> originalIndices, float maximumError)
			{
				int mergeStart = (start + 1) % polygon.Count;
				int mergeEnd = (end + polygon.Count - 1) % polygon.Count;
				Vector2 mergePoint;
				if (MathEx.MergeSpan.CalculateMergePoint(start, mergeStart, mergeEnd, end, polygon, out mergePoint))
				{
					float num = MathEx.MergeSpan.CalculateError(mergeStart, mergeEnd, mergePoint, polygon, originalPolygon, originalIndices);
					if (num <= maximumError)
					{
						return new MathEx.MergeSpan
						{
							start = start,
							mergeStart = mergeStart,
							mergeEnd = mergeEnd,
							end = end,
							mergePoint = mergePoint,
							error = num
						};
					}
				}
				return null;
			}

			public bool MergeRegionContains(int index)
			{
				if (this.mergeStart <= this.mergeEnd)
				{
					return this.mergeStart <= index && index <= this.mergeEnd;
				}
				return index <= this.mergeEnd || this.mergeStart <= index;
			}

			public void HandleRemovedPoints(int removeStart, int removeCount)
			{
				if (this.start > removeStart)
				{
					this.start -= removeCount;
				}
				if (this.mergeStart > removeStart)
				{
					this.mergeStart -= removeCount;
				}
				if (this.mergeEnd > removeStart)
				{
					this.mergeEnd -= removeCount;
				}
				if (this.end > removeStart)
				{
					this.end -= removeCount;
				}
			}

			private static bool CalculateMergePoint(int start, int mergeStart, int mergeEnd, int end, List<Vector2> polygon, out Vector2 mergePoint)
			{
				for (int num = start; num != mergeEnd; num = (num + 1) % polygon.Count)
				{
					int index = num;
					int index2 = (num + 1) % polygon.Count;
					int index3 = (num + 2) % polygon.Count;
					if (MathEx.IsPointLeftOfLine(polygon[index], polygon[index2], polygon[index3]))
					{
						mergePoint = Vector2.zero;
						return false;
					}
				}
				Vector2 normalized = (polygon[mergeStart] - polygon[start]).normalized;
				Vector2 normalized2 = (polygon[mergeEnd] - polygon[end]).normalized;
				return MathEx.LineLineIntersection(out mergePoint, polygon[start], normalized, polygon[end], normalized2);
			}

			private static float CalculateError(int mergeStart, int mergeEnd, Vector2 mergePoint, List<Vector2> polygon, IList<Vector2> originalPolygon, List<int> originalIndices)
			{
				float a = 0f;
				int num = originalIndices[mergeStart];
				int num2 = originalIndices[mergeEnd];
				List<Vector2> polyLine = new List<Vector2>
				{
					polygon[mergeStart],
					mergePoint,
					polygon[mergeEnd]
				};
				for (int num3 = num; num3 != num2; num3 = (num3 + 1) % originalPolygon.Count)
				{
					float distPointToPolyLine = MathEx.GetDistPointToPolyLine(polyLine, originalPolygon[num3]);
					a = Mathf.Max(a, distPointToPolyLine);
				}
				float distPointToPolyLine2 = MathEx.GetDistPointToPolyLine(originalPolygon, num, num2, mergePoint);
				return Mathf.Max(a, distPointToPolyLine2);
			}
		}

		public class Easing
		{
			public static float EaseInQuad(float time, float offset, float range, float duration)
			{
				time = Mathf.Clamp01(time / duration);
				return range * time * time + offset;
			}

			public static float EaseInOutQuad(float time, float offset, float range, float duration)
			{
				time = Mathf.Clamp(time / (duration / 2f), 0f, 2f);
				if (time < 1f)
				{
					return range / 2f * time * time + offset;
				}
				time -= 1f;
				return -range / 2f * (time * (time - 2f) - 1f) + offset;
			}
		}

		public static Vector3 AddVectorLength(Vector3 vector, float size)
		{
			float num = Vector3.Magnitude(vector);
			float num2 = num + size;
			float d = num2 / num;
			return vector * d;
		}

		public static Vector3 SetVectorLength(Vector3 vector, float size)
		{
			Vector3 a = Vector3.Normalize(vector);
			return a * size;
		}

		public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
		{
			return Quaternion.Inverse(A) * B;
		}

		public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
		{
			linePoint = Vector3.zero;
			lineVec = Vector3.zero;
			lineVec = Vector3.Cross(plane1Normal, plane2Normal);
			Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
			float num = Vector3.Dot(plane1Normal, vector);
			if (Mathf.Abs(num) > 0.006f)
			{
				Vector3 rhs = plane1Position - plane2Position;
				float d = Vector3.Dot(plane1Normal, rhs) / num;
				linePoint = plane2Position + d * vector;
				return true;
			}
			return false;
		}

		public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
		{
			intersection = Vector3.zero;
			float num = Vector3.Dot(planePoint - linePoint, planeNormal);
			float num2 = Vector3.Dot(lineVec, planeNormal);
			if (num2 != 0f)
			{
				float size = num / num2;
				Vector3 b = MathEx.SetVectorLength(lineVec, size);
				intersection = linePoint + b;
				return true;
			}
			return false;
		}

		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			intersection = Vector3.zero;
			Vector3 lhs = linePoint2 - linePoint1;
			Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
			Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
			float num = Vector3.Dot(lhs, rhs);
			if (num >= 1E-05f || num <= -1E-05f)
			{
				return false;
			}
			float num2 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
			if (num2 >= 0f && num2 <= 1f)
			{
				intersection = linePoint1 + lineVec1 * num2;
				return true;
			}
			return false;
		}

		public static bool LineLineIntersection(out Vector2 intersection, Vector2 pointA, Vector2 directionA, Vector2 pointB, Vector2 directionB)
		{
			intersection = Vector2.zero;
			Vector2 a = pointA + directionA * Vector2.Dot(pointB - pointA, directionA);
			Vector2 lhs = a - pointB;
			float magnitude = lhs.magnitude;
			if (magnitude == 0f)
			{
				intersection = pointB;
				return true;
			}
			float num = Vector2.Dot(lhs, directionB) / magnitude;
			if (num == 0f)
			{
				return false;
			}
			intersection = pointB + directionB * magnitude / num;
			return true;
		}

		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;
			float num = Vector3.Dot(lineVec1, lineVec1);
			float num2 = Vector3.Dot(lineVec1, lineVec2);
			float num3 = Vector3.Dot(lineVec2, lineVec2);
			float num4 = num * num3 - num2 * num2;
			if (num4 != 0f)
			{
				Vector3 rhs = linePoint1 - linePoint2;
				float num5 = Vector3.Dot(lineVec1, rhs);
				float num6 = Vector3.Dot(lineVec2, rhs);
				float d = (num2 * num6 - num5 * num3) / num4;
				float d2 = (num * num6 - num5 * num2) / num4;
				closestPointLine1 = linePoint1 + lineVec1 * d;
				closestPointLine2 = linePoint2 + lineVec2 * d2;
				return true;
			}
			return false;
		}

		public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
		{
			Vector3 lhs = point - linePoint;
			float d = Vector3.Dot(lhs, lineVec);
			return linePoint + lineVec * d;
		}

		public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 vector = MathEx.ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
			int num = MathEx.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector);
			if (num == 0)
			{
				return vector;
			}
			if (num == 1)
			{
				return linePoint1;
			}
			if (num == 2)
			{
				return linePoint2;
			}
			return Vector3.zero;
		}

		public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			float num = MathEx.SignedDistancePlanePoint(planeNormal, planePoint, point);
			num *= -1f;
			Vector3 b = MathEx.SetVectorLength(planeNormal, num);
			return point + b;
		}

		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
		{
			return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
		}

		public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			return Vector3.Dot(planeNormal, point - planePoint);
		}

		public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
		{
			Vector3 lhs = Vector3.Cross(normal, vectorA);
			return Vector3.Dot(lhs, vectorB);
		}

		public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
		{
			Vector3 lhs = Vector3.Cross(normal, referenceVector);
			float num = Vector3.Angle(referenceVector, otherVector);
			return num * Mathf.Sign(Vector3.Dot(lhs, otherVector));
		}

		public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
		{
			float f = Vector3.Dot(vector, normal);
			float num = Mathf.Acos(f);
			return 1.57079637f - num;
		}

		public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
		{
			float num = Vector3.Dot(vec1, vec2);
			if (num < -1f)
			{
				num = -1f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			return Mathf.Acos(num);
		}

		public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
		{
			planeNormal = Vector3.zero;
			planePoint = Vector3.zero;
			Vector3 vector = pointB - pointA;
			Vector3 vector2 = pointC - pointA;
			planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
			Vector3 vector3 = pointA + vector / 2f;
			Vector3 vector4 = pointA + vector2 / 2f;
			Vector3 lineVec = pointC - vector3;
			Vector3 lineVec2 = pointB - vector4;
			Vector3 vector5;
			MathEx.ClosestPointsOnTwoLines(out planePoint, out vector5, vector3, lineVec, vector4, lineVec2);
		}

		public static Vector3 GetForwardVector(Quaternion q)
		{
			return q * Vector3.forward;
		}

		public static Vector3 GetUpVector(Quaternion q)
		{
			return q * Vector3.up;
		}

		public static Vector3 GetRightVector(Quaternion q)
		{
			return q * Vector3.right;
		}

		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}

		public static Vector3 PositionFromMatrix(Matrix4x4 m)
		{
			Vector4 column = m.GetColumn(3);
			return new Vector3(column.x, column.y, column.z);
		}

		public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
		{
			Quaternion lhs = Quaternion.LookRotation(alignWithVector, alignWithNormal);
			Quaternion rotation = Quaternion.LookRotation(customForward, customUp);
			gameObjectInOut.transform.rotation = lhs * Quaternion.Inverse(rotation);
		}

		public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
		{
			MathEx.LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
			Vector3 b = gameObjectInOut.transform.TransformPoint(trianglePosition);
			Vector3 translation = alignWithPosition - b;
			gameObjectInOut.transform.Translate(translation, Space.World);
		}

		public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
		{
			gameObjectInOut.transform.position = positionVector;
			gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
		}

		public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 rhs = linePoint2 - linePoint1;
			Vector3 lhs = point - linePoint1;
			float num = Vector3.Dot(lhs, rhs);
			if (num <= 0f)
			{
				return 1;
			}
			if (lhs.magnitude <= rhs.magnitude)
			{
				return 0;
			}
			return 2;
		}

		public static float GetDistPointToLine(Vector3 origin, Vector3 direction, Vector3 point)
		{
			Vector3 vector = origin - point;
			return (vector - Vector3.Dot(vector, direction) * direction).magnitude;
		}

		public static float GetDistPointToLine(Vector2 origin, Vector2 direction, Vector2 point)
		{
			Vector2 vector = origin - point;
			return (vector - Vector2.Dot(vector, direction) * direction).magnitude;
		}

		public static float GetDistPointToLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			Vector2 vector = lineEnd - lineStart;
			Vector2 vector2 = point - lineStart;
			float num = Vector2.Dot(vector, vector2);
			if (num <= 0f)
			{
				return vector2.magnitude;
			}
			float magnitude = vector.magnitude;
			float num2 = num / magnitude;
			if (num2 < magnitude)
			{
				return (vector2 - vector / magnitude * num2).magnitude;
			}
			return (point - lineEnd).magnitude;
		}

		public static float GetDistPointToPolyLine(IList<Vector2> polyLine, int start, int end, Vector2 point)
		{
			float num = 3.40282347E+38f;
			int num3;
			for (int num2 = start; num2 != end; num2 = num3)
			{
				num3 = (num2 + 1) % polyLine.Count;
				num = Mathf.Min(num, MathEx.GetDistPointToLineSegment(polyLine[num2], polyLine[num3], point));
			}
			return num;
		}

		public static float GetDistPointToPolyLine(IList<Vector2> polyLine, Vector2 point)
		{
			return MathEx.GetDistPointToPolyLine(polyLine, 0, polyLine.Count - 1, point);
		}

		public static bool ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point, out Vector3 projectedPoint)
		{
			projectedPoint = MathEx.ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
			int num = MathEx.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);
			return num == 0;
		}

		public static Vector3 RotateX(this Vector3 v, float angle)
		{
			float num = Mathf.Sin(angle * 0.0174532924f);
			float num2 = Mathf.Cos(angle * 0.0174532924f);
			float y = v.y;
			float z = v.z;
			v.y = num2 * y - num * z;
			v.z = num2 * z + num * y;
			return v;
		}

		public static Vector3 RotateY(this Vector3 v, float angle)
		{
			float num = Mathf.Sin(angle * 0.0174532924f);
			float num2 = Mathf.Cos(angle * 0.0174532924f);
			float x = v.x;
			float z = v.z;
			v.x = num2 * x + num * z;
			v.z = num2 * z - num * x;
			return v;
		}

		public static Vector3 RotateZ(this Vector3 v, float angle)
		{
			float num = Mathf.Sin(angle * 0.0174532924f);
			float num2 = Mathf.Cos(angle * 0.0174532924f);
			float x = v.x;
			float y = v.y;
			v.x = num2 * x - num * y;
			v.y = num2 * y + num * x;
			return v;
		}

		public static bool IsPointInTriangleXY(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
		{
			float num = 0.5f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
			int num2 = (num >= 0f) ? 1 : -1;
			float num3 = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * (float)num2;
			float num4 = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * (float)num2;
			return num3 > 0f && num4 > 0f && num3 + num4 < 2f * num * (float)num2;
		}

		public static bool IsPointInTriangleXZ(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float num = 0.5f * (-p1.z * p2.x + p0.z * (-p1.x + p2.x) + p0.x * (p1.z - p2.z) + p1.x * p2.z);
			int num2 = (num >= 0f) ? 1 : -1;
			float num3 = (p0.z * p2.x - p0.x * p2.z + (p2.z - p0.z) * p.x + (p0.x - p2.x) * p.z) * (float)num2;
			float num4 = (p0.x * p1.z - p0.z * p1.x + (p0.z - p1.z) * p.x + (p1.x - p0.x) * p.z) * (float)num2;
			return num3 > 0f && num4 > 0f && num3 + num4 < 2f * num * (float)num2;
		}

		public static bool IsPointInPolygon(Vector3 point, IList<Vector2> polygon)
		{
			bool result = false;
			if (polygon.Count > 2)
			{
				int num = 0;
				int index = 1;
				int num2 = 2;
				int num3 = 3;
				int num4 = polygon.Count - 1;
				int num5 = polygon.Count - 2;
				if (Vector3.Distance(polygon[0], polygon[polygon.Count - 1]) < 0.01f)
				{
					num5--;
				}
				for (int i = 0; i < num5; i++)
				{
					if (MathEx.IsPointInTriangleXY(point, polygon[num], polygon[index], polygon[num2]))
					{
						result = true;
						break;
					}
					if (i % 2 == 0)
					{
						index = num;
						num = num4--;
					}
					else
					{
						index = num2;
						num2 = num3++;
					}
				}
			}
			return result;
		}

		private static bool LineIntersectHorizontalLine(Vector2 start, Vector2 end, float xStart, float xEnd, float y)
		{
			if (start.y == end.y)
			{
				return false;
			}
			if (xEnd < start.x && xEnd < end.x)
			{
				return false;
			}
			float y2;
			float y3;
			if (start.y < end.y)
			{
				y2 = start.y;
				y3 = end.y;
			}
			else
			{
				y2 = end.y;
				y3 = start.y;
			}
			if (y < y2 || y3 <= y)
			{
				return false;
			}
			float num = start.x + (end.x - start.x) * (y - start.y) / (end.y - start.y);
			return xStart <= num && num <= xEnd;
		}

		public static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon, Rect bounds)
		{
			if (bounds.Contains(point))
			{
				float xStart = bounds.xMin - 1f;
				int num = 0;
				for (int i = 0; i < polygon.Count; i++)
				{
					if (MathEx.LineIntersectHorizontalLine(polygon[i], polygon[(i + 1) % polygon.Count], xStart, point.x, point.y))
					{
						num++;
					}
				}
				return num % 2 == 1;
			}
			return false;
		}

		public static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
		{
			Rect bounds = new Rect(polygon[0], Vector2.zero);
			for (int i = 1; i < polygon.Count; i++)
			{
				bounds.min = Vector2.Min(bounds.min, polygon[i]);
				bounds.max = Vector2.Max(bounds.max, polygon[i]);
			}
			return MathEx.IsPointInPolygon(point, polygon, bounds);
		}

		public static bool IsPointInPolygon(Vector3 point, IList<Vector3> polygon)
		{
			bool result = false;
			if (polygon.Count > 2)
			{
				int num = 0;
				int index = 1;
				int num2 = 2;
				int num3 = 3;
				int num4 = polygon.Count - 1;
				int num5 = polygon.Count - 2;
				if (Vector3.Distance(polygon[0], polygon[polygon.Count - 1]) < 0.01f)
				{
					num5--;
					num4 = polygon.Count - 2;
				}
				for (int i = 0; i < num5; i++)
				{
					if (MathEx.IsPointInTriangleXZ(point, polygon[num], polygon[index], polygon[num2]))
					{
						result = true;
						break;
					}
					if (i % 2 == 0)
					{
						index = num;
						num = num4--;
					}
					else
					{
						index = num2;
						num2 = num3++;
					}
				}
			}
			return result;
		}

		public static List<Vector3> CalculateMeshPerimeter(Mesh mesh)
		{
			int[] triangles = mesh.triangles;
			MathEx.EdgeTable edgeTable = new MathEx.EdgeTable();
			for (int i = 0; i < triangles.Length - 2; i += 3)
			{
				edgeTable.AddFaceToEdge(triangles[i], triangles[i + 1]);
				edgeTable.AddFaceToEdge(triangles[i + 1], triangles[i + 2]);
				edgeTable.AddFaceToEdge(triangles[i + 2], triangles[i]);
			}
			MathEx.VertexTable vertexTable = new MathEx.VertexTable();
			int num = -1;
			foreach (KeyValuePair<MathEx.Edge, int> current in edgeTable.faceCounts)
			{
				if (current.Value == 1)
				{
					vertexTable.AddEdge(current.Key);
					num = current.Key.v1;
				}
			}
			List<Vector3> list = new List<Vector3>();
			int num2 = num;
			int num3 = -1;
			Vector3[] vertices = mesh.vertices;
			do
			{
				list.Add(vertices[num2]);
				MathEx.IntPair intPair = vertexTable.neighbours[num2];
				int num4;
				if (intPair.a != num3)
				{
					num4 = intPair.a;
				}
				else
				{
					num4 = intPair.b;
				}
				num3 = num2;
				num2 = num4;
				if (num2 == -1)
				{
					Debug.LogErrorFormat("Vertex {0} has only one neighbour", new object[]
					{
						num3
					});
				}
			}
			while (num2 != num);
			return list;
		}

		public static List<List<Vector2>> CalculateContours(float threshold, int cellGridSize, List<Vector2> perimeter, Func<Vector2, float> sampleFunction)
		{
			MathEx.ContourState contourState = new MathEx.ContourState(cellGridSize, perimeter, threshold, sampleFunction);
			return contourState.CalculateContours();
		}

		public static bool IsPolygonClockwise(IList<Vector2> polygon)
		{
			return MathEx.SignedArea(polygon) < 0f;
		}

		private static float SignedArea(IList<Vector2> polygon)
		{
			if (polygon.Count < 3)
			{
				return 0f;
			}
			float num = 0f;
			Vector2 vector = polygon[polygon.Count - 1];
			for (int i = 0; i < polygon.Count; i++)
			{
				num += vector.x * polygon[i].y - polygon[i].x * vector.y;
				vector = polygon[i];
			}
			return num / 2f;
		}

		public static bool IsPointLeftOfLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			Vector2 vector = lineEnd - lineStart;
			Vector2 vector2 = point - lineStart;
			float num = vector2.x * vector.y - vector.x * vector2.y;
			return num < 0f;
		}

		private static List<Vector2> EraseLeftTurns(IList<Vector2> polygon, float maximumError, out List<int> originalIndices)
		{
			List<MathEx.ErasureSpan> list = new List<MathEx.ErasureSpan>(polygon.Count);
			for (int i = 0; i < polygon.Count; i++)
			{
				list.Add(new MathEx.ErasureSpan(i, polygon));
			}
			while (true)
			{
				float num = 3.40282347E+38f;
				int num2 = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].maximumError < num)
					{
						num = list[j].maximumError;
						num2 = j;
					}
				}
				if (num > maximumError)
				{
					break;
				}
				int index = (num2 + list.Count - 1) % list.Count;
				int index2 = (num2 + 1) % list.Count;
				list[index].SetEnd(list[num2].end, polygon);
				list[index2].SetStart(list[num2].start, polygon);
				list.RemoveAt(num2);
			}
			List<Vector2> list2 = new List<Vector2>();
			originalIndices = new List<int>();
			for (int k = 0; k < list.Count; k++)
			{
				list2.Add(polygon[list[k].start]);
				originalIndices.Add(list[k].start);
			}
			return list2;
		}

		private static List<Vector2> MergeRightTurns(List<Vector2> polygon, float maximumError, IList<Vector2> originalPolygon, List<int> originalIndices)
		{
			List<MathEx.MergeSpan> list = new List<MathEx.MergeSpan>();
			for (int i = 0; i < polygon.Count; i++)
			{
				MathEx.MergeSpan mergeSpan = MathEx.MergeSpan.Create(i, (i + 3) % polygon.Count, polygon, originalPolygon, originalIndices, maximumError);
				if (mergeSpan != null)
				{
					list.Add(mergeSpan);
				}
			}
			List<MathEx.MergeSpan> list2 = new List<MathEx.MergeSpan>();
			bool[] array = new bool[polygon.Count];
			while (true)
			{
				float num = 3.40282347E+38f;
				int num2 = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].error < num)
					{
						num = list[j].error;
						num2 = j;
					}
				}
				if (num2 == -1)
				{
					break;
				}
				MathEx.MergeSpan mergeSpan2 = list[num2];
				list.RemoveAt(num2);
				while (true)
				{
					int num3 = (mergeSpan2.start + polygon.Count - 1) % polygon.Count;
					int num4 = (mergeSpan2.end + 1) % polygon.Count;
					MathEx.MergeSpan mergeSpan3 = null;
					if (!array[num3])
					{
						mergeSpan3 = MathEx.MergeSpan.Create(num3, mergeSpan2.end, polygon, originalPolygon, originalIndices, maximumError);
					}
					MathEx.MergeSpan mergeSpan4 = null;
					if (!array[num4])
					{
						mergeSpan4 = MathEx.MergeSpan.Create(mergeSpan2.start, num4, polygon, originalPolygon, originalIndices, maximumError);
					}
					if (mergeSpan3 != null && mergeSpan4 != null)
					{
						mergeSpan2 = ((mergeSpan3.error >= mergeSpan4.error) ? mergeSpan4 : mergeSpan3);
					}
					else if (mergeSpan3 != null)
					{
						mergeSpan2 = mergeSpan3;
					}
					else
					{
						if (mergeSpan4 == null)
						{
							break;
						}
						mergeSpan2 = mergeSpan4;
					}
				}
				int k = 0;
				while (k < list.Count)
				{
					if (mergeSpan2.MergeRegionContains(list[k].start) || mergeSpan2.MergeRegionContains(list[k].end))
					{
						list.RemoveAt(k);
					}
					else
					{
						k++;
					}
				}
				for (int num5 = mergeSpan2.mergeStart; num5 != mergeSpan2.end; num5 = (num5 + 1) % polygon.Count)
				{
					array[num5] = true;
				}
				list2.Add(mergeSpan2);
			}
			List<Vector2> list3 = new List<Vector2>(polygon);
			for (int l = 0; l < list2.Count; l++)
			{
				MathEx.MergeSpan mergeSpan5 = list2[l];
				list3[mergeSpan5.mergeStart] = mergeSpan5.mergePoint;
				if (mergeSpan5.mergeStart < mergeSpan5.mergeEnd)
				{
					int num6 = mergeSpan5.mergeStart + 1;
					int num7 = mergeSpan5.mergeEnd - mergeSpan5.mergeStart;
					list3.RemoveRange(num6, num7);
					for (int m = l + 1; m < list2.Count; m++)
					{
						list2[m].HandleRemovedPoints(num6, num7);
					}
				}
				else if (mergeSpan5.mergeEnd < mergeSpan5.mergeStart)
				{
					if (mergeSpan5.mergeStart < list3.Count - 1)
					{
						int num8 = mergeSpan5.mergeStart + 1;
						int num9 = list3.Count - 1 - mergeSpan5.mergeStart;
						list3.RemoveRange(num8, num9);
						for (int n = l + 1; n < list2.Count; n++)
						{
							list2[n].HandleRemovedPoints(num8, num9);
						}
					}
					int num10 = 0;
					int num11 = mergeSpan5.mergeEnd + 1;
					list3.RemoveRange(num10, num11);
					for (int num12 = l + 1; num12 < list2.Count; num12++)
					{
						list2[num12].HandleRemovedPoints(num10, num11);
					}
				}
			}
			return list3;
		}

		public static List<Vector2> SimplifyPolygon(IList<Vector2> polygon, float maximumError)
		{
			List<int> originalIndices;
			List<Vector2> polygon2 = MathEx.EraseLeftTurns(polygon, maximumError, out originalIndices);
			return MathEx.MergeRightTurns(polygon2, maximumError, polygon, originalIndices);
		}
	}
}
