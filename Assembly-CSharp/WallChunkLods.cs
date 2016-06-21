using Bolt;
using System;
using TheForest.Buildings.Creation;
using TheForest.Utils;
using UnityEngine;

public class WallChunkLods : EntityBehaviour
{
	public float _visibleDistanceLod1 = 50f;

	public float _visibleDistanceLod2 = 100f;

	public float _visibleDistanceBillboard = 200f;

	private int _wsToken = -1;

	private GameObject _wallRootGO;

	private GameObject _wallRootLOD1GO;

	private GameObject _wallRootLOD2GO;

	private byte _currentVisibility;

	private float _endStipplingTime;

	private static Vector3[] vertices;

	private static Vector2[] uv;

	private static Vector3[] normals;

	private static int[] triangles;

	private static Vector4[] tangents;

	private void Awake()
	{
		base.enabled = false;
	}

	private void Update()
	{
		if (this._endStipplingTime < Time.realtimeSinceStartup)
		{
			if (this._currentVisibility < 3)
			{
				this.EndHideQuad();
			}
			else
			{
				this.EndShowQuad();
			}
		}
	}

	private void OnDestroy()
	{
		if (this._wsToken > -1)
		{
			try
			{
				WorkScheduler.Unregister(new WorkScheduler.Task(this.RefreshVisibility), this._wsToken);
			}
			catch
			{
			}
		}
		if (this._wallRootLOD1GO)
		{
			UnityEngine.Object.Destroy(this._wallRootLOD1GO);
		}
		if (this._wallRootLOD2GO)
		{
			UnityEngine.Object.Destroy(this._wallRootLOD2GO);
		}
	}

