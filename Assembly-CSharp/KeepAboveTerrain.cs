using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class KeepAboveTerrain : MonoBehaviour
{
	public GameObject ShowAnchorArea;

	public LayerMask FloorLayers;

	public LayerMask DynFloorLayers;

	public Material RedMat;

	public Material ClearMat;

	public Renderer MyRender;

	public bool Locked;

	public bool Clear = true;

	public bool TreeStructure;

	public bool AllowFoundation;

	public bool Airborne;

	public float FoundationMinSlope = 0.02f;

	public float maxBuildingHeight = 100f;

	public float maxAirBorneHeight = 25f;

	private RaycastHit hit;

	private Terrain activeTerrain;

	private BoxCollider boxCollider;

	private float turn;

	private float curDir;

	private Vector3 curNormal = Vector3.up;

	private Quaternion curGroundTilt = Quaternion.identity;

	private Vector3 sinkHolePos;

	public bool IsInSinkHole
	{
		get;
		private set;
	}

	public float AirBorneHeight
	{
		get;
		set;
	}

	public float RegularHeight
	{
		get;
		set;
	}

	public Quaternion CurrentRotation
	{
		get;
		set;
	}

	public RaycastHit? LastHit
	{
		get;
		set;
	}

	public GameObject ForcedParent
	{
		get;
		set;
	}

	public LayerMask FloorLayersFinal
	{
		get;
		set;
	}

	private void Awake()
	{
		this.activeTerrain = Terrain.activeTerrain;
		this.boxCollider = base.gameObject.GetComponent<BoxCollider>();
	}

	private void Start()
	{
		this.sinkHolePos = Scene.SinkHoleCenter.position;
	}

	private void OnEnable()
	{
		this.turn = 0f;
		this.Clear = true;
		this.FloorLayersFinal = ((!LocalPlayer.Create.CurrentBlueprint._allowParentingToDynamic) ? this.FloorLayers : this.DynFloorLayers);
	}

	private void Update()
	{
		if (!TheForest.Utils.Input.IsGamePad)
		{
			if (TheForest.Utils.Input.GetButtonDown("Rotate"))
			{
				this.turn += 60f * Time.deltaTime;
			}
			else if (TheForest.Utils.Input.GetButtonUp("Rotate"))
			{
				this.turn = 0f;
			}
		}
		else
		{
			float num = Mathf.Clamp(TheForest.Utils.Input.GetAxis("Rotate"), -1f, 1f);
			if (!Mathf.Approximately(num, 0f))
			{
				this.turn = num * 60f * Time.deltaTime;
			}
			else
			{
				this.turn = 0f;
			}
		}
		this.curDir = (this.curDir + this.turn + 360f) % 360f;
		this.CurrentRotation = Quaternion.Euler(0f, this.curDir, 0f);
		if (!this.Locked && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
		{
			Vector3 position = this.boxCollider.center - this.boxCollider.size / 2f;
			Vector3 vector = this.boxCollider.center + this.boxCollider.size / 2f;
			Vector3 vector2 = base.transform.TransformPoint(position);
			Vector3 vector3 = base.transform.TransformPoint(new Vector3(position.x, position.y, vector.z));
			Vector3 vector4 = base.transform.TransformPoint(new Vector3(vector.x, position.y, position.z));
			Vector3 vector5 = base.transform.TransformPoint(new Vector3(vector.x, position.y, vector.z));
			this.sinkHolePos.y = base.transform.position.y;
			this.IsInSinkHole = (Vector3.Distance(this.sinkHolePos, base.transform.position) < 190f);
			float maxDistance = (!this.IsInSinkHole) ? this.maxBuildingHeight : (this.maxBuildingHeight + 300f);
			Vector3 position2 = Camera.main.transform.position;
			bool flag = false;
			Vector3 b;
			Vector3 point;
			if (MathEx.ClosestPointsOnTwoLines(out b, out point, position2, Camera.main.transform.forward, base.transform.position, Vector3.up))
			{
				if (!Physics.Raycast(position2, Camera.main.transform.forward, out this.hit, Vector3.Distance(position2, b) * 0.96f, this.GetValidLayers(this.FloorLayersFinal.value)))
				{
					this.LastHit = null;
					if (this.Airborne)
					{
						vector2.y = point.y;
						vector3.y = point.y;
						vector4.y = point.y;
						vector5.y = point.y;
					}
					else
					{
						vector2.y = point.y + 2f;
						vector3.y = point.y + 2f;
						vector4.y = point.y + 2f;
						vector5.y = point.y + 2f;
					}
				}
				else
				{
					float y = LocalPlayer.Transform.position.y + 2f;
					this.LastHit = new RaycastHit?(this.hit);
					point = this.hit.point;
					if (this.hit.normal.y > 0f)
					{
						flag = true;
						point.y = y;
						vector2.y = y;
						vector3.y = y;
						vector4.y = y;
						vector5.y = y;
					}
					else if (!this.Airborne)
					{
						point.y += this.maxBuildingHeight / 2f;
						vector2.y = this.hit.point.y + this.maxBuildingHeight / 2f;
						vector3.y = this.hit.point.y + this.maxBuildingHeight / 2f;
						vector4.y = this.hit.point.y + this.maxBuildingHeight / 2f;
						vector5.y = this.hit.point.y + this.maxBuildingHeight / 2f;
					}
				}
			}
			if (Physics.Raycast(vector2, Vector3.down, out this.hit, maxDistance, this.GetValidLayers(this.FloorLayersFinal.value)))
			{
				vector2.y = this.hit.point.y;
			}
			else
			{
				vector2.y = this.activeTerrain.SampleHeight(vector2) + this.activeTerrain.transform.position.y;
			}
			if (Physics.Raycast(vector3, Vector3.down, out this.hit, maxDistance, this.GetValidLayers(this.FloorLayersFinal.value)))
			{
				vector3.y = this.hit.point.y;
			}
			else
			{
				vector3.y = this.activeTerrain.SampleHeight(vector3) + this.activeTerrain.transform.position.y;
			}
			if (Physics.Raycast(vector4, Vector3.down, out this.hit, maxDistance, this.GetValidLayers(this.FloorLayersFinal.value)))
			{
				vector4.y = this.hit.point.y;
			}
			else
			{
				vector4.y = this.activeTerrain.SampleHeight(vector4) + this.activeTerrain.transform.position.y;
			}
			if (Physics.Raycast(vector5, Vector3.down, out this.hit, maxDistance, this.GetValidLayers(this.FloorLayersFinal.value)))
			{
				vector5.y = this.hit.point.y;
			}
			else
			{
				vector5.y = this.activeTerrain.SampleHeight(vector5) + this.activeTerrain.transform.position.y;
			}
			bool flag2 = false;
			Vector3 position3 = base.transform.position;
			position3.y = point.y;
			float num2;
			if (Physics.Raycast(position3, Vector3.down, out this.hit, maxDistance, this.GetValidLayers(this.FloorLayersFinal.value)))
			{
				num2 = this.hit.point.y;
				if (!this.LastHit.HasValue)
				{
					this.LastHit = new RaycastHit?(this.hit);
				}
				if (this.hit.transform.name.Equals("MainTerrain") || this.hit.transform.gameObject.layer == 17)
				{
					flag2 = true;
				}
			}
			else
			{
				num2 = this.activeTerrain.SampleHeight(base.transform.position) + this.activeTerrain.transform.position.y;
			}
			if (flag)
			{
				point.y = num2;
			}
			float num3 = (vector2.y + vector3.y + vector4.y + vector5.y + num2) / 5f;
			if (!flag2)
			{
				float num4 = 1f;
				if (Mathf.Abs((vector3.y + vector4.y + vector5.y + num2) / 4f - vector2.y) > num4)
				{
					num3 = (vector3.y + vector4.y + vector5.y + num2) / 4f;
					vector2.y = num3;
				}
				if (Mathf.Abs((vector2.y + vector4.y + vector5.y + num2) / 4f - vector3.y) > num4)
				{
					num3 = (vector2.y + vector4.y + vector5.y + num2) / 4f;
					vector3.y = num3;
				}
				if (Mathf.Abs((vector2.y + vector3.y + vector5.y + num2) / 4f - vector4.y) > num4)
				{
					num3 = (vector2.y + vector3.y + vector5.y + num2) / 4f;
					vector4.y = num3;
				}
				if (Mathf.Abs((vector2.y + vector3.y + vector4.y + num2) / 4f - vector5.y) > num4)
				{
					num3 = (vector2.y + vector3.y + vector4.y + num2) / 4f;
					vector5.y = num3;
				}
			}
			if (this.Airborne)
			{
				if (this.IsInSinkHole)
				{
					float num5 = LocalPlayer.Transform.position.y + this.maxAirBorneHeight;
					if (point.y > num5)
					{
						point.y = num5;
					}
				}
				else if (point.y - num3 > this.maxAirBorneHeight)
				{
					point.y = num3 + this.maxAirBorneHeight;
				}
			}
			Vector3 a = Vector3.Cross(vector3 - vector2, vector4 - vector2);
			Vector3 b2 = Vector3.Cross(vector4 - vector5, vector3 - vector5);
			this.curNormal = Vector3.Normalize((a + b2) / 2f);
			if (!this.AllowFoundation || Vector3.Angle(Vector3.up, this.curNormal) < this.FoundationMinSlope)
			{
				this.AirBorneHeight = Mathf.Max(point.y, Mathf.Min(num2, num3));
				this.RegularHeight = Mathf.Min(num2, num3);
				if (this.curNormal.y < 0.5f)
				{
					this.curNormal = Vector3.up;
					if (this.Airborne)
					{
						this.SetClear();
					}
				}
				this.curGroundTilt = Quaternion.FromToRotation(Vector3.up, this.curNormal);
				this.ApplyLeaningPosRot(base.transform, this.Airborne, this.TreeStructure);
			}
			else
			{
				this.AirBorneHeight = Mathf.Max(new float[]
				{
					vector2.y,
					vector3.y,
					vector4.y,
					vector5.y,
					num2,
					point.y
				});
				this.RegularHeight = Mathf.Max(new float[]
				{
					vector2.y,
					vector3.y,
					vector4.y,
					vector5.y,
					num2
				});
				base.transform.position = new Vector3(base.transform.position.x, (!this.Airborne) ? this.RegularHeight : this.AirBorneHeight, base.transform.position.z);
				base.transform.rotation = this.CurrentRotation;
			}
		}
		if (this.TreeStructure)
		{
			LocalPlayer.Create.CurrentGhost.transform.rotation = this.CurrentRotation;
			if (this.Locked != this.Clear)
			{
				if (this.Locked)
				{
					if (this.MyRender)
					{
						this.MyRender.material = this.ClearMat;
					}
					this.Clear = true;
				}
				else
				{
					if (this.MyRender)
					{
						this.MyRender.material = this.RedMat;
					}
					this.Clear = false;
				}
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((other.gameObject.CompareTag("Tree") && !this.TreeStructure) || other.gameObject.CompareTag("Block") || other.gameObject.CompareTag("jumpObject"))
		{
			this.SetNotclear();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.gameObject.CompareTag("Tree") && !this.TreeStructure) || other.gameObject.CompareTag("Block") || other.gameObject.CompareTag("jumpObject"))
		{
			this.SetClear();
		}
	}

	public void ApplyLeaningPosRot(Transform t, bool airborne, bool treeStructure)
	{
		t.position = new Vector3(t.position.x, (!airborne) ? this.RegularHeight : this.AirBorneHeight, t.position.z);
		t.rotation = ((!airborne && !treeStructure) ? (this.curGroundTilt * this.CurrentRotation) : this.CurrentRotation);
	}

	public void SetClear()
	{
		if (this.MyRender)
		{
			this.MyRender.sharedMaterial = this.ClearMat;
		}
		this.Clear = true;
	}

	public void SetNotclear()
	{
		if (this.MyRender)
		{
			this.MyRender.sharedMaterial = this.RedMat;
		}
		this.Clear = false;
	}

	public void SetRenderer(Renderer r)
	{
		this.MyRender = r;
		if (this.Clear)
		{
			this.SetClear();
		}
		else
		{
			this.SetNotclear();
		}
	}

	public int GetValidLayers(int layers)
	{
		return (!this.IsInSinkHole) ? layers : (layers ^ 67108864);
	}
}
