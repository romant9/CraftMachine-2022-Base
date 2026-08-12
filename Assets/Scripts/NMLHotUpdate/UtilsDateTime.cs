using System;

public static class UtilsDateTime
{
	public static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

	public static readonly long DayInMilliseconds = 86400000L;

	public static readonly long HourInMilliseconds = 3600000L;

	public static readonly long MinuteInMilliseconds = 60000L;

	public static DateTime Floor(this DateTime dateTime, TimeSpan interval)
	{
		return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
	}

	public static DateTime Ceiling(this DateTime dateTime, TimeSpan interval)
	{
		long num = ((interval.Ticks == 0L) ? 0 : (dateTime.Ticks % interval.Ticks));
		if (num != 0L)
		{
			return dateTime.AddTicks(interval.Ticks - num);
		}
		return dateTime;
	}

	public static DateTime Round(this DateTime dateTime, TimeSpan interval)
	{
		long num = interval.Ticks + 1 >> 1;
		return dateTime.AddTicks(num - (dateTime.Ticks + num) % interval.Ticks);
	}

	public static long TotalMilliseconds(this DateTime dateTime)
	{
		return (long)(dateTime - Origin).TotalMilliseconds;
	}

	public static DateTime MillisecondsToDateTime(long milliseconds)
	{
		return Origin + TimeSpan.FromMilliseconds(milliseconds);
	}
}