	public void DefineChunk(Vector3 p1, Vector3 p2, float height, Transform wallRoot, WallChunkArchitect.Additions wallAddition)
	{
		if (this._wsToken < 0)
		{
			this.SpawnQuad(p1, p2, height);
			this._wallRootLOD1GO = this.SpawnLOD(Prefabs.Instance.LogWallExBuiltPrefabLOD1, wallRoot);
			this._wallRootLOD2GO = this.SpawnLOD(Prefabs.Instance.LogWallExBuiltPrefabLOD2, wallRoot);
			this._wallRootGO = wallRoot.gameObject;
			this._wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.RefreshVisibility), base.transform.position, false);
			base.gameObject.SetActive(false);
		}
	}

	private void RefreshVisibility()
	{
		if (BoltNetwork.isRunning && !this.entity.isAttached)
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y = PlayerCamLocation.PlayerLoc.y;
		float num = (position - PlayerCamLocation.PlayerLoc).sqrMagnitude + (float)((this._currentVisibility - 1) * 2);
		byte b;
		if (num < this._visibleDistanceLod2 * this._visibleDistanceLod2)
		{
			if (num < this._visibleDistanceLod1 * this._visibleDistanceLod1)
			{
				b = 0;
			}
			else
			{
				b = 1;
			}
		}
		else if (num < this._visibleDistanceBillboard * this._visibleDistanceBillboard)
		{
			b = 2;
		}
		else
		{
			b = 3;
		}
		if (b != this._currentVisibility)
		{
			if (this._currentVisibility == 3)
			{
				this.BeginHideQuad(b);
			}
			else if (b == 3)
			{
				this.BeginShowQuad();
			}
			else
			{
				this.SetLod(b);
			}
			this._currentVisibility = b;
		}
	}

	private void SpawnQuad(Vector3 p1, Vector3 p2, float height)
	{
		MeshFilter meshFilter = base.GetComponent<MeshFilter>();
		Renderer renderer = base.GetComponent<Renderer>();
		base.transform.position = p1;
		base.transform.LookAt(p2);
		if (!meshFilter)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		if (!renderer)
		{
			renderer = base.gameObject.AddComponent<MeshRenderer>();
			renderer.gameObject.layer = 13;
		}
		if (!base.GetComponent<TimedStippling_Pool>())
		{
			base.gameObject.AddComponent<TimedStippling_Pool>();
		}
		if (WallChunkLods.uv == null)
		{
			WallChunkLods.uv = new Vector2[4];
			WallChunkLods.uv[0] = new Vector2(0f, 0f);
			WallChunkLods.uv[1] = new Vector2(1f, 0f);
			WallChunkLods.uv[2] = new Vector2(1f, 1f);
			WallChunkLods.uv[3] = new Vector2(0f, 1f);
		}
		if (WallChunkLods.triangles == null)
		{
			WallChunkLods.triangles = new int[12];
			WallChunkLods.triangles[0] = 0;
			WallChunkLods.triangles[2] = 1;
			WallChunkLods.triangles[1] = 2;
			WallChunkLods.triangles[3] = 2;
			WallChunkLods.triangles[5] = 3;
			WallChunkLods.triangles[4] = 0;
			WallChunkLods.triangles[6] = 2;
			WallChunkLods.triangles[7] = 0;
			WallChunkLods.triangles[8] = 1;
			WallChunkLods.triangles[9] = 3;
			WallChunkLods.triangles[10] = 0;
			WallChunkLods.triangles[11] = 2;
		}
		if (WallChunkLods.vertices == null)
		{
			WallChunkLods.vertices = new Vector3[4];
		}
		WallChunkLods.vertices[0] = base.transform.InverseTransformPoint(p1 - new Vector3(0f, height / 10f, 0f));
		WallChunkLods.vertices[1] = base.transform.InverseTransformPoint(p2 - new Vector3(0f, height / 10f, 0f));
		WallChunkLods.vertices[2] = base.transform.InverseTransformPoint(p2 + new Vector3(0f, height * 0.9f, 0f));
		WallChunkLods.vertices[3] = base.transform.InverseTransformPoint(p1 + new Vector3(0f, height * 0.9f, 0f));
		if (WallChunkLods.normals == null)
		{
			WallChunkLods.normals = new Vector3[WallChunkLods.vertices.Length];
		}
		for (int i = 0; i < WallChunkLods.normals.Length; i++)
		{
			WallChunkLods.normals[i] = base.transform.right;
		}
		if (WallChunkLods.tangents == null)
		{
			WallChunkLods.tangents = new Vector4[4];
			WallChunkLods.tangents[0] = new Vector4(1f, 0f, 0f, -1f);
			WallChunkLods.tangents[1] = new Vector4(1f, 0f, 0f, -1f);
			WallChunkLods.tangents[2] = new Vector4(1f, 0f, 0f, -1f);
			WallChunkLods.tangents[3] = new Vector4(1f, 0f, 0f, -1f);
		}
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = WallChunkLods.vertices;
		mesh.uv = WallChunkLods.uv;
		mesh.triangles = WallChunkLods.triangles;
		mesh.normals = WallChunkLods.normals;
		mesh.tangents = WallChunkLods.tangents;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(10f, 10f, 10f));
		mesh.Optimize();
		meshFilter.sharedMesh = mesh;
		renderer.sharedMaterial = Prefabs.Instance.WallChunkBillboardMat;
	}

	private GameObject SpawnLOD(Mesh lodM, Transform wallRoot)
	{
		Transform transform = (Transform)UnityEngine.Object.Instantiate(wallRoot, wallRoot.position, wallRoot.rotation);
		transform.parent = wallRoot.parent;
		transform.gameObject.SetActive(false);
		foreach (Transform transform2 in transform)
		{
			MeshFilter component = transform2.GetChild(0).GetComponent<MeshFilter>();
			if (component)
			{
				component.sharedMesh = lodM;
			}
		}
		return transform.gameObject;
	}

	private void BeginShowQuad()
	{
		base.enabled = true;
		base.gameObject.SetActive(true);
		base.GetComponent<TimedStippling_Pool>().OnSpawned();
		this._endStipplingTime = Time.realtimeSinceStartup + 1f;
	}

	private void EndShowQuad()
	{
		base.enabled = false;
		this.SetLod(this._currentVisibility);
	}

	private void BeginHideQuad(byte lod)
	{
		base.enabled = true;
		this.SetLod(lod);
		base.GetComponent<TimedStippling_Pool>().OnDespawned(null);
		this._endStipplingTime = Time.realtimeSinceStartup + 1f;
	}

	private void EndHideQuad()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	private void SetLod(byte lod)
	{
		if (lod < 2)
		{
			if (lod == 0)
			{
				this._wallRootGO.SetActive(true);
				this._wallRootLOD1GO.SetActive(false);
				this._wallRootLOD2GO.SetActive(false);
			}
			else
			{
				this._wallRootGO.SetActive(false);
				this._wallRootLOD1GO.SetActive(true);
				this._wallRootLOD2GO.SetActive(false);
			}
		}
		else if (lod == 2)
		{
			this._wallRootGO.SetActive(false);
			this._wallRootLOD1GO.SetActive(false);
			this._wallRootLOD2GO.SetActive(true);
		}
		else
		{
			this._wallRootGO.SetActive(false);
			this._wallRootLOD1GO.SetActive(false);
			this._wallRootLOD2GO.SetActive(false);
		}
	}
}
