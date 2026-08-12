using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassCampManager : MonoBehaviour
{
	private BattlePassModel battlePass;

	private PlayerModel player;

	private BuildingModel council;

	private ITimingManager timingManager;

	private IDisposable refreshInterval;

	private bool premiumFromSupportClaimed;

	[SerializeField]
	private GameObject newSeasonBadgeContainer;

	[Tooltip("Used to control the battle pass progress bar out of way for building menu")]
	[SerializeField]
	private TweenScale battlePassContainerTween;

	[SerializeField]
	private GameObject battlePassProgressBarButtonObject;

	[SerializeField]
	private BattlePassProgressBar battlePassProgressBar;

	private bool isInBuildingMode;

	private void Awake()
	{
		player = GameManager.Instance.playerModel;
		battlePass = GameManager.Instance.playerModel.BattlePass;
		timingManager = GameManager.Instance.TimingManager;
		council = player.Camp.GetBuilding("Council");
	}

	private void Start()
	{
		if (!premiumFromSupportClaimed && battlePass.PremiumFromSupport)
		{
			BattlePassClientHelpers.StartPremiumActivationFlow();
			premiumFromSupportClaimed = true;
		}
		RefreshNewSeasonBadgeState();
		Helpers.ExecuteCommand(new BattlePassSeasonRefreshCommand());
	}

	private void CouncilOnChanged(ModelObject model, string changed, object args)
	{
		if (changed == "level")
		{
			RefreshNewSeasonBadgeState();
			if ((bool)battlePassProgressBar)
			{
				battlePassProgressBar.Refresh();
			}
		}
	}

	private void OnEnable()
	{
		refreshInterval?.Dispose();
		refreshInterval = timingManager.Interval(TimeSpan.FromSeconds(1.0), RefreshTimerStates);
		battlePass.Changed += BattlePassOnChanged;
		UIEvent.OnUIEvent += OnUIEvent;
		timingManager.Timer(TimeSpan.FromSeconds(1.0), ShowUnclaimedRewardsPromptIfThereAreAny);
		council.Changed += CouncilOnChanged;
	}

	private void OnDisable()
	{
		refreshInterval?.Dispose();
		refreshInterval = null;
		if (battlePass != null)
		{
			battlePass.Changed -= BattlePassOnChanged;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
		council.Changed -= CouncilOnChanged;
	}

	private void BattlePassOnChanged(ModelObject model, string changed, object args)
	{
		if (changed == "SeasonChanged")
		{
			ShowUnclaimedRewardsPromptIfThereAreAny();
			RefreshNewSeasonBadgeState();
		}
	}

	private void RefreshTimerStates()
	{
		long utcTimeStamp = player.UtcTimeStamp;
		if ((battlePass.IsSeasonActive && utcTimeStamp >= battlePass.CurrentSeasonEndDate) || (utcTimeStamp >= battlePass.NextSeasonStartDate && battlePass.NextSeasonStartDate > 0) || (player.BeginnerBattlePassInfo.State != BeginnerBattlePassState.Ongoing && battlePass.IsBeginnerBattlePass))
		{
			Helpers.ExecuteCommand(new BattlePassSeasonRefreshCommand());
		}
		if (battlePass.IsSeasonActive && utcTimeStamp >= battlePass.KillCapExpiryDateMilliseconds)
		{
			Helpers.ExecuteCommand(new BattlePassDailyCapRefreshCommand());
		}
	}

	private void ShowUnclaimedRewardsPromptIfThereAreAny()
	{
		if (TWDPlayerPrefs.GetInt("LastBattlePassSeasonAutoConvertShown", -1) == battlePass.CurrentSeasonId)
		{
			return;
		}
		Rewards grantedUnclaimedRewards = battlePass.GrantedUnclaimedRewards;
		if (grantedUnclaimedRewards == null || grantedUnclaimedRewards.Count <= 0)
		{
			return;
		}
		Rewards rewards = new Rewards();
		List<RewardCurrency> convertRewards = new List<RewardCurrency>();
		foreach (IReward rewards2 in battlePass.GrantedUnclaimedRewards.RewardsList)
		{
			if (rewards2 is RewardCurrency rewardCurrency && GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType) && rewardCurrency.WasConverted)
			{
				convertRewards.Add(rewardCurrency);
			}
			else
			{
				rewards.RewardsList.Add(rewards2);
			}
		}
		if (rewards.Count > 0)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if ((bool)iAPConfirmPopupNew)
			{
				iAPConfirmPopupNew.OpenForRewards(rewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.BattlePass.RewardAutoClaim.Title"), LocalizationManager.GetText("Popup.BattlePass.RewardAutoClaim.Subtitle"));
				iAPConfirmPopupNew.SetCallbacks(delegate
				{
					ConvertTokens(convertRewards);
				}, delegate
				{
					ConvertTokens(convertRewards);
				});
			}
		}
		else
		{
			ConvertTokens(convertRewards);
		}
		TWDPlayerPrefs.SetInt("LastBattlePassSeasonAutoConvertShown", battlePass.CurrentSeasonId);
	}

	private void ConvertTokens(List<RewardCurrency> convertRewards)
	{
		if (convertRewards.Any((RewardCurrency currency) => currency.Amount > 0))
		{
			MultipleTokenConversionPopup multipleTokenConversionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MultipleTokenConversionPopup) as MultipleTokenConversionPopup;
			if ((bool)multipleTokenConversionPopup)
			{
				multipleTokenConversionPopup.OpenForCurrencies(convertRewards, shouldDisplayDeclineOption: false);
				multipleTokenConversionPopup.EnableCloseArea(enable: false);
			}
		}
	}

	private void OnUIEvent(string type, object param)
	{
		if (type == "OnBattlePassOpened")
		{
			if (player.IsInPreBeginnerBattlePassState())
			{
				Helpers.ExecuteCommand(new BeginnerBattlePassStartCommand());
			}
			Helpers.GameObjectSetActive(newSeasonBadgeContainer, value: false);
		}
		else if (type == "OnContextMenuBoxOpened" && param is BuildingMenu buildingMenu)
		{
			if (buildingMenu.BuildingView.Model is CageBuildingModel)
			{
				if (battlePassContainerTween != null)
				{
					battlePassContainerTween.PlayForward();
				}
				SetBattlePassButtonResponsive(responsive: false);
			}
		}
		else if (type == "OnContextMenuBoxClosed" && param is BuildingMenu buildingMenu2)
		{
			if (buildingMenu2.BuildingView.Model is CageBuildingModel)
			{
				if (battlePassContainerTween != null)
				{
					battlePassContainerTween.PlayReverse();
				}
				SetBattlePassButtonResponsive(responsive: true);
			}
		}
		else if (type == "OnBuildingConstructionRequested")
		{
			if (battlePassContainerTween != null)
			{
				battlePassContainerTween.PlayForward();
			}
			SetBattlePassButtonResponsive(responsive: false);
			isInBuildingMode = true;
		}
		else
		{
			if (!isInBuildingMode)
			{
				return;
			}
			switch (type)
			{
			case "OnBuildingMoveConfirmed":
			case "OnBuildingMoveCancelled":
			case "OnBuildingMoveEnded":
			case "OnPopUpOpen":
				if (battlePassContainerTween != null)
				{
					battlePassContainerTween.PlayReverse();
				}
				SetBattlePassButtonResponsive(responsive: true);
				isInBuildingMode = false;
				break;
			}
		}
	}

	private void SetBattlePassButtonResponsive(bool responsive)
	{
		if (battlePassProgressBarButtonObject != null)
		{
			if (battlePassProgressBarButtonObject.TryGetComponent<UIButton>(out var component))
			{
				component.enabled = responsive;
			}
			if (battlePassProgressBarButtonObject.TryGetComponent<BoxCollider>(out var component2))
			{
				component2.enabled = responsive;
			}
		}
	}

	private void RefreshNewSeasonBadgeState()
	{
		Helpers.GameObjectSetActive(newSeasonBadgeContainer, (battlePass.IsSeasonActive && battlePass.ReachedTier < 0 && !battlePass.AtMaxTier) || player.IsInPreBeginnerBattlePassState());
	}
}
