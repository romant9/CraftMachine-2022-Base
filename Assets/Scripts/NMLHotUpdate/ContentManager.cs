using BaseModel;
using BestHTTP;
using Client.Connectivity;
using ICSharpCode.SharpZipLib.Checksums;
using OdinSerializer.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class ContentManager : MonoBehaviour, IModelContentService
{
	private static ContentManager _instance;

	private readonly float _timeout = 60f;

	private IMessageSerializer _serializer;

	private Dictionary<string, ContentCache> _caches = new Dictionary<string, ContentCache>();

	private Dictionary<string, ContentRequest> _requests = new Dictionary<string, ContentRequest>();

	private Dictionary<string, ContentConfig> _configs = new Dictionary<string, ContentConfig>();

	private Dictionary<string, CDNRequest> _cdnRequests = new Dictionary<string, CDNRequest>();

	public static ContentManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject("ContentManager");
				UnityEngine.Object.DontDestroyOnLoad(obj);
				_instance = obj.AddComponent<ContentManager>();
				_instance._serializer = GameManager.Instance.jsonSerializer;
			}
			return _instance;
		}
	}

	private void Awake()
	{
		RegisterContentType("GED", new ContentConfig
		{
			Mode = (LoadMode.Server | LoadMode.Client),
			MaxCacheFiles = 1
		});
		RegisterContentType("Player", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 1
		});
		RegisterContentType("Banner", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 1
		});
		RegisterContentType("MediationData", new ContentConfig
		{
			Mode = (LoadMode.Server | LoadMode.Client),
			MaxCacheFiles = 1
		});
		RegisterContentType("NewsItem", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 1
		});
		RegisterContentType("EpisodeVideo", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 10
		});
		RegisterContentType("RunLocation", new ContentConfig
		{
			Mode = (LoadMode.Server | LoadMode.Client),
			LoadFromCDN = true,
			MaxCacheFiles = 50
		});
		RegisterContentType("Localization", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 4 // 1
		});
		RegisterContentType("Image", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 200 // 20
		});
		RegisterContentType("UnityAdsIds", new ContentConfig
		{
			Mode = LoadMode.Client,
			MaxCacheFiles = 1
		});
		RegisterContentType("CDNImage", new ContentConfig
		{
			MaxCacheFiles = 200 // 40
		});
		RegisterContentType("BannerImage", new ContentConfig
		{
			MaxCacheFiles = 1
		});
		RegisterContentType("LocalizationFile", new ContentConfig
		{
			MaxCacheFiles = 4 // 1
		});
		RegisterContentType("Guild", new ContentConfig
		{
			Mode = (LoadMode.Server | LoadMode.Client),
			MaxCacheFiles = 6
		});
		RegisterContentType("GuildWarParticipant", new ContentConfig
		{
			Mode = (LoadMode.Server | LoadMode.Client),
			MaxCacheFiles = 6
		});
		InvokeRepeating("CheckRequestTimeouts", 1f, 1f);
		ContentCache.CheckVersion(OfflineManager.ShortVersion);
	}

	private void CheckRequestTimeouts()
	{
		List<string> list = new List<string>();
		foreach (string key in _requests.Keys)
		{
			ContentRequest contentRequest = _requests[key];
			if (contentRequest.Content == null && Time.realtimeSinceStartup - contentRequest.StartTime > _timeout)
			{
				Debug.LogWarning("Timeout for " + contentRequest.ContentPath);
				list.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			FailRequest(list[i]);
		}
	}

	public void RegisterContentType(string contentType, ContentConfig type)
	{
		_configs[contentType] = type;
	}

	public string LoadContent(string contentPath, Action<string, bool> contentReadyCallback, int threshold = 0)
	{
		if (!GameManager.Instance.IsConnectedToServer && !OfflineManager.IsLoadDataManager && !OfflineManager.IsOfflineMode)
		{
			DebugTWD.LogMycode("if (!GameManager.Instance.IsConnectedToServer && !OfflineManager.IsLoadDataManager && !OfflineManager.IsOfflineMod)");
			return null;
		}
		string text = Guid.NewGuid().ToString();
		_requests[text] = new ContentRequest
		{
			StartTime = Time.realtimeSinceStartup,
			ContentPath = contentPath,
			Callback = contentReadyCallback
		};
		RequestLoadContent(text, contentPath, threshold);
		return text;
	}

	public void FakeContent(string content, Action<string, bool> contentReadyCallback)
	{
		string text = Guid.NewGuid().ToString();
		_requests.Add(text, new ContentRequest
		{
			Content = "[" + content + "]"
		});
		contentReadyCallback(text, arg2: true);
	}

	public string GetContent(string transactionId)
	{
		string content = _requests[transactionId].Content;
		_requests.Remove(transactionId);
		return content;
	}

	public void Reset()
	{
		new List<string>(_requests.Keys).ForEach(FailRequest);
		new List<string>(_cdnRequests.Keys).ForEach(FailCDNRequest);
	}

	public void OnLoadContent(string transactionId, string content, string checksum)
	{
		if (!_requests.ContainsKey(transactionId))
		{
			return;
		}
		ContentRequest request = _requests[transactionId];
		string contentType = request.ContentPath.Split('/')[0];
		ContentConfig contentConfig = _configs[contentType];
		ContentCache cache = GetCache(contentType);
		if (content == null)
		{
			content = cache.GetContent<string>(checksum);
			if (content == null)
			{
				Debug.LogWarning("No cached content for " + request.ContentPath + ", retrying");
				RequestLoadContent(transactionId, request.ContentPath);
				return;
			}
		}
		else
		{
			cache.SetContent(request.ContentPath, null, checksum, content);
		}
		if (contentConfig.LoadFromCDN)
		{
			List<string> urls = _serializer.DeserializeObject<List<string>>(content);
			if (urls == null || urls.Count == 0)
			{
				request.Content = content;
				request.Callback(transactionId, arg2: true);
				return;
			}
			List<string> cdnContents = new List<string>();
			int i;
			for (i = 0; i < urls.Count; i++)
			{
				GetCDNContent(urls[i], contentType, null, delegate(string cdnContent)
				{
					if (!CheckContentFormat(contentType, cdnContent))
					{
						Debug.LogError("Load content from CDN returned empty or incorrent json. Content Path: " + request.ContentPath + " - URL: " + urls[i] + " - Checksum: " + checksum);
						cache.DeleteContent(checksum);
						cache.DeleteContentWithId(urls[i]);
						FailRequest(transactionId);
					}
					else
					{
						cdnContents.Add(cdnContent);
						if (cdnContents.Count == urls.Count)
						{
							request.Content = "[" + string.Join(",", cdnContents.ToArray()) + "]";
							request.Callback(transactionId, arg2: true);
						}
					}
				});
			}
		}
		else
		{
			request.Content = content;
			request.Callback(transactionId, arg2: true);
		}
	}

	public void GetCDNContent<T>(string url, string contentType, string id, Action<T> contentCallback, string checksum = null) where T : class
	{
		url = CdnUrlHelper.RewriteCdnUrl(url);
		string text = Guid.NewGuid().ToString();
		_cdnRequests[text] = new CDNRequest<T>
		{
			Callback = contentCallback,
			StartTime = Time.realtimeSinceStartup,
			RetryCount = 0,
			Checksum = checksum
		};
		if (string.IsNullOrEmpty(id))
		{
			id = url;
		}
		if (contentType == "GED")
		{
			DebugTWD.Log("OnLoadGed. url:" + url + " | checksum: " + checksum + " | transactionId: " + text, DebugType.SignalR);
		}
		GetCDNContent(text, url, contentType, id, contentCallback);
	}

	private void GetCDNContent<T>(string transactionId, string url, string contentType, string id, Action<T> contentCallback) where T : class
	{
		ContentCache cache = GetCache(contentType);
		T contentByUrl = cache.GetContentByUrl<T>(url);
		if (contentByUrl != null)
		{
			contentCallback(contentByUrl);
			_cdnRequests.Remove(transactionId);
			return;
		}

		if (DataManager.Instance.IsVpnON)
		{
			new HTTPRequest(new Uri(url), isKeepAlive: true, disableCache: true, delegate (HTTPRequest req, HTTPResponse resp)
			{
				if (_cdnRequests.ContainsKey(transactionId))
				{
					if (resp == null || !resp.IsSuccess)
					{
						string text = ((resp != null) ? resp.StatusCode.ToString() : ((req.Exception != null) ? req.Exception.Message : ""));
						CDNRequest cDNRequest = _cdnRequests[transactionId];
						if (Time.realtimeSinceStartup - cDNRequest.StartTime < _timeout && ++cDNRequest.RetryCount <= 3)
						{
							Debug.LogWarning("GetCDNContent retry " + url + " " + text);
							GetCDNContent(transactionId, url, contentType, id, contentCallback);
						}
						else
						{
							Debug.LogError("GetCDNContent failed " + url + " " + text);
							FailCDNRequest(transactionId);
						}
						if (contentType == "GED")
						{
							DebugTWD.Log("GetCDNContent GED Failed", DebugType.SignalR);
							DataManager.Instance.IsGedFromGoogle = true;
							GetPlayerData.Instance.GetGedFromGoogle(url);
							return;
						}
					}
					else
					{
						if (typeof(T) == typeof(string))
						{
							T val = resp.DataAsText as T;
							string text2 = ContentCache.CalculateChecksum(val);
							CDNRequest cDNRequest2 = _cdnRequests[transactionId];
							if (cDNRequest2.Checksum != null && text2 != cDNRequest2.Checksum)
							{
								Debug.LogWarning("GetCDNContent retry due to mismatching checksums. Requested checksum " + cDNRequest2.Checksum + ", received checksum " + text2);
								GetCDNContent(transactionId, url, contentType, id, contentCallback);
								return;
							}
							cache.SetContent(id, url, text2, val);
							contentCallback(val);
						}
						else if (typeof(T) == typeof(byte[]))
						{
							T val2 = resp.Data as T;
							string text3 = ContentCache.CalculateChecksum(val2);
							CDNRequest cDNRequest3 = _cdnRequests[transactionId];
							if (cDNRequest3.Checksum != null && text3 != cDNRequest3.Checksum)
							{
								GetCDNContent(transactionId, url, contentType, id, contentCallback);
								return;
							}
							cache.SetContent(id, url, text3, val2);
							contentCallback(val2);
						}
						_cdnRequests.Remove(transactionId);
					}
				}
			}).Send();
		}
		else
		{
			GetContentFromGoogle(cache, id, url, contentCallback);
			_cdnRequests.Remove(transactionId);
		}
	}

	private async void GetContentFromGoogle<T>(ContentCache cache, string id, string url, Action<T> contentCallback) where T : class
	{
		if (typeof(T) == typeof(string))
		{
			if (await DataManager.Instance.GoogleSheetManager.GetJsonFromGoogle(url) is not T val) return;

			string text2 = ContentCache.CalculateChecksum(val);

			cache.SetContent(id, url, text2, val);
			contentCallback(val);
		}
		else if (typeof(T) == typeof(byte[]))
		{
			if (await DataManager.Instance.GoogleSheetManager.GetBytesFromGoogle(url) is not T val2) return;
			string text3 = ContentCache.CalculateChecksum(val2);

			cache.SetContent(id, url, text3, val2);
			contentCallback(val2);
		}
	}

	public ContentCache GetCache(string contentType)
	{
		if (!_caches.ContainsKey(contentType))
		{
			int maxCacheFiles = _configs[contentType].MaxCacheFiles;
			_caches[contentType] = new ContentCache(contentType, maxCacheFiles, _serializer);
		}
		return _caches[contentType];
	}

	private void RequestLoadContent(string transactionId, string contentPath, int threshold = 0)
	{
		string text = contentPath.Split('/')[0];
		LoadContentRequest value = new LoadContentRequest
		{
			TransactionId = transactionId,
			ContentPath = contentPath,
			Checksum = GetCache(text).GetChecksum(contentPath),
			Mode = _configs[text].Mode,
			ResultThreshold = threshold
		};
		SignalRClient.Instance.RequestCommand("LoadContent", _serializer.SerializeObject(value), delegate
		{
			if (SignalRClient.Instance.HasError)
			{
				FailRequest(transactionId);
				SignalRClient.Instance.ClearError();
			}
		}, waitForResponse: false);
	}

	private void FailRequest(string transactionId)
	{
		if (_requests.ContainsKey(transactionId))
		{
			_requests[transactionId].Callback(transactionId, arg2: false);
			_requests.Remove(transactionId);
		}
	}

	private void FailCDNRequest(string transactionId)
	{
		if (_cdnRequests.ContainsKey(transactionId))
		{
			CDNRequest cDNRequest = _cdnRequests[transactionId];
			if (cDNRequest is CDNRequest<string>)
			{
				(cDNRequest as CDNRequest<string>).Callback(null);
			}
			if (cDNRequest is CDNRequest<byte[]>)
			{
				(cDNRequest as CDNRequest<byte[]>).Callback(null);
			}
			_cdnRequests.Remove(transactionId);
		}
	}

	public bool CheckContentFormat(string contentType, string content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return false;
		}
		if (contentType == "RunLocation" && GameManager.Instance.modelManager != null)
		{
			try
			{
				GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<RunLocationModel>(content);
			}
			catch (Exception)
			{
				Debug.LogError("Content Json Integrity check Failed for content type: " + contentType + " with content: " + content);
				return false;
			}
		}
		return true;
	}

	public void CancelCdnTransactionCallback(string transactionId)
	{
		if (!string.IsNullOrEmpty(transactionId) && _requests.TryGetValue(transactionId, out var value))
		{
			value.Callback = delegate
			{
			};
		}
	}



	#region mycode
	public static string TryExtractChecksumFromUrl(string url)
	{
		string extractedChecksum = null;
		int num = url.LastIndexOf('/');
		if (num < 0)
		{
			DebugTWD.LogWarning("Invalid content url: " + url);
			return null;
		}
		num++;
		int num2 = url.IndexOf('.', num);
		int length = ((num2 >= 0) ? num2 : url.Length) - num;
		extractedChecksum = url.Substring(num, length);

		return extractedChecksum;
	}
	#endregion
}
