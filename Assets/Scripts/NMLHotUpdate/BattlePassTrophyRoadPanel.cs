using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassTrophyRoadPanel : MonoBehaviour
{
	private enum BookmarkState
	{
		Inactive = 0,
		Left = 1,
		Right = 2
	}

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private GameObject lastTierEntryPrefab;

	[SerializeField]
	private UIGrid entryParent;

	[SerializeField]
	private UIButton premiumButton;

	[SerializeField]
	private GameObject bonusChestItemPrefab;

	[SerializeField]
	private TimerComponent seasonTimer;

	[SerializeField]
	private GameObject premiumActiveContainer;

	[SerializeField]
	private UIScrollView scrollView;

	[SerializeField]
	private float scrollOffset;

	[SerializeField]
	private UILabel[] currentTierLabels;

	[SerializeField]
	private UILabel maxTierLabel;

	[Header("Next Tier Stuff")]
	[SerializeField]
	private float nextTierInitialXPos;

	[SerializeField]
	private Transform nextTierContainer;

	[SerializeField]
	private UILabel nextTierButtonLabel;

	[SerializeField]
	private float nextTierAnimationSpeed;

	[SerializeField]
	private AnimationCurve nextTierAnimationCurve;

	[Header("Bookmark Stuff")]
	[SerializeField]
	private GameObject currentTierBookmarkLeftObject;

	[SerializeField]
	private GameObject currentTierBookmarkRightObject;

	[SerializeField]
	private GameObject maxTierBookmarkObject;

	[SerializeField]
	private float bookmarkTravelTimeSeconds;

	private BattlePassModel battlePass;

	private IList<BattlePassTrophyRoadEntry> spawnedEntries;

	public const string hasFreeTrackCompleted = "hasFreeTrackCompleted";

	private bool isPremiumOn => GameManager.Instance.playerModel.BattlePass.PremiumActive;

	private int seasonID => GameManager.Instance.playerModel.BattlePass.CurrentSeasonId;

	private void Awake()
	{
		spawnedEntries = new List<BattlePassTrophyRoadEntry>();
		battlePass = GameManager.Instance.playerModel.BattlePass;
	}

	private void OnEnable()
	{
		Open();
		battlePass.Changed += OnChange;
	}

	private void OnDisable()
	{
		battlePass.Changed -= OnChange;
	}

	private void Open()
	{
		Transform transform = entryParent.transform;
		GameObject parent = entryParent.gameObject;
		while (transform.childCount > 0)
		{
			Transform child = transform.GetChild(0);
			child.SetParent(null);
			UnityEngine.Object.Destroy(child.gameObject);
		}
		spawnedEntries.Clear();
		int num = battlePass.TierClaimInfos.Length;
		for (int i = 0; i < num; i++)
		{
			BattlePassTrophyRoadEntry component = Helpers.InstantiateToParent((battlePass.GetIsPremiumRewardSpecialForTier(i) ? lastTierEntryPrefab : entryPrefab).gameObject, parent).GetComponent<BattlePassTrophyRoadEntry>();
			component.Bind(i, battlePass.GetIsPremiumRewardSpecialForTier(i));
			spawnedEntries.Add(component);
		}
		Helpers.InstantiateToParent(bonusChestItemPrefab, parent);
		entryParent.sorting = UIGrid.Sorting.None;
		entryParent.enabled = true;
		HelpersUI.SetContentToLabel(maxTierLabel, (battlePass.LastSpecialRewardTier + 1).ToString());
		RefreshPremiumButtonState();
		RefreshNextTier(isInstant: true);
		ActivePremiumContainer(isPremiumOn);
		GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(0.10000000149011612), delegate
		{
			ScrollTo(battlePass.ReachedTier);
		});
		DailyShowPurchaseInfoPopup();
	}

	private void Update()
	{
		RefreshSeasonTimer();
		RefreshBookmarkStates();
	}

	private void RefreshSeasonTimer()
	{
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(battlePass.CurrentSeasonEndDate - battlePass.manager.Player.UtcTime.TotalMilliseconds());
		seasonTimer.Set(timeSpan);
	}

	private void OnChange(ModelObject model, string changed, object args)
	{
		if (changed == "PremiumActivated")
		{
			RefreshPremiumButtonState();
			RefreshPremiumClaimStates();
			RefreshReachStates();
			HandleTokenConversion(battlePass.GetAllReachedUnclaimedPremiumRewards());
			ActivePremiumContainer(state: true);
		}
	}

	public void PremiumBuyClick()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPremiumPurchaseInfoPopup).Open();
	}

	private void RefreshPremiumButtonState()
	{
		Helpers.GameObjectSetActive(premiumButton, !battlePass.PremiumActive);
	}

	public void BuyNextTierClick()
	{
		if (battlePass.AtMaxTier)
		{
			return;
		}
		BuyResourcesPopup buyResourcesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup) as BuyResourcesPopup;
		if (!(buyResourcesPopup != null))
		{
			return;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
		buyResourcesPopup.SetConfirmContent(LocalizationManager.GetText("Popup.BattlePass.TrophyRoad.TierPurchase.Title"), LocalizationManager.GetText("Popup.BattlePass.TrophyRoad.TierPurchase.Subtitle"), battlePass.CurrentTierUnlockCashier.GetTotalCost(CurrencyType.Diamonds));
		buyResourcesPopup.SetCallbacks(delegate
		{
			if (battlePass.CurrentTierUnlockCashier.CanAfford())
			{
				Helpers.ExecuteCommand(new BattlePassTierPurchaseCommand());
				RefreshNextTier(isInstant: false);
				RefreshReachStates();
			}
			else
			{
				ShopPopupHelper.OpenForMissingCurrencyWithMissingAmount(battlePass.CurrentTierUnlockCashier.GetMissing(CurrencyType.Diamonds));
			}
		});
		buyResourcesPopup.Open();
	}

	private void RefreshNextTier(bool isInstant)
	{
		int num = battlePass.ReachedTier + 1;
		Vector3 localPosition = nextTierContainer.localPosition;
		localPosition.x = nextTierInitialXPos + (float)num * entryParent.cellWidth;
		if (isInstant)
		{
			nextTierContainer.transform.localPosition = localPosition;
		}
		else
		{
			TweenPosition.Begin(nextTierContainer.gameObject, nextTierAnimationSpeed, localPosition, nextTierAnimationCurve);
		}
		nextTierButtonLabel.text = (battlePass.AtMaxTier ? LocalizationManager.GetText("BattlePass.Progress.Max") : battlePass.CurrentTierUnlockCashier.GetTotalCost(CurrencyType.Diamonds).ToString());
		if (battlePass.AtMaxTier && !battlePass.PremiumActive && TWDPlayerPrefs.GetInt("hasFreeTrackCompleted") != seasonID)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPremiumPurchaseInfoPopup).Open();
			TWDPlayerPrefs.SetInt("hasFreeTrackCompleted", seasonID);
			TWDPlayerPrefs.Save();
		}
		UILabel[] array = currentTierLabels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = (battlePass.ReachedTier + 1).ToString();
		}
	}

	private void RefreshReachStates()
	{
		foreach (BattlePassTrophyRoadEntry spawnedEntry in spawnedEntries)
		{
			spawnedEntry.RefreshReachState();
		}
	}

	private void RefreshPremiumClaimStates()
	{
		foreach (BattlePassTrophyRoadEntry spawnedEntry in spawnedEntries)
		{
			spawnedEntry.RefreshPremiumClaimState();
		}
	}

	private void HandleTokenConversion(Rewards rewards)
	{
		List<BattlePassClaimRewardCommand> claimCommands = new List<BattlePassClaimRewardCommand>();
		if (rewards.RewardsList.Count <= 0)
		{
			return;
		}
		List<RewardCurrency> list = new List<RewardCurrency>();
		for (int i = 0; i < rewards.RewardsList.Count; i++)
		{
			RewardCurrency item = rewards.RewardsList[i] as RewardCurrency;
			list.Add(item);
		}
		MultipleTokenConversionPopup multipleTokenConversionPopup = (MultipleTokenConversionPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MultipleTokenConversionPopup);
		multipleTokenConversionPopup.OpenForCurrencies(list);
		for (int j = 0; j <= battlePass.ReachedTier; j++)
		{
			for (int k = 0; k < battlePass.TierClaimInfos[j].PremiumRewardsClaimed.Length; k++)
			{
				if (!battlePass.TierClaimInfos[j].PremiumRewardsClaimed[k])
				{
					BattlePassClaimRewardCommand item2 = new BattlePassClaimRewardCommand
					{
						TierNo = j,
						IsPremium = true,
						RewardIndex = k
					};
					claimCommands.Add(item2);
				}
			}
		}
		TWDModelResult result = TWDModelResult.Error;
		multipleTokenConversionPopup.SetConversionCallbacks(delegate
		{
			result = ExecuteClaimCommands(claimCommands);
			RefreshPremiumClaimStates();
		}, delegate
		{
			result = TWDModelResult.Cancelled;
			RefreshPremiumClaimStates();
		});
	}

	private TWDModelResult ExecuteClaimCommands(List<BattlePassClaimRewardCommand> commands)
	{
		foreach (BattlePassClaimRewardCommand command in commands)
		{
			if (battlePass.GetReward(command.TierNo, command.IsPremium, command.RewardIndex) != null)
			{
				TWDModelResult tWDModelResult = Helpers.ExecuteCommand(command);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
		}
		return TWDModelResult.OK;
	}

	private void ActivePremiumContainer(bool state)
	{
		Helpers.GameObjectSetActive(premiumActiveContainer, state);
	}

	private float CalculateTierScroll(int index)
	{
		return scrollOffset - (float)index * entryParent.cellWidth;
	}

	private void ScrollTo(int tierIndex)
	{
		scrollView.ResetPosition();
		float x = CalculateTierScroll(tierIndex);
		scrollView.MoveRelative(new Vector3(x, 0f));
		scrollView.RestrictWithinBounds(instant: true);
	}

	private BookmarkState GetBookmarkState(int tierIndex)
	{
		if (tierIndex < 0)
		{
			return BookmarkState.Inactive;
		}
		float num = CalculateTierScroll(tierIndex) - scrollView.transform.localPosition.x;
		float num2 = scrollView.bounds.extents.x * 0.5f;
		if (num > num2)
		{
			return BookmarkState.Left;
		}
		if (num < 0f - num2)
		{
			return BookmarkState.Right;
		}
		return BookmarkState.Inactive;
	}

	private void RefreshBookmarkStates()
	{
		BookmarkState bookmarkState = GetBookmarkState(battlePass.ReachedTier);
		Helpers.GameObjectSetActive(currentTierBookmarkLeftObject, bookmarkState == BookmarkState.Left);
		Helpers.GameObjectSetActive(currentTierBookmarkRightObject, bookmarkState == BookmarkState.Right);
		Helpers.GameObjectSetActive(maxTierBookmarkObject, GetBookmarkState(battlePass.LastSpecialRewardTier) == BookmarkState.Right);
	}

	private void AnimatedScrollTo(int tierIndex)
	{
		StopAllCoroutines();
		StartCoroutine(AnimatedScrollToCoroutine(tierIndex));
	}

	private void CancelAnimatedScroll()
	{
		StopAllCoroutines();
	}

	public void CurrentTierBookmarkClick()
	{
		AnimatedScrollTo(battlePass.ReachedTier);
	}

	public void HighlightedTierBookmarkClick()
	{
		AnimatedScrollTo(battlePass.LastSpecialRewardTier);
	}

	private IEnumerator AnimatedScrollToCoroutine(int tierIndex)
	{
		float step = (CalculateTierScroll(tierIndex) - scrollView.transform.localPosition.x) / bookmarkTravelTimeSeconds;
		scrollView.DisableSpring();
		for (float elapsedTime = 0f; elapsedTime < bookmarkTravelTimeSeconds; elapsedTime += Time.unscaledDeltaTime)
		{
			scrollView.currentMomentum = Vector3.zero;
			scrollView.MoveRelative(new Vector3(step * Time.unscaledDeltaTime, 0f));
			scrollView.RestrictWithinBounds(instant: true);
			yield return new WaitForEndOfFrame();
		}
	}

	private void DailyShowPurchaseInfoPopup()
	{
		if (battlePass.CanShowPremiumInfoPopup && Helpers.ExecuteCommand(new BattlePassPremiumInfoPopupViewedCommand()) == TWDModelResult.OK)
		{
			PremiumBuyClick();
		}
	}
}
