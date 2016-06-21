using FMOD.Studio;
using System;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class FirstPersonHeadBob : MonoBehaviour
{
	private const float SNOW_MAXIMUM_Z = -300f;

	[SerializeField]
	private Transform head;

	[SerializeField]
	private float headBobFrequency = 1.5f;

	[SerializeField]
	private float headBobHeight;

	[SerializeField]
	private float headBobSwayAngle = 0.5f;

	[SerializeField]
	private float headBobSideMovement = 0.05f;

	[SerializeField]
	private float bobHeightSpeedMultiplier = 0.3f;

	[SerializeField]
	private float bobStrideSpeedLengthen = 0.3f;

	[SerializeField]
	private float jumpLandMove = 3f;

	[SerializeField]
	private float jumpLandTilt = 60f;

	[Header("FMOD Event Paths"), SerializeField]
	private string footstepDefault;

	[SerializeField]
	private string footstepSand;

	[SerializeField]
	private string footstepMud;

	[SerializeField]
	private string footstepLeaves;

	[SerializeField]
	private string footstepRock;

	[SerializeField]
	private string footstepSnow;

	[SerializeField]
	private string footstepWood;

	[SerializeField]
	private string footstepMetal;

	[SerializeField]
	private string footstepCarpet;

	[SerializeField]
	private string jump;

	[SerializeField]
	private string landDefault;

	[SerializeField]
	private string landSand;

	[SerializeField]
	private string landMud;

	[SerializeField]
	private string landWater;

	[SerializeField]
	private string landSnow;

	[SerializeField]
	private string landVocals;

	[SerializeField, Space(10f), Tooltip("Air time required for landing SFX to be played")]
	private float airTimeThreshold = 0.4f;

	public bool isSnow;

	private Vector3 originalLocalPos;

	private float nextStepTime = 0.5f;

	private float headBobCycle;

	private float headBobFade;

	private float springPos;

	private float springVelocity;

	private float springElastic = 1.1f;

	private float springDampen = 0.8f;

	private float springVelocityThreshold = 0.05f;

	private float springPositionThreshold = 0.05f;

	[SerializeThis, Space(10f)]
	public int Steps;

	private Vector3 prevPosition;

	private Vector3 prevVelocity = Vector3.zero;

	private bool prevGrounded = true;

	private float airborneStartTime;

	private UnderfootSurfaceDetector surfaceDetector;

	private float snowStartHeight;

	private float snowFadeLength;

	private static bool isQuitting;

	private string[] allEventPaths()
	{
		return new string[]
		{
			this.footstepDefault,
			this.footstepSand,
			this.footstepMud,
			this.footstepLeaves,
			this.footstepRock,
			this.jump,
			this.landDefault,
			this.landSand,
			this.landMud,
			this.landWater,
			this.landSnow
		};
	}

	private void preloadFootsteps()
	{
		if (FMOD_StudioSystem.instance)
		{
			string[] array = this.allEventPaths();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Length > 0)
				{
					EventDescription eventDescription;
					UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getEvent(text, out eventDescription));
					if (eventDescription != null)
					{
						UnityUtil.ERRCHECK(eventDescription.loadSampleData());
					}
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		FirstPersonHeadBob.isQuitting = true;
	}

	private void OnDestroy()
	{
		if (!FirstPersonHeadBob.isQuitting)
		{
			string[] array = this.allEventPaths();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Length > 0)
				{
					EventDescription eventDescription;
					UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getEvent(text, out eventDescription));
					UnityUtil.ERRCHECK(eventDescription.unloadSampleData());
				}
			}
		}
	}

	private void Start()
	{
		this.preloadFootsteps();
		this.originalLocalPos = this.head.localPosition;
		this.prevPosition = base.GetComponent<Rigidbody>().position;
		this.surfaceDetector = base.GetComponentInChildren<UnderfootSurfaceDetector>();
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain && activeTerrain.materialTemplate)
		{
			this.snowStartHeight = activeTerrain.materialTemplate.GetFloat("_SnowStartHeight");
			this.snowFadeLength = activeTerrain.materialTemplate.GetFloat("_SnowFadeLength");
			this.snowStartHeight += this.snowFadeLength / 4f;
			this.snowFadeLength /= 2f;
		}
	}

	public string GetFootstepForPosition()
	{
		this.isSnow = false;
		switch (this.surfaceDetector.Surface)
		{
		case UnderfootSurfaceDetector.SurfaceType.None:
			if (this.IsOnSnow())
			{
				this.isSnow = true;
				return this.footstepSnow;
			}
			switch (TerrainHelper.GetProminantTextureIndex(base.transform.position))
			{
			case 0:
				return this.footstepDefault;
			case 1:
				return this.footstepMud;
			case 2:
			case 3:
			case 5:
			case 7:
				return this.footstepRock;
			case 4:
				return this.footstepSand;
			case 6:
				return this.footstepLeaves;
			default:
				return this.footstepDefault;
			}
			break;
		case UnderfootSurfaceDetector.SurfaceType.Wood:
			return this.footstepWood;
		case UnderfootSurfaceDetector.SurfaceType.Rock:
			return this.footstepRock;
		case UnderfootSurfaceDetector.SurfaceType.Carpet:
			return this.footstepCarpet;
		case UnderfootSurfaceDetector.SurfaceType.Dirt:
			return this.footstepDefault;
		case UnderfootSurfaceDetector.SurfaceType.Metal:
			return this.footstepMetal;
		default:
			return this.footstepDefault;
		}
	}

	private string GetLandEventForPosition()
	{
		if (this.IsOnSnow())
		{
			this.isSnow = true;
			return this.landSnow;
		}
		if (this.surfaceDetector.Surface != UnderfootSurfaceDetector.SurfaceType.None)
		{
			return this.landDefault;
		}
		if (LocalPlayer.FpCharacter.IsInWater())
		{
			return this.landWater;
		}
		switch (TerrainHelper.GetProminantTextureIndex(base.transform.position))
		{
		case 1:
			return this.landMud;
		case 4:
			return this.landSand;
		}
		return this.landDefault;
	}

	private void FixedUpdate()
	{
		if (!LocalPlayer.FpCharacter.Locked)
		{
			Vector3 a = (base.GetComponent<Rigidbody>().position - this.prevPosition) / Time.deltaTime;
			Vector3 vector = a - this.prevVelocity;
			this.prevPosition = base.GetComponent<Rigidbody>().position;
			this.prevVelocity = a;
			this.springVelocity -= vector.y;
			this.springVelocity -= this.springPos * this.springElastic;
			this.springVelocity *= this.springDampen;
			this.springPos += this.springVelocity * Time.deltaTime;
			this.springPos = Mathf.Clamp(this.springPos, -0.3f, 0.3f);
			if (Mathf.Abs(this.springVelocity) < this.springVelocityThreshold && Mathf.Abs(this.springPos) < this.springPositionThreshold)
			{
				this.springVelocity = 0f;
				this.springPos = 0f;
			}
			Vector3 vector2 = new Vector3(a.x, 0f, a.z);
			float magnitude = vector2.magnitude;
			float num = 1f + magnitude * this.bobStrideSpeedLengthen;
			this.headBobCycle += magnitude / num * (Time.deltaTime / this.headBobFrequency);
			float num2 = Mathf.Sin(this.headBobCycle * 3.14159274f * 2f);
			float num3 = Mathf.Sin(this.headBobCycle * 3.14159274f * 2f + 1.57079637f);
			num2 = 1f - (num2 * 0.5f + 1f);
			num2 *= num2;
			Vector3 vector3 = new Vector3(a.x, 0f, a.z);
			if (vector3.magnitude < 0.1f)
			{
				this.headBobFade = Mathf.Lerp(this.headBobFade, 0f, Time.deltaTime);
			}
			else
			{
				this.headBobFade = Mathf.Lerp(this.headBobFade, 1f, Time.deltaTime);
			}
			float num4 = 1f + magnitude * this.bobHeightSpeedMultiplier;
			float num5 = -this.headBobSideMovement * num3;
			float num6 = this.springPos * this.jumpLandMove + num2 * this.headBobHeight * this.headBobFade * num4;
			float num7 = -this.springPos * this.jumpLandTilt;
			float num8 = num3 * this.headBobSwayAngle * this.headBobFade;
			if (LocalPlayer.FpCharacter.Grounded && !LocalPlayer.AnimControl.swimming)
			{
				if (this.prevGrounded)
				{
					if (this.headBobCycle > this.nextStepTime)
					{
						this.Steps++;
						this.nextStepTime = this.headBobCycle + 0.5f;
						string footstepForPosition = this.GetFootstepForPosition();
						EventInstance eventInstance = null;
						if (FMOD_StudioSystem.instance)
						{
							eventInstance = FMOD_StudioSystem.instance.GetEvent(footstepForPosition);
						}
						if (eventInstance != null)
						{
							ATTRIBUTES_3D attributes = base.transform.position.to3DAttributes();
							UnityUtil.ERRCHECK(eventInstance.set3DAttributes(attributes));
							float value = LocalPlayer.FpCharacter.CalculateWaterDepth();
							float value2 = LocalPlayer.FpCharacter.CalculateSpeedParameter(magnitude);
							eventInstance.setParameterValue("depth", value);
							eventInstance.setParameterValue("speed", value2);
							UnityUtil.ERRCHECK(eventInstance.start());
							UnityUtil.ERRCHECK(eventInstance.release());
						}
					}
				}
				else
				{
					float num9 = Time.time - this.airborneStartTime;
					if (this.airborneStartTime > 0f && num9 > this.airTimeThreshold)
					{
						string landEventForPosition = this.GetLandEventForPosition();
						EventInstance eventInstance2 = null;
						if (FMOD_StudioSystem.instance)
						{
							eventInstance2 = FMOD_StudioSystem.instance.GetEvent(landEventForPosition);
						}
						if (eventInstance2 != null)
						{
							eventInstance2.setParameterValue("depth", LocalPlayer.FpCharacter.CalculateWaterDepth());
							UnityUtil.ERRCHECK(eventInstance2.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
							UnityUtil.ERRCHECK(eventInstance2.start());
							UnityUtil.ERRCHECK(eventInstance2.release());
						}
						this.nextStepTime = this.headBobCycle + 0.5f;
						this.PlayLandVocals();
					}
				}
				this.airborneStartTime = 0f;
			}
			else if (this.prevGrounded)
			{
				this.airborneStartTime = Time.time;
			}
			this.prevGrounded = LocalPlayer.FpCharacter.Grounded;
		}
	}

	private void PlayLandVocals()
	{
		float num = LocalPlayer.FpCharacter.PrevVelocity;
		float num2;
		if (num > 28.5f)
		{
			num2 = (num / 28.5f - 1f) * 0.5f + 0.5f;
		}
		else
		{
			num2 = Mathf.Clamp01(num / 28.5f) * 0.45f;
		}
		FMODCommon.PlayOneshot(this.landVocals, base.transform.position, FMODCommon.NetworkRole.Any, new object[]
		{
			"fall",
			num2
		});
	}

	private bool IsOnSnow()
	{
		if (base.transform.position.z < -300f && base.transform.position.y > this.snowStartHeight)
		{
			Terrain activeTerrain = Terrain.activeTerrain;
			if (!activeTerrain || this.snowFadeLength <= 0f)
			{
				return true;
			}
			Vector3 vector = activeTerrain.transform.InverseTransformPoint(base.transform.position);
			TerrainData terrainData = activeTerrain.terrainData;
			Vector2 vector2 = new Vector2(vector.x / terrainData.size.x, vector.z / terrainData.size.z);
			Vector3 interpolatedNormal = terrainData.GetInterpolatedNormal(vector2.x, vector2.y);
			float num = (base.transform.position.y - this.snowStartHeight) / this.snowFadeLength;
			num -= (1f - interpolatedNormal.y * interpolatedNormal.y) * 2f;
			num += 0.5f;
			if (num >= 1f || (num > 0f && UnityEngine.Random.value < num))
			{
				return true;
			}
		}
		return false;
	}
}
