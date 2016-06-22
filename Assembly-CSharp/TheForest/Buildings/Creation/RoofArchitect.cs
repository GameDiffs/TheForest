using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Interfaces;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Roof Architect")]
	public class RoofArchitect : MonoBehaviour, IEntityReplicationFilter, IHoleStructure, IStructureSupport, ICoopStructure
	{
		public enum LockModes
		{
			Shape,
			Height
		}

		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _closureSnappingDistance = 2.5f;

		public float _minLogScale = 0.3f;

		public float _maxLogScale = 2f;

		public float _maxScaleLogCost = 1f;

		public int _maxPoints = 50;

		public float _minAngleBetweenEdges = 90f;

		public float _minHeight = 2f;

		public LayerMask _supportLayers;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _logItemId;

		private bool _initialized;

		private bool _autofillmode = true;

		[SerializeThis]
		private List<Vector3> _multiPointsPositions;

		[SerializeThis]
		private int _multiPointsPositionsCount;

		[SerializeThis]
		private List<Hole> _holes;

		[SerializeThis]
		private int _holesCount;

		[SerializeThis]
		private float _roofHeight;

		private Transform _roofRoot;

		private float _logLength;

		private float _logWidth;

		private float _maxSegmentHorizontalLength;

		private float _minEdgeLength;

		private int _rowCount;

		private List<Vector3>[] _rowPointsBuffer;

		private List<Vector3>[] _holesRowPointsBuffer;

		private Stack<Transform> _logPool;

		private Stack<Transform> _newPool;

		private Material _logMat;

		private RoofArchitect.LockModes _lockMode;

		[SerializeThis]
		public bool Enslaved
		{
			get;
			set;
		}

		public IProtocolToken CustomToken
		{
			get
			{
				CoopRoofToken coopRoofToken = new CoopRoofToken();
				if (this._holes != null)
				{
					coopRoofToken.Holes = this._holes.ToArray();
				}
				coopRoofToken.Height = this._roofHeight;
				return coopRoofToken;
			}
			set
			{
				if ((value as CoopRoofToken).Holes != null)
				{
					this._holes = (value as CoopRoofToken).Holes.ToList<Hole>();
				}
				this._roofHeight = (value as CoopRoofToken).Height;
			}
		}

		public bool WasBuilt
		{
			get
			{
				return this._wasBuilt;
			}
			set
			{
				this._wasBuilt = value;
			}
		}

		public bool WasPlaced
		{
			get
			{
				return this._wasPlaced;
			}
			set
			{
				this._wasPlaced = value;
			}
		}

		public int MultiPointsCount
		{
			get
			{
				return this._multiPointsPositionsCount;
			}
			set
			{
				this._multiPointsPositionsCount = value;
			}
		}

		public List<Vector3> MultiPointsPositions
		{
			get
			{
				return this._multiPointsPositions;
			}
			set
			{
				this._multiPointsPositions = value;
			}
		}

		public float SupportHeight
		{
			get;
			set;
		}

		public IStructureSupport CurrentSupport
		{
			get;
			set;
		}

		public float LogWidth
		{
			get
			{
				return this._logWidth;
			}
		}

		bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
		{
			return this.CurrentSupport == null || connection.ExistsOnRemote((this.CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>()) == ExistsResult.Yes;
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			RoofArchitect.<DelayedAwake>c__Iterator13F <DelayedAwake>c__Iterator13F = new RoofArchitect.<DelayedAwake>c__Iterator13F();
			<DelayedAwake>c__Iterator13F.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator13F.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator13F.<>f__this = this;
			return <DelayedAwake>c__Iterator13F;
		}

		private void Update()
		{
			bool flag = this._multiPointsPositions.Count >= 3 && this._roofHeight > this._minHeight;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
			}
			if (this._lockMode == RoofArchitect.LockModes.Shape && TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f))
			{
				this._autofillmode = !this._autofillmode;
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
				this.UpdateAutoFill(true);
			}
			if (TheForest.Utils.Input.GetButtonDown("AltFire") && this._multiPointsPositions.Count > 0)
			{
				if (this._multiPointsPositions.Count == 0)
				{
					if (this._roofRoot)
					{
						UnityEngine.Object.Destroy(this._roofRoot.gameObject);
						this._roofRoot = null;
					}
					this._newPool.Clear();
					this._logPool.Clear();
				}
				this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
				this._lockMode = RoofArchitect.LockModes.Shape;
				this._roofHeight = 0f;
			}
			if (!this._autofillmode)
			{
				if (this._lockMode == RoofArchitect.LockModes.Shape)
				{
					this.UpdateShape();
				}
				else
				{
					this.UpdateHeight();
				}
			}
			else if (this._lockMode == RoofArchitect.LockModes.Shape)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(true);
				if (this._multiPointsPositions.Count >= 3 && TheForest.Utils.Input.GetButtonDown("Take"))
				{
					this._lockMode = RoofArchitect.LockModes.Height;
				}
			}
			else
			{
				this.UpdateHeight();
			}
			if (this._multiPointsPositions.Count > 0)
			{
				bool flag2 = false;
				bool flag3 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions[0], this._multiPointsPositions[this._multiPointsPositions.Count - 1]) < this._closureSnappingDistance;
				if (!flag3)
				{
					if (Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) > this._closureSnappingDistance)
					{
						flag2 = true;
						this._multiPointsPositions.Add(this.GetCurrentEdgePoint());
					}
					if (this._multiPointsPositions.Count > 2 || !flag2)
					{
						this._multiPointsPositions.Add(this._multiPointsPositions[0]);
					}
				}
				this.RefreshCurrentRoof();
				if (!flag3)
				{
					if (this._multiPointsPositions.Count > 2 || !flag2)
					{
						this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					}
					if (flag2)
					{
						this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					}
				}
			}
			Scene.HudGui.UnlockPositionIcon.SetActive(this._multiPointsPositions.Count > 0 && (!this._autofillmode || this._lockMode == RoofArchitect.LockModes.Height));
			Scene.HudGui.ToggleAutoFillIcon.SetActive(!this._autofillmode && this._lockMode == RoofArchitect.LockModes.Shape);
			Scene.HudGui.ToggleManualPlacementIcon.SetActive(this._autofillmode && this._lockMode == RoofArchitect.LockModes.Shape);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("structure") || other.CompareTag("jumpObject"))
			{
				IStructureSupport structureSupport = (IStructureSupport)other.GetComponentInParent(typeof(IStructureSupport));
				if (structureSupport != null && (this.CurrentSupport == null || this.CurrentSupport.GetLevel() < structureSupport.GetLevel()))
				{
					this.CurrentSupport = structureSupport;
					this.SupportHeight = this.CurrentSupport.GetLevel();
					this.UpdateAutoFill(false);
					return;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("structure") || other.CompareTag("jumpObject"))
			{
				IStructureSupport structureSupport = (IStructureSupport)other.GetComponentInParent(typeof(IStructureSupport));
				if (structureSupport == this.CurrentSupport)
				{
					this.CurrentSupport = null;
					this.UpdateAutoFill(false);
				}
			}
		}

		private void OnDestroy()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.LockPositionIcon.SetActive(false);
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
				Scene.HudGui.AutoFillPointsIcon.SetActive(false);
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
			}
			if (!this._wasPlaced && LocalPlayer.Create)
			{
				LocalPlayer.Create.Grabber.ClosePlace();
			}
			this.Clear();
		}

		private void OnSerializing()
		{
			this._multiPointsPositions.Capacity = this._multiPointsPositions.Count;
			this._multiPointsPositionsCount = this._multiPointsPositions.Count;
			this._holes.Capacity = this._holes.Count;
			this._holesCount = this._holes.Count;
		}

		private void OnDeserialized()
		{
			if (!this._initialized)
			{
				this._initialized = true;
				this._multiPointsPositions.RemoveRange(this._multiPointsPositionsCount, this._multiPointsPositions.Count - this._multiPointsPositionsCount);
				this._holes.RemoveRange(this._holesCount, this._holes.Count - this._holesCount);
				if (this._wasBuilt)
				{
					this.CreateStructure(false);
					this._rowPointsBuffer = null;
					this._holesRowPointsBuffer = null;
					this._roofRoot.transform.parent = base.transform;
				}
				else if (this._wasPlaced)
				{
					this.CreateStructure(false);
					base.StartCoroutine(this.OnPlaced());
				}
				this._logPool = null;
				this._newPool = null;
				this._rowPointsBuffer = null;
				this._holesRowPointsBuffer = null;
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			RoofArchitect.<OnPlaced>c__Iterator140 <OnPlaced>c__Iterator = new RoofArchitect.<OnPlaced>c__Iterator140();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		public void OnBuilt(GameObject built)
		{
			RoofArchitect component = built.GetComponent<RoofArchitect>();
			component._multiPointsPositions = this._multiPointsPositions;
			component._holes = this._holes;
			component._roofHeight = this._roofHeight;
			component._wasBuilt = true;
			component.OnSerializing();
			if (this.CurrentSupport != null)
			{
				this.CurrentSupport.Enslaved = true;
			}
		}

		public void Clear()
		{
			this._rowPointsBuffer = null;
			this._holesRowPointsBuffer = null;
			if (this._roofRoot)
			{
				UnityEngine.Object.Destroy(this._roofRoot.gameObject);
			}
		}

		public void CreateStructure(bool isRepair = false)
		{
			if (this._wasBuilt && isRepair)
			{
				this.Clear();
				base.StartCoroutine(this.DelayedAwake(true));
			}
			this.RefreshCurrentRoof();
			if (this._wasBuilt)
			{
				this._roofRoot.parent = base.transform;
			}
		}

		public Hole AddSquareHole(Vector3 position, float yRotation, Vector2 size)
		{
			Hole hole = new Hole
			{
				_position = position,
				_yRotation = yRotation,
				_size = size,
				_used = true
			};
			this._holes.Add(hole);
			return hole;
		}

		public void RemoveHole(Hole hole)
		{
			this._holes.Remove(hole);
		}

		private void UpdateAutoFill(bool doCleanUp)
		{
			bool flag = this._autofillmode && this.CurrentSupport != null && this.CurrentSupport.GetMultiPointsPositions() != null && this._lockMode == RoofArchitect.LockModes.Shape;
			if (flag)
			{
				this._multiPointsPositions = new List<Vector3>(this.CurrentSupport.GetMultiPointsPositions());
				int count = this._multiPointsPositions.Count;
				float height = this.CurrentSupport.GetHeight();
				float level = this.CurrentSupport.GetLevel();
				for (int i = this._multiPointsPositions.Count - 1; i >= 0; i--)
				{
					Vector3 value = this._multiPointsPositions[i];
					if (Mathf.Abs(value.y + height - level) < this._closureSnappingDistance)
					{
						value.y = level;
						this._multiPointsPositions[i] = value;
					}
					else
					{
						this._multiPointsPositions.RemoveAt(i);
					}
				}
				if (this._multiPointsPositions.Count >= ((count != 3) ? 4 : 3))
				{
					if (this._multiPointsPositions.Last<Vector3>() != this._multiPointsPositions.First<Vector3>())
					{
						this._multiPointsPositions.Add(this._multiPointsPositions.First<Vector3>());
					}
					this.RefreshCurrentRoof();
					base.GetComponent<Renderer>().enabled = false;
				}
				else
				{
					this._multiPointsPositions.Clear();
					base.GetComponent<Renderer>().enabled = true;
				}
			}
			else if ((this._autofillmode && this._lockMode == RoofArchitect.LockModes.Shape) || doCleanUp)
			{
				this._multiPointsPositions.Clear();
				this.RefreshCurrentRoof();
				base.GetComponent<Renderer>().enabled = true;
			}
		}

		private void UpdateShape()
		{
			bool flag = this.CurrentSupport != null && this.CurrentSupport.GetMultiPointsPositions() != null && Mathf.Abs(base.transform.position.y - this.SupportHeight) < this._closureSnappingDistance && (this._multiPointsPositions.Count == 0 || Mathf.Abs(this._multiPointsPositions[0].y - this.SupportHeight) < this._closureSnappingDistance) && this._multiPointsPositions.Count < this._maxPoints;
			if (flag && this._multiPointsPositions.Count > 0)
			{
				Vector3 to = base.transform.position - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
				flag = (to.sqrMagnitude > this._minEdgeLength * this._minEdgeLength);
				if (this._multiPointsPositions.Count > 1)
				{
					Vector3 vector = this._multiPointsPositions[this._multiPointsPositions.Count - 2] - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
					flag = (flag && Vector3.Angle(vector, to) >= this._minAngleBetweenEdges);
					Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(true);
					Scene.HudGui.AngleAndDistanceGizmo.position = this._multiPointsPositions.Last<Vector3>();
					Scene.HudGui.AngleAndDistanceGizmo.rotation = Quaternion.LookRotation(vector);
				}
				else
				{
					Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				}
			}
			else
			{
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
			}
			if (flag)
			{
				Vector3 vector2 = this.GetCurrentEdgePoint();
				bool flag2 = this._multiPointsPositions.Count > 2 && Vector3.Distance(vector2, this._multiPointsPositions[0]) < this._closureSnappingDistance;
				bool flag3 = !flag2;
				if (flag2)
				{
					vector2 = this._multiPointsPositions[0];
				}
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(flag3);
				if (flag3)
				{
					float y = vector2.y;
					vector2.y = this.CurrentSupport.GetMultiPointsPositions()[0].y;
					Vector3 a = this.CurrentSupport.GetMultiPointsPositions().ClosestPointToMultipoint(vector2);
					a.y = y;
					vector2.y = y;
					Scene.HudGui.SnappingGridGizmo.position = vector2;
					Scene.HudGui.SnappingGridGizmo.rotation = Quaternion.LookRotation(a - vector2);
				}
				if (TheForest.Utils.Input.GetButtonDown("Fire1"))
				{
					this._multiPointsPositions.Add(vector2);
					if (flag2)
					{
						this._lockMode = RoofArchitect.LockModes.Height;
						Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
			}
			bool flag4 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) > this._minEdgeLength;
			if (flag4 && TheForest.Utils.Input.GetButtonDown("Rotate"))
			{
				this._multiPointsPositions.Add(this._multiPointsPositions.First<Vector3>());
				this._lockMode = RoofArchitect.LockModes.Height;
			}
			Scene.HudGui.LockPositionIcon.SetActive(flag);
		}

		private void UpdateHeight()
		{
			this._roofHeight = base.transform.position.y - this._multiPointsPositions[0].y;
			bool flag = this._roofHeight > this._minHeight;
			if (TheForest.Utils.Input.GetButtonDown("Fire1") && flag)
			{
				LocalPlayer.Create.PlaceGhost(false);
			}
			Scene.HudGui.LockPositionIcon.SetActive(flag);
		}

		private Vector3 GetCurrentEdgePoint()
		{
			Vector3 position = base.transform.position;
			if (this._multiPointsPositions.Count > 0)
			{
				position.y = this._multiPointsPositions[0].y;
			}
			else
			{
				position.y = this.SupportHeight;
			}
			return position;
		}

		private void InitRowPointsBuffer(int rowCount, List<Vector3>[] buffer, out List<Vector3>[] outBuffer)
		{
			if (buffer != null && buffer.Length >= rowCount)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					if (i > rowCount && buffer[i].Count == 0)
					{
						break;
					}
					buffer[i].Clear();
				}
			}
			else
			{
				if (buffer != null)
				{
					rowCount = Mathf.Max(new int[]
					{
						15,
						rowCount,
						buffer.Length * 2
					});
				}
				buffer = new List<Vector3>[rowCount];
				for (int j = 0; j < rowCount; j++)
				{
					buffer[j] = new List<Vector3>(6);
				}
			}
			outBuffer = buffer;
		}

		private int GetLowerRowIndex(float zPosition, float minZ)
		{
			return Mathf.FloorToInt(Mathf.Abs(zPosition - minZ) / this._logWidth);
		}

		private int GetUpperRowIndex(float zPosition, float minZ)
		{
			return Mathf.CeilToInt(Mathf.Abs(zPosition - minZ) / this._logWidth);
		}

		private void RefreshCurrentRoof()
		{
			if (this._logPool == null)
			{
				this._logPool = new Stack<Transform>();
			}
			this._newPool = new Stack<Transform>();
			Transform roofRoot = this._roofRoot;
			if (this._multiPointsPositions.Count >= 3)
			{
				this._roofRoot = new GameObject("RoofRoot").transform;
				this.SpawnRoof();
			}
			if (roofRoot)
			{
				UnityEngine.Object.Destroy(roofRoot.gameObject);
			}
			this._logPool = this._newPool;
		}

		private void CalcRowPointBufferForArray(Vector3[] localPoints, float minLocalZ, List<Vector3>[] rowPointsBuffer)
		{
			for (int i = 1; i < localPoints.Length; i++)
			{
				Vector3 vector = localPoints[i] - localPoints[i - 1];
				Vector3 item = localPoints[i - 1];
				int num = (vector.z <= 0f) ? this.GetLowerRowIndex(item.z, minLocalZ) : this.GetUpperRowIndex(item.z, minLocalZ);
				int num2 = 0;
				float num3;
				float num4;
				float num5;
				if (vector.z > 0f)
				{
					num3 = Vector3.Angle(Vector3.forward, vector) * 0.0174532924f;
					if (Vector3.Cross(Vector3.forward, vector).y < 0f)
					{
						num3 *= -1f;
					}
					num4 = this._logWidth;
					num5 = (float)num * num4 + minLocalZ - item.z;
				}
				else
				{
					num3 = Vector3.Angle(Vector3.back, vector) * 0.0174532924f;
					if (Vector3.Cross(Vector3.back, vector).y < 0f)
					{
						num3 *= -1f;
					}
					num4 = -this._logWidth;
					num5 = -((float)num * num4 - minLocalZ + item.z);
				}
				float num6 = Mathf.Tan(num3);
				float num7 = num6 * num4;
				float num8 = num6 * num5;
				num2 += Mathf.CeilToInt((Mathf.Abs(vector.z) - Mathf.Abs(num5)) / this._logWidth);
				item.z += num5;
				item.x += num8;
				for (int j = 0; j < num2; j++)
				{
					if (num >= 0 && num < rowPointsBuffer.Length)
					{
						rowPointsBuffer[num].Add(item);
					}
					num += ((vector.z <= 0f) ? -1 : 1);
					item.z += num4;
					item.x += num7;
				}
			}
		}

		private void SpawnRoof()
		{
			float num = 3.40282347E+38f;
			float num2 = -3.40282347E+38f;
			Vector3 forward = this._multiPointsPositions[1] - this._multiPointsPositions[0];
			base.transform.rotation = Quaternion.LookRotation(forward);
			Vector3[] array = (from p in this._multiPointsPositions
			select base.transform.InverseTransformPoint(p)).ToArray<Vector3>();
			float y = array[0].y;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].y = 0f;
				if (array[i].z < num)
				{
					num = array[i].z;
				}
				if (array[i].z > num2)
				{
					num2 = array[i].z;
				}
			}
			float num3 = Mathf.Abs(num2 - num);
			this._rowCount = Mathf.CeilToInt(num3 / this._logWidth);
			this.InitRowPointsBuffer(this._rowCount, this._rowPointsBuffer, out this._rowPointsBuffer);
			this.CalcRowPointBufferForArray(array, num, this._rowPointsBuffer);
			if (this._holes != null)
			{
				for (int j = 0; j < this._holes.Count; j++)
				{
					Vector3[] array2 = new Vector3[5];
					Hole hole = this._holes[j];
					hole._used = false;
					array2[0] = base.transform.InverseTransformPoint(hole._position + new Vector3(hole._size.x, 0f, hole._size.y).RotateY(hole._yRotation));
					array2[1] = base.transform.InverseTransformPoint(hole._position + new Vector3(hole._size.x, 0f, -hole._size.y).RotateY(hole._yRotation));
					array2[2] = base.transform.InverseTransformPoint(hole._position + new Vector3(-hole._size.x, 0f, -hole._size.y).RotateY(hole._yRotation));
					array2[3] = base.transform.InverseTransformPoint(hole._position + new Vector3(-hole._size.x, 0f, hole._size.y).RotateY(hole._yRotation));
					array2[4] = array2[0];
					for (int k = 0; k < 5; k++)
					{
						array2[k].y = array[0].y;
					}
					this.InitRowPointsBuffer(this._rowCount, this._holesRowPointsBuffer, out this._holesRowPointsBuffer);
					this.CalcRowPointBufferForArray(array2, num, this._holesRowPointsBuffer);
					for (int l = 0; l < this._rowCount; l++)
					{
						this._rowPointsBuffer[l].Sort((Vector3 x1, Vector3 x2) => x1.x.CompareTo(x2.x));
						this._holesRowPointsBuffer[l].Sort((Vector3 x1, Vector3 x2) => x1.x.CompareTo(x2.x));
						List<Vector3> list = this._rowPointsBuffer[l];
						List<Vector3> list2 = this._holesRowPointsBuffer[l];
						int count = list.Count;
						int count2 = list2.Count;
						for (int m = 1; m < count; m += 2)
						{
							int num4 = 0;
							while (m < count && list[m].y < 0f)
							{
								num4 += 2;
								m += 2;
							}
							int n = 1;
							while (n < count2)
							{
								if (list[m - 1 - num4].x <= list2[n - 1].x || list[m].x >= list2[n].x)
								{
									goto IL_49C;
								}
								Vector3 value = list[m];
								value.y = 1f;
								list[m] = value;
								value = list[m - 1 - num4];
								value.y = 1f;
								list[m - 1 - num4] = value;
								hole._used = true;
								if (n + 2 >= count2)
								{
									goto IL_49C;
								}
								IL_836:
								n += 2;
								continue;
								IL_49C:
								if (list[m - 1 - num4].x < list2[n - 1].x && list[m].x > list2[n].x)
								{
									Vector3 vector = list2[n - 1];
									vector.y = -Mathf.Abs(list2[n - 1].x - list[m].x) / Mathf.Abs(list[m].x - list[m - 1 - num4].x);
									Vector3 vector2 = list2[n];
									vector2.y = -Mathf.Abs(list2[n].x - list[m - 1 - num4].x) / Mathf.Abs(list[m].x - list[m - 1 - num4].x);
									if (num4 == 0)
									{
										list.Insert(m, vector);
										list.Insert(m + 1, vector2);
									}
									else
									{
										if (list[m - num4].y > vector.y)
										{
											list[m - num4] = vector;
										}
										if (list[m + 1 - num4].y > vector2.y)
										{
											list[m + 1 - num4] = vector2;
										}
									}
									hole._used = true;
									goto IL_836;
								}
								if (list[m - 1 - num4].x > list2[n - 1].x && list[m - 1 - num4].x < list2[n].x)
								{
									Vector3 value2 = list[m - 1 - num4];
									value2.y = Mathf.Abs(list2[n].x - list[m - 1 - num4].x) / Mathf.Abs(list[m].x - list[m - 1 - num4].x);
									list[m - 1 - num4] = value2;
									hole._used = true;
									goto IL_836;
								}
								if (list[m].x > list2[n - 1].x && list[m].x < list2[n].x)
								{
									Vector3 value3 = list[m];
									value3.y = Mathf.Abs(list2[n - 1].x - list[m].x) / Mathf.Abs(list[m].x - list[m - 1 - num4].x);
									list[m] = value3;
									hole._used = true;
									goto IL_836;
								}
								goto IL_836;
							}
						}
					}
				}
			}
			Transform roofRoot = this._roofRoot;
			float num5 = 0f;
			float logStackScaleRatio = (this._maxScaleLogCost <= 0f) ? 0f : (this._maxLogScale / this._maxScaleLogCost);
			float num6 = -3.40282347E+38f;
			float num7 = 3.40282347E+38f;
			for (int num8 = 0; num8 < this._rowCount; num8++)
			{
				this._rowPointsBuffer[num8].Sort((Vector3 x1, Vector3 x2) => x1.x.CompareTo(x2.x));
				int count3 = this._rowPointsBuffer[num8].Count;
				for (int num9 = 1; num9 < count3; num9 += 2)
				{
					while (num9 < count3 && this._rowPointsBuffer[num8][num9].y < 0f)
					{
						num9 += 2;
					}
					if (num9 < count3)
					{
						float num10 = Mathf.Abs(this._rowPointsBuffer[num8][num9 - 1].x - this._rowPointsBuffer[num8][num9].x);
						if (num10 > num6)
						{
							num6 = num10;
						}
						if (num10 < num7)
						{
							num7 = num10;
						}
					}
				}
			}
			for (int num11 = 0; num11 < this._rowCount; num11++)
			{
				int count4 = this._rowPointsBuffer[num11].Count;
				for (int num12 = 1; num12 < count4; num12 += 2)
				{
					int num13 = 0;
					while (num12 < count4 && this._rowPointsBuffer[num11][num12].y < 0f)
					{
						num13 += 2;
						num12 += 2;
					}
					Vector3 vector3 = this._rowPointsBuffer[num11][num12 - 1 - num13];
					Vector3 vector4 = this._rowPointsBuffer[num11][num12];
					float y2 = vector3.y;
					float y3 = vector4.y;
					float num14 = Mathf.Max(y3 * 2f - 1f, (num13 <= 0) ? 0f : (Mathf.Abs(this._rowPointsBuffer[num11][num12 - num13].y) * 2f - 1f));
					float num15 = Mathf.Max(y2 * 2f - 1f, (num13 <= 0) ? 0f : (Mathf.Abs(this._rowPointsBuffer[num11][num12 - num13 + 1].y) * 2f - 1f));
					vector3.y = y;
					vector4.y = y;
					vector3 = base.transform.TransformPoint(vector3);
					vector4 = base.transform.TransformPoint(vector4);
					Vector3 vector5 = Vector3.Lerp(vector3, vector4, 0.5f);
					vector5.y += (Mathf.InverseLerp(num7, num6, Mathf.Abs(vector3.x - vector4.x)) / 2f + 0.5f) * this._roofHeight;
					for (int num16 = 0; num16 < 2; num16++)
					{
						if (num16 == 0)
						{
							if (y2 < 0.5f)
							{
								if (y2 * 2f < 1f - num14)
								{
									Vector3 vector6 = Vector3.Lerp(vector3, vector5, y2 * 2f);
									Vector3 chunk = Vector3.Lerp(vector5, vector6, Mathf.Clamp01(num14)) - vector6;
									this.SpawnChunk(chunk, vector6, roofRoot, logStackScaleRatio, ref num5);
								}
								if (num15 < 0f)
								{
									Vector3 vector6 = Vector3.Lerp(vector5, vector3, Mathf.Abs(num15));
									Vector3 chunk = vector5 - vector6;
									this.SpawnChunk(chunk, vector6, roofRoot, logStackScaleRatio, ref num5);
								}
							}
						}
						else if (y3 < 0.5f)
						{
							if (y3 * 2f < 1f - num15)
							{
								Vector3 vector6 = Vector3.Lerp(vector4, vector5, y3 * 2f);
								Vector3 chunk = Vector3.Lerp(vector5, vector6, Mathf.Clamp01(num15)) - vector6;
								this.SpawnChunk(chunk, vector6, roofRoot, logStackScaleRatio, ref num5);
							}
							if (num14 < 0f)
							{
								Vector3 vector6 = Vector3.Lerp(vector5, vector4, Mathf.Abs(num14));
								Vector3 chunk = vector5 - vector6;
								this.SpawnChunk(chunk, vector6, roofRoot, logStackScaleRatio, ref num5);
							}
						}
					}
				}
			}
		}

		private void SpawnChunk(Vector3 chunk, Vector3 chunkStart, Transform currentLogStackTr, float logStackScaleRatio, ref float currentLogStackRemainingScale)
		{
			Quaternion rotation = Quaternion.LookRotation(chunk);
			float magnitude = chunk.magnitude;
			int num = Mathf.CeilToInt(magnitude / this._maxSegmentHorizontalLength);
			Vector3 one = Vector3.one;
			one.z = magnitude / (float)num / this._logLength;
			Vector3 b = chunk / (float)num;
			for (int i = 0; i < num; i++)
			{
				Transform transform = this.NewLog(chunkStart, rotation);
				transform.parent = null;
				if (currentLogStackRemainingScale > 0f)
				{
					currentLogStackRemainingScale -= one.z;
				}
				else
				{
					currentLogStackTr = new GameObject("ls").transform;
					currentLogStackTr.position = transform.position;
					currentLogStackTr.parent = this._roofRoot;
					currentLogStackRemainingScale = logStackScaleRatio - one.z;
				}
				transform.parent = currentLogStackTr;
				transform.localScale = one;
				this._newPool.Push(transform);
				chunkStart += b;
			}
		}

		private Transform NewLog(Vector3 position, Quaternion rotation)
		{
			if (this._logPool.Count > 0)
			{
				Transform transform = this._logPool.Pop();
				transform.position = position;
				transform.rotation = rotation;
				return transform;
			}
			Transform transform2 = (Transform)UnityEngine.Object.Instantiate(this._logPrefab, position, rotation);
			if (!this._wasBuilt && !this._wasPlaced)
			{
				transform2.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
			}
			return transform2;
		}

		public float GetLevel()
		{
			return this._multiPointsPositions[0].y + this.GetHeight();
		}

		public float GetHeight()
		{
			return this._logWidth / 2f;
		}

		public List<Vector3> GetMultiPointsPositions()
		{
			return this._multiPointsPositions;
		}
	}
}
