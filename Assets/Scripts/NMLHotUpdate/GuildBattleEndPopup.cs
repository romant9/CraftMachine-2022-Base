using System.Collections.Generic;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class GuildBattleEndPopup : HUDElement
{
	[Header("Animations")]
	[SerializeField]
	private int introTweenGroup;

	[SerializeField]
	private int scoresTweenGroup;

	[SerializeField]
	private int rewardsTweenGroup;

	[SerializeField]
	private int battleRewardsTweenGroup;

	[SerializeField]
	private int loserRewardsTweenGroup;

	[Header("Player Guild")]
	[SerializeField]
	private UIGuildTierProgressBar guildTierProgressBar;

	[SerializeField]
	private UILabel guildName;

	[SerializeField]
	private UILabel guildBattlePointsLabel;

	[SerializeField]
	private UILabel guildVictoryBonusLabel;

	[SerializeField]
	private GameObject winnerTag;

	[SerializeField]
	private GameObject drawTag;

	[SerializeField]
	private GameObject defeatTag;

	[SerializeField]
	private GameObject winnerBonusInfo;

	[Header("Enemy Guild")]
	[SerializeField]
	private UILabel opponentGuildName;

	[SerializeField]
	private UILabel opponentFakeGuildName;

	[SerializeField]
	private UILabel opponentPointsLabel;

	[SerializeField]
	private GameObject opponentWinnerTag;

	[SerializeField]
	private GameObject opponentDrawTag;

	[SerializeField]
	private GameObject opponentDefeatTag;

	[SerializeField]
	private GameObject opponentGuildEmblem;

	[SerializeField]
	private GvGFakeBattleContainer opponentFakeGuildEmblem;

	[Header("Rewards")]
	[SerializeField]
	private UILabel bonusBattlePointsLabel;

	[SerializeField]
	private UILabel bonusBattleRewardPointsLabel;

	[SerializeField]
	private GameObject guildShopNewIndicator;

	[SerializeField]
	private UILabel rpRewardsMultiplier;

	[Header("Container")]
	[SerializeField]
	private GameObject waitForBattleParent;

	[SerializeField]
	private GameObject battleEndParent;

	private TweenTimeline tweenTimeLine = new TweenTimeline();

	private TweenTimeline rewardTimeLine = new TweenTimeline();

	private GuildBattleModel shownBattle;

	private bool isVictory;

	private bool isDefeat;

	private bool isDraw;

	private int rewardPointsForPlayer;

	private int selectedRewardsTweenGroup;

	private bool animationsCompleted;

	private bool battleSolved;

	private float refreshInterval = 0.5f;

	private float refreshTimer;

	public static bool CanShowFullPopup()
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("GuildBattleEndPopup").Enabled)
		{
			return false;
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup);
		HUDElement noCreation2 = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.SocialPopupGuild);
		HUDElement noCreation3 = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleHighscorePopup);
		bool flag = noCreation2 != null && noCreation2.IsOpen;
		bool flag2 = noCreation3 != null && noCreation3.IsOpen;
		if (noCreation != null && noCreation.IsOpen && !noCreation.IsClosing && !flag && !flag2)
		{
			return GuildWarHelper.CanShowBattleEnd();
		}
		return false;
	}

	public static bool CanShowOnlyRewardsPopup()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer();
		bool flag2 = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle();
		if (flag)
		{
			return !flag2;
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		shownBattle = GuildWarHelper.GetCurrentBattle();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GvGStartBattleFlowPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MapTeamSelection);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (guildBattleMapPopup != null && guildBattleMapPopup.IsOpen)
		{
			GuildBattleMapView viewInstance = guildBattleMapPopup.GetViewInstance();
			if (viewInstance != null)
			{
				viewInstance.Clear();
			}
		}
		OpenWaitingForBattleEnd();
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OpenWaitingForBattleEnd()
	{
		animationsCompleted = false;
		TweenManager.PlayTweenGroup(base.gameObject, introTweenGroup);
		UpdateUI();
	}

	private void OpenBattleEnded()
	{
		isVictory = shownBattle.IsVictory();
		isDefeat = shownBattle.IsDefeat();
		isDraw = shownBattle.IsDraw();
		selectedRewardsTweenGroup = ((isVictory || isDraw) ? battleRewardsTweenGroup : loserRewardsTweenGroup);
		rewardPointsForPlayer = 0;
		if (isVictory || isDraw)
		{
			List<RewardCurrency> claimableBattleRewardsClientSide = GuildWarHelper.GetClaimableBattleRewardsClientSide();
			rewardPointsForPlayer = ((claimableBattleRewardsClientSide.Count > 0) ? claimableBattleRewardsClientSide[claimableBattleRewardsClientSide.Count - 1].Amount : 0);
		}
		if (shownBattle.IsFakeBattle)
		{
			Helpers.GameObjectSetActive(opponentGuildEmblem, value: false);
			Helpers.GameObjectSetActive(opponentFakeGuildEmblem, value: true);
			opponentFakeGuildEmblem.Setup();
		}
		else
		{
			Helpers.GameObjectSetActive(opponentFakeGuildEmblem, value: false);
			Helpers.GameObjectSetActive(opponentGuildEmblem, value: true);
		}
		UpdateUI();
		PlayResultsAnimation();
		CampHUD hud = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (hud != null)
		{
			hud.PauseCurrencyMeters = true;
		}
		Helpers.ExecuteCommandDelayed(new ResolveEndBattleCommand(), delegate
		{
			battleSolved = true;
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GuildBattleRP);
			hud.GetHUDMeter(CurrencyType.GuildBattleRP).SetValue(currency.Value - rewardPointsForPlayer);
			GameManager.Instance.GuildManager.UpdateGvGRelatedInfo();
			SingularityMonoBehaviour<GuildWarManager>.Instance.CheckGuildWarStatus();
		});
	}

	private void PlayResultsAnimation()
	{
		animationsCompleted = false;
		tweenTimeLine = new TweenTimeline();
		tweenTimeLine.Queue(TweenObjects.Group(base.gameObject, introTweenGroup));
		tweenTimeLine.Queue(TweenObjects.Group(base.gameObject, scoresTweenGroup));
		tweenTimeLine.Play();
	}

	private void PlayTierProgressionAnimation()
	{
		if (guildTierProgressBar != null)
		{
			guildTierProgressBar.OnComplete(PlayCollectAnimation);
			guildTierProgressBar.PlayProgressionUpdate();
		}
		else
		{
			PlayCollectAnimation();
		}
	}

	private void PlayRewardsAnimation()
	{
		HelpersUI.SetContentToLabel(bonusBattleRewardPointsLabel, rewardPointsForPlayer.ToString());
		if (shownBattle.IsVictory())
		{
			HelpersUI.SetContentToLabel(bonusBattlePointsLabel, shownBattle.GetBattleWonBonusVictoryPoints().ToString());
			HelpersUI.SetContentToLabel(rpRewardsMultiplier, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.BattleEnd.RewardsMutliplier{parameter}", GuildWarHelper.GetCurrentBattle().GetGuildBattleVictoryRewardPointsMultiplier() + 1f));
		}
		else if (shownBattle.IsDraw())
		{
			HelpersUI.SetContentToLabel(bonusBattlePointsLabel, shownBattle.GetBattleDrawPoints().ToString());
			HelpersUI.SetContentToLabel(rpRewardsMultiplier, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.BattleEnd.RewardsMutliplier{parameter}", GuildWarHelper.GetCurrentBattle().GetGuildBattleDrawRewardPointsMultiplier() + 1f));
		}
		else
		{
			HelpersUI.SetContentToLabel(bonusBattlePointsLabel, "0");
			HelpersUI.SetContentToLabel(rpRewardsMultiplier, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.BattleEnd.RewardsMutliplier{parameter}", 0));
		}
		rewardTimeLine = new TweenTimeline();
		rewardTimeLine.Add(TweenObjects.Group(base.gameObject, rewardsTweenGroup));
		rewardTimeLine.Add(TweenObjects.Group(base.gameObject, selectedRewardsTweenGroup));
		rewardTimeLine.OnComplete(PlayTierProgressionAnimation);
		rewardTimeLine.Play();
		guildName.alignment = NGUIText.Alignment.Center;
	}

	private void PlayCollectAnimation()
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(CurrencyType.GuildBattleRP, bonusBattleRewardPointsLabel.gameObject, rewardPointsForPlayer, null, BuildingsHUD.CollectSoundTrigger.OnStart, bonusBattleRewardPointsLabel.gameObject);
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GuildBattleRP);
			campHUD.GetHUDMeter(CurrencyType.GuildBattleRP).SetValue(currency.Value);
			campHUD.PauseCurrencyMeters = false;
		}
		animationsCompleted = true;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(waitForBattleParent, !GuildWarHelper.HasCurrentBattleEnded());
		Helpers.GameObjectSetActive(battleEndParent, GuildWarHelper.HasCurrentBattleEnded());
		if (!GuildWarHelper.HasCurrentBattleEnded())
		{
			return;
		}
		Helpers.GameObjectSetActive(guildShopNewIndicator, value: false);
		Helpers.GameObjectSetActive(winnerBonusInfo, value: false);
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		HelpersUI.SetContentToLabel(guildName, guildModel.Name);
		HelpersUI.SetContentToLabel(guildBattlePointsLabel, shownBattle.EndVictoryPoints.ToString());
		if (shownBattle.IsFakeBattle)
		{
			FakeBattleDefinition fakeBattleDefinition = GameManager.Instance.gameEconomyData.FindFakeBattleDefinition(shownBattle.GuildTier);
			if (fakeBattleDefinition != null)
			{
				HelpersUI.SetContentToLabel(opponentPointsLabel, fakeBattleDefinition.TargetScore.ToString());
			}
			HelpersUI.SetContentToLabel(opponentFakeGuildName, shownBattle.EnemyGuildName);
		}
		else
		{
			HelpersUI.SetContentToLabel(opponentGuildName, shownBattle.EnemyGuildName);
			HelpersUI.SetContentToLabel(opponentPointsLabel, shownBattle.EndEnemyVictoryPoints.ToString());
		}
		if (guildTierProgressBar != null)
		{
			guildTierProgressBar.UpdateToOldProgression();
		}
		Helpers.GameObjectSetActive(winnerTag, isVictory);
		Helpers.GameObjectSetActive(defeatTag, isDefeat);
		Helpers.GameObjectSetActive(opponentWinnerTag, isDefeat);
		Helpers.GameObjectSetActive(opponentDefeatTag, isVictory);
		Helpers.GameObjectSetActive(drawTag, isDraw);
		Helpers.GameObjectSetActive(opponentDrawTag, isDraw);
	}

	public override void Update()
	{
		base.Update();
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0f)
		{
			if (!battleSolved && GuildWarHelper.HasCurrentBattleEnded())
			{
				OpenBattleEnded();
			}
			refreshTimer = refreshInterval;
		}
	}

	public override void Close()
	{
		base.Close();
		UIEvent.OnUIEvent -= OnUIEvent;
		SingularityMonoBehaviour<GuildWarManager>.Instance.CheckGuildWarStatus();
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.PauseCurrencyMeters = false;
		}
	}

	public void OnClickNext()
	{
		TweenManager.ResetToBeginningTweenGroup(base.gameObject, scoresTweenGroup);
		if (!animationsCompleted)
		{
			PlayRewardsAnimation();
			return;
		}
		TweenManager.FinishTweenGroup(base.gameObject, rewardsTweenGroup);
		TweenManager.FinishTweenGroup(base.gameObject, selectedRewardsTweenGroup);
	}

	public void OnClickStats()
	{
		animationsCompleted = true;
		rewardTimeLine.Stop();
		TweenManager.FinishTweenGroup(base.gameObject, scoresTweenGroup);
		TweenManager.ResetToBeginningTweenGroup(base.gameObject, rewardsTweenGroup);
		TweenManager.ResetToBeginningTweenGroup(base.gameObject, selectedRewardsTweenGroup);
	}

	public void OnClickGuildShop()
	{
		Close();
		GuildShopPopup.OpenGuildShop();
	}

	private void ShowNewShopIndicator()
	{
		Helpers.GameObjectSetActive(guildShopNewIndicator, value: true);
	}

	private void AnimateVictoryPointsLabel()
	{
		HelpersUI.AnimateLabel(guildBattlePointsLabel, int.Parse(guildBattlePointsLabel.text), shownBattle.FinalVictoryPoints, 1f, PlayTierProgressionAnimation);
	}

	public override void OnBackButtonClicked()
	{
		if (animationsCompleted)
		{
			OnClickClose();
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnGuildTierIncreased")
		{
			ShowNewShopIndicator();
		}
	}
}
