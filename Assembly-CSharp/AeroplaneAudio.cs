using System;
using UnityEngine;

public class AeroplaneAudio : MonoBehaviour
{
	[Serializable]
	public class AdvancedSetttings
	{
		public float engineMinDistance = 50f;

		public float engineMaxDistance = 1000f;

		public float engineDopplerLevel = 1f;

		[Range(0f, 1f)]
		public float engineMasterVolume = 0.5f;

		public float windMinDistance = 10f;

		public float windMaxDistance = 100f;

		public float windDopplerLevel = 1f;

		[Range(0f, 1f)]
		public float windMasterVolume = 0.5f;
	}

	[SerializeField]
	private AudioClip engineSound;

	[SerializeField]
	private float engineMinThrottlePitch = 0.4f;

	[SerializeField]
	private float engineMaxThrottlePitch = 2f;

	[SerializeField]
	private float engineFwdSpeedMultiplier = 0.002f;

	[SerializeField]
	private AudioClip windSound;

	[SerializeField]
	private float windBasePitch = 0.2f;

	[SerializeField]
	private float windSpeedPitchFactor = 0.004f;

	[SerializeField]
	private float windMaxSpeedVolume = 100f;

	private AudioSource engineSoundSource;

	private AudioSource windSoundSource;

	private AeroplaneController plane;

	[SerializeField]
	private AeroplaneAudio.AdvancedSetttings advanced = new AeroplaneAudio.AdvancedSetttings();

	private void Awake()
	{
		this.plane = base.GetComponent<AeroplaneController>();
		this.engineSoundSource = base.gameObject.AddComponent<AudioSource>();
		this.windSoundSource = base.gameObject.AddComponent<AudioSource>();
		this.engineSoundSource.clip = this.engineSound;
		this.windSoundSource.clip = this.windSound;
		this.engineSoundSource.minDistance = this.advanced.engineMinDistance;
		this.engineSoundSource.maxDistance = this.advanced.engineMaxDistance;
		this.engineSoundSource.loop = true;
		this.engineSoundSource.dopplerLevel = this.advanced.engineDopplerLevel;
		this.windSoundSource.minDistance = this.advanced.windMinDistance;
		this.windSoundSource.maxDistance = this.advanced.windMaxDistance;
		this.windSoundSource.loop = true;
		this.windSoundSource.dopplerLevel = this.advanced.windDopplerLevel;
		this.engineSoundSource.Play();
		this.windSoundSource.Play();
	}

	private void Update()
	{
		float t = Mathf.InverseLerp(0f, this.plane.MaxEnginePower, this.plane.EnginePower);
		this.engineSoundSource.pitch = Mathf.Lerp(this.engineMinThrottlePitch, this.engineMaxThrottlePitch, t);
		this.engineSoundSource.pitch += this.plane.ForwardSpeed * this.engineFwdSpeedMultiplier;
		this.engineSoundSource.volume = Mathf.InverseLerp(0f, this.plane.MaxEnginePower * this.advanced.engineMasterVolume, this.plane.EnginePower);
		float magnitude = base.GetComponent<Rigidbody>().velocity.magnitude;
		this.windSoundSource.pitch = this.windBasePitch + magnitude * this.windSpeedPitchFactor;
		this.windSoundSource.volume = Mathf.InverseLerp(0f, this.windMaxSpeedVolume, magnitude) * this.advanced.windMasterVolume;
	}
}
