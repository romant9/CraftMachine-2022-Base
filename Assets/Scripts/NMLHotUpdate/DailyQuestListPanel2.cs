using BaseModel;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class DailyQuestListPanel2 : ScrollableListPanel<DailyQuestItemModel>
{
	[SerializeField]
	private UILabel nextDailyQuestTimerLabel;

	[SerializeField]
	private UILabel questNameLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UIButton claimButton;

	[SerializeField]
	private UIButton goButton;

	[SerializeField]
	private UIProgressBar chestProgressBar;

	[SerializeField]
	private UILabel chestProgressLabel;

	[SerializeField]
	private UILabel questProgressLabel;

	[SerializeField]
	private UIProgressBar questProgressBar;

	[SerializeField]
	private GameObject questCollectTarget;

	[SerializeField]
	private UIButton claimQuestChest;

	[SerializeField]
	private UISprite questRewardIcon;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private GameObject questTrophyRewards;

	[SerializeField]
	private UILabel questTrophyRewardAmountLabel;

	[SerializeField]
	private GameObject questRewards;

	[SerializeField]
	private UILabel questRewardAmountLabel;

	[SerializeField]
	private UILabel pointsRewardedFromCompleteAllLabel;

	[SerializeField]
	private UITexture questChestTexture;

	[SerializeField]
	private Material questChestSilverMaterial;

	[SerializeField]
	private Material questChestGoldMaterial;

	[SerializeField]
	private Material questChestClassTokenMaterial;

	[SerializeField]
	private Material questChestHeroTokenMaterial;

	[SerializeField]
	private UILabel questCompleteLabel;

	private int nextDailyQuestTimer;

	private DailyQuestListCard2 selectedCard;

	private void SelectFirstCard()
	{
		if (cards.Count > 0 && cards[cards.Count - 1] is DailyQuestListCard2)
		{
			OnQuestSelected(cards[cards.Count - 1] as DailyQuestListCard2);
		}
		else
		{
			OnQuestSelected(null);
		}
	}

	public void Init()
	{
		if (GameManager.Instance.playerModel.DailyQuestManager != null)
		{
			SetCards(GameManager.Instance.playerModel.DailyQuestManager.ActiveQuests);
			UpdateCardLayouts();
			SelectQuestChestVisual();
			GameManager.Instance.playerModel.DailyQuestManager.Changed += OnDailyQuestsChanged;
			UpdateProgressBar();
			SelectFirstCard();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		}
		SetNavigationButtonState();
	}

	private void SetNavigationButtonState()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatVictoryScreen, null, createIfNotExist: false);
		if (hUDElement != null && hUDElement.IsOpen)
		{
			goButton.isEnabled = false;
		}
		else
		{
			goButton.isEnabled = selectedCard == null || !selectedCard.Item.IsCompleteAllQuest;
		}
	}

	public void OnGoClicked()
	{
		if (!(selectedCard != null))
		{
			return;
		}
		string text = ((selectedCard.Item.Definition != null) ? selectedCard.Item.Definition.DeepLink : null);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (!DeepLinkNavigation.HandleDeepLink(text))
		{
			Debug.LogError($"Invalid deep link {text} in daily quest {selectedCard.Item.Id}.");
			return;
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.QuestsPopup);
		if (noCreation != null)
		{
			noCreation.Close();
		}
	}

	private void SelectQuestChestVisual()
	{
		Material material = questChestGoldMaterial;
		switch (GameManager.Instance.playerModel.DailyQuestManager.CurrentQuestChestDefinition.Tag)
		{
		case DropEventDefinition.DropEventTag.QuestChestSilver:
			material = questChestSilverMaterial;
			break;
		case DropEventDefinition.DropEventTag.QuestChestClassToken:
			material = questChestClassTokenMaterial;
			break;
		case DropEventDefinition.DropEventTag.QuestChestHeroToken:
			material = questChestHeroTokenMaterial;
			break;
		}
		if (questChestTexture != null)
		{
			questChestTexture.material = material;
		}
	}

	private void UpdateProgressBar()
	{
		if (chestProgressBar != null)
		{
			int questPoints = GameManager.Instance.playerModel.DailyQuestManager.QuestPoints;
			int questPointsRequired = GameManager.Instance.playerModel.DailyQuestManager.CurrentQuestChestDefinition.QuestPointsRequired;
			float num = (float)questPoints / (float)questPointsRequired;
			chestProgressBar.Set((num > 1f) ? 1f : num);
			chestProgressLabel.text = $"{questPoints}/{questPointsRequired}";
			claimQuestChest.gameObject.SetActive(questPoints >= questPointsRequired);
		}
	}

	public void OnClaimChest()
	{
		DailyQuestModel dailyQuestManager = GameManager.Instance.playerModel.DailyQuestManager;
		if (dailyQuestManager == null || dailyQuestManager.CurrentQuestChestDefinition == null)
		{
			Debug.LogError("No daily quest or reward chest currently selected.");
			return;
		}
		int questPoints = dailyQuestManager.QuestPoints;
		int questPointsRequired = dailyQuestManager.CurrentQuestChestDefinition.QuestPointsRequired;
		if (questPoints >= questPointsRequired && dailyQuestManager.QuestChestRewards.Count == 0)
		{
			Helpers.ExecuteCommand(new ClaimQuestChestCommand());
		}
	}

	public void OnClaimQuest()
	{
		if (!(selectedCard != null) || !selectedCard.Item.IsCompleted)
		{
			return;
		}
		BuildingsHUD buildingsHUD = BuildingsHUD.Get();
		IReward reward = selectedCard.Item.Rewards?.GetRewardAt(0);
		if (Helpers.ExecuteCommand(new ClaimDailyQuestCommand(selectedCard.Item.ModelId)) == TWDModelResult.OK && buildingsHUD != null)
		{
			if (reward != null && questRewardIcon != null && reward is RewardCurrency rewardCurrency)
			{
				buildingsHUD.CreateCollectAnim(rewardCurrency.CurrencyType, questRewardIcon.gameObject, rewardCurrency.Amount);
			}
			else if (reward is RewardEquipment equipment)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForConsumable(equipment, "Popup.IAPConfirm.Title.GenericReward");
			}
			else if (reward is RewardTimedBonus timedReward)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForTimedReward(timedReward, "Popup.IAPConfirm.Title.GenericReward");
			}
			CollectAnimation collectAnimation = buildingsHUD.InstantiateQuestPoint();
			if (collectAnimation != null && questRewardIcon != null && questCollectTarget != null)
			{
				collectAnimation.FollowTarget(questRewardIcon.gameObject);
				collectAnimation.StartAnimation(0, questCollectTarget.transform);
			}
		}
		UpdateProgressBar();
		SelectFirstCard();
	}

	private void OnQuestSelected(DailyQuestListCard2 card)
	{
		selectedCard = card;
		bool flag = false;
		bool flag2 = false;
		string content = "";
		string content2 = "";
		if (card != null)
		{
			content = card.GetLocalizedDisplayName();
			content2 = card.GetLocalizedDisplayDescription();
			flag = card.Item.IsCompleted;
			flag2 = card.Item.Claimed;
			DailyQuestItemModel item = card.Item;
			int completedCount = item.CompletedCount;
			int completionTotalCap = item.CompletionTotalCap;
			questProgressBar.Set((completionTotalCap == 0) ? 1f : ((float)completedCount / (float)completionTotalCap));
			questProgressLabel.text = $"{completedCount}/{completionTotalCap}";
			bool flag3 = !flag2;
			Helpers.GameObjectSetActive(questRewards, flag3);
			bool flag4 = card.Item.Rewards?.GetRewardAt(0) is RewardEquipment;
			if (flag3)
			{
				card.GetRewardIconAndAmount(out var rewardIconName, out var rewardAmount);
				if (flag4)
				{
					weaponTexture.mainTexture = UnityUtils.LoadFromAssetBundle<Texture>(rewardIconName, "itemgraphics");
					Helpers.GameObjectSetActive(questRewardIcon, value: false);
					Helpers.GameObjectSetActive(weaponTexture, value: true);
				}
				else
				{
					HelpersUI.SetSprite(questRewardIcon, rewardIconName);
					Helpers.GameObjectSetActive(weaponTexture, value: false);
					Helpers.GameObjectSetActive(questRewardIcon, value: true);
				}
				HelpersUI.SetContentToLabel(questRewardAmountLabel, Helpers.FormatNumber(rewardAmount, 0, 1));
				HelpersUI.SetContentToLabel(questTrophyRewardAmountLabel, selectedCard.Item.DetermineQuestPointsFromComplete().ToString());
			}
			Helpers.GameObjectSetActive(claimButton, flag3 && flag);
			Helpers.GameObjectSetActive(questTrophyRewards, flag3 && !selectedCard.Item.IsCompleteAllQuest);
			Helpers.GameObjectSetActive(questCompleteLabel, flag2);
		}
		HelpersUI.SetContentToLabel(questNameLabel, content);
		HelpersUI.SetContentToLabel(descriptionLabel, content2);
		Helpers.GameObjectSetActive(goButton, !flag);
		for (int i = 0; i < cards.Count; i++)
		{
			DailyQuestListCard2 dailyQuestListCard = cards[i] as DailyQuestListCard2;
			if (dailyQuestListCard != null && dailyQuestListCard.gameObject != null)
			{
				dailyQuestListCard.IsSelected = dailyQuestListCard == card;
				dailyQuestListCard.UpdateUI();
			}
		}
		SetNavigationButtonState();
	}

	private void OnDailyQuestsChanged(ModelObject model, string changed, object args)
	{
		OnQuestSelected(null);
		SetCards(GameManager.Instance.playerModel.DailyQuestManager.ActiveQuests);
		UpdateCardLayouts();
		SelectFirstCard();
		SelectQuestChestVisual();
	}

	private void UpdateCardLayouts()
	{
		DailyQuestModel dailyQuestManager = GameManager.Instance.playerModel.DailyQuestManager;
		DailyQuestRewardSetDefinition dailyQuestRewardSetDefinition = ((dailyQuestManager != null) ? GameManager.Instance.gameEconomyData.GetDailyQuestRewardSetDefinition(dailyQuestManager.RewardSetId) : null);
		if (dailyQuestRewardSetDefinition != null)
		{
			HelpersUI.SetContentToLabel(pointsRewardedFromCompleteAllLabel, dailyQuestRewardSetDefinition.PointsFromFinishAll.ToString());
		}
		for (int i = 0; i < cards.Count; i++)
		{
			UIListCard<DailyQuestItemModel> uIListCard = cards[i];
			if (uIListCard != null && uIListCard.gameObject != null)
			{
				uIListCard.UpdateUI();
				uIListCard.GetComponent<DailyQuestListCard2>().OnCardSelected += OnQuestSelected;
			}
		}
	}

	public void Update()
	{
		DailyQuestModel dailyQuestManager = GameManager.Instance.playerModel.DailyQuestManager;
		OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi, null, createIfNotExist: false) as OpenLootInUi;
		if (dailyQuestManager.QuestChestRewards.Count > 0 && (openLootInUi == null || !openLootInUi.IsOpen))
		{
			openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.OpenForModel(dailyQuestManager);
			}
			else
			{
				Debug.LogError("Could not find UIType.OpenLootInUi.");
			}
			UpdateProgressBar();
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance.playerModel.DailyQuestManager != null)
		{
			GameManager.Instance.playerModel.DailyQuestManager.Changed -= OnDailyQuestsChanged;
		}
	}
}
