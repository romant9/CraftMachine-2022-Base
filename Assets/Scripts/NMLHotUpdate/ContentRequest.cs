using System;

public class ContentRequest
{
	public float StartTime;

	public string ContentPath;

	public string Content;

	public Action<string, bool> Callback;
}
