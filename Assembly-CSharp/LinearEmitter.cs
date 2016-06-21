using FMOD.Studio;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class LinearEmitter : MonoBehaviour, FMOD_Listener.ILinearEmitter
{
	[Serializable]
	public struct ParameterInfo
	{
		public string name;

		public List<float> values;

		[NonSerialized]
		public int index;
	}

	private struct Segment
	{
		public Vector3 start;

		public Vector3 delta;

		public Vector3 closestPoint;

		public float closestT;

		public float length;

		public float sqrDistance;
	}

	[Flags]
	public enum SegmentFlags : byte
	{
		None = 0,
		Occluded = 1,
		Active = 2
	}

	private struct Source
	{
		public int segment;

		public EventInstance eventInstance;

		public Vector3 position;
	}

	private struct GridPosition
	{
		public int x;

		public int y;

		public int z;
	}

	private const float SOURCE_WIDTH = 90f;

	public string eventPath;

	public int maximumSourceCount = 4;

	public float sourceSpeed = 100f;

	public bool loop;

	public List<Vector3> points = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 0f, 0f)
	};

	public List<LinearEmitter.ParameterInfo> parameters;

	private List<LinearEmitter.Segment> segments;

	private List<LinearEmitter.SegmentFlags> segmentFlags;

	private List<int> audibleSegments;

	private Bounds bounds;

	private List<int>[,,] grid;

	private Vector3 gridCellSizes;

	private EventDescription eventDescription;

	private float sqrEventMaximumDistance;

	private int windParameterIndex;

	private int rainParameterIndex;

	private List<LinearEmitter.Source> sources = new List<LinearEmitter.Source>();

	private Comparison<LinearEmitter.Source> compareSources;

	private static Texture2D lineTexture;

	private static Texture2D occludedLineTexture;

	private static Texture2D occluderBorderTexture;

	private static Texture2D occluderCentreTexture;

	private static Texture2D sourceTexture;

	private static Texture2D pointTexture;

	private static Texture2D occludedPointTexture;

	private void Start()
	{
		this.bounds = new Bounds(base.transform.TransformPoint(this.points[0]), Vector3.zero);
		this.points.ForEach(delegate(Vector3 point)
		{
			this.bounds.Encapsulate(base.transform.TransformPoint(point));
		});
		int capacity = (!this.loop) ? (this.points.Count - 1) : this.points.Count;
		this.segments = new List<LinearEmitter.Segment>(capacity);
		this.segmentFlags = new List<LinearEmitter.SegmentFlags>(capacity);
		this.audibleSegments = new List<int>();
		this.ForEachSegment(delegate(Vector3 start, Vector3 end)
		{
			LinearEmitter.Segment item;
			item.start = base.transform.TransformPoint(start);
			item.delta = base.transform.TransformPoint(end) - item.start;
			item.length = item.delta.magnitude;
			item.sqrDistance = 0f;
			item.closestPoint = Vector3.zero;
			item.closestT = 0f;
			this.segments.Add(item);
			this.segmentFlags.Add(LinearEmitter.SegmentFlags.None);
		});
		if (FMOD_StudioSystem.instance)
		{
			this.eventDescription = FMOD_StudioSystem.instance.GetEventDescription(this.eventPath);
		}
		if (this.eventDescription == null || !FMOD_StudioSystem.instance)
		{
			base.enabled = false;
		}
		else
		{
			float num;
			UnityUtil.ERRCHECK(this.eventDescription.getMaximumDistance(out num));
			this.sqrEventMaximumDistance = num * num;
			this.windParameterIndex = FMODCommon.FindParameterIndex(this.eventDescription, "wind");
			this.rainParameterIndex = FMODCommon.FindParameterIndex(this.eventDescription, "rain");
			this.compareSources = ((LinearEmitter.Source a, LinearEmitter.Source b) => Math.Sign(this.segments[a.segment].sqrDistance - this.segments[b.segment].sqrDistance));
			if (this.parameters != null)
			{
				for (int i = 0; i < this.parameters.Count; i++)
				{
					LinearEmitter.ParameterInfo value = this.parameters[i];
					value.index = FMODCommon.FindParameterIndex(this.eventDescription, this.parameters[i].name);
					this.parameters[i] = value;
				}
			}
			this.CreateGrid(num);
		}
	}

	private void CreateGrid(float eventMaximumDistance)
	{
		this.gridCellSizes = new Vector3(eventMaximumDistance, eventMaximumDistance, eventMaximumDistance);
		int[] array = new int[3];
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Mathf.CeilToInt(this.bounds.size[i] / this.gridCellSizes[i]);
			if (array[i] > 50)
			{
				array[i] = 50;
				this.gridCellSizes[i] = this.bounds.size[i] / 50f;
			}
			array[i] = Mathf.Max(array[i], 1);
			flag = (flag || array[i] > 2);
		}
		if (flag)
		{
			this.grid = new List<int>[array[0], array[1], array[2]];
			for (int j = 0; j < this.segments.Count; j++)
			{
				this.AddSegmentToGrid(j);
			}
		}
		else
		{
			this.grid = null;
		}
	}

	private void AddSegmentToGrid(int segmentIndex)
	{
		Vector3 start = this.segments[segmentIndex].start;
		Vector3 rhs = start + this.segments[segmentIndex].delta;
		LinearEmitter.GridPosition gridPosition = this.CalculateGridPosition(Vector3.Min(start, rhs));
		LinearEmitter.GridPosition gridPosition2 = this.CalculateGridPosition(Vector3.Max(start, rhs));
		for (int i = gridPosition.x; i <= gridPosition2.x; i++)
		{
			for (int j = gridPosition.y; j <= gridPosition2.y; j++)
			{
				for (int k = gridPosition.z; k <= gridPosition2.z; k++)
				{
					if (this.grid[i, j, k] == null)
					{
						this.grid[i, j, k] = new List<int>();
					}
					this.grid[i, j, k].Add(segmentIndex);
				}
			}
		}
	}

	private LinearEmitter.GridPosition CalculateGridPosition(Vector3 point)
	{
		LinearEmitter.GridPosition result;
		result.x = Mathf.FloorToInt((point.x - this.bounds.min.x) / this.gridCellSizes.x);
		result.x = Mathf.Clamp(result.x, 0, this.grid.GetLength(0) - 1);
		result.y = Mathf.FloorToInt((point.y - this.bounds.min.y) / this.gridCellSizes.y);
		result.y = Mathf.Clamp(result.y, 0, this.grid.GetLength(1) - 1);
		result.z = Mathf.FloorToInt((point.z - this.bounds.min.z) / this.gridCellSizes.z);
		result.z = Mathf.Clamp(result.z, 0, this.grid.GetLength(2) - 1);
		return result;
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
		this.UpdateSegments();
		this.UpdateSources();
	}

	private void ForEachSegment(Action<Vector3, Vector3> action)
	{
		for (int i = 1; i < this.points.Count; i++)
		{
			action(this.points[i - 1], this.points[i]);
		}
		if (this.loop)
		{
			action(this.points[this.points.Count - 1], this.points[0]);
		}
	}

	private int WrapSegmentIndex(int index)
	{
		return (index + this.segments.Count) % this.segments.Count;
	}

	private void UpdateSegment(int index)
	{
		if (this.IsSegmentActive(index))
		{
			return;
		}
		LinearEmitter.Segment value = this.segments[index];
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
		this.SetSegmentOccluded(index, false);
		if (value.sqrDistance < this.sqrEventMaximumDistance)
		{
			this.audibleSegments.Add(index);
			this.SetSegmentActive(index, true);
		}
		this.segments[index] = value;
	}

	private void UpdateSegments()
	{
		this.audibleSegments.Clear();
		if (this.grid != null)
		{
			this.UpdateSegmentsWithGrid();
		}
		else
		{
			for (int i = 0; i < this.segments.Count; i++)
			{
				this.SetSegmentActive(i, false);
				this.UpdateSegment(i);
			}
		}
	}

	private void UpdateSegmentsWithGrid()
	{
		for (int i = 0; i < this.segments.Count; i++)
		{
			this.SetSegmentActive(i, false);
		}
		Vector3 position = LocalPlayer.Transform.position;
		LinearEmitter.GridPosition gridPosition = this.CalculateGridPosition(position);
		LinearEmitter.GridPosition gridPosition2 = gridPosition;
		if (0 < gridPosition.x && position.x < this.bounds.max.x)
		{
			gridPosition.x--;
		}
		if (gridPosition2.x < this.grid.GetLength(0) - 1 && this.bounds.min.x < position.x)
		{
			gridPosition2.x++;
		}
		if (0 < gridPosition.y && position.y < this.bounds.max.y)
		{
			gridPosition.y--;
		}
		if (gridPosition2.y < this.grid.GetLength(1) - 1 && this.bounds.min.y < position.y)
		{
			gridPosition2.y++;
		}
		if (0 < gridPosition.z && position.z < this.bounds.max.z)
		{
			gridPosition.z--;
		}
		if (gridPosition2.z < this.grid.GetLength(2) - 1 && this.bounds.min.z < position.z)
		{
			gridPosition2.z++;
		}
		for (int j = gridPosition.x; j <= gridPosition2.x; j++)
		{
			for (int k = gridPosition.y; k <= gridPosition2.y; k++)
			{
				for (int l = gridPosition.z; l <= gridPosition2.z; l++)
				{
					List<int> list = this.grid[j, k, l];
					if (list != null)
					{
						for (int m = 0; m < list.Count; m++)
						{
							this.UpdateSegment(list[m]);
						}
					}
				}
			}
		}
	}

	private void UpdateSource(LinearEmitter.Source source)
	{
		this.OccludeSegments(source);
		ATTRIBUTES_3D attributes;
		attributes.position = source.position.toFMODVector();
		attributes.forward = base.transform.forward.toFMODVector();
		attributes.up = base.transform.up.toFMODVector();
		attributes.velocity = Vector3.zero.toFMODVector();
		UnityUtil.ERRCHECK(source.eventInstance.set3DAttributes(attributes));
		if (this.windParameterIndex >= 0 && TheForestAtmosphere.Instance != null)
		{
			UnityUtil.ERRCHECK(source.eventInstance.setParameterValueByIndex(this.windParameterIndex, TheForestAtmosphere.Instance.WindIntensity));
		}
		if (this.rainParameterIndex >= 0)
		{
			UnityUtil.ERRCHECK(source.eventInstance.setParameterValueByIndex(this.rainParameterIndex, (!Scene.WeatherSystem.Raining) ? 0f : 1f));
		}
		this.SetSourceParameterValues(source);
	}

	private void UpdateSources()
	{
		this.sources.Sort(this.compareSources);
		int i = 0;
		while (i < this.sources.Count)
		{
			LinearEmitter.Source source = this.sources[i];
			int num = this.DescendToClosestSegment(source.segment);
			if (this.IsSegmentOccluded(num))
			{
				num = this.AscendToAudibleSegment(source.segment);
			}
			if (!this.IsSegmentOccluded(num))
			{
				source.segment = num;
			}
			LinearEmitter.Segment segment = this.segments[source.segment];
			if (this.IsSegmentOccluded(source.segment) || segment.sqrDistance > this.sqrEventMaximumDistance)
			{
				UnityUtil.ERRCHECK(source.eventInstance.stop(STOP_MODE.ALLOWFADEOUT));
				UnityUtil.ERRCHECK(source.eventInstance.release());
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
		while (this.sources.Count < this.maximumSourceCount)
		{
			int num3 = this.FindClosestAudibleSegment();
			if (num3 < 0 || this.segments[num3].sqrDistance > this.sqrEventMaximumDistance)
			{
				break;
			}
			LinearEmitter.Source source2;
			source2.segment = num3;
			source2.position = this.segments[num3].closestPoint;
			UnityUtil.ERRCHECK(this.eventDescription.createInstance(out source2.eventInstance));
			this.UpdateSource(source2);
			UnityUtil.ERRCHECK(source2.eventInstance.start());
			this.sources.Add(source2);
		}
	}

	private void SetSourceParameterValues(LinearEmitter.Source source)
	{
		if (this.parameters != null)
		{
			float closestT = this.segments[source.segment].closestT;
			int segment = source.segment;
			int index = (source.segment + 1) % this.points.Count;
			for (int i = 0; i < this.parameters.Count; i++)
			{
				if (this.parameters[i].index >= 0)
				{
					float num = this.parameters[i].values[segment];
					float num2 = this.parameters[i].values[index];
					float value = num + (num2 - num) * closestT;
					UnityUtil.ERRCHECK(source.eventInstance.setParameterValueByIndex(this.parameters[i].index, value));
				}
			}
		}
	}

	private int FindClosestAudibleSegment()
	{
		int result = -1;
		float num = 3.40282347E+38f;
		for (int i = 0; i < this.audibleSegments.Count; i++)
		{
			int num2 = this.audibleSegments[i];
			LinearEmitter.Segment segment = this.segments[num2];
			if (!this.IsSegmentOccluded(num2) && segment.sqrDistance < num)
			{
				num = segment.sqrDistance;
				result = num2;
			}
		}
		return result;
	}

	private static float WrapDegrees(float angle)
	{
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle % 360f;
	}

	private void OccludeSegments(LinearEmitter.Source occluder)
	{
		if (this.audibleSegments.Count == 0)
		{
			return;
		}
		Vector3 closestPoint = this.segments[occluder.segment].closestPoint;
		Vector3 v = closestPoint - LocalPlayer.Transform.position;
		float sqrMagnitude = v.sqrMagnitude;
		v.y = 0f;
		float num = -45f;
		Vector3 rhs = v.RotateY(num);
		Vector3 rhs2 = v.RotateY(-num);
		int i = 0;
		while (i < this.audibleSegments.Count)
		{
			int index = this.audibleSegments[i];
			LinearEmitter.Segment segment = this.segments[index];
			bool flag = false;
			if (segment.sqrDistance >= sqrMagnitude)
			{
				Vector3 lhs = segment.closestPoint - LocalPlayer.Transform.position;
				lhs.y = 0f;
				if (Vector3.Dot(lhs, rhs) >= 0f && Vector3.Dot(lhs, rhs2) >= 0f)
				{
					this.SetSegmentOccluded(index, true);
					int index2 = this.audibleSegments.Count - 1;
					this.audibleSegments[i] = this.audibleSegments[index2];
					this.audibleSegments.RemoveAt(index2);
					flag = true;
				}
			}
			if (!flag)
			{
				i++;
			}
		}
	}

	private int DescendToClosestSegment(int segmentIndex)
	{
		int num = segmentIndex - 1;
		int num2 = segmentIndex + 1;
		int num3 = 0;
		if (this.loop)
		{
			num = this.WrapSegmentIndex(num);
			num2 = this.WrapSegmentIndex(num2);
		}
		else if (num < 0)
		{
			num3 = 1;
		}
		else if (num2 >= this.segments.Count)
		{
			num3 = -1;
		}
		if (num3 == 0)
		{
			num3 = ((this.segments[num].sqrDistance >= this.segments[num2].sqrDistance) ? 1 : -1);
		}
		while (true)
		{
			int num4 = segmentIndex + num3;
			if (this.loop)
			{
				num4 = this.WrapSegmentIndex(num4);
			}
			else if (num4 < 0 || num4 >= this.segments.Count)
			{
				break;
			}
			if (!this.IsSegmentActive(num4))
			{
				break;
			}
			if (this.IsSegmentOccluded(num4) && !this.IsSegmentOccluded(segmentIndex))
			{
				break;
			}
			if (this.segments[num4].sqrDistance > this.segments[segmentIndex].sqrDistance)
			{
				break;
			}
			segmentIndex = num4;
		}
		return segmentIndex;
	}

	private int AscendToAudibleSegment(int segmentIndex)
	{
		int num = segmentIndex - 1;
		int num2 = segmentIndex + 1;
		int num3 = 0;
		if (this.loop)
		{
			num = this.WrapSegmentIndex(num);
			num2 = this.WrapSegmentIndex(num2);
		}
		else if (num < 0)
		{
			num3 = 1;
		}
		else if (num2 >= this.segments.Count)
		{
			num3 = -1;
		}
		if (num3 == 0)
		{
			num3 = ((this.segments[num].sqrDistance <= this.segments[num2].sqrDistance) ? 1 : -1);
		}
		while (this.IsSegmentOccluded(segmentIndex))
		{
			int num4 = segmentIndex + num3;
			if (this.loop)
			{
				num4 = this.WrapSegmentIndex(num4);
			}
			else if (num4 < 0 || num4 >= this.segments.Count)
			{
				break;
			}
			LinearEmitter.Segment segment = this.segments[segmentIndex];
			LinearEmitter.Segment segment2 = this.segments[num4];
			if (!this.IsSegmentActive(num4))
			{
				break;
			}
			if (segment2.sqrDistance < segment.sqrDistance)
			{
				break;
			}
			segmentIndex = num4;
		}
		return segmentIndex;
	}

	private void OnEnable()
	{
		FMOD_Listener.LinearEmitters.Add(this);
	}

	private void OnDisable()
	{
		this.StopAllSources();
		FMOD_Listener.LinearEmitters.Remove(this);
	}

	private void StopAllSources()
	{
		if (this.sources.Count > 0)
		{
			foreach (LinearEmitter.Source current in this.sources)
			{
				if (current.eventInstance.isValid())
				{
					UnityUtil.ERRCHECK(current.eventInstance.stop(STOP_MODE.ALLOWFADEOUT));
					UnityUtil.ERRCHECK(current.eventInstance.release());
				}
			}
			this.sources.Clear();
		}
	}

	private bool IsSegmentOccluded(int index)
	{
		return (byte)(this.segmentFlags[index] & LinearEmitter.SegmentFlags.Occluded) != 0;
	}

	private bool IsSegmentActive(int index)
	{
		return (byte)(this.segmentFlags[index] & LinearEmitter.SegmentFlags.Active) != 0;
	}

	private void SetSegmentFlag(int index, LinearEmitter.SegmentFlags flagToSet, bool value)
	{
		this.segmentFlags[index] = ((!value) ? (this.segmentFlags[index] & ~flagToSet) : (this.segmentFlags[index] | flagToSet));
	}

	private void SetSegmentOccluded(int index, bool value)
	{
		this.SetSegmentFlag(index, LinearEmitter.SegmentFlags.Occluded, value);
	}

	private void SetSegmentActive(int index, bool value)
	{
		this.SetSegmentFlag(index, LinearEmitter.SegmentFlags.Active, value);
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
		LinearEmitter.lineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		LinearEmitter.lineTexture.SetPixel(0, 0, Color.white);
		LinearEmitter.lineTexture.Apply();
		LinearEmitter.occludedLineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		LinearEmitter.occludedLineTexture.SetPixel(0, 0, Color.black);
		LinearEmitter.occludedLineTexture.Apply();
		LinearEmitter.occluderBorderTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		LinearEmitter.occluderBorderTexture.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.25f));
		LinearEmitter.occluderBorderTexture.Apply();
		LinearEmitter.occluderCentreTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		LinearEmitter.occluderCentreTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.25f));
		LinearEmitter.occluderCentreTexture.Apply();
		LinearEmitter.sourceTexture = new Texture2D(10, 10, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(LinearEmitter.sourceTexture, new Color(0f, 0f, 0f, 0f));
		LinearEmitter.DrawCross(LinearEmitter.sourceTexture, new Color(1f, 1f, 1f, 0.5f));
		LinearEmitter.sourceTexture.Apply();
		LinearEmitter.pointTexture = new Texture2D(5, 5, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(LinearEmitter.pointTexture, new Color(1f, 1f, 1f, 0.5f));
		Color color = new Color(1f, 1f, 1f, 0.25f);
		LinearEmitter.pointTexture.SetPixel(0, 0, color);
		LinearEmitter.pointTexture.SetPixel(0, 4, color);
		LinearEmitter.pointTexture.SetPixel(4, 0, color);
		LinearEmitter.pointTexture.SetPixel(4, 4, color);
		LinearEmitter.pointTexture.Apply();
		LinearEmitter.occludedPointTexture = new Texture2D(5, 5, TextureFormat.ARGB32, false);
		FMOD_Listener.Fill(LinearEmitter.occludedPointTexture, new Color(0f, 0f, 0f, 0.5f));
		color = new Color(0f, 0f, 0f, 0.25f);
		LinearEmitter.occludedPointTexture.SetPixel(0, 0, color);
		LinearEmitter.occludedPointTexture.SetPixel(0, 4, color);
		LinearEmitter.occludedPointTexture.SetPixel(4, 0, color);
		LinearEmitter.occludedPointTexture.SetPixel(4, 4, color);
		LinearEmitter.occludedPointTexture.Apply();
	}

	public float GetMaximumDistance()
	{
		if (this.sources.Count == 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (Vector3 current in this.points)
		{
			num = Math.Max(num, LocalPlayer.Transform.InverseTransformPoint(base.transform.TransformPoint(current)).sqrMagnitude);
		}
		return Mathf.Sqrt(num);
	}

	public void DrawDebug(Vector2 centre, float radius, float maximumDistance)
	{
		if (this.sources.Count == 0)
		{
			return;
		}
		if (LinearEmitter.lineTexture == null)
		{
			this.CreateDebugTextures();
		}
		float scale = radius / maximumDistance;
		float num = maximumDistance * maximumDistance;
		for (int i = 0; i < this.segments.Count; i++)
		{
			LinearEmitter.Segment segment = this.segments[i];
			if (segment.sqrDistance <= num)
			{
				Vector3 vector = LocalPlayer.Transform.InverseTransformPoint(segment.start);
				Vector3 vector2 = LocalPlayer.Transform.InverseTransformPoint(segment.start + segment.delta);
				bool flag = this.IsSegmentActive(i) && !this.IsSegmentOccluded(i);
				Texture2D texture = (!flag) ? LinearEmitter.occludedLineTexture : LinearEmitter.lineTexture;
				Vector2 start = new Vector2(vector.x, -vector.z) * scale + centre;
				Vector2 end = new Vector2(vector2.x, -vector2.z) * scale + centre;
				this.DrawLine(start, end, texture);
				if (this.IsSegmentActive(i))
				{
					Vector3 vector3 = LocalPlayer.Transform.InverseTransformPoint(segment.closestPoint);
					Vector2 position = new Vector2(vector3.x, -vector3.z) * scale + centre;
					this.DrawTexture(position, (!this.IsSegmentOccluded(i)) ? LinearEmitter.pointTexture : LinearEmitter.occludedPointTexture);
				}
			}
		}
		this.sources.ForEach(delegate(LinearEmitter.Source source)
		{
			Vector3 vector4 = LocalPlayer.Transform.InverseTransformPoint(this.segments[source.segment].closestPoint);
			this.DrawTexture(new Vector2(vector4.x, -vector4.z) * scale + centre, LinearEmitter.sourceTexture);
			Vector3 vector5 = LocalPlayer.Transform.InverseTransformPoint(source.position);
			this.DrawTexture(new Vector2(vector5.x, -vector5.z) * scale + centre, LinearEmitter.pointTexture);
			float num2 = 57.29578f * Mathf.Atan2(-vector4.z, vector4.x);
			this.DrawLine(centre, num2, radius, LinearEmitter.occluderCentreTexture);
			this.DrawLine(centre, num2 - 45f, radius, LinearEmitter.occluderBorderTexture);
			this.DrawLine(centre, num2 + 45f, radius, LinearEmitter.occluderBorderTexture);
		});
		this.DrawTexture(centre, LinearEmitter.pointTexture);
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
