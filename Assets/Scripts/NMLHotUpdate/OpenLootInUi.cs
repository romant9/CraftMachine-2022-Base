using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class OpenLootInUi : HUDElement
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

	private LootEntry loot;

	private GameObject cardGameobject;

	private GameObject cardGameobject2;

	private bool closedByBackButton;

	public static int DefaultBackgroundHeight = 380;

	public static int GuildGiftBackgroundHeight = 500;

	private bool isDiscordReward;

	public bool ShowShopWhenClosed { get; set; }

	public bool IsPlayerLoot => model is PlayerModel;

	public bool IsTradeCrate => model is LootManagerModel;

	public override void OpenForModel(ModelObject model)
	{
		if (base.IsOpen)
		{
			return;
		}
		base.OpenForModel(model);
		if (title != null)
		{
			title.text = LocalizationManager.GetText("Popup.Quest.YourRewardTitle");
		}
		isDiscordReward = false;
		LootScreenType lootScreenType = LootScreenType.InUi;
		if (model is WeeklyChallengeModel)
		{
			if (WeeklyChallengeHelper.IsNormalChallenge)
			{
				if (GetModel<WeeklyChallengeModel>().CanCollectRewards)
				{
					loot = GetModel<WeeklyChallengeModel>().GetCollectRewards[0];
				}
			}
			else if (GetModel<WeeklyChallengeModel>().CanCollectApocalypticRewards)
			{
				loot = GetModel<WeeklyChallengeModel>().GetCollectApocalypticRewards[0];
			}
		}
		else if (model is WeeklySurvivalModel)
		{
			WeeklySurvivalModel weeklySurvivalModel = GetModel<WeeklySurvivalModel>();
			loot = weeklySurvivalModel.Rewards[0];
			lootScreenType = LootScreenType.InUiSurvival;
		}
		else if (model is BundleManagerModel)
		{
			loot = GetModel<BundleManagerModel>().IAPBonusGiftLootEntry;
			if (loot == null)
			{
				if (GetModel<BundleManagerModel>().WebShopLootEntrys.Count > 0)
				{
					loot = GetModel<BundleManagerModel>().WebShopLootEntrys[0];
				}
				else
				{
					loot = GetModel<BundleManagerModel>().ShareRewardEntrys[0];
					isDiscordReward = true;
				}
			}
			lootScreenType = LootScreenType.IAPBonusGift;
		}
		else if (model is DailyQuestModel)
		{
			DailyQuestModel dailyQuestModel = GetModel<DailyQuestModel>();
			if (dailyQuestModel.QuestChestRewards.Count > 0)
			{
				loot = dailyQuestModel.QuestChestRewards[0];
				lootScreenType = LootScreenType.DailyQuestChest;
			}
		}
		else if (model is BattlePassModel)
		{
			lootScreenType = LootScreenType.BattlePassBonusChest;
			messageLabel.text = LocalizationManager.GetText("Popup.BattlePass.BonusChest.PopupContent");
		}
		else if (IsTradeCrate)
		{
			LootManagerModel lootManagerModel = GetModel<LootManagerModel>();
			if (lootManagerModel.PendingTradeCrates == null || lootManagerModel.PendingTradeCrates.Count <= 0)
			{
				return;
			}
			loot = lootManagerModel.PendingTradeCrates[0];
			lootScreenType = LootScreenType.TradeCrate;
			if (!string.IsNullOrEmpty(loot.GeneratorIdentifier))
			{
				SetupForTrade(loot.GeneratorIdentifier);
			}
		}
		else if (IsPlayerLoot)
		{
			PlayerModel playerModel = GetModel<PlayerModel>();
			if (playerModel != null && playerModel.HasLootBoxesToOpen)
			{
				loot = GetModel<PlayerModel>().LootBoxesToOpen[0];
			}
			lootScreenType = LootScreenType.InUIPlayer;
		}
		if (loot != null && lootScreenType != LootScreenType.BattlePassBonusChest)
		{
			DropEventDefinition dropEventDefinition = loot.DropEventDefinition;
			if (dropEventDefinition != null && dropEventDefinition.Tag != DropEventDefinition.DropEventTag.None && loot.Type != LootEntryType.SurvivalPersonalReward && loot.Type != LootEntryType.SurvivalFullCompletionReward)
			{
				title.text = LocalizationManager.GetText("Popup.OpenLoot.Title." + dropEventDefinition.Tag);
			}
			messageLabel.text = LocalizationManager.GetText("Popup.OpenLoot.Message." + loot.Type, loot.Control.ToString());
		}
		if (background != null)
		{
			background.height = DefaultBackgroundHeight;
		}
		closeButton.gameObject.SetActive(value: false);
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
			RewardScreenHandler.Instance.OnRewardBoxOpened += OnRewardBoxOpened;
			RewardScreenHandler.Instance.ShowScene(lootScreenType, loot);
		}
		closedByBackButton = false;
	}

	private void SetupForTrade(string tradeCrateId)
	{
		if (!string.IsNullOrEmpty(tradeCrateId) && IsTradeCrate && title != null)
		{
			title.text = LocalizationManager.GetText("TradeItems.Name." + tradeCrateId);
		}
	}

	public void SetupForGuildGift()
	{
		if (base.IsOpen)
		{
			return;
		}
		Open();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.PendingGuildGiftsToOpen != null && playerModel.PendingGuildGiftsToOpen.Count > 0)
		{
			GuildGift guildGift = playerModel.PendingGuildGiftsToOpen[0];
			if (title != null)
			{
				title.text = LocalizationManager.GetText("Popup.OpenLoot.Message.GuildGift{playerName}", GameManager.Instance.GetFilteredText(guildGift.SenderName));
			}
			if (messageLabel != null)
			{
				messageLabel.text = GameManager.Instance.GetFilteredText(guildGift.SenderMessage);
			}
		}
		closeButton.gameObject.SetActive(value: false);
		RewardScreenHandler.Instance.ShowScene(LootScreenType.GuildGift);
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
			RewardScreenHandler.Instance.OnRewardBoxOpened += OnRewardBoxOpened;
		}
		if (background != null)
		{
			background.height = GuildGiftBackgroundHeight;
		}
	}

	public override void Close()
	{
		CampHUD.Get().PauseCurrencyMeters = false;
		base.Close();
		DestroyGameObject();
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.HideScene();
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
		}
		if (CampView.Instance != null)
		{
			CampView.Instance.Hud.UpdateIndicators();
		}
		if (!closedByBackButton && !ReopenIfMoreChallengeRewards())
		{
			ReopenIfMoreSurvivalRewards();
		}
		reopenIfMoreIAPAndBananaRewards();
	}

	public void reopenIfMoreIAPAndBananaRewards()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.BundleManager != null && (playerModel.BundleManager.IAPBonusGiftLootEntry != null || playerModel.BundleManager.WebShopLootEntrys.Count > 0 || playerModel.BundleManager.ShareRewardEntrys.Count > 0) && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OpenLootInUi) == null)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.OpenForModel(GameManager.Instance.playerModel.BundleManager);
			}
		}
	}

	public override void OnBackButtonClicked()
	{
		if (IsTradeCrate)
		{
			Helpers.ExecuteCommand(new OpenLootBoxCommand
			{
				ScreenType = LootScreenType.TradeCrate
			});
		}
		closedByBackButton = true;
		Close();
	}

	private bool ReopenIfMoreChallengeRewards()
	{
		if (model is WeeklyChallengeModel weeklyChallengeModel)
		{
			if (weeklyChallengeModel.CanCollectRewards && WeeklyChallengeHelper.IsNormalChallenge)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklyChallengeModel);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
				return true;
			}
			if (weeklyChallengeModel.CanCollectApocalypticRewards && !WeeklyChallengeHelper.IsNormalChallenge)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklyChallengeModel);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
				return true;
			}
		}
		return false;
	}

	private bool ReopenIfMoreSurvivalRewards()
	{
		if (model is WeeklySurvivalModel { CanCollectRewards: not false } weeklySurvivalModel)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklySurvivalModel);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
			return true;
		}
		return false;
	}

	protected override void OnCloseAnimOver()
	{
		base.OnCloseAnimOver();
		if (!closedByBackButton)
		{
			if (IsPlayerLoot)
			{
				if (GameManager.Instance.playerModel.HasLootBoxesToOpen)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(model);
				}
			}
			else if (!IsTradeCrate && loot != null && loot.Type != LootEntryType.IAPBonusGift && loot.Type != LootEntryType.Quiz)
			{
				if (loot.Type == LootEntryType.SurvivalPersonalReward || loot.Type == LootEntryType.SurvivalFullCompletionReward)
				{
					if (GameManager.Instance.playerModel.WeeklySurvival.CanCollectRewards)
					{
						SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(model);
					}
				}
				else
				{
					if (GameManager.Instance.playerModel.WeeklyChallenge.CanCollectRewards && WeeklyChallengeHelper.IsNormalChallenge)
					{
						SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(model);
					}
					if (GameManager.Instance.playerModel.WeeklyChallenge.CanCollectApocalypticRewards && !WeeklyChallengeHelper.IsNormalChallenge)
					{
						SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(model);
					}
				}
			}
		}
		reopenIfMoreIAPAndBananaRewards();
	}

	private void SetCardTransform(GameObject cardObj)
	{
		cardObj.transform.SetParent(SingularityMonoBehaviour<HUDManager>.Instance.UIContainerTopCameras.transform);
		Vector3 localPosition = cardObj.transform.localPosition;
		localPosition.z = 0f;
		cardObj.transform.localPosition = localPosition;
	}

	private void OnRewardBoxOpened(GameObject box, LootEntry reward, LootEntry reward2)
	{
		RewardScreenHandler.LootCardPlacement placement = ((reward2 != null) ? RewardScreenHandler.LootCardPlacement.DualFirst : RewardScreenHandler.LootCardPlacement.Single);
		cardGameobject = RewardScreenHandler.Instance.CreateLootCard(box, reward, base.transform, placement);
		SetCardTransform(cardGameobject);
		if (reward2 != null)
		{
			cardGameobject2 = RewardScreenHandler.Instance.CreateLootCard(box, reward2, base.transform, RewardScreenHandler.LootCardPlacement.DualSecond);
			SetCardTransform(cardGameobject2);
		}
		StartCoroutine(DelayShowButtonAfterRewards());
	}

	private IEnumerator DelayShowButtonAfterRewards()
	{
		yield return new WaitForSeconds(2.2f);
		closeButton.gameObject.SetActive(value: true);
	}

	public void RestTitle(string titleDes)
	{
		if (title != null)
		{
			title.text = LocalizationManager.GetText(titleDes);
		}
	}

	private void OnDestroy()
	{
		DestroyGameObject();
	}

	private void DestroyGameObject()
	{
		if (cardGameobject2 != null)
		{
			Object.Destroy(cardGameobject2);
		}
		if (cardGameobject != null)
		{
			Object.Destroy(cardGameobject);
		}
	}
}
