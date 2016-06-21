using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class CoopSteamVoicePlayer : MonoBehaviour
{
	public const int FREQUENCY = 11025;

	public const int SAMPLE_BUFFER_SIZE = 110250;

	public const int BYTE_BUFFER_SIZE = 65536;

	private static Dictionary<int, float[]> buffers = new Dictionary<int, float[]>();

	public static float GLOBAL_VOLUME = 1f;

	private byte[] vc_decomp = new byte[65536];

	private short[] vc_pcm = new short[32768];

	private int samplesPlayed;

	private int samplesReceived;

	private int playbackDelay = -1;

	private int timeSamplesPrev;

	private AudioClip clip;

	[SerializeField]
	private AudioSource audioSource;

	private AudioSource audio
	{
		get
		{
			return this.audioSource;
		}
	}

	public static float[] GetBuffer(int size)
	{
		float[] array;
		if (!CoopSteamVoicePlayer.buffers.TryGetValue(size, out array))
		{
			array = (CoopSteamVoicePlayer.buffers[size] = new float[size]);
		}
		Array.Clear(array, 0, array.Length);
		return array;
	}

	private void Awake()
	{
		this.clip = AudioClip.Create("VoiceChat", 110250, 1, 11025, false, false);
		if (this.audio)
		{
			this.audio.volume = CoopSteamVoicePlayer.GLOBAL_VOLUME;
			this.audio.loop = true;
			this.audio.clip = this.clip;
			this.audio.Stop();
		}
		else
		{
			base.enabled = false;
			Debug.LogError("Missing AudioSource for VoIP");
		}
	}

	public void DataReceived(byte[] data, int size)
	{
		uint num = 0u;
		if (SteamUser.DecompressVoice(data, (uint)size, this.vc_decomp, (uint)this.vc_decomp.Length, out num, 11025u) == EVoiceResult.k_EVoiceResultOK && num > 0u)
		{
			int num2 = (int)(num / 2u);
			if (num2 >= this.vc_pcm.Length)
			{
				Array.Resize<short>(ref this.vc_pcm, num2);
			}
			Array.Clear(this.vc_pcm, 0, this.vc_pcm.Length);
			Buffer.BlockCopy(this.vc_decomp, 0, this.vc_pcm, 0, num2 * 2);
			float a = 2.14748365E+09f;
			float a2 = -2.14748365E+09f;
			uint num3 = 0u;
			while ((ulong)num3 < (ulong)((long)num2))
			{
				a = Mathf.Min(a, (float)this.vc_pcm[(int)((UIntPtr)num3)]);
				a2 = Mathf.Max(a2, (float)this.vc_pcm[(int)((UIntPtr)num3)]);
				num3 += 1u;
			}
			float[] buffer = CoopSteamVoicePlayer.GetBuffer(num2);
			uint num4 = 0u;
			while ((ulong)num4 < (ulong)((long)num2))
			{
				buffer[(int)((UIntPtr)num4)] = Mathf.Clamp((float)this.vc_pcm[(int)((UIntPtr)num4)] / 32767f, -1f, 1f);
				num4 += 1u;
			}
			this.clip.SetData(buffer, this.samplesReceived % 110250);
			this.samplesReceived += num2;
			if (this.audio && !this.audio.isPlaying)
			{
				if (this.playbackDelay > 0)
				{
					if (--this.playbackDelay == 0)
					{
						this.audio.volume = CoopSteamVoicePlayer.GLOBAL_VOLUME;
						this.audio.Play();
					}
				}
				else
				{
					this.playbackDelay = 2;
				}
			}
		}
	}

	private void Update()
	{
		if (this.audio.isPlaying)
		{
			if (this.timeSamplesPrev < this.audio.timeSamples)
			{
				this.samplesPlayed += this.audio.timeSamples - this.timeSamplesPrev;
			}
			else if (this.timeSamplesPrev > this.audio.timeSamples)
			{
				this.samplesPlayed += 110250 - this.timeSamplesPrev;
				this.samplesPlayed += this.audio.timeSamples;
			}
			if (this.samplesPlayed >= this.samplesReceived)
			{
				this.samplesPlayed = 0;
				this.samplesReceived = 0;
				this.timeSamplesPrev = 0;
				this.audio.timeSamples = 0;
				this.audio.volume = 0f;
				this.audio.Pause();
			}
			else
			{
				this.timeSamplesPrev = this.audio.timeSamples;
			}
		}
	}
}
