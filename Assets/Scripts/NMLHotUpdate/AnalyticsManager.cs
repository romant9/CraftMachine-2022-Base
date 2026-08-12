using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using BestHTTP;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class AnalyticsManager
{
	private static readonly string rootPath = Application.persistentDataPath + "/Analytics";

	private static readonly string prevAnalyticsEventCounterPath = Application.persistentDataPath + "/Analytics/PrevAnalyticsEventId.txt";

	private static readonly string analyticsBlackListPath = Application.persistentDataPath + "/Analytics/BlackList.txt";

	private const int FileEventCountCutThreshold = 100;

	private const long SendThresholdInMilliseconds = 5000L;

	private const long SplitFileAfterMilliseconds = 10000L;

	private const int MaxAnalyticsContentInBytesToSend = 204800;

	private static AnalyticsManager instanceObj = new AnalyticsManager();

	private string analyticsUrl;

	private string sessionToken;

	private Thread analyticsFileProcessThread;

	private bool killThread;

	private ConcurrentQueue<AnalyticsEvent> eventsToWrite = new ConcurrentQueue<AnalyticsEvent>();

	private List<AnalyticsEvent> cachedEvents = new List<AnalyticsEvent>();

	private ConcurrentQueue<string> workerErrorMessages = new ConcurrentQueue<string>();

	private long networkConnectionPollCounter;

	private static long networkConnectionPollCounterInterval = 500L;

	private bool hasNetworkConnection;

	private ConcurrentQueue<string> successfullySentFiles = new ConcurrentQueue<string>();

	private ConcurrentQueue<string> unsuccessfullySentFiles = new ConcurrentQueue<string>();

	private string blackListString;

	private const string EventsFieldStart = "\"Events\":[";

	private const string EventsFieldEnd = "]}";

	public static AnalyticsManager instance
	{
		get
		{
			if (instanceObj == null)
			{
				instanceObj = new AnalyticsManager();
			}
			return instanceObj;
		}
	}

	public string BlackListString
	{
		set
		{
			blackListString = value ?? "";
		}
	}

	private static bool ValidateSessionToken(string token)
	{
		if (string.IsNullOrEmpty(token))
		{
			return false;
		}
		foreach (char c in token)
		{
			if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9'))
			{
				return false;
			}
		}
		return true;
	}

	private AnalyticsManager()
	{
		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}
		if (HelpersModel.IsOffThinkingAnalytics)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsOffAnalyticsManager) return");
			return;
		}

		networkConnectionPollCounter = 0L;
		UpdateNetworkConnectionStatus();
		if (GameConfiguration.Instance.Config.ConnectedToServer)
		{
			analyticsFileProcessThread = new Thread(ProcessAnalyticsFiles);
			analyticsFileProcessThread.Start();
		}
	}

	public void Deinit()
	{
		killThread = true;
		instanceObj = null;
	}

	private static string StartNewFile(string sessionToken, string hashedId, string installationId)
	{
		if (sessionToken == null)
		{
			sessionToken = "";
		}
		if (hashedId == null)
		{
			hashedId = "";
		}
		if (installationId == null)
		{
			installationId = "";
		}
		string text = null;
		int num = 0;
		do
		{
			text = string.Format("{0}/{1}_{2}.analytics", rootPath, DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture), num);
			if (File.Exists(text))
			{
				num++;
				text = null;
			}
		}
		while (text == null);
		using StreamWriter streamWriter = File.AppendText(text);
		streamWriter.WriteLine(sessionToken);
		streamWriter.WriteLine(hashedId);
		streamWriter.WriteLine(installationId);
		streamWriter.WriteLine("{");
		streamWriter.WriteLine($"\"InstallationId\":\"{installationId}\",");
		streamWriter.WriteLine($"\"SessionToken\":\"{sessionToken}\",");
		streamWriter.WriteLine($"\"HashedId\":\"{hashedId}\",");
		streamWriter.Write("\"Events\":[" + Environment.NewLine);
		streamWriter.Flush();
		return text;
	}

	private static bool IsEmptyFile(string fileContent)
	{
		if (string.IsNullOrEmpty(fileContent))
		{
			return true;
		}
		string value = "\"Events\":[" + Environment.NewLine;
		return fileContent.EndsWith(value);
	}

	private bool SendFile(string url, string analyticsSessionToken, string hashedId, string installationId, string analyticsFilePath, string fileContents)
	{
		try
		{
			if (analyticsSessionToken == null)
			{
				analyticsSessionToken = "";
			}
			return new HTTPRequest(new Uri($"{url}/{analyticsSessionToken}"), isKeepAlive: true, disableCache: true, delegate(HTTPRequest req, HTTPResponse resp)
			{
				if (resp != null && resp.IsSuccess)
				{
					successfullySentFiles.Enqueue(analyticsFilePath);
				}
				else
				{
					unsuccessfullySentFiles.Enqueue(analyticsFilePath);
				}
			})
			{
				MethodType = HTTPMethods.Post,
				RawData = Encoding.UTF8.GetBytes(fileContents)
			}.Send() != null;
		}
		catch (Exception ex)
		{
			workerErrorMessages.Enqueue(string.Format("Exception was thrown on the analytics worker thread, while sending analytics file {0} to URL {1}: {2}", (analyticsFilePath != null) ? analyticsFilePath : "<null>", (url != null) ? url : "<null>", ex.ToString()));
			return false;
		}
	}

	private void ProcessAnalyticsFiles(object arg)
	{
		string text = null;
		string text2 = null;
		string text3 = null;
		string text4 = null;
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		DateTime dateTime = DateTime.Now;
		TimeSpan zero = TimeSpan.Zero;
		TimeSpan zero2 = TimeSpan.Zero;
		long num2 = 0L;
		long num3 = 10L;
		TimeSpan timeSpan = new TimeSpan(0, 1, 0);
		DateTime dateTime2 = DateTime.Now;
		string text5 = null;
		Dictionary<string, List<string>> dictionary = null;
		try
		{
			if (File.Exists(analyticsBlackListPath))
			{
				text5 = File.ReadAllText(analyticsBlackListPath);
			}
		}
		catch (Exception ex)
		{
			workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread while reading blacklist file {analyticsBlackListPath}: {ex.ToString()}");
		}
		if (text5 == null)
		{
			text5 = "";
		}
		List<string> list = new List<string>();
		while (!killThread)
		{
			Thread.Sleep(500);
			string text6 = blackListString;
			if ((text6 != null && text6 != text5) || dictionary == null)
			{
				dictionary = new Dictionary<string, List<string>>();
				if (text6 != null)
				{
					text5 = text6;
				}
				try
				{
					File.WriteAllText(analyticsBlackListPath, text5);
				}
				catch (Exception ex2)
				{
					workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread while storing blacklist file {analyticsBlackListPath}: {ex2.ToString()}");
				}
				dictionary.Clear();
				string[] array = text5.Split('|');
				for (int i = 0; i < array.Length; i++)
				{
					int num4 = array[i].IndexOf('=');
					string text7 = null;
					string text8 = null;
					if (num4 >= 0)
					{
						text7 = array[i].Substring(0, num4);
						text8 = array[i].Substring(num4 + 1);
					}
					else
					{
						text7 = array[i];
					}
					if (text7 == "LogEntryCap")
					{
						long result = 0L;
						if (long.TryParse(text8, out result))
						{
							num3 = result;
						}
						continue;
					}
					List<string> value = null;
					if (!dictionary.TryGetValue(text7, out value))
					{
						value = new List<string>();
						dictionary.Add(text7, value);
					}
					value.Add(text8);
				}
			}
			string result2 = null;
			while (successfullySentFiles.TryDequeue(out result2))
			{
				if (list.Contains(result2))
				{
					list.Remove(result2);
				}
				try
				{
					if (File.Exists(result2))
					{
						File.Delete(result2);
					}
				}
				catch (Exception ex3)
				{
					workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread while removing successfully sent files: {ex3.ToString()}");
				}
			}
			string result3 = null;
			while (unsuccessfullySentFiles.TryDequeue(out result3))
			{
				if (list.Contains(result3))
				{
					list.Remove(result3);
				}
			}
			DateTime now = DateTime.Now;
			TimeSpan timeSpan2 = now - dateTime;
			dateTime = now;
			AnalyticsEvent result4 = null;
			while (eventsToWrite.TryDequeue(out result4))
			{
				List<string> value2 = null;
				bool flag = false;
				if (dictionary.TryGetValue(result4.Name, out value2))
				{
					for (int j = 0; j < value2.Count; j++)
					{
						if (value2[j] == null)
						{
							flag = true;
							break;
						}
						if (result4.Message.IndexOf(value2[j]) >= 0)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				if (result4.Name == "LogEntry")
				{
					num2++;
					if (num2 > num3)
					{
						continue;
					}
					if (num2 == num3)
					{
						result4.AddProperty("FollowingEntriesCapped", "true");
					}
				}
				try
				{
					if (num >= 100 || text == null || result4.SessionToken != text2 || result4.HashedId != text3 || result4.InstallationId != text4)
					{
						text2 = result4.SessionToken;
						text3 = result4.HashedId;
						text4 = result4.InstallationId;
						text = StartNewFile(text2, text3, text4);
						num = 0;
					}
					long result5 = 1L;
					try
					{
						if (File.Exists(prevAnalyticsEventCounterPath))
						{
							result5 = ((!long.TryParse(File.ReadAllText(prevAnalyticsEventCounterPath), out result5)) ? (-1) : (result5 + 1));
						}
						long num5 = ((result5 > 0) ? result5 : 0);
						File.WriteAllText(prevAnalyticsEventCounterPath, num5.ToString());
					}
					catch (Exception ex4)
					{
						workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread, while determining analytics event counter value: {ex4.ToString()}");
					}
					result4.AddProperty("Counter", result5);
					stringBuilder.Length = 0;
					string value3 = result4.Finish();
					if (num > 0)
					{
						stringBuilder.AppendLine(",");
					}
					stringBuilder.Append(value3);
					File.AppendAllText(text, stringBuilder.ToString());
					num++;
					lock (cachedEvents)
					{
						cachedEvents.Add(result4);
					}
				}
				catch (Exception ex5)
				{
					workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread, while processing queued events: {ex5.ToString()}");
				}
			}
			if (now - dateTime2 > timeSpan)
			{
				num2 = 0L;
				dateTime2 = now;
			}
			if (zero.TotalMilliseconds < 10000.0)
			{
				zero += timeSpan2;
			}
			else
			{
				zero = TimeSpan.Zero;
				if (num > 0)
				{
					text = StartNewFile(text2, text3, text4);
					num = 0;
				}
			}
			if (analyticsUrl == null || !hasNetworkConnection)
			{
				continue;
			}
			if (zero2.TotalMilliseconds < 5000.0)
			{
				zero2 += timeSpan2;
				continue;
			}
			zero2 = TimeSpan.Zero;
			string[] array2 = null;
			try
			{
				array2 = Directory.GetFiles(rootPath, "*.analytics");
			}
			catch (Exception ex6)
			{
				workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread: {ex6.ToString()}");
				continue;
			}
			if (array2.Length == 0)
			{
				continue;
			}
			Array.Sort(array2, StringComparer.InvariantCulture);
			long num6 = 0L;
			int num7 = 0;
			int num8 = -1;
			for (int num9 = array2.Length - 1; num9 >= 0; num9--)
			{
				if (array2[num9] == text)
				{
					num8 = num9;
					continue;
				}
				FileInfo fileInfo = null;
				try
				{
					fileInfo = new FileInfo(array2[num9]);
				}
				catch (Exception ex7)
				{
					workerErrorMessages.Enqueue(string.Format("Exception was thrown on the analytics worker thread, while retrieving file information for file {0}: {1}", (array2[num9] != null) ? array2[num9] : "<null>", ex7.ToString()));
					continue;
				}
				num6 += fileInfo.Length;
				if (num6 <= 204800)
				{
					continue;
				}
				num7 = num9;
				break;
			}
			for (int k = 0; k < num7; k++)
			{
				if (k == num8)
				{
					continue;
				}
				try
				{
					if (list.Contains(array2[k]))
					{
						list.Remove(array2[k]);
					}
					File.Delete(array2[k]);
				}
				catch (Exception ex8)
				{
					workerErrorMessages.Enqueue(string.Format("Exception was thrown on the analytics worker thread, while removing analytics file {0}: {1}", (array2[k] != null) ? array2[k] : "<null>", ex8.ToString()));
				}
			}
			for (int l = num7; l < array2.Length; l++)
			{
				if (array2[l] == text || list.Contains(array2[l]))
				{
					continue;
				}
				string text9 = null;
				try
				{
					text9 = File.ReadAllText(array2[l]);
				}
				catch (Exception ex9)
				{
					workerErrorMessages.Enqueue(string.Format("Exception was thrown on the analytics worker thread, while reading analytics file {0}: {1}", (array2[l] != null) ? array2[l] : "<null>", ex9.ToString()));
					try
					{
						File.Delete(array2[l]);
					}
					catch
					{
					}
					continue;
				}
				if (!IsEmptyFile(text9))
				{
					text9 += "]}";
					string hashedId = null;
					string installationId = null;
					string fileContents = null;
					try
					{
						using StringReader stringReader = new StringReader(text9);
						stringReader.ReadLine();
						hashedId = stringReader.ReadLine();
						installationId = stringReader.ReadLine();
						fileContents = stringReader.ReadToEnd();
					}
					catch (Exception ex10)
					{
						workerErrorMessages.Enqueue($"Exception was thrown on the analytics worker thread, while reading analytics file content: {ex10.ToString()}");
						continue;
					}
					if (SendFile(analyticsUrl, sessionToken, hashedId, installationId, array2[l], fileContents))
					{
						list.Add(array2[l]);
					}
				}
				else
				{
					try
					{
						File.Delete(array2[l]);
					}
					catch (Exception ex11)
					{
						workerErrorMessages.Enqueue(string.Format("Exception was thrown on the analytics worker thread, while deleting empty analytics file {0}: {1}", (array2[l] != null) ? array2[l] : "<null>", ex11.ToString()));
					}
				}
			}
		}
	}

	public AnalyticsEvent CreateEvent(string eventName)
	{
		string hashedId = null;
		if (GameManager.Instance != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (playerModel != null)
			{
				hashedId = playerModel.HashedId;
			}
		}
		string text = null;
		if (GameManager.Instance.IsConnectedToServer && ValidateSessionToken(SignalRClient.Instance.CurrentSessionToken))
		{
			text = SignalRClient.Instance.CurrentSessionToken;
		}
		AnalyticsEvent analyticsEvent = null;
		lock (cachedEvents)
		{
			if (cachedEvents.Count > 0)
			{
				analyticsEvent = cachedEvents[cachedEvents.Count - 1];
				cachedEvents.RemoveAt(cachedEvents.Count - 1);
			}
			else
			{
				analyticsEvent = new AnalyticsEvent();
			}
		}
		analyticsEvent.Init(eventName, DateTime.UtcNow, hashedId, text, TWDPlayerPrefs.GetString("InstallationId"), (analyticsFileProcessThread != null) ? eventsToWrite : null);
		return analyticsEvent;
	}

	private void UpdateNetworkConnectionStatus()
	{
		hasNetworkConnection = UnityUtils.InternetReachability != NetworkReachability.NotReachable;
	}

	public void Update()
	{
		if (HelpersModel.IsOffThinkingAnalytics) return;

		networkConnectionPollCounter += (long)(Time.deltaTime * 1000f);
		if (networkConnectionPollCounter >= networkConnectionPollCounterInterval)
		{
			networkConnectionPollCounter = 0L;
			UpdateNetworkConnectionStatus();
		}
		if (GameManager.Instance.IsConnectedToServer)
		{
			string currentHostPort = SignalRClient.Instance.CurrentHostPort;
			if (!string.IsNullOrEmpty(currentHostPort) && analyticsUrl != currentHostPort)
			{
				sessionToken = null;
				currentHostPort += "/player/clientanalytics/send";
				analyticsUrl = currentHostPort;
			}
			sessionToken = SignalRClient.Instance.CurrentSessionToken;
		}
		string result = null;
		while (workerErrorMessages.TryDequeue(out result))
		{
			Debug.LogError(result);
		}
	}
}
