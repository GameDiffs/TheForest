using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Pathfinding
{
	public class AstarProfiler
	{
		public class ProfilePoint
		{
			public Stopwatch watch = new Stopwatch();

			public int totalCalls;

			public long tmpBytes;

			public long totalBytes;
		}

		private static Dictionary<string, AstarProfiler.ProfilePoint> profiles = new Dictionary<string, AstarProfiler.ProfilePoint>();

		private static DateTime startTime = DateTime.UtcNow;

		public static AstarProfiler.ProfilePoint[] fastProfiles;

		public static string[] fastProfileNames;

		private AstarProfiler()
		{
		}

		[Conditional("ProfileAstar")]
		public static void InitializeFastProfile(string[] profileNames)
		{
			AstarProfiler.fastProfileNames = new string[profileNames.Length + 2];
			Array.Copy(profileNames, AstarProfiler.fastProfileNames, profileNames.Length);
			AstarProfiler.fastProfileNames[AstarProfiler.fastProfileNames.Length - 2] = "__Control1__";
			AstarProfiler.fastProfileNames[AstarProfiler.fastProfileNames.Length - 1] = "__Control2__";
			AstarProfiler.fastProfiles = new AstarProfiler.ProfilePoint[AstarProfiler.fastProfileNames.Length];
			for (int i = 0; i < AstarProfiler.fastProfiles.Length; i++)
			{
				AstarProfiler.fastProfiles[i] = new AstarProfiler.ProfilePoint();
			}
		}

		[Conditional("ProfileAstar")]
		public static void StartFastProfile(int tag)
		{
			AstarProfiler.fastProfiles[tag].watch.Start();
		}

		[Conditional("ProfileAstar")]
		public static void EndFastProfile(int tag)
		{
			AstarProfiler.ProfilePoint profilePoint = AstarProfiler.fastProfiles[tag];
			profilePoint.totalCalls++;
			profilePoint.watch.Stop();
		}

		[Conditional("ASTAR_UNITY_PRO_PROFILER")]
		public static void EndProfile()
		{
		}

		[Conditional("ProfileAstar")]
		public static void StartProfile(string tag)
		{
			AstarProfiler.ProfilePoint profilePoint;
			AstarProfiler.profiles.TryGetValue(tag, out profilePoint);
			if (profilePoint == null)
			{
				profilePoint = new AstarProfiler.ProfilePoint();
				AstarProfiler.profiles[tag] = profilePoint;
			}
			profilePoint.tmpBytes = GC.GetTotalMemory(false);
			profilePoint.watch.Start();
		}

		[Conditional("ProfileAstar")]
		public static void EndProfile(string tag)
		{
			if (!AstarProfiler.profiles.ContainsKey(tag))
			{
				UnityEngine.Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
				return;
			}
			AstarProfiler.ProfilePoint profilePoint = AstarProfiler.profiles[tag];
			profilePoint.totalCalls++;
			profilePoint.watch.Stop();
			profilePoint.totalBytes += GC.GetTotalMemory(false) - profilePoint.tmpBytes;
		}

		[Conditional("ProfileAstar")]
		public static void Reset()
		{
			AstarProfiler.profiles.Clear();
			AstarProfiler.startTime = DateTime.UtcNow;
			if (AstarProfiler.fastProfiles != null)
			{
				for (int i = 0; i < AstarProfiler.fastProfiles.Length; i++)
				{
					AstarProfiler.fastProfiles[i] = new AstarProfiler.ProfilePoint();
				}
			}
		}

		[Conditional("ProfileAstar")]
		public static void PrintFastResults()
		{
			for (int i = 0; i < 1000; i++)
			{
			}
			double num = AstarProfiler.fastProfiles[AstarProfiler.fastProfiles.Length - 2].watch.Elapsed.TotalMilliseconds / 1000.0;
			TimeSpan timeSpan = DateTime.UtcNow - AstarProfiler.startTime;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("============================\n\t\t\t\tProfile results:\n============================\n");
			stringBuilder.Append("Name\t\t|\tTotal Time\t|\tTotal Calls\t|\tAvg/Call\t|\tBytes");
			for (int j = 0; j < AstarProfiler.fastProfiles.Length; j++)
			{
				string text = AstarProfiler.fastProfileNames[j];
				AstarProfiler.ProfilePoint profilePoint = AstarProfiler.fastProfiles[j];
				int totalCalls = profilePoint.totalCalls;
				double num2 = profilePoint.watch.Elapsed.TotalMilliseconds - num * (double)totalCalls;
				if (totalCalls >= 1)
				{
					stringBuilder.Append("\n").Append(text.PadLeft(10)).Append("|   ");
					stringBuilder.Append(num2.ToString("0.0 ").PadLeft(10)).Append(profilePoint.watch.Elapsed.TotalMilliseconds.ToString("(0.0)").PadLeft(10)).Append("|   ");
					stringBuilder.Append(totalCalls.ToString().PadLeft(10)).Append("|   ");
					stringBuilder.Append((num2 / (double)totalCalls).ToString("0.000").PadLeft(10));
				}
			}
			stringBuilder.Append("\n\n============================\n\t\tTotal runtime: ");
			stringBuilder.Append(timeSpan.TotalSeconds.ToString("F3"));
			stringBuilder.Append(" seconds\n============================");
			UnityEngine.Debug.Log(stringBuilder.ToString());
		}

		[Conditional("ProfileAstar")]
		public static void PrintResults()
		{
			TimeSpan timeSpan = DateTime.UtcNow - AstarProfiler.startTime;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("============================\n\t\t\t\tProfile results:\n============================\n");
			int num = 5;
			foreach (KeyValuePair<string, AstarProfiler.ProfilePoint> current in AstarProfiler.profiles)
			{
				num = Math.Max(current.Key.Length, num);
			}
			stringBuilder.Append(" Name ".PadRight(num)).Append("|").Append(" Total Time\t".PadRight(20)).Append("|").Append(" Total Calls ".PadRight(20)).Append("|").Append(" Avg/Call ".PadRight(20));
			foreach (KeyValuePair<string, AstarProfiler.ProfilePoint> current2 in AstarProfiler.profiles)
			{
				double totalMilliseconds = current2.Value.watch.Elapsed.TotalMilliseconds;
				int totalCalls = current2.Value.totalCalls;
				if (totalCalls >= 1)
				{
					string key = current2.Key;
					stringBuilder.Append("\n").Append(key.PadRight(num)).Append("| ");
					stringBuilder.Append(totalMilliseconds.ToString("0.0").PadRight(20)).Append("| ");
					stringBuilder.Append(totalCalls.ToString().PadRight(20)).Append("| ");
					stringBuilder.Append((totalMilliseconds / (double)totalCalls).ToString("0.000").PadRight(20));
					stringBuilder.Append(AstarMath.FormatBytesBinary((int)current2.Value.totalBytes).PadLeft(10));
				}
			}
			stringBuilder.Append("\n\n============================\n\t\tTotal runtime: ");
			stringBuilder.Append(timeSpan.TotalSeconds.ToString("F3"));
			stringBuilder.Append(" seconds\n============================");
			UnityEngine.Debug.Log(stringBuilder.ToString());
		}
	}
}
