using TWDModel;
using UnityEngine;

public class StartWarBannerPopup : HUDElement
{
	[SerializeField]
	private UILabel endWarTimerLabel;

	[Header("Guild Info")]
	[SerializeField]
	private UILabel guildNameLabel;

	[SerializeField]
	private UISprite currentTierIcon;

	[SerializeField]
	private UILabel currentTierLabel;

	[SerializeField]
	private UILabel currentVictoryPointsLabel;

	[Header("Next tier Info")]
	[SerializeField]
	private GameObject nextTierContainer;

	[SerializeField]
	private GameObject maxTierContainer;

	[Header("Shop Unlocks")]
	[SerializeField]
	private GuildShopItemPreview nextTierUnlock;

	private float refreshTimer;

	private int refreshRate = 1;

	public static bool CanShow()
	{
		bool num = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasSeenWarStart();
		bool flag = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		bool flag2 = GuildWarHelper.IsWarOngoing() && GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasWarStarted();
		if (!num && flag2)
		{
			return !flag;
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
		UpdateDynamicUI();
	}

	public override void Update()
	{
		base.Update();
		refreshTimer -= Time.deltaTime;
		if (refreshTimer < 0f)
		{
			UpdateDynamicUI();
			refreshTimer = refreshRate;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		GuildTierDefinition guildTierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(guildModel.GuildBattleTier);
		GuildTierDefinition nextGuildTier = GuildTierHelper.GetNextGuildTier(guildTierDefinition.Tier);
		HelpersUI.SetContentToLabel(guildNameLabel, guildModel.Name);
		HelpersUI.SetSprite(currentTierIcon, guildTierDefinition.IconSprite);
		HelpersUI.SetContentToLabel(currentTierLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(guildTierDefinition.NameLocalizationKey));
		HelpersUI.SetContentToLabel(currentVictoryPointsLabel, guildModel.CurrentVictoryPoints.ToString());
		Helpers.GameObjectSetActive(nextTierContainer, nextGuildTier != null);
		Helpers.GameObjectSetActive(maxTierContainer, nextGuildTier == null);
		if (nextGuildTier != null && nextTierUnlock != null)
		{
			nextTierUnlock.OpenForTier(nextGuildTier);
		}
	}

	private void UpdateDynamicUI()
	{
		HelpersUI.SetContentToLabel(endWarTimerLabel, Helpers.FormatTimeNoZero(GuildWarHelper.GetTimeLeftToCurrentWarEnd()));
	}

	public void OnCloseButtonClick()
	{
		Close();
	}

	public void OnGoToButonClick()
	{
		Close();
		MissionHubNavigation.OpenGuildBattleMap();
	}

	public void OnClickLearnMore()
	{
		base.OnClose += delegate
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleOverviewPopup).Open();
		};
		Close();
	}

	public override void Close()
	{
		base.Close();
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenGuildWarStart"))
		{
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand("HasSeenGuildWarStart"));
		}
	}
}
