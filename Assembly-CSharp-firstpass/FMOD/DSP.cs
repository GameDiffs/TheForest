using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public class DSP : HandleBase
	{
		public DSP(IntPtr raw) : base(raw)
		{
		}

		public RESULT release()
		{
			RESULT rESULT = DSP.FMOD5_DSP_Release(base.getRaw());
			if (rESULT == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return rESULT;
		}

		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = DSP.FMOD5_DSP_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		public RESULT addInput(DSP target, out DSPConnection connection, DSPCONNECTION_TYPE type)
		{
			connection = null;
			IntPtr raw;
			RESULT result = DSP.FMOD5_DSP_AddInput(this.rawPtr, target.getRaw(), out raw, type);
			connection = new DSPConnection(raw);
			return result;
		}

		public RESULT disconnectFrom(DSP target, DSPConnection connection)
		{
			return DSP.FMOD5_DSP_DisconnectFrom(this.rawPtr, target.getRaw(), connection.getRaw());
		}

		public RESULT disconnectAll(bool inputs, bool outputs)
		{
			return DSP.FMOD5_DSP_DisconnectAll(this.rawPtr, inputs, outputs);
		}

		public RESULT getNumInputs(out int numinputs)
		{
			return DSP.FMOD5_DSP_GetNumInputs(this.rawPtr, out numinputs);
		}

		public RESULT getNumOutputs(out int numoutputs)
		{
			return DSP.FMOD5_DSP_GetNumOutputs(this.rawPtr, out numoutputs);
		}

		public RESULT getInput(int index, out DSP input, out DSPConnection inputconnection)
		{
			input = null;
			inputconnection = null;
			IntPtr raw;
			IntPtr raw2;
			RESULT result = DSP.FMOD5_DSP_GetInput(this.rawPtr, index, out raw, out raw2);
			input = new DSP(raw);
			inputconnection = new DSPConnection(raw2);
			return result;
		}

		public RESULT getOutput(int index, out DSP output, out DSPConnection outputconnection)
		{
			output = null;
			outputconnection = null;
			IntPtr raw;
			IntPtr raw2;
			RESULT result = DSP.FMOD5_DSP_GetOutput(this.rawPtr, index, out raw, out raw2);
			output = new DSP(raw);
			outputconnection = new DSPConnection(raw2);
			return result;
		}

		public RESULT setActive(bool active)
		{
			return DSP.FMOD5_DSP_SetActive(this.rawPtr, active);
		}

		public RESULT getActive(out bool active)
		{
			return DSP.FMOD5_DSP_GetActive(this.rawPtr, out active);
		}

		public RESULT setBypass(bool bypass)
		{
			return DSP.FMOD5_DSP_SetBypass(this.rawPtr, bypass);
		}

		public RESULT getBypass(out bool bypass)
		{
			return DSP.FMOD5_DSP_GetBypass(this.rawPtr, out bypass);
		}

		public RESULT setWetDryMix(float prewet, float postwet, float dry)
		{
			return DSP.FMOD5_DSP_SetWetDryMix(this.rawPtr, prewet, postwet, dry);
		}

		public RESULT getWetDryMix(out float prewet, out float postwet, out float dry)
		{
			return DSP.FMOD5_DSP_GetWetDryMix(this.rawPtr, out prewet, out postwet, out dry);
		}

		public RESULT setChannelFormat(CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD5_DSP_SetChannelFormat(this.rawPtr, channelmask, numchannels, source_speakermode);
		}

		public RESULT getChannelFormat(out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD5_DSP_GetChannelFormat(this.rawPtr, out channelmask, out numchannels, out source_speakermode);
		}

		public RESULT getOutputChannelFormat(CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode)
		{
			return DSP.FMOD5_DSP_GetOutputChannelFormat(this.rawPtr, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
		}

		public RESULT reset()
		{
			return DSP.FMOD5_DSP_Reset(this.rawPtr);
		}

		public RESULT setParameterFloat(int index, float value)
		{
			return DSP.FMOD5_DSP_SetParameterFloat(this.rawPtr, index, value);
		}

		public RESULT setParameterInt(int index, int value)
		{
			return DSP.FMOD5_DSP_SetParameterInt(this.rawPtr, index, value);
		}

		public RESULT setParameterBool(int index, bool value)
		{
			return DSP.FMOD5_DSP_SetParameterBool(this.rawPtr, index, value);
		}

		public RESULT setParameterData(int index, byte[] data)
		{
			return DSP.FMOD5_DSP_SetParameterData(this.rawPtr, index, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), (uint)data.Length);
		}

		public RESULT getParameterFloat(int index, out float value)
		{
			IntPtr zero = IntPtr.Zero;
			return DSP.FMOD5_DSP_GetParameterFloat(this.rawPtr, index, out value, zero, 0);
		}

		public RESULT getParameterInt(int index, out int value)
		{
			IntPtr zero = IntPtr.Zero;
			return DSP.FMOD5_DSP_GetParameterInt(this.rawPtr, index, out value, zero, 0);
		}

		public RESULT getParameterBool(int index, out bool value)
		{
			return DSP.FMOD5_DSP_GetParameterBool(this.rawPtr, index, out value, IntPtr.Zero, 0);
		}

		public RESULT getParameterData(int index, out IntPtr data, out uint length)
		{
			return DSP.FMOD5_DSP_GetParameterData(this.rawPtr, index, out data, out length, IntPtr.Zero, 0);
		}

		public RESULT getNumParameters(out int numparams)
		{
			return DSP.FMOD5_DSP_GetNumParameters(this.rawPtr, out numparams);
		}

		public RESULT getParameterInfo(int index, out DSP_PARAMETER_DESC desc)
		{
			IntPtr ptr;
			RESULT rESULT = DSP.FMOD5_DSP_GetParameterInfo(this.rawPtr, index, out ptr);
			if (rESULT == RESULT.OK)
			{
				desc = (DSP_PARAMETER_DESC)Marshal.PtrToStructure(ptr, typeof(DSP_PARAMETER_DESC));
			}
			else
			{
				desc = default(DSP_PARAMETER_DESC);
			}
			return rESULT;
		}

		public RESULT getDataParameterIndex(int datatype, out int index)
		{
			return DSP.FMOD5_DSP_GetDataParameterIndex(this.rawPtr, datatype, out index);
		}

		public RESULT showConfigDialog(IntPtr hwnd, bool show)
		{
			return DSP.FMOD5_DSP_ShowConfigDialog(this.rawPtr, hwnd, show);
		}

		public RESULT getInfo(StringBuilder name, out uint version, out int channels, out int configwidth, out int configheight)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(32);
			RESULT result = DSP.FMOD5_DSP_GetInfo(this.rawPtr, intPtr, out version, out channels, out configwidth, out configheight);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getType(out DSP_TYPE type)
		{
			return DSP.FMOD5_DSP_GetType(this.rawPtr, out type);
		}

		public RESULT getIdle(out bool idle)
		{
			return DSP.FMOD5_DSP_GetIdle(this.rawPtr, out idle);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return DSP.FMOD5_DSP_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return DSP.FMOD5_DSP_GetUserData(this.rawPtr, out userdata);
		}

		public RESULT setMeteringEnabled(bool inputEnabled, bool outputEnabled)
		{
			return DSP.FMOD5_DSP_SetMeteringEnabled(this.rawPtr, inputEnabled, outputEnabled);
		}

		public RESULT getMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
		{
			return DSP.FMOD5_DSP_GetMeteringEnabled(this.rawPtr, out inputEnabled, out outputEnabled);
		}

		public RESULT getMeteringInfo(DSP_METERING_INFO inputInfo, DSP_METERING_INFO outputInfo)
		{
			return DSP.FMOD5_DSP_GetMeteringInfo(this.rawPtr, inputInfo, outputInfo);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_Release(IntPtr dsp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetSystemObject(IntPtr dsp, out IntPtr system);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPCONNECTION_TYPE type);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetNumInputs(IntPtr dsp, out int numinputs);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetNumOutputs(IntPtr dsp, out int numoutputs);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetActive(IntPtr dsp, bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetActive(IntPtr dsp, out bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetBypass(IntPtr dsp, bool bypass);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetBypass(IntPtr dsp, out bool bypass);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetChannelFormat(IntPtr dsp, CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetChannelFormat(IntPtr dsp, out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetOutputChannelFormat(IntPtr dsp, CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_Reset(IntPtr dsp);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetParameterFloat(IntPtr dsp, int index, float value);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetParameterInt(IntPtr dsp, int index, int value);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetParameterBool(IntPtr dsp, int index, bool value);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetParameterFloat(IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetParameterInt(IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetParameterBool(IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetNumParameters(IntPtr dsp, out int numparams);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetParameterInfo(IntPtr dsp, int index, out IntPtr desc);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetInfo(IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetType(IntPtr dsp, out DSP_TYPE type);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetIdle(IntPtr dsp, out bool idle);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_SetUserData(IntPtr dsp, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_DSP_GetUserData(IntPtr dsp, out IntPtr userdata);

		[DllImport("fmod")]
		public static extern RESULT FMOD5_DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);

		[DllImport("fmod")]
		public static extern RESULT FMOD5_DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);

		[DllImport("fmod")]
		public static extern RESULT FMOD5_DSP_GetMeteringInfo(IntPtr dsp, [Out] DSP_METERING_INFO inputInfo, [Out] DSP_METERING_INFO outputInfo);
	}
}
