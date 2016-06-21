using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAudio : MonoBehaviour
{
	public enum EngineAudioOptions
	{
		Simple,
		FourChannel
	}

	public CarAudio.EngineAudioOptions engineSoundStyle = CarAudio.EngineAudioOptions.FourChannel;

	public AudioClip lowAccelClip;

	public AudioClip lowDecelClip;

	public AudioClip highAccelClip;

	public AudioClip highDecelClip;

	public AudioClip skidClip;

	public float pitchMultiplier = 1f;

	public float lowPitchMin = 1f;

	public float lowPitchMax = 6f;

	public float highPitchMultiplier = 0.25f;

	public float maxRolloffDistance = 500f;

	public float dopplerLevel = 1f;

	public bool useDoppler = true;

	private AudioSource lowAccel;

	private AudioSource lowDecel;

	private AudioSource highAccel;

	private AudioSource highDecel;

	private AudioSource skidSource;

	private bool startedSound;

	private CarController carController;

	private void StartSound()
	{
		this.carController = base.GetComponent<CarController>();
		this.highAccel = this.SetUpEngineAudioSource(this.highAccelClip);
		if (this.engineSoundStyle == CarAudio.EngineAudioOptions.FourChannel)
		{
			this.lowAccel = this.SetUpEngineAudioSource(this.lowAccelClip);
			this.lowDecel = this.SetUpEngineAudioSource(this.lowDecelClip);
			this.highDecel = this.SetUpEngineAudioSource(this.highDecelClip);
		}
		this.skidSource = this.SetUpEngineAudioSource(this.skidClip);
		this.startedSound = true;
	}

	private void StopSound()
	{
		AudioSource[] components = base.GetComponents<AudioSource>();
		for (int i = 0; i < components.Length; i++)
		{
			AudioSource obj = components[i];
			UnityEngine.Object.Destroy(obj);
		}
		this.startedSound = false;
	}

	private void Update()
	{
		float sqrMagnitude = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
		if (this.startedSound && sqrMagnitude > this.maxRolloffDistance * this.maxRolloffDistance)
		{
			this.StopSound();
		}
		if (!this.startedSound && sqrMagnitude < this.maxRolloffDistance * this.maxRolloffDistance)
		{
			this.StartSound();
		}
		if (this.startedSound)
		{
			float num = this.ULerp(this.lowPitchMin, this.lowPitchMax, this.carController.RevsFactor);
			num = Mathf.Min(this.lowPitchMax, num);
			if (this.engineSoundStyle == CarAudio.EngineAudioOptions.Simple)
			{
				this.highAccel.pitch = num * this.pitchMultiplier * this.highPitchMultiplier;
				this.highAccel.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
				this.highAccel.volume = 1f;
			}
			else
			{
				this.lowAccel.pitch = num * this.pitchMultiplier;
				this.lowDecel.pitch = num * this.pitchMultiplier;
				this.highAccel.pitch = num * this.highPitchMultiplier * this.pitchMultiplier;
				this.highDecel.pitch = num * this.highPitchMultiplier * this.pitchMultiplier;
				float num2 = Mathf.Abs(this.carController.AccelInput);
				float num3 = 1f - num2;
				float num4 = Mathf.InverseLerp(0.2f, 0.8f, this.carController.RevsFactor);
				float num5 = 1f - num4;
				num4 = 1f - (1f - num4) * (1f - num4);
				num5 = 1f - (1f - num5) * (1f - num5);
				num2 = 1f - (1f - num2) * (1f - num2);
				num3 = 1f - (1f - num3) * (1f - num3);
				this.lowAccel.volume = num5 * num2;
				this.lowDecel.volume = num5 * num3;
				this.highAccel.volume = num4 * num2;
				this.highDecel.volume = num4 * num3;
				this.highAccel.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
				this.lowAccel.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
				this.highDecel.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
				this.lowDecel.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
			}
			this.skidSource.volume = Mathf.Clamp01(this.carController.AvgSkid * 3f - 1f);
			this.skidSource.pitch = Mathf.Lerp(0.8f, 1.3f, this.carController.SpeedFactor);
			this.skidSource.dopplerLevel = ((!this.useDoppler) ? 0f : this.dopplerLevel);
		}
	}

	private AudioSource SetUpEngineAudioSource(AudioClip clip)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0f;
		audioSource.loop = true;
		audioSource.time = UnityEngine.Random.Range(0f, clip.length);
		audioSource.Play();
		audioSource.minDistance = 5f;
		audioSource.maxDistance = this.maxRolloffDistance;
		audioSource.dopplerLevel = 0f;
		return audioSource;
	}

	private float ULerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	private float UInverseLerp(float from, float to, float value)
	{
		return (value - from) / (to - from);
	}
}
