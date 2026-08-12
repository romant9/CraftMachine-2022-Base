using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ActiveFoundationDayPanel : MonoBehaviour
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
	private TimerComponent seasonTimer;

	[SerializeField]
	private UIScrollView scrollView;

	[SerializeField]
	private float scrollOffset;

	[SerializeField]
	private UILabel[] currentTierLabels;

	[SerializeField]
	private UILabel maxTierLabel;

	[Header("Bookmark Stuff")]
	[SerializeField]
	private GameObject currentTierBookmarkLeftObject;

	[SerializeField]
	private GameObject currentTierBookmarkRightObject;

	[SerializeField]
	private float bookmarkTravelTimeSeconds;

	[SerializeField]
	private GameObject premiumNormal;

	[SerializeField]
	private GameObject premiumClaimed;

	[SerializeField]
	private GameObject premiumClaimable;

	private ActiveFoundationManager activeFoundation;

	private IList<ActiveFoundationDayEntry> spawnedEntries;

	private void Awake()
	{
		spawnedEntries = new List<ActiveFoundationDayEntry>();
		activeFoundation = GameManager.Instance.playerModel.ActiveFoundationManager;
	}

	private void OnEnable()
	{
		if (activeFoundation != null && activeFoundation.CurrentPeriodModel != null)
		{
			Open();
			activeFoundation.CurrentPeriodModel.Changed += OnChange;
		}
	}

	private void OnDisable()
	{
		if (activeFoundation != null && activeFoundation.CurrentPeriodModel != null)
		{
			activeFoundation.CurrentPeriodModel.Changed -= OnChange;
		}
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
		int count = activeFoundation.CurrentPeriodModel.RewardDays.Count;
		for (int i = 0; i < count; i++)
		{
			ActiveFoundationDayEntry component = Helpers.InstantiateToParentAndLayer(activeFoundation.CurrentPeriodModel.GetIsPremiumRewardSpecialForTier(i) ? lastTierEntryPrefab : entryPrefab, parent).GetComponent<ActiveFoundationDayEntry>();
			component.Bind(i, activeFoundation.CurrentPeriodModel.GetIsPremiumRewardSpecialForTier(i));
			spawnedEntries.Add(component);
		}
		entryParent.sorting = UIGrid.Sorting.None;
		entryParent.enabled = true;
		HelpersUI.SetContentToLabel(maxTierLabel, count.ToString());
		RefreshPremiumButtonState();
		RefreshReachStates();
		int initDay = activeFoundation.CurrentPeriodModel.CurrentDay;
		GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(0.10000000149011612), delegate
		{
			ScrollTo(initDay);
		});
	}

	private void Update()
	{
		RefreshSeasonTimer();
		RefreshBookmarkStates();
	}

	private void RefreshSeasonTimer()
	{
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(activeFoundation.CurrentPeriodModel.CurrentPeriodEndTimeUtc - activeFoundation.manager.Player.UtcTime.TotalMilliseconds());
		seasonTimer.Set(timeSpan);
	}

	private void OnChange(ModelObject model, string changed, object args)
	{
		if (changed == "ActiveFoundationChangeToday")
		{
			RefreshPremiumButtonState();
			RefreshReachStates();
		}
	}

	public void PremiumBuyClick()
	{
		if (!activeFoundation.CurrentPeriodModel.IsUnlockPremium)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActiveFoundationPremiumPurchaseInfoPopup).Open();
		}
	}

	public void PremiumPreviewClick()
	{
		if (activeFoundation.CurrentPeriodModel.IsUnlockPremium)
		{
			if (!activeFoundation.CurrentPeriodModel.HaveClaimedPremiumExtraRewards && Helpers.ExecuteCommand(new ActiveFoundationClaimPremiumExtraRewardCommand()) == TWDModelResult.OK)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if (iAPConfirmPopupNew != null)
				{
					iAPConfirmPopupNew.OpenForRewards(activeFoundation.CurrentPeriodModel.PremiumExtraRewards.RewardsList);
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
				}
				RefreshPremiumButtonState();
			}
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActiveFoundationPremiumPreviewPopup).Open();
		}
	}

	private void RefreshPremiumButtonState()
	{
		Helpers.GameObjectSetActive(premiumNormal, value: false);
		Helpers.GameObjectSetActive(premiumClaimed, value: false);
		Helpers.GameObjectSetActive(premiumClaimable, value: false);
		if (activeFoundation.CurrentPeriodModel.IsUnlockPremium)
		{
			if (activeFoundation.CurrentPeriodModel.HaveClaimedPremiumExtraRewards)
			{
				Helpers.GameObjectSetActive(premiumNormal, value: false);
				Helpers.GameObjectSetActive(premiumClaimed, value: true);
				Helpers.GameObjectSetActive(premiumClaimable, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(premiumNormal, value: false);
				Helpers.GameObjectSetActive(premiumClaimed, value: false);
				Helpers.GameObjectSetActive(premiumClaimable, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(premiumNormal, value: true);
			Helpers.GameObjectSetActive(premiumClaimed, value: false);
			Helpers.GameObjectSetActive(premiumClaimable, value: false);
		}
	}

	private void RefreshReachStates()
	{
		foreach (ActiveFoundationDayEntry spawnedEntry in spawnedEntries)
		{
			spawnedEntry.RefreshReachState();
		}
		UILabel[] array = currentTierLabels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = activeFoundation.CurrentPeriodModel.CurrentDay.ToString();
		}
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
		BookmarkState bookmarkState = GetBookmarkState(activeFoundation.CurrentPeriodModel.CurrentDay);
		Helpers.GameObjectSetActive(currentTierBookmarkLeftObject, bookmarkState == BookmarkState.Left);
		Helpers.GameObjectSetActive(currentTierBookmarkRightObject, bookmarkState == BookmarkState.Right);
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
		AnimatedScrollTo(activeFoundation.CurrentPeriodModel.CurrentDay);
	}

	public void HighlightedTierBookmarkClick()
	{
		int count = activeFoundation.CurrentPeriodModel.RewardDays.Count;
		AnimatedScrollTo(count);
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
}
