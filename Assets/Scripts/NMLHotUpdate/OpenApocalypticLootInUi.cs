using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OpenApocalypticLootInUi : HUDElement
{
	private const float secondsToWaitToContineAfterBoxOpened = 2.2f;

	[SerializeField]
	private UIButton closeButton;

	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UIChallengeRewardsWidgetList rewardsList;

	[SerializeField]
	private GameObject currencyEffectTarget;

	private List<LootEntry> lootEntryList = new List<LootEntry>();

	private bool isCollected;

	public static int DefaultBackgroundHeight = 380;

	public override void Open()
	{
		if (base.IsOpen)
		{
			return;
		}
		base.Open();
		isCollected = false;
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
		{
			lootEntryList.Clear();
			ClaimChallengeRewardsCommand claimChallengeRewardsCommand = new ClaimChallengeRewardsCommand(LootEntryType.ChallengeRoundCompletionReward, LootEntryType.ChallengePersonalReward);
			if (Helpers.ExecuteCommand(claimChallengeRewardsCommand) == TWDModelResult.OK)
			{
				lootEntryList = claimChallengeRewardsCommand.LootEntries;
			}
		}
		if (title != null)
		{
			title.text = LocalizationManager.GetText("Popup.Quest.YourRewardTitle");
		}
		if (messageLabel != null)
		{
			messageLabel.text = LocalizationManager.GetText("Popup.OpenLoot.Message.TradeCrate");
			Helpers.GameObjectSetActive(messageLabel, value: true);
		}
		LootScreenType screenType = LootScreenType.InUi;
		if (background != null)
		{
			background.height = DefaultBackgroundHeight;
		}
		closeButton.gameObject.SetActive(value: false);
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
			RewardScreenHandler.Instance.OnRewardBoxOpened += OnRewardBoxOpened;
			RewardScreenHandler.Instance.ShowScene(screenType, null, isApocalypticCrate: true);
		}
	}

	public override void Close()
	{
		CampHUD.Get().PauseCurrencyMeters = false;
		base.Close();
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.HideScene();
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
		}
		if (CampView.Instance != null)
		{
			CampView.Instance.Hud.UpdateIndicators();
		}
	}

	public override void OnBackButtonClicked()
	{
		Close();
	}

	protected override void OnCloseAnimOver()
	{
		base.OnCloseAnimOver();
		if (rewardsList != null)
		{
			rewardsList.ClearCards();
		}
	}

	private void OnRewardBoxOpened(GameObject box, LootEntry reward, LootEntry reward2)
	{
		OnPlayClaimAni();
		StartCoroutine(DelayShowButtonAfterRewards());
	}

	private IEnumerator DelayShowButtonAfterRewards()
	{
		yield return new WaitForSeconds(2.2f);
		Helpers.GameObjectSetActive(messageLabel, value: false);
		if (lootEntryList != null && rewardsList != null)
		{
			rewardsList.ClearCards();
			for (int i = 0; i < lootEntryList.Count; i++)
			{
				if (lootEntryList[i] != null)
				{
					rewardsList.CreateItemForLootEntry(lootEntryList[i]);
				}
			}
			rewardsList.Position();
		}
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.HideScene();
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
		}
		closeButton.gameObject.SetActive(value: true);
	}

	private void OnPlayClaimAni()
	{
		if (isCollected)
		{
			return;
		}
		isCollected = true;
		if (lootEntryList == null)
		{
			return;
		}
		int num = ((lootEntryList.Count > 5) ? 5 : lootEntryList.Count);
		for (int i = 0; i < num; i++)
		{
			if (lootEntryList[i] != null)
			{
				LootEntry lootEntry = lootEntryList[i];
				if (lootEntry != null && currencyEffectTarget != null)
				{
					CampView.Instance.BuildingsHud.CreateCollectAnim(lootEntry.RewardedCurrency, currencyEffectTarget, lootEntry.RewardedAmount);
				}
			}
		}
	}

	public static void TryOpenOnLootEnter()
	{
		bool isChallengeApocalypticMode90RoundRewards = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge.IsChallengeApocalypticMode90RoundRewards;
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		bool flag = weeklyChallengeModel != null && weeklyChallengeModel.GetRewardsPerType(LootEntryType.ChallengeRoundCompletionReward, LootEntryType.ChallengePersonalReward).Count > 0;
		if (isChallengeApocalypticMode90RoundRewards && flag)
		{
			OpenApocalypticLootInUi openApocalypticLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenApocalypticLootInUi) as OpenApocalypticLootInUi;
			if (openApocalypticLootInUi != null)
			{
				openApocalypticLootInUi.Open();
			}
		}
	}
}
