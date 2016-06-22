using FMOD.Studio;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Graphics;
using TheForest.Utils;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WaterViz : MonoBehaviour
{
	public Transform WaterLevelSensor;

	public GameObject Bubbles;

	public GameObject LightGO;

	public Light Light1;

	public Light Light2;

	public float LightsDimDuration = 5f;

	public bool InWater;

	public Buoyancy Buoyancy;

	public float Offset;

	public float threshold = 0.1f;

	public float WaterLevel;

	[Header("FMOD")]
	public string UnderwaterEvent;

	public string OceanUnderwaterEvent;

	public float MaximumWaterDepth = 30f;

	public float OceanLevelOffset;

	private FMOD.Studio.EventInstance UnderwaterInstance;

	private ParameterInstance DepthParameter;

	private float intensityL1;

	private float intensityL2;

	private bool lightOffRoutineActive;

	public float ScreenCoverage
	{
		get;
		set;
	}

	private void Awake()
	{
		if (!this.Buoyancy)
		{
			UnityEngine.Debug.LogWarning("Please add Buyancy reference to WaterViz");
		}
		this.Bubbles.SetActive(false);
		this.LightGO.SetActive(false);
		this.intensityL1 = this.Light1.intensity;
		this.intensityL2 = this.Light2.intensity;
		GameObject gameObject = new GameObject("PlayerWaterSensor");
		FollowTarget followTarget = gameObject.AddComponent<FollowTarget>();
		followTarget.target = this.WaterLevelSensor.transform;
		followTarget.offset = Vector3.zero;
		this.WaterLevelSensor = gameObject.transform;
	}

	private float CalculateAdjustedWaterLevel()
	{
		return this.WaterLevel + ((!this.InWater) ? 0f : this.threshold) + ((!this.Buoyancy.IsOcean) ? 0f : this.OceanLevelOffset);
	}

	public float CalculateDepthParameter()
	{
		float num = -0.1f;
		float num2 = this.CalculateAdjustedWaterLevel() - 0.2f;
		if (this.WaterLevelSensor.position.y <= num2)
		{
			num = num2 - this.WaterLevelSensor.position.y;
			if (this.MaximumWaterDepth > 0f)
			{
				num /= this.MaximumWaterDepth;
			}
		}
		return num;
	}

	private void CalculateScreenCoverage()
	{
		Vector3 vector = LocalPlayer.MainCam.ViewportToWorldPoint(new Vector3(0f, 0f, LocalPlayer.MainCam.nearClipPlane));
		Vector3 vector2 = LocalPlayer.MainCam.ViewportToWorldPoint(new Vector3(0f, 1f, LocalPlayer.MainCam.nearClipPlane));
		Vector3 vector3 = LocalPlayer.MainCam.ViewportToWorldPoint(new Vector3(1f, 0f, LocalPlayer.MainCam.nearClipPlane));
		float num = vector.y - this.WaterLevel;
		float num2 = vector2.y - vector.y;
		this.ScreenCoverage = -num / num2;
	}

	private void UpdateUnderwaterEvent()
	{
		if (this.UnderwaterInstance != null && this.UnderwaterInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.UnderwaterInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			if (this.DepthParameter != null && this.DepthParameter.isValid())
			{
				UnityUtil.ERRCHECK(this.DepthParameter.setValue(this.CalculateDepthParameter()));
			}
		}
	}

	private void Update()
	{
		if (base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			this.UpdateUnderwaterEvent();
		}
		if (this.Buoyancy && this.Buoyancy.InWater)
		{
			bool inWater = false;
			if (this.Buoyancy.IsOcean && WaterEngine.Ocean)
			{
				this.WaterLevel = WaterEngine.Ocean.HeightAt(this.WaterLevelSensor.position);
			}
			else if (this.Buoyancy.WaterCollider)
			{
				this.WaterLevel = this.Buoyancy.WaterLevel;
				if (!this.Buoyancy.IsOcean)
				{
					MeshFilter component = this.Buoyancy.WaterCollider.transform.parent.GetComponent<MeshFilter>();
					if (component)
					{
						Vector3 position = base.transform.position;
						Vector3 position2 = position + base.transform.forward * 2f;
						Vector3 center = this.Buoyancy.WaterCollider.transform.parent.InverseTransformPoint(position2);
						Mesh sharedMesh = component.sharedMesh;
						sharedMesh.bounds = new Bounds(center, sharedMesh.bounds.size);
					}
				}
			}
			else
			{
				this.WaterLevel = -10000f;
			}
			if (this.WaterLevel < this.Buoyancy.WaterLevel && this.WaterLevel > -10000f)
			{
				this.WaterLevel = this.Buoyancy.WaterLevel;
			}
			this.WaterLevel -= this.Offset;
			if (this.WaterLevelSensor.position.y <= this.CalculateAdjustedWaterLevel())
			{
				this.CalculateScreenCoverage();
				inWater = true;
				if (this.UnderwaterInstance == null && FMOD_StudioSystem.instance)
				{
					this.UnderwaterInstance = FMOD_StudioSystem.instance.GetEvent((!this.Buoyancy.IsOcean) ? this.UnderwaterEvent : this.OceanUnderwaterEvent);
					UnityUtil.ERRCHECK(this.UnderwaterInstance.getParameter("depth", out this.DepthParameter));
					this.UpdateUnderwaterEvent();
					UnityUtil.ERRCHECK(this.UnderwaterInstance.start());
				}
				this.TurnOnRebreatherLights();
				if (this.ScreenCoverage > 0.8f)
				{
					this.Bubbles.SetActive(true);
				}
				else
				{
					this.Bubbles.SetActive(false);
				}
			}
			else
			{
				this.AudioOff();
				this.DepthParameter = null;
				this.ScreenCoverage = 0f;
			}
			this.InWater = inWater;
		}
		else if (this.InWater)
		{
			this.AudioOff();
			this.InWater = false;
			this.ScreenCoverage = 0f;
		}
	}

	public void AudioOff()
	{
		if (this.UnderwaterInstance != null && this.UnderwaterInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.UnderwaterInstance.setParameterValue("stop", 1f));
			UnityUtil.ERRCHECK(this.UnderwaterInstance.release());
		}
		this.UnderwaterInstance = null;
		this.Bubbles.SetActive(false);
		if (!this.lightOffRoutineActive && base.gameObject.activeInHierarchy && this.LightGO.activeSelf)
		{
			base.StartCoroutine("OutOfWaterTurnOffLights");
		}
	}

	private void TurnOnRebreatherLights()
	{
		if (!this.LightGO.activeSelf || this.lightOffRoutineActive)
		{
			base.StopCoroutine("OutOfWaterTurnOffLights");
			this.Light1.intensity = this.intensityL1;
			this.Light2.intensity = this.intensityL2;
			this.LightGO.SetActive(true);
			this.lightOffRoutineActive = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator OutOfWaterTurnOffLights()
	{
		WaterViz.<OutOfWaterTurnOffLights>c__Iterator1C5 <OutOfWaterTurnOffLights>c__Iterator1C = new WaterViz.<OutOfWaterTurnOffLights>c__Iterator1C5();
		<OutOfWaterTurnOffLights>c__Iterator1C.<>f__this = this;
		return <OutOfWaterTurnOffLights>c__Iterator1C;
	}

	private void OnDestroy()
	{
		this.AudioOff();
		if (this.WaterLevelSensor)
		{
			UnityEngine.Object.Destroy(this.WaterLevelSensor.gameObject);
		}
	}
}
