using TWDModel;
using UnityEngine;

public class CampaignPopup : MonoBehaviour
{
	[SerializeField]
	private UITabs tabs;

	[SerializeField]
	private UILabel campaignNameLabel;

	[SerializeField]
	private UILabel campaignDescriptionLabel;

	[SerializeField]
	private UILabel campaignRewardNameLabel;

	[SerializeField]
	private CampaignRewardsGridPanel campaignRewardsGridPanel;

	[SerializeField]
	private ResourcesDeeplinksGridPanel deepLinksGridPanel;

	[SerializeField]
	private UILabel noMoreTokensLabel;

	[SerializeField]
	private UISprite campaignTokenIcon;

	[SerializeField]
	private UILabel amountTokensLabel;

	[SerializeField]
	private UILabel timeLeftLabel;

	[SerializeField]
	private UILabel campaignEndLabel;

	[SerializeField]
	private UILabel rewardingEndLabel;

	[Header("Visual Config Objects")]
	[SerializeField]
	private UISprite backgroundSprite;

	[SerializeField]
	private UIButtonToggle[] tabButtons;

	[SerializeField]
	private UISprite contentsBackgroundSprite;

	[SerializeField]
	private UILabel rewardHighlightLabel;

	[SerializeField]
	private UITexture rewardHighlightPrimary;

	[SerializeField]
	private UITexture rewardHighlightSecondary;

	[SerializeField]
	private UITexture rewardHighlightCenter;

	[SerializeField]
	private UITexture rewardTexture;

	[SerializeField]
	private UILabel currencyLabel;

	[Header("FX")]
	[SerializeField]
	private GameObject godRayEffect;

	[SerializeField]
	private GameObject fallingLeavesEffect;

	[SerializeField]
	private GameObject fallingSakuraLeavesEffect;

	[SerializeField]
	private GameObject fallingSnowflakesEffect;

	[SerializeField]
	private GameObject steamEffect;

	[SerializeField]
	private GameObject meltingIceEffect;

	[SerializeField]
	private GameObject fireEffect;

	[SerializeField]
	private GameObject confettiEffect;

	private bool Rewarding;

	private float RefreshTimer { get; set; }

	public void Update()
	{
		RefreshTimer -= Time.deltaTime;
		if (!(RefreshTimer <= 0f))
		{
			return;
		}
		CampaignModel campaignModel = GameManager.Instance.playerModel.CampaignModel;
		CampaignDefinition campaignDefinition = GameManager.Instance.gameEconomyData.GetCampaignDefinition(campaignModel.Id);
		if (campaignDefinition != null)
		{
			if (!Rewarding)
			{
				long num = campaignDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				if (num > 0)
				{
					UpdateTimeLeft(num);
				}
				else
				{
					Rewarding = true;
					Helpers.GameObjectSetActive(campaignEndLabel, value: false);
					Helpers.GameObjectSetActive(rewardingEndLabel, value: true);
				}
			}
			if (Rewarding)
			{
				long num2 = campaignDefinition.RewardsAvailableMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				if (num2 > 0)
				{
					UpdateTimeLeft(num2);
				}
			}
		}
		RefreshTimer = 1f;
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		if (GameManager.Instance != null && GameManager.Instance.gameEconomyData != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.CampaignModel != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			CampaignDefinition campaignDefinition = GameManager.Instance.gameEconomyData.GetCampaignDefinition(playerModel.CampaignModel.Id);
			if (campaignDefinition != null)
			{
				HelpersUI.SetSprite(campaignTokenIcon, campaignDefinition.TokenIcon);
				HelpersUI.SetContentToLabel(amountTokensLabel, playerModel.CampaignModel.CampaignTokens.ToString());
				HelpersUI.SetContentToLabel(campaignNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(campaignDefinition.NameLocKey));
				HelpersUI.SetContentToLabel(campaignDescriptionLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(campaignDefinition.DescLocKey));
				HelpersUI.SetContentToLabel(campaignRewardNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(campaignDefinition.CaptionLocKey));
				CampaignModel campaignModel = GameManager.Instance.playerModel.CampaignModel;
				CampaignDefinition campaignDefinition2 = GameManager.Instance.gameEconomyData.GetCampaignDefinition(campaignModel.Id);
				if (campaignDefinition2 != null)
				{
					if (campaignDefinition2.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp > 0)
					{
						Helpers.GameObjectSetActive(campaignEndLabel, value: true);
						Helpers.GameObjectSetActive(rewardingEndLabel, value: false);
					}
					else
					{
						Helpers.GameObjectSetActive(campaignEndLabel, value: false);
						Helpers.GameObjectSetActive(rewardingEndLabel, value: true);
					}
				}
				Texture texture = UnityUtils.LoadFromAssetBundle<Texture>(campaignDefinition.HighlightedRewardTexture, "itemgraphics");
				if (!OfflineManager.IsLoadDataManager)
                {
                    Texture value = UnityUtils.LoadFromAssetBundle<Texture>(campaignDefinition.HighlightedRewardTexture + "_alpha", "itemgraphics");
                    rewardTexture.material.SetTexture("_AlphaTex", value);
                    rewardTexture.material.SetTexture("_MainTex", texture);
                }
                rewardTexture.mainTexture = texture;
			}
			else
			{
				Helpers.GameObjectSetActive(amountTokensLabel, value: false);
			}
		}
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		tabs.OnNewTabSelectedEvent += OnTabSelected;
		UpdateVisuals(SingularityMonoBehaviour<HUDManager>.Instance.CampaignVisualConfig);
	}

