using System;
using UnityEngine;

public class WorldHeightmap : MonoBehaviour
{
	private static WorldHeightmap instance;

	public RenderTexture Heightmap;

	[HideInInspector]
	public Camera HeightmapCamera;

	public Shader DepthShader;

	public LayerMask OccluderMask = 0;

	public int Resolution = 1024;

	public float PhysicalSize = 3500f;

	[Range(0f, 25f)]
	public float HeightBias = 7.5f;

	public static WorldHeightmap Instance
	{
		get
		{
			if (!WorldHeightmap.instance)
			{
				Debug.LogWarning("No World Heightmap Found, please add one. (From the Menu: GameObject/The Forest/World Heightmap)");
				WorldHeightmap.AddWorldHeightmap();
			}
			return WorldHeightmap.instance;
		}
	}

	public Bounds Bounds
	{
		get
		{
			return new Bounds(Vector3.zero, new Vector3(this.PhysicalSize, this.PhysicalSize, this.PhysicalSize));
		}
	}

	private static void AddWorldHeightmap()
	{
		if (GameObject.Find("World Heightmap") == null)
		{
			GameObject gameObject = new GameObject("World Heightmap");
			WorldHeightmap.instance = gameObject.AddComponent<WorldHeightmap>();
		}
		else
		{
			Debug.Log("World Heightmap already added. :)");
		}
	}

	public static void Initialize()
	{
		if (WorldHeightmap.instance == null)
		{
			WorldHeightmap.AddWorldHeightmap();
		}
	}

	private void Awake()
	{
		if (WorldHeightmap.instance)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			WorldHeightmap.instance = this;
		}
	}

	private void Start()
	{
		if (this.OccluderMask == 0)
		{
			this.OccluderMask = (1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("ReflectBig"));
		}
		this.InitHeightmap();
		this.UpdateHeightmap();
	}

	private void LateUpdate()
	{
		if (Time.frameCount == 2)
		{
			this.UpdateHeightmap();
		}
		Shader.SetGlobalVector("WorldHeightmapStart", this.Bounds.min + new Vector3(0f, -this.HeightBias, 0f));
	}

	private void InitHeightmap()
	{
		if (!this.Heightmap)
		{
			this.Heightmap = new RenderTexture(this.Resolution, this.Resolution, 16, RenderTextureFormat.ARGB32);
			this.Heightmap.name = "World Heightmap";
			this.Heightmap.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!this.HeightmapCamera)
		{
			this.HeightmapCamera = new GameObject("World Heightmap Camera")
			{
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<Camera>();
			this.HeightmapCamera.useOcclusionCulling = false;
			this.HeightmapCamera.enabled = false;
		}
		if (this.DepthShader == null)
		{
			this.DepthShader = Shader.Find("Hidden/Sunshine/Shadow Caster");
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			if (this.Heightmap)
			{
				UnityEngine.Object.Destroy(this.Heightmap);
			}
			this.Heightmap = null;
			this.UpdateHeightmap();
		}
	}

	private void OnDestroy()
	{
		if (this.Heightmap != null)
		{
			UnityEngine.Object.Destroy(this.Heightmap);
			this.Heightmap = null;
		}
	}

	public void UpdateHeightmap()
	{
		this.InitHeightmap();
		if (this.DepthShader == null)
		{
			Debug.Log("World Heightmap Depth Shader not configured...");
			return;
		}
		this.HeightmapCamera.orthographic = true;
		this.HeightmapCamera.transform.position = new Vector3(0f, this.PhysicalSize / 2f, 0f);
		this.HeightmapCamera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		this.HeightmapCamera.targetTexture = this.Heightmap;
		this.HeightmapCamera.cullingMask = this.OccluderMask;
		this.HeightmapCamera.farClipPlane = this.PhysicalSize;
		this.HeightmapCamera.orthographicSize = this.PhysicalSize / 2f;
		this.HeightmapCamera.clearFlags = CameraClearFlags.Color;
		this.HeightmapCamera.backgroundColor = Color.white;
		Terrain activeTerrain = Terrain.activeTerrain;
		float heightmapPixelError = 0f;
		if (activeTerrain)
		{
			heightmapPixelError = activeTerrain.heightmapPixelError;
			activeTerrain.heightmapPixelError = 0f;
		}
		this.HeightmapCamera.targetTexture = this.Heightmap;
		this.HeightmapCamera.RenderWithShader(this.DepthShader, "RenderType");
		this.HeightmapCamera.targetTexture = null;
		if (activeTerrain)
		{
			activeTerrain.heightmapPixelError = heightmapPixelError;
		}
		Shader.SetGlobalTexture("WorldHeightmap", this.Heightmap);
		Shader.SetGlobalVector("WorldHeightmapSize", this.Bounds.size);
	}
}
