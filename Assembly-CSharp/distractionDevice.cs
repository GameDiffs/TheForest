using Bolt;
using FMOD.Studio;
using System;
using TheForest.Items;
using TheForest.Utils;

public class distractionDevice : EntityBehaviour<IDistractionDevice>
{
	[ItemIdPicker]
	public int _itemId;

	public Item.EquipmentSlot _slot;

	private EventInstance MusicEvent;

	private PlayerSfx playerSfx;

	public static bool IsActive
	{
		get;
		private set;
	}

	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
		base.state.AddCallback("MusicTrack", new PropertyCallbackSimple(this.MusicTrackChanged));
	}

	public override void Detached()
	{
		this.DeactivateDevice();
	}

	private void MusicTrackChanged()
	{
		EventInstance eventInstance = LocalPlayer.Sfx.RelinquishMusicTrack();
		if (eventInstance == null)
		{
			eventInstance = LocalPlayer.Sfx.InstantiateMusicTrack(base.state.MusicTrack - 10);
		}
		if (eventInstance != null)
		{
			this.ActivateDevice(eventInstance);
		}
		this.SetPlayerSfx(LocalPlayer.Sfx);
	}

	private void ActivateDevice(EventInstance evt)
	{
		this.MusicEvent = evt;
		if (this.MusicEvent != null)
		{
			distractionDevice.IsActive = true;
			UnityUtil.ERRCHECK(this.MusicEvent.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			this.MusicEvent.setParameterValue("Distraction", 1f);
			PLAYBACK_STATE pLAYBACK_STATE;
			UnityUtil.ERRCHECK(this.MusicEvent.getPlaybackState(out pLAYBACK_STATE));
			if (pLAYBACK_STATE != PLAYBACK_STATE.PLAYING)
			{
				UnityUtil.ERRCHECK(this.MusicEvent.start());
			}
		}
	}

	private void DeactivateDevice()
	{
		if (this.MusicEvent != null)
		{
			if (this.playerSfx != null && !BoltNetwork.isRunning)
			{
				this.MusicEvent.setParameterValue("Distraction", 0f);
				this.playerSfx.SetMusicTrack(this.MusicEvent);
			}
			else
			{
				UnityUtil.ERRCHECK(this.MusicEvent.stop(STOP_MODE.ALLOWFADEOUT));
				UnityUtil.ERRCHECK(this.MusicEvent.release());
			}
			this.MusicEvent = null;
			distractionDevice.IsActive = false;
		}
	}

	private void SetPlayerSfx(PlayerSfx sfx)
	{
		this.playerSfx = sfx;
	}

	private void OnDespawned()
	{
		this.DeactivateDevice();
	}

	private void OnDestroy()
	{
		this.DeactivateDevice();
	}
}
