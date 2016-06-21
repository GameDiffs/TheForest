using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Bolt
{
	public static class ConsoleWriter
	{
		private static class PInvoke
		{
			public const int STD_OUTPUT_HANDLE = -11;

			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool AttachConsole(uint dwProcessId);

			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool AllocConsole();

			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool FreeConsole();

			[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr GetStdHandle(int nStdHandle);

			[DllImport("kernel32.dll")]
			public static extern bool SetConsoleTitle(string lpConsoleTitle);
		}

		private static TextWriter realOut;

		public static void Open()
		{
			if (ConsoleWriter.realOut == null)
			{
				ConsoleWriter.realOut = Console.Out;
			}
			if (!ConsoleWriter.PInvoke.AttachConsole(4294967295u))
			{
				ConsoleWriter.PInvoke.AllocConsole();
			}
			try
			{
				IntPtr stdHandle = ConsoleWriter.PInvoke.GetStdHandle(-11);
				FileStream stream = new FileStream(stdHandle, FileAccess.Write);
				Console.SetOut(new StreamWriter(stream, Encoding.ASCII)
				{
					AutoFlush = true
				});
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}

		public static void Close()
		{
			ConsoleWriter.PInvoke.FreeConsole();
			Console.SetOut(ConsoleWriter.realOut);
			ConsoleWriter.realOut = null;
		}
	}
}
