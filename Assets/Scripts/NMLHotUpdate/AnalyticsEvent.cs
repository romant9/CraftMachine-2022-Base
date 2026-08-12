using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using TWDModel;
using UnityEngine;

public class AnalyticsEvent
{
	public const string NameLogEntry = "LogEntry";

	public const string PropertyMessage = "Message";

	private StringBuilder builder = new StringBuilder();

	private int propertyCount;

	private ConcurrentQueue<AnalyticsEvent> queue;

	public string Name { get; private set; }

	public string Message { get; private set; }

	public string HashedId { get; private set; }

	public string SessionToken { get; private set; }

	public string InstallationId { get; private set; }

	private void Clear()
	{
		HashedId = null;
		SessionToken = null;
		InstallationId = null;
		builder.Length = 0;
		propertyCount = 0;
		queue = null;
	}

	public void Init(string name, DateTime time, string hashedId, string sessionToken, string installationId, ConcurrentQueue<AnalyticsEvent> queue)
	{
		Clear();
		Name = name;
		this.queue = queue;
		HashedId = hashedId;
		SessionToken = sessionToken;
		InstallationId = installationId;
		builder.Append("{");
		AddProperty("Name", name);
		AddProperty("Time", time.ToString("o", CultureInfo.InvariantCulture));
		AddProperty("TimeSinceStartup", Time.realtimeSinceStartup);
	}

	public AnalyticsEvent AddProperty(string propertyName, object value)
	{
		if (value == null)
		{
			return this;
		}
		string text = value.ToString().Replace("\"", "\\\"");
		if (propertyName == "Message")
		{
			Message = text;
		}
		if (propertyCount > 0)
		{
			builder.Append(",");
		}
		builder.Append("\"");
		builder.Append(propertyName);
		builder.Append("\":\"");
		builder.Append(text);
		builder.Append("\"");
		propertyCount++;
		return this;
	}

	public string Finish()
	{
		builder.Append("}");
		return builder.ToString();
	}

	public void Send()
	{
		if (HelpersModel.IsOffThinkingAnalytics)
		{
			Clear();
			return;
		}
		if (queue != null)
		{
			queue.Enqueue(this);
		}
	}
}
