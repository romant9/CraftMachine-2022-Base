using UnityEngine;

public class PromoTweenIn : PromoBase
{
	[Header("Parent of all visual content")]
	[SerializeField]
	private GameObject contentParent;

	[Header("Labels")]
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descLabel;

	[Header("Images")]
	[SerializeField]
	private UITexture bigImage;

	[SerializeField]
	private UITexture defaultTexture;

	private int bigImageInitHeight;

	[SerializeField]
	private UITexture thumbImage;

	private int thumbImageInitHeight;

	[SerializeField]
	private Material defaultMaterial;

	[SerializeField]
	private Material gvgWarMaterial;

	[Header("Tween group on start")]
	[SerializeField]
	private int tweenGroupOnEnable;

	[Header("Tween group on after the first start")]
	[SerializeField]
	private int tweenGroupOnEnableAfterFirst = 5;

	private LoadImageFromUrl imageLoaderCached;

	public override void Awake()
	{
		DebugIdString = "PromoTweenIn";
		if (bigImageInitHeight == 0 && bigImage != null)
		{
			bigImageInitHeight = bigImage.height;
		}
		if (thumbImageInitHeight == 0 && thumbImage != null)
		{
			thumbImageInitHeight = thumbImage.height;
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
		DynamicTexture();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void OnUIEvent(string type, object parameter)
	{
		if (type == "OnLoadImageComplete" && cachedItem != null && parameter is string)
		{
			string text = (string)parameter;
			if (text == cachedItem.ImageUrl || text == cachedItem.ThumbnailUrl)
			{
				UpdateUI();
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (cachedItem != null && GetImageLoader() != null && !string.IsNullOrEmpty(cachedItem.ImageUrl) && !string.IsNullOrEmpty(cachedItem.ThumbnailUrl))
		{
			if (LoadImageFromUrl.downloadedImages.ContainsKey(cachedItem.ImageUrl) && LoadImageFromUrl.downloadedImages.ContainsKey(cachedItem.ThumbnailUrl))
			{
				Helpers.GameObjectSetActive(contentParent, value: true);
				int num = ((GameManager.Instance.PlayerHubManager.GetShownCount(cachedItem) == 0) ? tweenGroupOnEnable : tweenGroupOnEnableAfterFirst);
				TweenManager.PlayTweenGroup(base.gameObject, num, forward: true, PromoShowComplete);
			}
			else
			{
				Helpers.GameObjectSetActive(contentParent, value: false);
			}
			HelpersUI.SetContentToLabel(titleLabel, cachedItem.Title, cachedItem.Title != "");
			HelpersUI.SetContentToLabel(descLabel, cachedItem.Abstract, cachedItem.Abstract != "");
		}
		GetImageLoader().LoadImage(cachedItem.ImageUrl, bigImage, bigImageInitHeight);
		GetImageLoader().LoadImage(cachedItem.ThumbnailUrl, thumbImage, thumbImageInitHeight);
	}

	private void DynamicTexture()
	{
		if (GameManager.Instance.gameEconomyData.GetFeature("MissionHubBackgroundOverride").Enabled)
		{
			defaultTexture.material = (GuildWarHelper.ShowWarIsOnOnMissionHub() ? gvgWarMaterial : defaultMaterial);
		}
	}

	private void PromoShowComplete()
	{
		GameManager.Instance.PlayerHubManager.SaveItemShown(cachedItem);
	}

	private LoadImageFromUrl GetImageLoader()
	{
		if (imageLoaderCached == null)
		{
			imageLoaderCached = GetComponent<LoadImageFromUrl>();
		}
		return imageLoaderCached;
	}
}
