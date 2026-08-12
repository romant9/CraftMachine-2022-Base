using BaseModel;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class NewBieQuestListPanel : ScrollableListPanel<DailyQuestItemModel>
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
	private UILabel questCompleteLabel;

	[SerializeField]
	private GameObject newBieRewardCard;

	[SerializeField]
	private GameObject newBieRewardCardContent;

	private int nextDailyQuestTimer;

	private NewBieQuestListCard selectedCard;

	private int day;

	private void SelectFirstCard()
	{
		if (cards.Count > 0 && cards[cards.Count - 1] is NewBieQuestListCard)
		{
			OnQuestSelected(cards[cards.Count - 1] as NewBieQuestListCard);
		}
		else
		{
			OnQuestSelected(null);
		}
	}

	public void Init(int dayNum)
	{
		day = 0;
		day = dayNum;
		SetCards(GameManager.Instance.playerModel.NewbieSenvenQuest.Quests[day]);
		UpdateCardLayouts();
		SelectQuestChestVisual();
		GameManager.Instance.playerModel.NewbieSenvenQuest.Changed -= OnDailyQuestsChanged;
		GameManager.Instance.playerModel.NewbieSenvenQuest.Changed += OnDailyQuestsChanged;
		UpdateProgressBar();
		SelectFirstCard();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
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
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.NewBieQuestsPopup);
		if (noCreation != null)
		{
			noCreation.Close();
		}
	}

	private void SelectQuestChestVisual()
	{
	}

	private void UpdateProgressBar()
	{
		if (chestProgressBar != null)
		{
			int questPoints = GameManager.Instance.playerModel.NewbieSenvenQuest.QuestPoints;
			NewbieStageReward[] newbieStageRewards = GameManager.Instance.gameEconomyData.NewbieStageRewards;
			int pointNeeded = newbieStageRewards[^1].PointNeeded;
			float num = -170f;
			float num2 = 400f + num;
			float y = 98f;
			newBieRewardCardContent.RemoveAllChildren();
			for (int i = 0; i < newbieStageRewards.Length; i++)
			{
				float num3 = (float)newbieStageRewards[i].PointNeeded / (float)pointNeeded;
				num3 = ((num3 > 1f) ? 1f : num3);
				float x = num + num3 * (num2 - num);
				GameObject obj = Helpers.InstantiateToParent(newBieRewardCard, newBieRewardCardContent);
				obj.transform.localPosition = new Vector3(x, y, 0f);
				obj.GetComponent<NewBieRewardCard>().UpdateUI(newbieStageRewards[i], questPoints, i == newbieStageRewards.Length - 1);
			}
			float num4 = (float)questPoints / (float)pointNeeded;
			chestProgressBar.Set((num4 > 1f) ? 1f : num4);
			chestProgressLabel.text = $"{questPoints}/{pointNeeded}";
		}
	}

	public void OnClaimChest()
	{
		NewbieStageReward[] newbieStageRewards = GameManager.Instance.gameEconomyData.NewbieStageRewards;
		if (Helpers.ExecuteCommand(new NewbieSevenQuestStageRewardCommand(newbieStageRewards[0].PointNeeded)) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(newbieStageRewards[0].RewardEntries.RewardsList);
			}
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
		if (Helpers.ExecuteCommand(new NewbieSevenQuestCailmRewardCommand(selectedCard.Item.SlotIndex)) == TWDModelResult.OK && buildingsHUD != null)
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
			CollectAnimation collectAnimation = buildingsHUD.InstantiateNewBiePoint();
			if (collectAnimation != null && questRewardIcon != null && questCollectTarget != null)
			{
				collectAnimation.FollowTarget(questRewardIcon.gameObject);
				collectAnimation.StartAnimation(0, questCollectTarget.transform);
			}
		}
		UpdateProgressBar();
		SelectFirstCard();
	}

	private void OnQuestSelected(NewBieQuestListCard card)
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
				if (rewardAmount <= 0)
				{
					rewardAmount = 1;
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
			NewBieQuestListCard newBieQuestListCard = cards[i] as NewBieQuestListCard;
			if (newBieQuestListCard != null && newBieQuestListCard.gameObject != null)
			{
				newBieQuestListCard.IsSelected = newBieQuestListCard == card;
				newBieQuestListCard.UpdateUI();
				if (day + 1 > GameManager.Instance.playerModel.NewbieSenvenQuest.UnlockDay)
				{
					newBieQuestListCard.SetBgGray();
				}
			}
		}
		SetNavigationButtonState();
	}

	private void OnDailyQuestsChanged(ModelObject model, string changed, object args)
	{
		OnQuestSelected(null);
		SetCards(GameManager.Instance.playerModel.NewbieSenvenQuest.Quests[day]);
		UpdateCardLayouts();
		SelectFirstCard();
		SelectQuestChestVisual();
	}

	private void UpdateCardLayouts()
	{
		for (int i = 0; i < cards.Count; i++)
		{
			UIListCard<DailyQuestItemModel> uIListCard = cards[i];
			if (uIListCard != null && uIListCard.gameObject != null)
			{
				uIListCard.UpdateUI();
				uIListCard.GetComponent<NewBieQuestListCard>().OnCardSelected += OnQuestSelected;
			}
		}
	}

	public void Update()
	{
	}

	private void OnDisable()
	{
		if (GameManager.Instance.playerModel.NewbieSenvenQuest != null)
		{
			GameManager.Instance.playerModel.NewbieSenvenQuest.Changed -= OnDailyQuestsChanged;
		}
	}
}
