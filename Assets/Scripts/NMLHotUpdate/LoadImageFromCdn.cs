using Client.Connectivity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadImageFromCdn : MonoBehaviour
{
	private UITexture currentTarget;

	private bool loading;

	private string currentContentPath = "";

	public string currentURL = "";

	private bool imageWasCached;

	private int tweenGroupOnComplete = -1;

	private string currentTransactionId;

	private int currentLoadVersion;

	private static Dictionary<string, string> pathToUrlDic = new Dictionary<string, string>();

	public bool isLoading => loading;

	public static LoadImageFromCdn LoadImageToTarget(UITexture textureTarget, string contentPath, bool clearLocalCachedUrls = false, int tweenGroupOnLoadComplete = -1)
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			DebugTWD.LogMycode("if (!GameManager.Instance.IsConnectedToServer)");
			GetTextureFromCache(textureTarget, contentPath);
		}
		else
		{
			if (textureTarget != null)
			{
				LoadImageFromCdn loadImageFromCdn = textureTarget.GetComponent<LoadImageFromCdn>();
				if (!string.IsNullOrEmpty(contentPath))
				{
					if (loadImageFromCdn == null)
					{
						loadImageFromCdn = Helpers.AddComponent<LoadImageFromCdn>(textureTarget.gameObject);
					}
					loadImageFromCdn.LoadImageWithParams(textureTarget, contentPath, clearLocalCachedUrls, tweenGroupOnLoadComplete);
					return loadImageFromCdn;
				}
				if (loadImageFromCdn != null)
				{
					loadImageFromCdn.CancelCurrentTransaction();
				}
			}
			Helpers.GameObjectSetActive(textureTarget, value: false);
		}
		return null;
	}

	public static void LoadImageAsync(string contentPath, Action<Texture> callback, Action failedCallback, bool clearLocalCachedUrls = false, int tweenGroupOnLoadComplete = -1)
	{
		if (!string.IsNullOrEmpty(contentPath))
		{
			LoadImageWithParams(contentPath, callback, failedCallback, clearLocalCachedUrls, tweenGroupOnLoadComplete);
		}
	}

	public void LoadImageWithParams(UITexture textureTarget, string contentPath, bool clearSessionCache = false, int tweenGroupOnLoadComplete = -1)
	{
		CancelCurrentTransaction();
		if (!string.IsNullOrEmpty(contentPath) && textureTarget != null && !loading)
		{
			currentTarget = textureTarget;
			currentContentPath = contentPath;
			currentURL = "";
			tweenGroupOnComplete = tweenGroupOnLoadComplete;
			if (clearSessionCache)
			{
				pathToUrlDic = new Dictionary<string, string>();
			}
			if (pathToUrlDic != null && pathToUrlDic.TryGetValue(contentPath, out currentURL))
			{
				RetrieveContentWithUrl(currentURL);
				return;
			}
			loading = true;
			Helpers.GameObjectSetActive(currentTarget, value: false);
			currentTransactionId = ContentManager.Instance.LoadContent(contentPath, GetUrlWithTransactionId);
		}
	}

	public static void LoadImageWithParams(string contentPath, Action<Texture> callback, Action failedCallback, bool clearSessionCache = false, int tweenGroupOnLoadComplete = -1)
	{
		if (string.IsNullOrEmpty(contentPath))
		{
			return;
		}
		if (clearSessionCache)
		{
			pathToUrlDic = new Dictionary<string, string>();
		}
		string localURL = "";
		if (pathToUrlDic != null && pathToUrlDic.TryGetValue(contentPath, out localURL))
		{
			DebugTWD.Log("Loaded from Dic: " + contentPath + " | " + localURL, DebugType.Load);
			ContentManager.Instance.GetCDNContent(localURL, "Image", null, delegate(byte[] imageBytes)
			{
				Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false)
				{
					wrapMode = TextureWrapMode.Clamp
				};
				texture2D.LoadImage(imageBytes);
				callback(texture2D);
			});
			return;
		}
		ContentManager.Instance.LoadContent(contentPath, delegate (string transactionId, bool loaded)
		{
			if (!loaded || !OfflineManager.IsInternetOn)
			{
				DebugTWD.LogMycode("if (!loaded || !SignalRClient.Instance.IsConnected)");
				DebugTWD.Log("Content is not Loaded: " + contentPath, DebugType.Load);
				failedCallback();
			}
			else
			{
				DebugTWD.Log("Loaded from Cache: " + contentPath + " | " + transactionId, DebugType.Load);

				string content = ContentManager.Instance.GetContent(transactionId);
				List<string> list = GameManager.Instance.jsonSerializer.DeserializeObject<List<string>>(content);
				if (list != null && list.Count > 0 && !string.IsNullOrEmpty(list[0]))
				{
					localURL = list[0];
					ContentManager.Instance.GetCDNContent(localURL, "Image", null, delegate (byte[] imageBytes)
					{
						Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false)
						{
							wrapMode = TextureWrapMode.Clamp
						};
						texture2D.LoadImage(imageBytes);
						if (pathToUrlDic != null && !pathToUrlDic.ContainsKey(contentPath) && !string.IsNullOrEmpty(localURL))
						{
							pathToUrlDic[contentPath] = localURL;
						}
						callback(texture2D);
					});
				}
			}
		});
	}

	public void SetImageWithBytes(byte[] imageBytes)
	{
		if (currentTarget != null && imageBytes != null && imageBytes.Length != 0)
		{
			loading = false;
			Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.LoadImage(imageBytes);
			currentTarget.mainTexture = texture2D;
			Helpers.GameObjectSetActive(currentTarget, value: true);
			if (tweenGroupOnComplete != -1)
			{
				if (!imageWasCached)
				{
					TweenManager.PlayTweenGroup(base.gameObject, tweenGroupOnComplete);
				}
				else
				{
					TweenManager.FinishTweenGroup(base.gameObject, tweenGroupOnComplete);
				}
			}
			if (pathToUrlDic != null && !pathToUrlDic.ContainsKey(currentContentPath) && !string.IsNullOrEmpty(currentURL))
			{
				pathToUrlDic[currentContentPath] = currentURL;
				currentURL = "";
				currentContentPath = "";
			}
		}
		else
		{
			Helpers.GameObjectSetActive(currentTarget, value: false);
		}
	}

	private bool IsImageCached(string url)
	{
		return ContentManager.Instance.GetCache("Image").GetContentByUrl<byte[]>(url) != null;
	}

	private void OnDisable()
	{
		loading = false;
		imageWasCached = false;
		if (tweenGroupOnComplete != -1)
		{
			TweenManager.FinishTweenGroup(base.gameObject, tweenGroupOnComplete);
		}
		tweenGroupOnComplete = -1;
	}

	private void GetUrlWithTransactionId(string transactionId, bool loaded)
	{
		if (loaded)
		{
			string content = ContentManager.Instance.GetContent(transactionId);
			List<string> list = GameManager.Instance.jsonSerializer.DeserializeObject<List<string>>(content);
			if (list != null && list.Count > 0 && !string.IsNullOrEmpty(list[0]))
			{
				currentURL = list[0];
				RetrieveContentWithUrl(currentURL);
			}
		}
	}

	private void RetrieveContentWithUrl(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		imageWasCached = IsImageCached(url);
		int loadVersion = currentLoadVersion;
		ContentManager.Instance.GetCDNContent(url, "Image", null, delegate(byte[] imageBytes)
		{
			if (loadVersion == currentLoadVersion)
			{
				SetImageWithBytes(imageBytes);
			}
		});
	}

	private void CancelCurrentTransaction()
	{
		ContentManager.Instance.CancelCdnTransactionCallback(currentTransactionId);
		loading = false;
		currentLoadVersion++;
	}


	#region mycode
	public static void GetTextureFromCache(UITexture texture, string contentPath)
	{
		string contentChecksum = ContentManager.Instance.GetCache("Image").GetChecksum(contentPath);
		if (string.IsNullOrEmpty(contentChecksum) || texture == null)
		{
			DebugTWD.Log("Can't load content from " + contentPath, DebugType.Load);
			return;
		}

		string content = ContentManager.Instance.GetCache("Image").GetContent<string>(contentChecksum);
		DebugTWD.Log("Content is " + content, DebugType.Load);

		List<string> list = OfflineManager.JsonSerializer.DeserializeObject<List<string>>(content);

		if (list != null && list.Count > 0 && !string.IsNullOrEmpty(list[0]))
		{
			string localURL = list[0];

			string contentFirst = ContentManager.TryExtractChecksumFromUrl(localURL);

			string filenameFirst = Application.persistentDataPath + "/ContentCache/Image/" + contentFirst + ".txt";

			DebugTWD.Log("Filename First: " + filenameFirst, DebugType.Load);

			if (System.IO.File.Exists(filenameFirst))
			{
				var rawData = System.IO.File.ReadAllBytes(filenameFirst);

				Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false)
				{
					wrapMode = TextureWrapMode.Clamp
				};
				texture2D.LoadImage(rawData);

				texture.mainTexture = texture2D;
				Helpers.GameObjectSetActive(texture, value: true);
			}
		}
	}
	#endregion
}
