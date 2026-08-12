using System;

public interface ITimingManager
{
	IDisposable Timer(TimeSpan timeSpan, Action action);

	IDisposable Interval(TimeSpan timeSpan, Action action);
}
