using System;
using System.Text;

namespace DigitalOpus.MB.Core
{
	public class ObjectLog
	{
		private int pos;

		private string[] logMessages;

		public ObjectLog(short bufferSize)
		{
			this.logMessages = new string[(int)bufferSize];
		}

		private void _CacheLogMessage(string msg)
		{
			if (this.logMessages.Length == 0)
			{
				return;
			}
			this.logMessages[this.pos] = msg;
			this.pos++;
			if (this.pos >= this.logMessages.Length)
			{
				this.pos = 0;
			}
		}

		public void Log(MB2_LogLevel l, string msg, MB2_LogLevel currentThreshold)
		{
			MB2_Log.Log(l, msg, currentThreshold);
			this._CacheLogMessage(msg);
		}

		public void Error(string msg, params object[] args)
		{
			this._CacheLogMessage(MB2_Log.Error(msg, args));
		}

		public void Warn(string msg, params object[] args)
		{
			this._CacheLogMessage(MB2_Log.Warn(msg, args));
		}

		public void Info(string msg, params object[] args)
		{
			this._CacheLogMessage(MB2_Log.Info(msg, args));
		}

		public void LogDebug(string msg, params object[] args)
		{
			this._CacheLogMessage(MB2_Log.LogDebug(msg, args));
		}

		public void Trace(string msg, params object[] args)
		{
			this._CacheLogMessage(MB2_Log.Trace(msg, args));
		}

		public string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			if (this.logMessages[this.logMessages.Length - 1] != null)
			{
				num = this.pos;
			}
			for (int i = 0; i < this.logMessages.Length; i++)
			{
				int num2 = (num + i) % this.logMessages.Length;
				if (this.logMessages[num2] == null)
				{
					break;
				}
				stringBuilder.AppendLine(this.logMessages[num2]);
			}
			return stringBuilder.ToString();
		}
	}
}
