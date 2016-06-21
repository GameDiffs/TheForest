using System;
using UnityEngine;

[ExecuteInEditMode]
public class AfsGrassDisplacementController : MonoBehaviour
{
	public enum DisplacementRenderTexSize
	{
		_128 = 128,
		_256 = 256,
		_512 = 512,
		_1024 = 1024
	}

	[Header("Camera Settings")]
	public Camera MainCamera;

	private Vector3 MainCamPos;

	private float Step;

	public Camera DisplacementCamera;

	public GameObject DisplacementCameraGO;

	public LayerMask DisplacementLayer = 256;

	[Header("Render Texture Settings")]
	public RenderTexture DisplacementTexture;

	public AfsGrassDisplacementController.DisplacementRenderTexSize RenderTexSize = AfsGrassDisplacementController.DisplacementRenderTexSize._512;

	private AfsGrassDisplacementController.DisplacementRenderTexSize OldRenderTexSize;

	public float DisplacementTexCoverage = 40f;

	[Header("Terrain")]
	public Terrain assignedTerrain;

	private float TerrainSizeX;

	private float TerrainSizeZ;

	private Vector2 terrainSizeOverTexturesize;

	private Vector2 DisplacementTexOrigin;

	[Header("Testing & Debugging")]
	public bool UseFullRotation = true;

	public bool ShowColors;

	private void createDisplacementTexture()
	{
		this.DisplacementTexture = new RenderTexture((int)this.RenderTexSize, (int)this.RenderTexSize, 0, RenderTextureFormat.ARGBHalf);
		this.DisplacementTexture.generateMips = false;
		this.DisplacementTexture.antiAliasing = 1;
		this.DisplacementTexture.filterMode = FilterMode.Point;
		this.OldRenderTexSize = this.RenderTexSize;
		if (this.DisplacementCamera)
		{
			this.DisplacementCamera.targetTexture = this.DisplacementTexture;
		}
	}

	private void CreateComponents()
	{
		if (!this.assignedTerrain)
		{
			this.assignedTerrain = Terrain.activeTerrain;
		}
		this.TerrainSizeX = this.assignedTerrain.terrainData.size.x;
		this.TerrainSizeZ = this.assignedTerrain.terrainData.size.z;
		this.terrainSizeOverTexturesize = new Vector2(this.TerrainSizeX / this.DisplacementTexCoverage, this.TerrainSizeZ / this.DisplacementTexCoverage);
		Shader.SetGlobalVector("_AfsGrassTerrainDisplacementTexCoverage", new Vector4(this.TerrainSizeX, this.TerrainSizeZ, this.terrainSizeOverTexturesize.x, this.terrainSizeOverTexturesize.y));
		float x = this.assignedTerrain.transform.position.x / this.TerrainSizeX;
		float y = this.assignedTerrain.transform.position.z / this.TerrainSizeZ;
		Shader.SetGlobalVector("_AfsGrassTerrainNormalizedPos", new Vector2(x, y));
		GameObject exists = GameObject.Find("AFSGrassDisplacementCameraTest");
		if (!exists)
		{
			this.DisplacementCameraGO = new GameObject("AFSGrassDisplacementCameraTest");
			this.DisplacementCameraGO.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
			this.createDisplacementTexture();
			this.DisplacementCamera = this.DisplacementCameraGO.AddComponent<Camera>();
			this.DisplacementCamera.orthographic = true;
			this.DisplacementCamera.orthographicSize = this.DisplacementTexCoverage * 0.5f;
			this.DisplacementCamera.renderingPath = RenderingPath.Forward;
			this.DisplacementCamera.nearClipPlane = 1f;
			this.DisplacementCamera.farClipPlane = 100f;
			this.DisplacementCamera.cullingMask = this.DisplacementLayer;
			this.DisplacementCamera.useOcclusionCulling = false;
			this.DisplacementCamera.backgroundColor = Color.black;
			this.DisplacementCamera.clearFlags = CameraClearFlags.Color;
			this.DisplacementCamera.targetTexture = this.DisplacementTexture;
			this.DisplacementCamera.depth = Camera.main.depth - 1f;
		}
	}

	private void Awake()
	{
		if (CoopPeerStarter.DedicatedHost)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			this.CreateComponents();
		}
	}

	private void Update()
	{
		if (!this.MainCamera)
		{
			this.MainCamera = Camera.main;
			if (!this.MainCamera)
			{
				return;
			}
		}
		if (this.OldRenderTexSize != this.RenderTexSize || this.DisplacementCamera.targetTexture == null || this.DisplacementTexture == null)
		{
			this.createDisplacementTexture();
			Debug.Log("Displacement Rendertex recreated");
		}
		this.DisplacementCamera.orthographicSize = this.DisplacementTexCoverage * 0.5f;
		this.terrainSizeOverTexturesize = new Vector2(this.TerrainSizeX / this.DisplacementTexCoverage, this.TerrainSizeZ / this.DisplacementTexCoverage);
		Shader.SetGlobalVector("_AfsGrassTerrainDisplacementTexSize", new Vector4(this.TerrainSizeX, this.TerrainSizeZ, this.terrainSizeOverTexturesize.x, this.terrainSizeOverTexturesize.y));
		this.Step = this.DisplacementTexCoverage / (float)this.RenderTexSize;
		this.MainCamPos = this.MainCamera.transform.position;
		this.MainCamPos.x = Mathf.Floor(this.MainCamPos.x / this.Step) * this.Step;
		this.MainCamPos.z = Mathf.Floor(this.MainCamPos.z / this.Step) * this.Step;
		this.DisplacementCamera.transform.position = new Vector3(this.MainCamPos.x, this.MainCamPos.y + 50f, this.MainCamPos.z);
		this.DisplacementTexOrigin = new Vector2(this.DisplacementCamera.transform.position.x - this.DisplacementTexCoverage * 0.5f, this.DisplacementCamera.transform.position.z - this.DisplacementTexCoverage * 0.5f);
		this.DisplacementTexOrigin = new Vector2(this.DisplacementTexOrigin.x / this.TerrainSizeX, this.DisplacementTexOrigin.y / this.TerrainSizeZ);
		Shader.SetGlobalVector("_AfsGrassDisplacementTexPos", this.DisplacementTexOrigin);
		Shader.SetGlobalTexture("_AfsGrassDisplacementTex", this.DisplacementTexture);
		if (this.UseFullRotation)
		{
		}
		if (this.ShowColors)
		{
		}
	}
}
