using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class ChannelControl : HandleBase
	{
		protected ChannelControl(IntPtr raw) : base(raw)
		{
		}

		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = ChannelControl.FMOD5_ChannelGroup_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		public RESULT stop()
		{
			return ChannelControl.FMOD5_ChannelGroup_Stop(this.rawPtr);
		}

		public RESULT setPaused(bool paused)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetPaused(this.rawPtr, paused);
		}

		public RESULT getPaused(out bool paused)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetPaused(this.rawPtr, out paused);
		}

		public RESULT setVolume(float volume)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetVolume(this.rawPtr, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetVolume(this.rawPtr, out volume);
		}

		public RESULT setVolumeRamp(bool ramp)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetVolumeRamp(this.rawPtr, ramp);
		}

		public RESULT getVolumeRamp(out bool ramp)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetVolumeRamp(this.rawPtr, out ramp);
		}

		public RESULT getAudibility(out float audibility)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetAudibility(this.rawPtr, out audibility);
		}

		public RESULT setPitch(float pitch)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetPitch(this.rawPtr, pitch);
		}

		public RESULT getPitch(out float pitch)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetPitch(this.rawPtr, out pitch);
		}

		public RESULT setMute(bool mute)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetMute(this.rawPtr, mute);
		}

		public RESULT getMute(out bool mute)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetMute(this.rawPtr, out mute);
		}

		public RESULT setReverbProperties(int instance, float wet)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetReverbProperties(this.rawPtr, instance, wet);
		}

		public RESULT getReverbProperties(int instance, out float wet)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetReverbProperties(this.rawPtr, instance, out wet);
		}

		public RESULT setLowPassGain(float gain)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetLowPassGain(this.rawPtr, gain);
		}

		public RESULT getLowPassGain(out float gain)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetLowPassGain(this.rawPtr, out gain);
		}

		public RESULT setMode(MODE mode)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetMode(this.rawPtr, mode);
		}

		public RESULT getMode(out MODE mode)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetMode(this.rawPtr, out mode);
		}

		public RESULT setCallback(CHANNEL_CALLBACK callback)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetCallback(this.rawPtr, callback);
		}

		public RESULT isPlaying(out bool isplaying)
		{
			return ChannelControl.FMOD5_ChannelGroup_IsPlaying(this.rawPtr, out isplaying);
		}

		public RESULT setPan(float pan)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetPan(this.rawPtr, pan);
		}

		public RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetMixLevelsOutput(this.rawPtr, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
		}

		public RESULT setMixLevelsInput(float[] levels, int numlevels)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetMixLevelsInput(this.rawPtr, levels, numlevels);
		}

		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetMixMatrix(this.rawPtr, matrix, outchannels, inchannels, inchannel_hop);
		}

		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetMixMatrix(this.rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		public RESULT getDSPClock(out ulong dspclock, out ulong parentclock)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetDSPClock(this.rawPtr, out dspclock, out parentclock);
		}

		public RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetDelay(this.rawPtr, dspclock_start, dspclock_end, stopchannels);
		}

		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetDelay(this.rawPtr, out dspclock_start, out dspclock_end, out stopchannels);
		}

		public RESULT addFadePoint(ulong dspclock, float volume)
		{
			return ChannelControl.FMOD5_ChannelGroup_AddFadePoint(this.rawPtr, dspclock, volume);
		}

		public RESULT setFadePointRamp(ulong dspclock, float volume)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetFadePointRamp(this.rawPtr, dspclock, volume);
		}

		public RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end)
		{
			return ChannelControl.FMOD5_ChannelGroup_RemoveFadePoints(this.rawPtr, dspclock_start, dspclock_end);
		}

		public RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetFadePoints(this.rawPtr, ref numpoints, point_dspclock, point_volume);
		}

		public RESULT getDSP(int index, out DSP dsp)
		{
			dsp = null;
			IntPtr raw;
			RESULT result = ChannelControl.FMOD5_ChannelGroup_GetDSP(this.rawPtr, index, out raw);
			dsp = new DSP(raw);
			return result;
		}

		public RESULT addDSP(int index, DSP dsp)
		{
			return ChannelControl.FMOD5_ChannelGroup_AddDSP(this.rawPtr, index, dsp.getRaw());
		}

		public RESULT removeDSP(DSP dsp)
		{
			return ChannelControl.FMOD5_ChannelGroup_RemoveDSP(this.rawPtr, dsp.getRaw());
		}

		public RESULT getNumDSPs(out int numdsps)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetNumDSPs(this.rawPtr, out numdsps);
		}

		public RESULT setDSPIndex(DSP dsp, int index)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetDSPIndex(this.rawPtr, dsp.getRaw(), index);
		}

		public RESULT getDSPIndex(DSP dsp, out int index)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetDSPIndex(this.rawPtr, dsp.getRaw(), out index);
		}

		public RESULT overridePanDSP(DSP pan)
		{
			return ChannelControl.FMOD5_ChannelGroup_OverridePanDSP(this.rawPtr, pan.getRaw());
		}

		public RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DAttributes(this.rawPtr, ref pos, ref vel, ref alt_pan_pos);
		}

		public RESULT get3DAttributes(out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DAttributes(this.rawPtr, out pos, out vel, out alt_pan_pos);
		}

		public RESULT set3DMinMaxDistance(float mindistance, float maxdistance)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DMinMaxDistance(this.rawPtr, mindistance, maxdistance);
		}

		public RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DMinMaxDistance(this.rawPtr, out mindistance, out maxdistance);
		}

		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DConeSettings(this.rawPtr, insideconeangle, outsideconeangle, outsidevolume);
		}

		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DConeSettings(this.rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		public RESULT set3DConeOrientation(ref VECTOR orientation)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DConeOrientation(this.rawPtr, ref orientation);
		}

		public RESULT get3DConeOrientation(out VECTOR orientation)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DConeOrientation(this.rawPtr, out orientation);
		}

		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DCustomRolloff(this.rawPtr, ref points, numpoints);
		}

		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DCustomRolloff(this.rawPtr, out points, out numpoints);
		}

		public RESULT set3DOcclusion(float directocclusion, float reverbocclusion)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DOcclusion(this.rawPtr, directocclusion, reverbocclusion);
		}

		public RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DOcclusion(this.rawPtr, out directocclusion, out reverbocclusion);
		}

		public RESULT set3DSpread(float angle)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DSpread(this.rawPtr, angle);
		}

		public RESULT get3DSpread(out float angle)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DSpread(this.rawPtr, out angle);
		}

		public RESULT set3DLevel(float level)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DLevel(this.rawPtr, level);
		}

		public RESULT get3DLevel(out float level)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DLevel(this.rawPtr, out level);
		}

		public RESULT set3DDopplerLevel(float level)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DDopplerLevel(this.rawPtr, level);
		}

		public RESULT get3DDopplerLevel(out float level)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DDopplerLevel(this.rawPtr, out level);
		}

		public RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
		{
			return ChannelControl.FMOD5_ChannelGroup_Set3DDistanceFilter(this.rawPtr, custom, customLevel, centerFreq);
		}

		public RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
		{
			return ChannelControl.FMOD5_ChannelGroup_Get3DDistanceFilter(this.rawPtr, out custom, out customLevel, out centerFreq);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return ChannelControl.FMOD5_ChannelGroup_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return ChannelControl.FMOD5_ChannelGroup_GetUserData(this.rawPtr, out userdata);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Stop(IntPtr channelgroup);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetMute(IntPtr channelgroup, bool mute);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetMode(IntPtr channelgroup, MODE mode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetMode(IntPtr channelgroup, out MODE mode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetCallback(IntPtr channelgroup, CHANNEL_CALLBACK callback);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetPan(IntPtr channelgroup, float pan);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DAttributes(IntPtr channelgroup, out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref VECTOR orientation);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out VECTOR orientation);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref VECTOR points, int numpoints);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetVolume(IntPtr channelgroup, float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_OverridePanDSP(IntPtr channelgroup, IntPtr pan);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata);
	}
}
