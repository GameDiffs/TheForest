using System;

namespace TheForest.Utils
{
	public static class DateEx
	{
		public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
		{
			DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			result = result.AddSeconds((double)unixTimeStamp).ToLocalTime();
			return result;
		}

		public static long ToUnixTimestamp(this DateTime dateTime)
		{
			DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64((dateTime - dateTime2.ToLocalTime()).TotalSeconds);
		}
	}
}
