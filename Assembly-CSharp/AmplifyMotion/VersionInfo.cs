using System;
using UnityEngine;

namespace AmplifyMotion
{
	[Serializable]
	public class VersionInfo
	{
		public const byte Major = 1;

		public const byte Minor = 7;

		public const byte Release = 1;

		private static string StageSuffix = "_dev001";

		private static string TrialSuffix = string.Empty;

		[SerializeField]
		private int m_major;

		[SerializeField]
		private int m_minor;

		[SerializeField]
		private int m_release;

		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		private VersionInfo()
		{
			this.m_major = 1;
			this.m_minor = 7;
			this.m_release = 1;
		}

		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 1, 7, 1) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		public static VersionInfo Current()
		{
			return new VersionInfo(1, 7, 1);
		}

		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 7 && 1 == version.m_release;
		}
	}
}
