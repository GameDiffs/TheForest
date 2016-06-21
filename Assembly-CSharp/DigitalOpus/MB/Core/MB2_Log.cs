using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB2_Log
	{
		public static void Log(MB2_LogLevel l, string msg, MB2_LogLevel currentThreshold)
		{
			if (l <= currentThreshold)
			{
				if (l == MB2_LogLevel.error)
				{
					Debug.LogError(msg);
				}
				if (l == MB2_LogLevel.warn)
				{
					Debug.LogWarning(string.Format("frm={0} WARN {1}", Time.frameCount, msg));
				}
				if (l == MB2_LogLevel.info)
				{
					Debug.Log(string.Format("frm={0} INFO {1}", Time.frameCount, msg));
				}
				if (l == MB2_LogLevel.debug)
				{
					Debug.Log(string.Format("frm={0} DEBUG {1}", Time.frameCount, msg));
				}
				if (l == MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("frm={0} TRACE {1}", Time.frameCount, msg));
				}
			}
		}

		public static string Error(string msg, params object[] args)
		{
			string arg = string.Format(msg, args);
			string text = string.Format("f={0} ERROR {1}", Time.frameCount, arg);
			Debug.LogError(text);
			return text;
		}

		public static string Warn(string msg, params object[] args)
		{
			string arg = string.Format(msg, args);
			string text = string.Format("f={0} WARN {1}", Time.frameCount, arg);
			Debug.LogWarning(text);
			return text;
		}

		public static string Info(string msg, params object[] args)
		{
			string arg = string.Format(msg, args);
			string text = string.Format("f={0} INFO {1}", Time.frameCount, arg);
			Debug.Log(text);
			return text;
		}

		public static string LogDebug(string msg, params object[] args)
		{
			string arg = string.Format(msg, args);
			string text = string.Format("f={0} DEBUG {1}", Time.frameCount, arg);
			Debug.Log(text);
			return text;
		}

		public static string Trace(string msg, params object[] args)
		{
			string arg = string.Format(msg, args);
			string text = string.Format("f={0} TRACE {1}", Time.frameCount, arg);
			Debug.Log(text);
			return text;
		}
	}
}
