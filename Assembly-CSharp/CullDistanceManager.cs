using System;
using TheForest.Utils;
using UnityEngine;

[ExecuteInEditMode]
public class CullDistanceManager : MonoBehaviour
{
	public bool useDrawDistanceSetting;

	public static CullDistanceManager Instance;

	[HideInInspector]
	public float[] LayerCullDistances = new float[32];

	[HideInInspector]
	public bool[] ScaleLayerCullDistances = new bool[32];

	private float[] layerCullDistances = new float[32];

	public Camera[] Cameras;

	public bool Radial = true;

	private static void AddCullDistanceManager()
	{
		if (GameObject.Find("Cull Distance Manager") == null)
		{
			GameObject gameObject = new GameObject("Cull Distance Manager");
			gameObject.AddComponent<CullDistanceManager>();
		}
	}

	private void Start()
	{
		if (CullDistanceManager.Instance == null)
		{
			CullDistanceManager.Instance = this;
		}
		if (Application.isPlaying && (this.Cameras == null || this.Cameras.Length <= 0))
		{
			this.Cameras = Camera.allCameras;
		}
	}

	private void Update()
	{
		if (this.Cameras == null || this.layerCullDistances == null || LocalPlayer.MainCam == null)
		{
			return;
		}
		if (this.LayerCullDistances == null || this.LayerCullDistances.Length != 32)
		{
			this.layerCullDistances = new float[32];
		}
		if (this.ScaleLayerCullDistances == null || this.ScaleLayerCullDistances.Length != 32)
		{
			this.ScaleLayerCullDistances = new bool[32];
		}
		if (this.useDrawDistanceSetting)
		{
			float drawDistanceRatio = TheForestQualitySettings.UserSettings.DrawDistanceRatio;
			for (int i = 0; i < this.layerCullDistances.Length; i++)
			{
				if (this.ScaleLayerCullDistances[i])
				{
					this.layerCullDistances[i] = (float)((int)(this.LayerCullDistances[i] * drawDistanceRatio));
				}
				else
				{
					this.layerCullDistances[i] = this.LayerCullDistances[i];
				}
				if (i == 11)
				{
					this.layerCullDistances[i] *= LOD_Manager.TreeOcclusionBonusRatio;
				}
			}
		}
		if (Clock.InCave)
		{
			this.layerCullDistances[17] = 0f;
		}
		LocalPlayer.MainCam.layerCullDistances = this.layerCullDistances;
		LocalPlayer.MainCam.layerCullSpherical = this.Radial;
	}
}
