using System;
using TWDModel;
using UnityEngine;

public class GuildBattleEndSeasonRewardsOnlyPopup : HUDElement
{
	public const string PREFS_SEASON_RESET_WARNING_LAST_SHOWN_DATE = "PREFS_SEASON_RESET_WARNING_LAST_SHOWN_DATE";

	[SerializeField]
	private UILabel rewardPointsLabel;

	[SerializeField]
	private GameObject resetTimerParent;

	[SerializeField]
	private UILabel resetTimerLabel;

	private float refreshTimer;

	private int refreshRate = 1;

	public static bool CanShow()
	{
		GvGSeasonModelPlayer gvGSeasonModelPlayer = GuildWarHelper.GetGvGSeasonModelPlayer();
		if (gvGSeasonModelPlayer.HasSeenClaimSeasonRewardsPopup)
		{
			return false;
		}
		bool flag = GuildWarHelper.IsSeasonOngoing();
		bool flag2 = GuildWarHelper.HasSeenGvGSeasonEnd();
		bool flag3 = gvGSeasonModelPlayer.IsCurrentSeasonEnded();
		bool flag4 = GameManager.Instance.playerModel.GuildShopModel.HasAnyAffordableItem();
		bool flag5 = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		if (!HasRecentlyShown() && !flag && flag3 && flag2 && flag4)
		{
			return !flag5;
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		SaveShownDate();
		UpdateUI();
		UpdateDynamicUI();
		GuildWarHelper.GetGvGSeasonModelPlayer().HasSeenClaimSeasonRewardsPopup = true;
		if (GuildWarHelper.GetTimeLeftToNextSeason() == 0L)
		{
			refreshTimer = float.MaxValue;
			Helpers.GameObjectSetActive(resetTimerParent, value: false);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		HelpersUI.SetContentToLabel(rewardPointsLabel, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GuildBattleRP).ToString());
	}

	private void UpdateDynamicUI()
	{
		HelpersUI.SetContentToLabel(resetTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.SeasonResetIn{parameter}", GuildWarHelper.GetFormatedTimeLeftToNextSeason()));
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

	private void SaveShownDate()
	{
		TWDPlayerPrefs.SetString("PREFS_SEASON_RESET_WARNING_LAST_SHOWN_DATE", DateTime.Now.ToBinary().ToString());
	}

	protected static bool HasRecentlyShown()
	{
		if (TWDPlayerPrefs.HasKey("PREFS_SEASON_RESET_WARNING_LAST_SHOWN_DATE"))
		{
			long dateData = Convert.ToInt64(TWDPlayerPrefs.GetString("PREFS_SEASON_RESET_WARNING_LAST_SHOWN_DATE"));
			return DateTime.Now.Subtract(DateTime.FromBinary(dateData)).TotalMilliseconds < (double)GameManager.Instance.gameEconomyData.GuildWarConfig.SeasonResetPopupCooldownInMilliseconds;
		}
		return false;
	}
}
