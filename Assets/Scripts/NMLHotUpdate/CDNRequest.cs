using System;

public class CDNRequest
{
	public float StartTime;

	public int RetryCount;

	public string Checksum;
}
public class CDNRequest<T> : CDNRequest
{
	public Action<T> Callback;
}
