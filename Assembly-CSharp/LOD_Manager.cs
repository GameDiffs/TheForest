using System;
using UnityEngine;

[ExecuteInEditMode]
public class LOD_Manager : MonoBehaviour
{
	private static LOD_Manager instance;

	[Range(30f, 120f)]
	public float TargetFPS = 30f;

	[Range(0.01f, 5.5f)]
	public float Padding = 1f;

	public LOD_Settings BigProp = new LOD_Settings(new float[]
	{
		30f,
		100f,
		450f
	});

	public LOD_Settings Bush = new LOD_Settings(new float[]
	{
		20f,
		40f
	});

	public LOD_Settings Cave = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	public LOD_Settings CaveEntrance = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	public LOD_Settings Creature = new LOD_Settings(new float[]
	{
		20f
	});

	public LOD_Settings Enemy = new LOD_Settings(new float[]
	{
		20f
	});

	public LOD_Settings PickUps = new LOD_Settings(new float[]
	{
		20f,
		80f
	});

	public LOD_Settings Plant = new LOD_Settings(new float[]
	{
		20f,
		80f
	});

	public LOD_Settings Rocks = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	public LOD_Settings Trees = new LOD_Settings(new float[]
	{
		10f,
		150f,
		3000f
	});

	private float fps = 60f;

	private float currentQuality = 1f;

	private float currentCaveQuality = 1f;

	public static float TreeOcclusionBonusRatio = 1f;

	public static LOD_Manager Instance
	{
		get
		{
			if (!LOD_Manager.instance)
			{
				Debug.LogWarning("No LOD Manager Found, please add one. (From the Menu: GameObject/The Forest/LOD Manager)");
				LOD_Manager.AddLODManager();
			}
			return LOD_Manager.instance;
		}
	}

	private static void AddLODManager()
	{
		if (GameObject.Find("LOD Manager") == null)
		{
			GameObject gameObject = new GameObject("LOD Manager");
			gameObject.AddComponent<LOD_Manager>();
		}
		else
		{
			Debug.Log("LOD Manager already added. :)");
		}
	}

	private void Awake()
	{
		if (LOD_Manager.instance == null)
		{
			LOD_Manager.instance = this;
		}
		else if (LOD_Manager.instance != this)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			this.fps = Mathf.Lerp(this.fps, 1f / Time.smoothDeltaTime, 0.05f);
			float num = Mathf.Clamp01(this.fps / this.TargetFPS);
			float num2 = Mathf.Abs(num - this.currentQuality);
			if (num2 > 0.075f)
			{
				this.currentQuality = Mathf.Lerp(this.currentQuality, num, 0.01f);
			}
			this.currentQuality = Mathf.Max(0.33f, this.currentQuality);
		}
		float to = (!Clock.InCave) ? 1f : 0.25f;
		this.currentCaveQuality = Mathf.Lerp(this.currentCaveQuality, to, 0.02f);
		this.Bush.Update(this.currentQuality * this.currentCaveQuality);
		this.Plant.Update(this.currentQuality * this.currentCaveQuality);
		this.Rocks.Update(this.currentQuality * this.currentCaveQuality);
		this.Trees.Update(this.currentQuality * this.currentCaveQuality * LOD_Manager.TreeOcclusionBonusRatio);
		this.BigProp.Update(this.currentQuality);
		this.Cave.Update(this.currentQuality);
		this.CaveEntrance.Update(this.currentQuality);
		this.Creature.Update(this.currentQuality);
		this.Enemy.Update(this.currentQuality);
		this.PickUps.Update(this.currentQuality);
	}
}
