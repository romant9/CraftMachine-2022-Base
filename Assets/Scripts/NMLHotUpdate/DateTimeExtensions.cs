using System;

public static class DateTimeExtensions
{
	private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static DateTime FromUnixTimeSeconds(this long seconds)
	{
		DateTime ePOCH = EPOCH;
		return ePOCH.AddSeconds(seconds);
	}

	public static long ToUnixTimeSeconds(this DateTime dateTime)
	{
		return (dateTime.ToUniversalTime() - EPOCH).Ticks / 10000000;
	}
}
