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
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Floor Architect")]
	public class FloorArchitect : MonoBehaviour, IEntityReplicationFilter, IHoleStructure, IStructureSupport, ICoopStructure
	{
		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		public bool _autoSupported;

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _closureSnappingDistance = 2.5f;

		public float _minLogScale = 0.3f;

		public float _maxLogScale = 2f;

		public float _maxScaleLogCost = 1f;

		public float _minAngleBetweenEdges = 90f;

		public int _maxPoints = 50;

		public LayerMask _floorLayers;

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

		private Transform _floorRoot;

		private float _logLength;

		private float _logWidth;

		private float _maxChunkLength;

		private float _minChunkLength;

		private int _rowCount;

		private List<Vector3>[] _rowPointsBuffer;

		private List<Vector3>[] _holesRowPointsBuffer;

		private Stack<Transform> _logPool;

		private Stack<Transform> _newPool;

		private Material _logMat;

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
				CoopFloorToken coopFloorToken = new CoopFloorToken();
				if (this._holes != null)
				{
					coopFloorToken.Holes = this._holes.ToArray();
				}
				return coopFloorToken;
			}
			set
			{
				if ((value as CoopFloorToken).Holes != null)
				{
					this._holes = (value as CoopFloorToken).Holes.ToList<Hole>();
					this.CreateStructure(false);
				}
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
			this._autofillmode = !this._autoSupported;
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			FloorArchitect.<DelayedAwake>c__Iterator133 <DelayedAwake>c__Iterator = new FloorArchitect.<DelayedAwake>c__Iterator133();
			<DelayedAwake>c__Iterator.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			bool flag = this._multiPointsPositions.Count >= 3;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
			}
			bool flag2 = false;
			if (!this._autoSupported && TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f))
			{
				this._autofillmode = !this._autofillmode;
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
				this.UpdateAutoFill(true);
			}
			bool flag3;
			if (!this._autofillmode)
			{
				if (TheForest.Utils.Input.GetButtonDown("AltFire") && this._multiPointsPositions.Count > 0)
				{
					if (this._multiPointsPositions.Count == 0)
					{
						if (this._floorRoot)
						{
							UnityEngine.Object.Destroy(this._floorRoot.gameObject);
							this._floorRoot = null;
						}
						this._newPool.Clear();
						this._logPool.Clear();
						base.GetComponent<Renderer>().enabled = true;
					}
					this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
				}
				flag3 = ((this._autoSupported || (this.CurrentSupport != null && this.CurrentSupport.GetMultiPointsPositions() != null && Mathf.Abs(base.transform.position.y - this.SupportHeight) < this._closureSnappingDistance && (this._multiPointsPositions.Count == 0 || Mathf.Abs(this._multiPointsPositions[0].y - this.SupportHeight) < this._closureSnappingDistance))) && this._multiPointsPositions.Count < this._maxPoints);
				if (flag3 && this._multiPointsPositions.Count > 0)
				{
					Vector3 vector = base.transform.position - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
					flag3 = (vector.sqrMagnitude > this._minChunkLength * this._minChunkLength);
					if (this._multiPointsPositions.Count > 1)
					{
						Vector3 forward = this._multiPointsPositions[this._multiPointsPositions.Count - 2] - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
						flag3 = (flag3 && Vector3.Angle(forward.normalized, vector.normalized) >= this._minAngleBetweenEdges);
						Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(true);
						Scene.HudGui.AngleAndDistanceGizmo.position = this._multiPointsPositions.Last<Vector3>();
						Scene.HudGui.AngleAndDistanceGizmo.rotation = Quaternion.LookRotation(forward);
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
				if (flag3)
				{
					Vector3 vector2 = this.GetCurrentEdgePoint();
					bool flag4 = this._multiPointsPositions.Count > 2 && Vector3.Distance(vector2, this._multiPointsPositions[0]) < this._closureSnappingDistance;
					bool flag5 = !flag4;
					if (flag4)
					{
						vector2 = this._multiPointsPositions[0];
					}
					Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(flag5);
					if (flag5 && !this._autoSupported)
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
						if (this._multiPointsPositions.Count > 1)
						{
							base.GetComponent<Renderer>().enabled = false;
						}
						flag2 = flag4;
					}
				}
				else
				{
					Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
				}
				bool flag6 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) > this._minChunkLength;
				if (flag6 && (TheForest.Utils.Input.GetButtonDown("Rotate") || TheForest.Utils.Input.GetButtonDown("Take")))
				{
					this._multiPointsPositions.Add(this._multiPointsPositions.First<Vector3>());
					this.RefreshCurrentFloor();
					flag2 = true;
				}
			}
			else
			{
				flag3 = false;
				if (this._autofillmode && flag && TheForest.Utils.Input.GetButtonDown("Take"))
				{
					flag2 = true;
				}
			}
			if (this._multiPointsPositions.Count > 0 && !this._autofillmode)
			{
				bool flag7 = false;
				bool flag8 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions[0], this._multiPointsPositions[this._multiPointsPositions.Count - 1]) < this._closureSnappingDistance;
				if (!flag8)
				{
					if (Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) > this._closureSnappingDistance)
					{
						flag7 = true;
						this._multiPointsPositions.Add(this.GetCurrentEdgePoint());
					}
					if (this._multiPointsPositions.Count > 2 || !flag7)
					{
						this._multiPointsPositions.Add(this._multiPointsPositions[0]);
					}
				}
				this.RefreshCurrentFloor();
				if (!flag8)
				{
					if (this._multiPointsPositions.Count > 2 || !flag7)
					{
						this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					}
					if (flag7)
					{
						this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					}
				}
			}
			if (flag2)
			{
				base.enabled = false;
				base.GetComponent<Renderer>().enabled = false;
				LocalPlayer.Create.PlaceGhost(false);
			}
			Scene.HudGui.LockPositionIcon.SetActive(flag3 && !this._autofillmode);
			Scene.HudGui.UnlockPositionIcon.SetActive(this._multiPointsPositions.Count > 0 && !this._autofillmode);
			Scene.HudGui.ToggleAutoFillIcon.SetActive(!this._autoSupported && !this._autofillmode);
			Scene.HudGui.ToggleManualPlacementIcon.SetActive(!this._autoSupported && this._autofillmode);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("structure") || other.CompareTag("jumpObject"))
			{
				IStructureSupport structureSupport = (IStructureSupport)other.GetComponentInParent(typeof(IStructureSupport));
				if (structureSupport != null && (this.CurrentSupport == null || this.CurrentSupport.GetLevel() < structureSupport.GetLevel()))
				{
					this.CurrentSupport = structureSupport;
					this.SupportHeight = this.CurrentSupport.GetLevel() - 0.5f;
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
				Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
				Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
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
				if (Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) > this._closureSnappingDistance)
				{
					this._multiPointsPositions.Add(this._multiPointsPositions[0]);
				}
				if (this._wasBuilt)
				{
					this.CreateStructure(false);
					this._rowPointsBuffer = null;
					this._holesRowPointsBuffer = null;
					this._floorRoot.transform.parent = base.transform;
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
			FloorArchitect.<OnPlaced>c__Iterator134 <OnPlaced>c__Iterator = new FloorArchitect.<OnPlaced>c__Iterator134();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		private void UpdateAutoFill(bool doCleanUp)
		{
			bool flag = !this._autoSupported && this._autofillmode && this.CurrentSupport != null && this.CurrentSupport.GetMultiPointsPositions() != null;
			if (flag)
			{
				this._multiPointsPositions = new List<Vector3>(this.CurrentSupport.GetMultiPointsPositions());
				int count = this._multiPointsPositions.Count;
				float num = this.CurrentSupport.GetHeight() - 0.5f;
				float num2 = this.CurrentSupport.GetLevel() - 0.5f;
				for (int i = this._multiPointsPositions.Count - 1; i >= 0; i--)
				{
					Vector3 value = this._multiPointsPositions[i];
					if (Mathf.Abs(value.y + num - num2) < this._closureSnappingDistance)
					{
						value.y = num2;
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
					this.RefreshCurrentFloor();
					base.GetComponent<Renderer>().enabled = false;
				}
				else
				{
					this._multiPointsPositions.Clear();
					base.GetComponent<Renderer>().enabled = true;
				}
			}
			else if (this._autofillmode || doCleanUp)
			{
				this._multiPointsPositions.Clear();
				this.RefreshCurrentFloor();
				base.GetComponent<Renderer>().enabled = true;
			}
		}

		private void OnBuilt(GameObject built)
		{
			FloorArchitect component = built.GetComponent<FloorArchitect>();
			component._multiPointsPositions = this._multiPointsPositions;
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
			if (this._floorRoot)
			{
				UnityEngine.Object.Destroy(this._floorRoot.gameObject);
			}
		}

		public void CreateStructure(bool isRepair = false)
		{
			if (this._wasBuilt && isRepair)
			{
				this.Clear();
				base.StartCoroutine(this.DelayedAwake(true));
			}
			this.RefreshCurrentFloor();
			if (this._wasBuilt)
			{
				this._floorRoot.parent = base.transform;
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

		public void RefreshCurrentFloor()
		{
			if (this._logPool == null)
			{
				this._logPool = new Stack<Transform>();
			}
			this._newPool = new Stack<Transform>();
			Transform floorRoot = this._floorRoot;
			if (this._multiPointsPositions.Count >= 3)
			{
				this._floorRoot = new GameObject("FloorRoot").transform;
				this.SpawnFloor();
			}
			if (floorRoot)
			{
				UnityEngine.Object.Destroy(floorRoot.gameObject);
			}
			this._logPool = this._newPool;
		}

		private Vector3 GetCurrentEdgePoint()
		{
			Vector3 position = base.transform.position;
			if (this._multiPointsPositions.Count > 0)
			{
				position.y = this._multiPointsPositions[0].y;
			}
			else if (!this._autoSupported)
			{
				position.y = this.SupportHeight;
			}
			return position;
		}

		private Vector3 GetSegmentPointFloorPosition(Vector3 segmentPoint)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(segmentPoint, Vector3.down, out raycastHit, 500f, Scene.ValidateFloorLayers(segmentPoint, this._floorLayers.value)))
			{
				return raycastHit.point;
			}
			segmentPoint.y -= this._logLength / 2f;
			return segmentPoint;
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

		private void SpawnFloor()
		{
			float num = 3.40282347E+38f;
			float num2 = -3.40282347E+38f;
			Vector3 forward = this._multiPointsPositions[1] - this._multiPointsPositions[0];
			base.transform.rotation = Quaternion.LookRotation(forward);
			Vector3[] array = (from p in this._multiPointsPositions
			select base.transform.InverseTransformPoint(p)).ToArray<Vector3>();
			for (int i = 0; i < array.Length; i++)
			{
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
						int num4 = list.Count;
						int count = list2.Count;
						for (int m = 1; m < num4; m += 2)
						{
							for (int n = 1; n < count; n += 2)
							{
								if (list[m - 1].x > list2[n - 1].x && list[m].x < list2[n].x)
								{
									list.RemoveAt(m);
									list.RemoveAt(m - 1);
									hole._used = true;
									if (n + 2 >= count)
									{
										if (m + 2 >= num4)
										{
											break;
										}
										m -= 2;
										num4 -= 2;
									}
								}
								else if (list[m - 1].x < list2[n - 1].x && list[m].x > list2[n].x)
								{
									list.Insert(m, list2[n - 1]);
									list.Insert(m + 1, list2[n]);
									hole._used = true;
								}
								else if (list[m - 1].x > list2[n - 1].x && list[m - 1].x < list2[n].x)
								{
									list[m - 1] = list2[n];
									hole._used = true;
								}
								else if (list[m].x > list2[n - 1].x && list[m].x < list2[n].x)
								{
									list[m] = list2[n - 1];
									hole._used = true;
								}
							}
						}
					}
				}
			}
			Transform transform = this._floorRoot;
			float num5 = 0f;
			float num6 = (this._maxScaleLogCost <= 0f) ? 0f : (this._maxLogScale / this._maxScaleLogCost);
			Vector3 one = Vector3.one;
			Quaternion rotation = Quaternion.LookRotation(base.transform.right);
			for (int num7 = 0; num7 < this._rowCount; num7++)
			{
				this._rowPointsBuffer[num7].Sort((Vector3 x1, Vector3 x2) => x1.x.CompareTo(x2.x));
				int count2 = this._rowPointsBuffer[num7].Count;
				for (int num8 = 1; num8 < count2; num8 += 2)
				{
					Vector3 vector = base.transform.TransformPoint(this._rowPointsBuffer[num7][num8 - 1]);
					Vector3 a = base.transform.TransformPoint(this._rowPointsBuffer[num7][num8]) - vector;
					Vector3 normalized = a.normalized;
					if (num8 - 1 == 0)
					{
						vector -= normalized;
					}
					if (num8 + 1 == count2)
					{
						a += normalized * 2f;
					}
					else
					{
						a += normalized;
					}
					float magnitude = a.magnitude;
					int num9 = Mathf.CeilToInt(magnitude / this._maxChunkLength);
					one.z = magnitude / (float)num9 / this._logLength;
					Vector3 b = a / (float)num9;
					for (int num10 = 0; num10 < num9; num10++)
					{
						Transform transform2 = this.NewLog(vector, rotation);
						transform2.parent = null;
						if (num5 > 0f)
						{
							num5 -= one.z;
							transform2.parent = transform;
							Vector3 localScale = one;
							localScale.z /= transform.localScale.z;
							transform2.localScale = localScale;
						}
						else
						{
							transform = transform2;
							num5 = num6 - one.z;
							transform2.parent = this._floorRoot;
							transform2.localScale = one;
						}
						this._newPool.Push(transform2);
						vector += b;
					}
				}
			}
		}

		private void SpawnFloorCollider()
		{
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
			if (!this._wasBuilt)
			{
				UnityEngine.Object.Destroy(transform2.GetComponent<Collider>());
				if (!this._wasPlaced)
				{
					transform2.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
				}
			}
			else
			{
				transform2.rotation *= this.RandomLogRotation();
			}
			return transform2;
		}

		private Quaternion RandomLogRotation()
		{
			return Quaternion.Euler(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
		}

		public float GetLevel()
		{
			return this._multiPointsPositions[0].y + this.GetHeight();
		}

		public float GetHeight()
		{
			return this._logWidth * 0.4f;
		}

		public List<Vector3> GetMultiPointsPositions()
		{
			return this._multiPointsPositions;
		}
	}
}
