using System;
using System.Collections.Generic;
using BaseModel.ContentTypes;
using Client.Connectivity;
using UnityEngine;

public class BannerManager
{
	private const string BannerIdKey = "CurrentBannerId";

	private const string BannerShowCountKey = "CurrentBannerShowCount";

	private const string BannerLastShownDateKey = "BannerLastShownDate";

	private const string BannerClickedKey = "BannerClickedKey";

	public Banner CurrentBannerInfo;

	private int currentBannerShowCount;

	private Texture cachedBannerTexture;

	private bool debugEnableBanner;

	public void UpdateBannerInfo()
	{
		if (!(SignalRClient.Instance == null) && GameManager.Instance.IsConnectedToServer)
		{
			Startup.LogStartupEvent("RequestBanner");
			ContentManager.Instance.LoadContent(typeof(Banner).Name, OnBannerContent);
		}
	}

	public bool CanShowBannerEarlyCheck()
	{
		if (CurrentBannerInfo == null)
		{
			return false;
		}
		if (TutorialView.Instance.Running)
		{
			return false;
		}
		if (debugEnableBanner)
		{
			return true;
		}
		if (HasClickedBanner())
		{
			return false;
		}
		if (currentBannerShowCount >= CurrentBannerInfo.ShowTimes)
		{
			return false;
		}
		return true;
	}

	public bool CanShowBanner()
	{
		if (!CanShowBannerEarlyCheck())
		{
			return false;
		}
		if (debugEnableBanner)
		{
			return true;
		}
		int bannerShowInterval = GameManager.Instance.gameEconomyData.ConfigData.BannerShowInterval;
		int num = (int)GetSecondsSinceLastOpen();
		if (num != -1 && bannerShowInterval > 0 && num < bannerShowInterval)
		{
			return false;
		}
		if (cachedBannerTexture == null)
		{
			return false;
		}
		return true;
	}

	public void IncrementShowCount()
	{
		if (CurrentBannerInfo != null)
		{
			TWDPlayerPrefs.SetInt("CurrentBannerShowCount", ++currentBannerShowCount);
			MakeTimestamp();
		}
	}

	private bool HasClickedBanner()
	{
		return TWDPlayerPrefs.GetInt("BannerClickedKey") == 1;
	}

	private void OnBannerContent(string transactionId, bool loaded)
	{
		if (!loaded)
		{
			return;
		}
		string content = ContentManager.Instance.GetContent(transactionId);
		List<Banner> list = GameManager.Instance.jsonSerializer.Deserialize<List<Banner>>(content);
		if (list != null && list.Count != 0)
		{
			CurrentBannerInfo = list[0];
			if (CurrentBannerInfo == null)
			{
				Debug.LogWarning("Invalid banner info received");
				return;
			}
			if (TWDPlayerPrefs.GetString("CurrentBannerId") == CurrentBannerInfo.EntryId)
			{
				currentBannerShowCount = TWDPlayerPrefs.GetInt("CurrentBannerShowCount");
				return;
			}
			TWDPlayerPrefs.SetString("CurrentBannerId", CurrentBannerInfo.EntryId);
			TWDPlayerPrefs.SetInt("CurrentBannerShowCount", 0);
			TWDPlayerPrefs.SetInt("BannerClickedKey", 0);
			TWDPlayerPrefs.DeleteKey("BannerLastShownDate");
		}
	}

	public void LoadBannerImage()
	{
		if (cachedBannerTexture == null)
		{
			ContentManager.Instance.GetCDNContent<byte[]>(CurrentBannerInfo.ImageUrl, "BannerImage", null, OnBannerImage);
		}
	}

	private void OnBannerImage(byte[] bannerImage)
	{
		if (bannerImage == null)
		{
			Debug.LogWarning("Failed to get banner image");
			return;
		}
		Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false);
		texture2D.name = "BannerAd";
		if (texture2D.LoadImage(bannerImage))
		{
			cachedBannerTexture = texture2D;
		}
	}

	protected static void MakeTimestamp()
	{
		TWDPlayerPrefs.SetString("BannerLastShownDate", DateTime.Now.ToBinary().ToString());
	}

	protected static double GetSecondsSinceLastOpen()
	{
		if (TWDPlayerPrefs.HasKey("BannerLastShownDate"))
		{
			long dateData = Convert.ToInt64(TWDPlayerPrefs.GetString("BannerLastShownDate"));
			return DateTime.Now.Subtract(DateTime.FromBinary(dateData)).TotalSeconds;
		}
		return -1.0;
	}

	public Texture GetBannerTexture()
	{
		return cachedBannerTexture;
	}

	private string GetBannerURL()
	{
		if (CurrentBannerInfo != null)
		{
			return CurrentBannerInfo.NavigationLink;
		}
		return null;
	}

	public void DebugEnableBanner()
	{
		debugEnableBanner = true;
	}

	public void OnBannerClicked()
	{
		TWDPlayerPrefs.SetInt("BannerClickedKey", 1);
		Application.OpenURL(GetBannerURL());
		cachedBannerTexture = null;
	}
}