	public void Close()
	{
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void Clean()
	{
		if (campaignRewardsGridPanel != null)
		{
			campaignRewardsGridPanel.Clean();
		}
		if (deepLinksGridPanel != null)
		{
			deepLinksGridPanel.Clean();
		}
	}

	private void UpdateTimeLeft(long timeLeft)
	{
		HelpersUI.SetContentToLabel(timeLeftLabel, Helpers.FormatTime(timeLeft));
	}

	private void OnTabSelected(int tabindex)
	{
		if (!(tabs.GetContent(tabindex) != null))
		{
			return;
		}
		switch (tabindex)
		{
		case 0:
			if (campaignRewardsGridPanel != null)
			{
				campaignRewardsGridPanel.Clean();
				deepLinksGridPanel.Clean();
				campaignRewardsGridPanel.Init();
			}
			break;
		case 1:
			if (Rewarding)
			{
				Helpers.GameObjectSetActive(deepLinksGridPanel, value: false);
				Helpers.GameObjectSetActive(noMoreTokensLabel, value: true);
			}
			else if (deepLinksGridPanel != null)
			{
				campaignRewardsGridPanel.Clean();
				deepLinksGridPanel.Clean();
				deepLinksGridPanel.Init();
			}
			break;
		}
	}

	public void UpdateVisuals(CampaignVisualConfig config)
	{
		backgroundSprite.gradientTop = config.bgColorGradientTop;
		backgroundSprite.gradientBottom = config.bgColorGradientBottom;
		contentsBackgroundSprite.color = config.tabContentsBgColor;
		UIButtonToggle[] array = tabButtons;
		foreach (UIButtonToggle obj in array)
		{
			obj.defaultColor = config.tabContentsBgColor;
			obj.hover = config.tabContentsBgColor;
			obj.pressed = config.tabContentsBgColor;
			obj.disabledColor = config.tabContentsBgColor;
			obj.UpdateColor(instant: true);
		}
		campaignNameLabel.gradientTop = config.headerColorGradientTop;
		campaignNameLabel.gradientBottom = config.headerColorGradientBottom;
		campaignNameLabel.effectColor = config.headerShadowColor;
		campaignDescriptionLabel.color = config.paragraphColor;
		campaignDescriptionLabel.effectColor = config.paragraphShadowColor;
		rewardHighlightLabel.gradientTop = config.headerColorGradientTop;
		rewardHighlightLabel.gradientBottom = config.headerColorGradientBottom;
		rewardHighlightLabel.effectColor = config.headerShadowColor;
		rewardHighlightPrimary.material = config.highlightPrimaryGlowMaterial;
		rewardHighlightSecondary.color = config.highlightSecondaryGlowColor;
		rewardHighlightCenter.color = config.highlightCenterGlowColor;
		currencyLabel.color = config.currencyTextColor;
		currencyLabel.effectColor = config.currencyTextShadowColor;
		godRayEffect.SetActive(config.enableGodRay);
		fallingLeavesEffect.SetActive(!OfflineManager.IsLoadDataManager && config.enableFallingLeaves);
		fallingSakuraLeavesEffect.SetActive(!OfflineManager.IsLoadDataManager && config.enableFallingSakuraLeaves);
		fallingSnowflakesEffect.SetActive(!OfflineManager.IsLoadDataManager && config.enableFallingSnowFlakes);
		steamEffect.SetActive(config.enableSteam);
		meltingIceEffect.SetActive(config.enableMeltingIce);
		fireEffect.SetActive(!OfflineManager.IsLoadDataManager && config.enableFire);
		confettiEffect.SetActive(!OfflineManager.IsLoadDataManager && config.enableConfetti);
	}
}
