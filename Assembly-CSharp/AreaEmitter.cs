using FMOD.Studio;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class AreaEmitter : MonoBehaviour, FMOD_Listener.IAreaEmitter
{
	[Serializable]
	public class Polygon
	{
		[Flags]
		private enum SegmentFlags
		{
			None = 0,
			Occluded = 1,
			Active = 2
		}

		private struct GridPosition
		{
			public int row;

			public int column;
		}

		public List<Vector2> points;

		private List<AreaEmitter.Segment> segments;

		private List<AreaEmitter.Polygon.SegmentFlags> segmentFlags;

		private List<int> activeSegments;

		private Rect bounds;

		private List<int>[,] grid;

		private Vector2 gridSize;

		private float sqrMaximumDistance;

		private int audibleSegmentCount;

		public Polygon()
		{
			this.points = new List<Vector2>
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f)
			};
			this.segments = new List<AreaEmitter.Segment>();
			this.segmentFlags = new List<AreaEmitter.Polygon.SegmentFlags>();
			this.activeSegments = new List<int>();
			this.bounds = default(Rect);
			this.grid = null;
			this.gridSize = Vector2.zero;
			this.sqrMaximumDistance = 0f;
			this.audibleSegmentCount = 0;
		}

		public Polygon(AreaEmitter.Polygon other)
		{
			this.points = new List<Vector2>(other.points);
			this.segments = new List<AreaEmitter.Segment>();
			this.segmentFlags = new List<AreaEmitter.Polygon.SegmentFlags>();
			this.activeSegments = new List<int>();
			this.bounds = default(Rect);
			this.grid = null;
			this.gridSize = Vector2.zero;
			this.sqrMaximumDistance = 0f;
			this.audibleSegmentCount = 0;
		}

		public AreaEmitter.Segment GetSegment(int index)
		{
			return this.segments[index];
		}

		public bool IsSegmentActive(int index)
		{
			return (this.segmentFlags[index] & AreaEmitter.Polygon.SegmentFlags.Active) != AreaEmitter.Polygon.SegmentFlags.None;
		}

		public bool IsSegmentOccluded(int index)
		{
			return (this.segmentFlags[index] & AreaEmitter.Polygon.SegmentFlags.Occluded) != AreaEmitter.Polygon.SegmentFlags.None;
		}

		public bool HasActiveSegments()
		{
			return this.activeSegments.Count > 0;
		}

		public void Start(float maximumDistance, Func<Vector2, Vector3> TransformPoint)
		{
			this.sqrMaximumDistance = maximumDistance * maximumDistance;
			this.gridSize = new Vector2(maximumDistance, maximumDistance);
			this.CalculateBounds();
			this.CreateGrid();
			this.CalculateSegments(TransformPoint);
		}

		private void CalculateBounds()
		{
			this.bounds.position = this.points[0];
			this.bounds.size = Vector2.zero;
			foreach (Vector2 current in this.points)
			{
				this.bounds.xMin = Mathf.Min(this.bounds.xMin, current.x);
				this.bounds.yMin = Mathf.Min(this.bounds.yMin, current.y);
				this.bounds.xMax = Mathf.Max(this.bounds.xMax, current.x);
				this.bounds.yMax = Mathf.Max(this.bounds.yMax, current.y);
			}
		}

		private void CreateGrid()
		{
			int num = Mathf.CeilToInt(this.bounds.height / this.gridSize.y);
			if (num > 50)
			{
				num = 50;
				this.gridSize.y = this.bounds.height / 50f;
			}
			int num2 = Mathf.CeilToInt(this.bounds.width / this.gridSize.x);
			if (num2 > 50)
			{
				num2 = 50;
				this.gridSize.x = this.bounds.width / 50f;
			}
			if (num >= 2 || num2 >= 2)
			{
				num = Mathf.Max(num, 1);
				num2 = Mathf.Max(num2, 1);
				this.grid = new List<int>[num, num2];
			}
			else
			{
				this.grid = null;
			}
		}

		private void CalculateSegments(Func<Vector2, Vector3> TransformPoint)
		{
			this.segments = new List<AreaEmitter.Segment>(this.points.Count);
			this.segmentFlags = new List<AreaEmitter.Polygon.SegmentFlags>(this.points.Count);
			this.ForEachSegment(delegate(Vector2 start, Vector2 end)
			{
				AreaEmitter.Segment item;
				item.start = TransformPoint(start);
				item.delta = TransformPoint(end) - item.start;
				item.length = item.delta.magnitude;
				item.sqrDistance = 0f;
				item.closestPoint = Vector3.zero;
				item.closestT = 0f;
				this.AddSegmentToGrid(start, end, this.segments.Count);
				this.segments.Add(item);
				this.segmentFlags.Add(AreaEmitter.Polygon.SegmentFlags.None);
			});
		}

		private void AddSegmentToGrid(Vector2 start, Vector2 end, int segmentIndex)
		{
			if (this.grid != null)
			{
				Vector2 vector = Vector2.Min(start, end);
				Vector2 vector2 = Vector2.Max(start, end);
				AreaEmitter.Polygon.GridPosition gridPosition = this.CalculateGridPosition(Vector2.Min(start, end));
				AreaEmitter.Polygon.GridPosition gridPosition2 = this.CalculateGridPosition(Vector2.Max(start, end));
				for (int i = gridPosition.row; i <= gridPosition2.row; i++)
				{
					for (int j = gridPosition.column; j <= gridPosition2.column; j++)
					{
						if (this.grid[i, j] == null)
						{
							this.grid[i, j] = new List<int>();
						}
						this.grid[i, j].Add(segmentIndex);
					}
				}
			}
		}

		private AreaEmitter.Polygon.GridPosition CalculateGridPosition(Vector2 point)
		{
			AreaEmitter.Polygon.GridPosition result;
			result.row = Mathf.FloorToInt((point.y - this.bounds.yMin) / this.gridSize.y);
			result.row = Mathf.Clamp(result.row, 0, this.grid.GetLength(0) - 1);
			result.column = Mathf.FloorToInt((point.x - this.bounds.xMin) / this.gridSize.x);
			result.column = Mathf.Clamp(result.column, 0, this.grid.GetLength(1) - 1);
			return result;
		}

		public bool ContainsPoint(Vector2 point)
		{
			return MathEx.IsPointInPolygon(point, this.points, this.bounds);
		}

		public void ForEachSegment(Action<Vector2, Vector2> action)
		{
			for (int i = 1; i < this.points.Count; i++)
			{
				action(this.points[i - 1], this.points[i]);
			}
			action(this.points[this.points.Count - 1], this.points[0]);
		}

		public void ForEachSegment(Action<AreaEmitter.Segment, bool> action)
		{
			for (int i = 0; i < this.segments.Count; i++)
			{
				action(this.segments[i], this.segmentFlags[i] == AreaEmitter.Polygon.SegmentFlags.Active);
			}
		}

		public int WrapSegmentIndex(int index)
		{
			return (index + this.segments.Count) % this.segments.Count;
		}

		public void UpdateSegments(Func<Vector3, Vector2> InverseTransformPoint)
		{
			for (int i = 0; i < this.activeSegments.Count; i++)
			{
				List<AreaEmitter.Polygon.SegmentFlags> list;
				List<AreaEmitter.Polygon.SegmentFlags> expr_0D = list = this.segmentFlags;
				int index;
				int expr_1B = index = this.activeSegments[i];
				AreaEmitter.Polygon.SegmentFlags segmentFlags = list[index];
				expr_0D[expr_1B] = (segmentFlags & ~AreaEmitter.Polygon.SegmentFlags.Active);
			}
			this.activeSegments.Clear();
			if (this.grid != null)
			{
				this.UpdateSegmentsWithGrid(InverseTransformPoint);
			}
			else
			{
				this.UpdateSegmentsWithoutGrid();
			}
			this.audibleSegmentCount = this.activeSegments.Count;
		}

		private void UpdateSegmentsWithGrid(Func<Vector3, Vector2> InverseTransformPoint)
		{
			Vector2 point = InverseTransformPoint(LocalPlayer.Transform.position);
			AreaEmitter.Polygon.GridPosition gridPosition = this.CalculateGridPosition(point);
			AreaEmitter.Polygon.GridPosition gridPosition2 = gridPosition;
			if (0 < gridPosition.row && point.y < this.bounds.yMax)
			{
				gridPosition.row--;
			}
			if (gridPosition2.row < this.grid.GetLength(0) - 1 && this.bounds.yMin < point.y)
			{
				gridPosition2.row++;
			}
			if (0 < gridPosition.column && point.x < this.bounds.xMax)
			{
				gridPosition.column--;
			}
			if (gridPosition2.column < this.grid.GetLength(1) - 1 && this.bounds.xMin < point.x)
			{
				gridPosition2.column++;
			}
			for (int i = gridPosition.row; i <= gridPosition2.row; i++)
			{
				for (int j = gridPosition.column; j <= gridPosition2.column; j++)
				{
					List<int> list = this.grid[i, j];
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							this.UpdateSegment(list[k]);
						}
					}
				}
			}
		}

		private void UpdateSegmentsWithoutGrid()
		{
			for (int i = 0; i < this.segments.Count; i++)
			{
				this.UpdateSegment(i);
			}
		}

		private void UpdateSegment(int index)
		{
			if (this.IsSegmentActive(index))
			{
				return;
			}
			AreaEmitter.Segment value = this.segments[index];
			Vector3 rhs = LocalPlayer.Transform.position - value.start;
			float num = Vector3.Dot(value.delta, rhs);
			if (num <= 0f)
			{
				value.closestT = 0f;
				value.closestPoint = value.start;
			}
			else
			{
				float num2 = num / value.length;
				if (num2 >= value.length)
				{
					value.closestT = 1f;
					value.closestPoint = value.start + value.delta;
				}
				else
				{
					value.closestT = num2 / value.length;
					value.closestPoint = value.start + value.delta * value.closestT;
				}
			}
			value.sqrDistance = (value.closestPoint - LocalPlayer.Transform.position).sqrMagnitude;
			List<AreaEmitter.Polygon.SegmentFlags> list;
			List<AreaEmitter.Polygon.SegmentFlags> expr_10D = list = this.segmentFlags;
			AreaEmitter.Polygon.SegmentFlags segmentFlags = list[index];
			expr_10D[index] = (segmentFlags & ~AreaEmitter.Polygon.SegmentFlags.Occluded);
			if (value.sqrDistance < this.sqrMaximumDistance)
			{
				this.activeSegments.Add(index);
				List<AreaEmitter.Polygon.SegmentFlags> list2;
				List<AreaEmitter.Polygon.SegmentFlags> expr_14D = list2 = this.segmentFlags;
				segmentFlags = list2[index];
				expr_14D[index] = (segmentFlags | AreaEmitter.Polygon.SegmentFlags.Active);
			}
			this.segments[index] = value;
		}

		public int FindClosestAudibleSegment()
		{
			int result = -1;
			float num = 3.40282347E+38f;
			for (int i = 0; i < this.audibleSegmentCount; i++)
			{
				int index = this.activeSegments[i];
				AreaEmitter.Segment segment = this.segments[index];
				if (!this.IsSegmentOccluded(index) && segment.sqrDistance < num)
				{
					num = segment.sqrDistance;
					result = this.activeSegments[i];
				}
			}
			return result;
		}

		public int DescendToClosestSegment(int segmentIndex)
		{
			int index = this.WrapSegmentIndex(segmentIndex - 1);
			int index2 = this.WrapSegmentIndex(segmentIndex + 1);
			int num = (this.segments[index].sqrDistance >= this.segments[index2].sqrDistance) ? 1 : -1;
			while (true)
			{
				int num2 = this.WrapSegmentIndex(segmentIndex + num);
				if (!this.IsSegmentActive(num2))
				{
					break;
				}
				if (this.IsSegmentOccluded(num2) && !this.IsSegmentOccluded(segmentIndex))
				{
					break;
				}
				if (this.segments[num2].sqrDistance > this.segments[segmentIndex].sqrDistance)
				{
					break;
				}
				segmentIndex = num2;
			}
			return segmentIndex;
		}

		public int AscendToAudibleSegment(int segmentIndex)
		{
			int index = this.WrapSegmentIndex(segmentIndex - 1);
			int index2 = this.WrapSegmentIndex(segmentIndex + 1);
			int num = (this.segments[index].sqrDistance <= this.segments[index2].sqrDistance) ? 1 : -1;
			while (this.IsSegmentOccluded(segmentIndex))
			{
				int num2 = this.WrapSegmentIndex(segmentIndex + num);
				if (!this.IsSegmentActive(num2))
				{
					break;
				}
				if (this.segments[num2].sqrDistance < this.segments[segmentIndex].sqrDistance)
				{
					break;
				}
				segmentIndex = num2;
			}
			return segmentIndex;
		}

		public void OccludeSegments(float sqrOccluderDistance, Vector3 rightOfLeft, Vector3 leftOfRight)
		{
			int i = 0;
			while (i < this.audibleSegmentCount)
			{
				int num = this.activeSegments[i];
				bool flag = false;
				if (this.segments[num].sqrDistance >= sqrOccluderDistance)
				{
					Vector3 lhs = this.segments[num].closestPoint - LocalPlayer.Transform.position;
					lhs.y = 0f;
					if (Vector3.Dot(lhs, rightOfLeft) >= 0f && Vector3.Dot(lhs, leftOfRight) >= 0f)
					{
						List<AreaEmitter.Polygon.SegmentFlags> list;
						List<AreaEmitter.Polygon.SegmentFlags> expr_8A = list = this.segmentFlags;
						int index;
						int expr_8E = index = num;
						AreaEmitter.Polygon.SegmentFlags segmentFlags = list[index];
						expr_8A[expr_8E] = (segmentFlags | AreaEmitter.Polygon.SegmentFlags.Occluded);
						this.audibleSegmentCount--;
						this.activeSegments[i] = this.activeSegments[this.audibleSegmentCount];
						this.activeSegments[this.audibleSegmentCount] = num;
						flag = true;
					}
				}
				if (!flag)
				{
					i++;
				}
			}
		}
	}

	public struct Segment
	{
		public Vector3 start;

		public Vector3 delta;

		public Vector3 closestPoint;

		public float closestT;

		public float length;

		public float sqrDistance;
	}

	private struct Source
	{
		public AreaEmitter.Polygon polygon;

		public int segment;

		public EventInstance eventInstance;

		public Vector3 position;
	}

	private const float SOURCE_WIDTH = 90f;

	public string eventPath;

	public int maximumSourceCount = 4;

	public float sourceSpeed = 100f;

	public MeshFilter contourArea;

	public int contourResolution = 50;

	[Range(1f, 20f)]
	public float simplifyMaximumError = 1f;

	public AreaEmitter.Polygon perimeter = new AreaEmitter.Polygon();

	public List<AreaEmitter.Polygon> voids = new List<AreaEmitter.Polygon>();

	private Bounds bounds;

	private EventDescription eventDescription;

	private float sqrEventMaximumDistance;

	private int windParameterIndex = -1;

	private List<AreaEmitter.Source> sources = new List<AreaEmitter.Source>();

	private Comparison<AreaEmitter.Source> compareSources;

	private Func<Vector3, Vector2> inverseTransformDelegate;

	private EventInstance eventInstanceFollowingPlayer;

	private static Texture2D lineTexture;

	private static Texture2D voidLineTexture;

	private static Texture2D occludedLineTexture;

	private static Texture2D occludedVoidLineTexture;

	private static Texture2D occluderBorderTexture;

	private static Texture2D occluderCentreTexture;

	private static Texture2D activeAreaTexture;

	private static Texture2D sourceTexture;

	private static Texture2D pointTexture;

	private static Texture2D occludedPointTexture;

	public Vector3 TransformPoint(Vector2 point)
	{
		return base.transform.TransformPoint(point.x, 0f, point.y);
	}

	public Vector2 InverseTransformPoint(Vector3 point)
	{
		Vector3 vector = base.transform.InverseTransformPoint(point);
		return new Vector2(vector.x, vector.z);
	}

	private void Start()
	{
		if (FMOD_StudioSystem.instance)
		{
			this.eventDescription = FMOD_StudioSystem.instance.GetEventDescription(this.eventPath);
		}
		if (this.eventDescription == null)
		{
			base.enabled = false;
			return;
		}
		float num;
		UnityUtil.ERRCHECK(this.eventDescription.getMaximumDistance(out num));
		this.sqrEventMaximumDistance = num * num;
		this.windParameterIndex = FMODCommon.FindParameterIndex(this.eventDescription, "wind");
		this.compareSources = delegate(AreaEmitter.Source a, AreaEmitter.Source b)
		{
			float sqrDistance = a.polygon.GetSegment(a.segment).sqrDistance;
			float sqrDistance2 = b.polygon.GetSegment(b.segment).sqrDistance;
			return Math.Sign(sqrDistance - sqrDistance2);
		};
		this.inverseTransformDelegate = new Func<Vector3, Vector2>(this.InverseTransformPoint);
		this.bounds = new Bounds(this.TransformPoint(this.perimeter.points[0]), Vector3.zero);
		this.perimeter.points.ForEach(delegate(Vector2 point)
		{
			this.bounds.Encapsulate(this.TransformPoint(point));
		});
		for (int i = -1; i < this.voids.Count; i++)
		{
			this.GetPolygon(i).Start(num, new Func<Vector2, Vector3>(this.TransformPoint));
		}
	}

	public AreaEmitter.Polygon GetPolygon(int index)
	{
		if (index == -1)
		{
			return this.perimeter;
		}
		return this.voids[index];
	}

	private void Update()
	{
		if (LocalPlayer.Transform == null)
		{
			return;
		}
		if (this.bounds.SqrDistance(LocalPlayer.Transform.position) > this.sqrEventMaximumDistance)
		{
			this.StopAllSources();
			return;
		}
		AreaEmitter.Polygon polygon;
		if (this.ContainsPlayer(out polygon))
		{
			if (this.sources.Count > 0)
			{
				int num = 0;
				if (this.sources.Count > 1)
				{
					float num2 = (this.sources[num].position - LocalPlayer.Transform.position).sqrMagnitude;
					for (int i = 1; i < this.sources.Count; i++)
					{
						float sqrMagnitude = (this.sources[i].position - LocalPlayer.Transform.position).sqrMagnitude;
						if (sqrMagnitude < num2)
						{
							num = i;
							num2 = sqrMagnitude;
						}
					}
					for (int j = 0; j < this.sources.Count; j++)
					{
						if (j != num)
						{
							AreaEmitter.StopSource(this.sources[j]);
						}
					}
				}
				this.eventInstanceFollowingPlayer = this.sources[num].eventInstance;
				this.sources.Clear();
			}
			Vector3 position = this.TransformPoint(this.InverseTransformPoint(LocalPlayer.Transform.position));
			if (this.eventInstanceFollowingPlayer != null)
			{
				this.UpdateEvent(this.eventInstanceFollowingPlayer, position);
			}
			else
			{
				UnityUtil.ERRCHECK(this.eventDescription.createInstance(out this.eventInstanceFollowingPlayer));
				this.UpdateEvent(this.eventInstanceFollowingPlayer, position);
				UnityUtil.ERRCHECK(this.eventInstanceFollowingPlayer.start());
			}
		}
		else
		{
			polygon.UpdateSegments(this.inverseTransformDelegate);
			this.UpdateSources(polygon);
		}
	}

	private bool ContainsPlayer(out AreaEmitter.Polygon boundaryPolygon)
	{
		Vector2 point = this.InverseTransformPoint(LocalPlayer.Transform.position);
		if (this.perimeter.ContainsPoint(point))
		{
			for (int i = 0; i < this.voids.Count; i++)
			{
				if (this.voids[i].ContainsPoint(point))
				{
					boundaryPolygon = this.voids[i];
					return false;
				}
			}
			boundaryPolygon = null;
			return true;
		}
		boundaryPolygon = this.perimeter;
		return false;
	}

	private void UpdateSource(AreaEmitter.Source source)
	{
		this.OccludeSegments(source);
		this.UpdateEvent(source.eventInstance, source.position);
	}

	private void UpdateEvent(EventInstance eventInstance, Vector3 position)
	{
		ATTRIBUTES_3D attributes;
		attributes.position = position.toFMODVector();
		attributes.forward = base.transform.forward.toFMODVector();
		attributes.up = base.transform.up.toFMODVector();
		attributes.velocity = Vector3.zero.toFMODVector();
		UnityUtil.ERRCHECK(eventInstance.set3DAttributes(attributes));
		if (this.windParameterIndex >= 0)
		{
			UnityUtil.ERRCHECK(eventInstance.setParameterValueByIndex(this.windParameterIndex, TheForestAtmosphere.Instance.WindIntensity));
		}
	}

	private static void StopSource(AreaEmitter.Source source)
	{
		UnityUtil.ERRCHECK(source.eventInstance.stop(STOP_MODE.ALLOWFADEOUT));
		UnityUtil.ERRCHECK(source.eventInstance.release());
	}

	private void UpdateSources(AreaEmitter.Polygon boundary)
	{
		this.sources.Sort(this.compareSources);
		int i = 0;
		while (i < this.sources.Count)
		{
			AreaEmitter.Source source = this.sources[i];
			if (source.polygon != boundary)
			{
				AreaEmitter.StopSource(source);
				this.sources.RemoveAt(i);
			}
			else
			{
				int num = boundary.DescendToClosestSegment(source.segment);
				if (boundary.IsSegmentOccluded(num))
				{
					num = boundary.AscendToAudibleSegment(source.segment);
				}
				if (!boundary.IsSegmentOccluded(num))
				{
					source.segment = num;
				}
				AreaEmitter.Segment segment = boundary.GetSegment(source.segment);
				if (boundary.IsSegmentOccluded(source.segment) || segment.sqrDistance > this.sqrEventMaximumDistance)
				{
					AreaEmitter.StopSource(source);
					this.sources.RemoveAt(i);
				}
				else
				{
					Vector3 vector = segment.closestPoint - source.position;
					float sqrMagnitude = vector.sqrMagnitude;
					float num2 = this.sourceSpeed * Time.deltaTime;
					if (num2 * num2 >= sqrMagnitude)
					{
						source.position = segment.closestPoint;
					}
					else
					{
						source.position += vector.normalized * num2;
					}
					this.UpdateSource(source);
					this.sources[i] = source;
					i++;
				}
			}
		}
		while (this.sources.Count < this.maximumSourceCount)
		{
			int num3 = boundary.FindClosestAudibleSegment();
			if (num3 < 0)
			{
				break;
			}
			this.CreateSource(boundary, num3);
		}
	}

	private void CreateSource(AreaEmitter.Polygon polygon, int segment)
	{
		AreaEmitter.Source source;
		source.polygon = polygon;
		source.segment = segment;
		if (this.eventInstanceFollowingPlayer != null)
		{
			source.position = this.TransformPoint(this.InverseTransformPoint(LocalPlayer.Transform.position));
			source.eventInstance = this.eventInstanceFollowingPlayer;
			this.eventInstanceFollowingPlayer = null;
		}
		else
		{
			source.position = polygon.GetSegment(segment).closestPoint;
			UnityUtil.ERRCHECK(this.eventDescription.createInstance(out source.eventInstance));
		}
		this.UpdateSource(source);
		UnityUtil.ERRCHECK(source.eventInstance.start());
		this.sources.Add(source);
	}

	private static float WrapDegrees(float angle)
	{
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle % 360f;
	}

	private void OccludeSegments(AreaEmitter.Source occluder)
	{
		if (this.voids.Count == 0 && !this.perimeter.HasActiveSegments())
		{
			return;
		}
		Vector3 closestPoint = occluder.polygon.GetSegment(occluder.segment).closestPoint;
		Vector3 v = closestPoint - LocalPlayer.Transform.position;
		float sqrMagnitude = v.sqrMagnitude;
		v.y = 0f;
		float num = -45f;
		Vector3 rightOfLeft = v.RotateY(num);
		Vector3 leftOfRight = v.RotateY(-num);
		for (int i = -1; i < this.voids.Count; i++)
		{
			this.GetPolygon(i).OccludeSegments(sqrMagnitude, rightOfLeft, leftOfRight);
		}
	}

	private void OnEnable()
	{
		FMOD_Listener.AreaEmitters.Add(this);
	}

	private void OnDisable()
	{
		this.StopAllSources();
		FMOD_Listener.AreaEmitters.Remove(this);
	}

	private void StopAllSources()
	{
		if (this.sources.Count > 0)
		{
			foreach (AreaEmitter.Source current in this.sources)
			{
				if (current.eventInstance.isValid())
				{
					UnityUtil.ERRCHECK(current.eventInstance.stop(STOP_MODE.ALLOWFADEOUT));
					UnityUtil.ERRCHECK(current.eventInstance.release());
				}
			}
			this.sources.Clear();
		}
		if (this.eventInstanceFollowingPlayer != null)
		{
			UnityUtil.ERRCHECK(this.eventInstanceFollowingPlayer.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(this.eventInstanceFollowingPlayer.release());
			this.eventInstanceFollowingPlayer = null;
		}
	}

	private static void DrawCross(Texture2D texture, Color color)
	{
		for (int i = 0; i < texture.width; i++)
		{
			texture.SetPixel(i, i, color);
			texture.SetPixel(texture.width - 1 - i, i, color);
		}
	}

	private void CreateDebugTextures()
	{
		AreaEmitter.lineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.lineTexture.SetPixel(0, 0, Color.white);
		AreaEmitter.lineTexture.Apply();
		AreaEmitter.voidLineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.voidLineTexture.SetPixel(0, 0, new Color(0.5f, 0f, 0f));
		AreaEmitter.voidLineTexture.Apply();
		AreaEmitter.occludedLineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.occludedLineTexture.SetPixel(0, 0, Color.black);
		AreaEmitter.occludedLineTexture.Apply();
		AreaEmitter.occludedVoidLineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.occludedVoidLineTexture.SetPixel(0, 0, new Color(0.125f, 0f, 0f));
		AreaEmitter.occludedVoidLineTexture.Apply();
		AreaEmitter.occluderBorderTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.occluderBorderTexture.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.25f));
		AreaEmitter.occluderBorderTexture.Apply();
		AreaEmitter.occluderCentreTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.occluderCentreTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.25f));
		AreaEmitter.occluderCentreTexture.Apply();
		AreaEmitter.activeAreaTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		AreaEmitter.activeAreaTexture.SetPixel(0, 0, Color.green);
		AreaEmitter.activeAreaTexture.Apply();
		AreaEmitter.sourceTexture = new Texture2D(10, 10, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(AreaEmitter.sourceTexture, new Color(0f, 0f, 0f, 0f));
		AreaEmitter.DrawCross(AreaEmitter.sourceTexture, new Color(1f, 1f, 1f, 0.5f));
		AreaEmitter.sourceTexture.Apply();
		AreaEmitter.pointTexture = new Texture2D(5, 5, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(AreaEmitter.pointTexture, new Color(1f, 1f, 1f, 0.5f));
		Color color = new Color(1f, 1f, 1f, 0.25f);
		AreaEmitter.pointTexture.SetPixel(0, 0, color);
		AreaEmitter.pointTexture.SetPixel(0, 4, color);
		AreaEmitter.pointTexture.SetPixel(4, 0, color);
		AreaEmitter.pointTexture.SetPixel(4, 4, color);
		AreaEmitter.pointTexture.Apply();
		AreaEmitter.occludedPointTexture = new Texture2D(5, 5, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(AreaEmitter.occludedPointTexture, new Color(0f, 0f, 0f, 0.5f));
		color = new Color(0f, 0f, 0f, 0.25f);
		AreaEmitter.occludedPointTexture.SetPixel(0, 0, color);
		AreaEmitter.occludedPointTexture.SetPixel(0, 4, color);
		AreaEmitter.occludedPointTexture.SetPixel(4, 0, color);
		AreaEmitter.occludedPointTexture.SetPixel(4, 4, color);
		AreaEmitter.occludedPointTexture.Apply();
	}

	private bool IsActive()
	{
		return this.sources.Count > 0 || this.eventInstanceFollowingPlayer != null;
	}

	public float GetMaximumDistance()
	{
		if (!this.IsActive())
		{
			return 0f;
		}
		float num = 0f;
		for (int i = -1; i < this.voids.Count; i++)
		{
			foreach (Vector2 current in this.GetPolygon(i).points)
			{
				num = Math.Max(num, LocalPlayer.Transform.InverseTransformPoint(this.TransformPoint(current)).sqrMagnitude);
			}
		}
		return Mathf.Sqrt(num);
	}

	public void DrawDebug(Vector2 centre, float radius, float maximumDistance)
	{
		if (!this.IsActive())
		{
			return;
		}
		if (AreaEmitter.lineTexture == null)
		{
			this.CreateDebugTextures();
		}
		float scale = radius / maximumDistance;
		float sqrMaximumDistance = maximumDistance * maximumDistance;
		bool flag = this.eventInstanceFollowingPlayer != null;
		if (flag)
		{
			this.DrawPolygon(this.perimeter, centre, scale, sqrMaximumDistance, AreaEmitter.activeAreaTexture, AreaEmitter.activeAreaTexture, false);
		}
		else
		{
			this.DrawPolygon(this.perimeter, centre, scale, sqrMaximumDistance, AreaEmitter.lineTexture, AreaEmitter.occludedLineTexture, true);
		}
		foreach (AreaEmitter.Polygon current in this.voids)
		{
			this.DrawPolygon(current, centre, scale, sqrMaximumDistance, AreaEmitter.voidLineTexture, AreaEmitter.occludedVoidLineTexture, !flag);
		}
		this.sources.ForEach(delegate(AreaEmitter.Source source)
		{
			Vector3 closestPoint = source.polygon.GetSegment(source.segment).closestPoint;
			Vector3 vector2 = LocalPlayer.Transform.InverseTransformPoint(closestPoint);
			this.DrawTexture(new Vector2(vector2.x, -vector2.z) * scale + centre, AreaEmitter.sourceTexture);
			Vector3 vector3 = LocalPlayer.Transform.InverseTransformPoint(source.position);
			this.DrawTexture(new Vector2(vector3.x, -vector3.z) * scale + centre, AreaEmitter.pointTexture);
			float num = 57.29578f * Mathf.Atan2(-vector2.z, vector2.x);
			this.DrawLine(centre, num, radius, AreaEmitter.occluderCentreTexture);
			this.DrawLine(centre, num - 45f, radius, AreaEmitter.occluderBorderTexture);
			this.DrawLine(centre, num + 45f, radius, AreaEmitter.occluderBorderTexture);
		});
		if (this.eventInstanceFollowingPlayer != null)
		{
			Vector3 position = this.TransformPoint(this.InverseTransformPoint(LocalPlayer.Transform.position));
			Vector3 vector = LocalPlayer.Transform.InverseTransformPoint(position);
			this.DrawTexture(new Vector2(vector.x, -vector.z) * scale + centre, AreaEmitter.sourceTexture);
		}
		this.DrawTexture(centre, AreaEmitter.pointTexture);
	}

	private void DrawPolygon(AreaEmitter.Polygon polygon, Vector2 centre, float scale, float sqrMaximumDistance, Texture2D audibleTexture, Texture2D occludedTexture, bool drawPoints)
	{
		polygon.ForEachSegment(delegate(AreaEmitter.Segment segment, bool audible)
		{
			if (segment.sqrDistance <= sqrMaximumDistance)
			{
				Vector3 vector = LocalPlayer.Transform.InverseTransformPoint(segment.start);
				Vector3 vector2 = LocalPlayer.Transform.InverseTransformPoint(segment.start + segment.delta);
				Texture2D texture = (!audible) ? occludedTexture : audibleTexture;
				Vector2 start = new Vector2(vector.x, -vector.z) * scale + centre;
				Vector2 end = new Vector2(vector2.x, -vector2.z) * scale + centre;
				this.DrawLine(start, end, texture);
				if (drawPoints)
				{
					Vector3 vector3 = LocalPlayer.Transform.InverseTransformPoint(segment.closestPoint);
					Vector2 position = new Vector2(vector3.x, -vector3.z) * scale + centre;
					this.DrawTexture(position, (!audible) ? AreaEmitter.occludedPointTexture : AreaEmitter.pointTexture);
				}
			}
		});
	}

	private void DrawLine(Vector2 start, Vector2 end, Texture2D texture)
	{
		Vector2 vector = end - start;
		this.DrawLine(start, 57.29578f * Mathf.Atan2(vector.y, vector.x), vector.magnitude, texture);
	}

	private void DrawLine(Vector2 start, float angle, float length, Texture2D texture)
	{
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(angle, start);
		GUI.DrawTexture(new Rect(start.x, start.y, length, 1f), texture);
		GUI.matrix = matrix;
	}

	private void DrawTexture(Vector2 position, Texture2D texture)
	{
		float num = (float)texture.width;
		float height = (float)texture.height;
		GUI.DrawTexture(new Rect(position.x - num / 2f, position.y - num / 2f, num, height), texture);
	}
}
