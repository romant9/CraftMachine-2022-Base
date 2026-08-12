using TWD.Externals;
using TWDModel;
using UnityEngine;

public class ReturnLoginTaskItem : UIListCard<ReturnLoginTaskItem.TaskData>
{
	public enum TaskStatus
	{
		Claimable = 0,
		Incomplete = 1,
		Completed = 2
	}

	public class TaskData
	{
		public ReturnDailyQuestItemModel DailyTask;

		public ReturnRepeatQuestItemModel RepeatTask;

		public ReturnRepeatQuestModel RepeatQuest;

		public bool IsDaily => DailyTask != null;

		public int DefinitionId
		{
			get
			{
				if (!IsDaily)
				{
					if (RepeatTask == null)
					{
						return 0;
					}
					return RepeatTask.DefinitionId;
				}
				return DailyTask.DefinitionId;
			}
		}

		public int CurrentProgress
		{
			get
			{
				if (!IsDaily)
				{
					if (RepeatTask == null)
					{
						return 0;
					}
					return RepeatTask.CurrentProgress;
				}
				if (DailyTask == null)
				{
					return 0;
				}
				return DailyTask.CurrentProgress;
			}
		}

		public int RequiredAmount => ReturnQuestRuleHelper.GetRequiredAmount((!IsDaily) ? RepeatTask?.Definition?.Params : DailyTask?.Definition?.Params);

		public string Description
		{
			get
			{
				if (!IsDaily)
				{
					return RepeatTask?.Definition?.DisplayDescription;
				}
				return DailyTask?.Definition?.DisplayDescription;
			}
		}

		public string DeepLink
		{
			get
			{
				if (!IsDaily)
				{
					return RepeatTask?.Definition?.DeepLink;
				}
				return DailyTask?.Definition?.DeepLink;
			}
		}

		public Rewards RewardEntries
		{
			get
			{
				if (!IsDaily)
				{
					return RepeatTask?.Definition?.RewardEntries;
				}
				return DailyTask?.Definition?.RewardEntries;
			}
		}

		public TaskStatus Status
		{
			get
			{
				int requiredAmount = RequiredAmount;
				if (IsDaily)
				{
					if (DailyTask == null)
					{
						return TaskStatus.Incomplete;
					}
					if (DailyTask.Claimed)
					{
						return TaskStatus.Completed;
					}
					if (DailyTask.CurrentProgress < requiredAmount)
					{
						return TaskStatus.Incomplete;
					}
					return TaskStatus.Claimable;
				}
				if (RepeatTask == null || RepeatQuest == null)
				{
					return TaskStatus.Incomplete;
				}
				if (RepeatQuest.GetRemainingCount(RepeatTask.DefinitionId) == 0)
				{
					return TaskStatus.Completed;
				}
				if (RepeatTask.CurrentProgress < requiredAmount)
				{
					return TaskStatus.Incomplete;
				}
				return TaskStatus.Claimable;
			}
		}
	}

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UILabel rewardAmount;

	[SerializeField]
	private UILabel taskLabel;

	[Header("Action button (single button, state driven)")]
	[SerializeField]
	private UIButton actionButton;

	[SerializeField]
	private UILabel actionButtonLabel;

	private string claimSpriteName = "button_detail_purchase_button_10";

	private string goSpriteName = "button_go_button_2";

	private string completedSpriteName = "button_Completed_button_4";

	[SerializeField]
	private GameObject collectAnimationPrefab;

	private Transform _collectTarget;

	public TaskData Data => base.Item;

	public void SetCollectTarget(Transform collectTarget)
	{
		_collectTarget = collectTarget;
	}

	private void Awake()
	{
		if (actionButton != null)
		{
			EventDelegate.Set(actionButton.onClick, OnActionButtonClicked);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			UpdateDescription();
			UpdateRewardDisplay();
			UpdateButtonState();
		}
	}

	public override int GetSortValue()
	{
		if (base.Item == null)
		{
			return -1;
		}
		int num = (int)(2 - base.Item.Status) * 10000000;
		int num2 = (base.Item.IsDaily ? 1000000 : 0);
		return num + num2 - base.Item.DefinitionId;
	}

	private void UpdateDescription()
	{
		string text = LocalizationManager.GetText(base.Item.Description);
		if (base.Item.Status != TaskStatus.Completed)
		{
			int num = Mathf.Min(base.Item.CurrentProgress, base.Item.RequiredAmount);
			text = $"{text} ({num}/{base.Item.RequiredAmount})";
		}
		HelpersUI.SetContentToLabel(taskLabel, text);
	}

	private void UpdateRewardDisplay()
	{
		if (base.Item.RewardEntries == null || base.Item.RewardEntries.Count <= 0)
		{
			return;
		}
		IReward rewardAt = base.Item.RewardEntries.GetRewardAt(0);
		if (rewardAt != null)
		{
			HelpersGfx.GetIconNameForIReward(rewardAt, out var spriteName, null, null, null);
			HelpersUI.SetSprite(rewardIcon, spriteName);
			if (rewardAmount != null && rewardAt is RewardCurrency rewardCurrency)
			{
				HelpersUI.SetContentToLabel(rewardAmount, Helpers.FormatNumber(rewardCurrency.Amount, 0, 1));
			}
		}
	}

	private void UpdateButtonState()
	{
		string normalSprite;
		string textId;
		bool isEnabled;
		switch (base.Item.Status)
		{
		case TaskStatus.Claimable:
			normalSprite = claimSpriteName;
			textId = "return.task.button.claim";
			isEnabled = true;
			break;
		case TaskStatus.Incomplete:
			normalSprite = goSpriteName;
			textId = "return.task.button.go";
			isEnabled = true;
			break;
		default:
			normalSprite = completedSpriteName;
			textId = "return.quest.completed";
			isEnabled = false;
			break;
		}
		if (actionButton != null)
		{
			actionButton.normalSprite = normalSprite;
			actionButton.isEnabled = isEnabled;
		}
		HelpersUI.SetContentToLabel(actionButtonLabel, LocalizationManager.GetText(textId));
	}

	private void OnActionButtonClicked()
	{
		if (base.Item != null)
		{
			switch (base.Item.Status)
			{
			case TaskStatus.Claimable:
				Claim();
				break;
			case TaskStatus.Incomplete:
				Jump();
				break;
			}
		}
	}

	private void Claim()
	{
		if (base.Item != null && (base.Item.IsDaily ? Helpers.ExecuteCommand(new ClaimReturnDailyQuestRewardCommand(base.Item.DefinitionId)) : Helpers.ExecuteCommand(new ClaimReturnRepeatQuestRewardCommand(base.Item.DefinitionId))) == TWDModelResult.OK)
		{
			PlayClaimEffect();
			UpdateUI();
		}
	}

	private void Jump()
	{
		if (!string.IsNullOrEmpty(base.Item.DeepLink))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ReturnLoginPopup)?.Close();
			DeepLinkNavigation.HandleDeepLink(base.Item.DeepLink);
		}
	}

	private void PlayClaimEffect()
	{
		if (base.Item.RewardEntries == null || base.Item.RewardEntries.Count <= 0 || rewardIcon == null || !(base.Item.RewardEntries.GetRewardAt(0) is RewardCurrency rewardCurrency))
		{
			return;
		}
		if (collectAnimationPrefab != null && _collectTarget != null)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(collectAnimationPrefab, _collectTarget.parent.gameObject);
			CollectAnimation collectAnimation = gameObject?.GetComponent<CollectAnimation>();
			if (collectAnimation != null)
			{
				collectAnimation.FollowTarget(rewardIcon.gameObject);
				collectAnimation.StartAnimation(rewardCurrency.Amount, rewardCurrency.CurrencyType, _collectTarget);
				collectAnimation.SetSprite(rewardCurrency.CurrencyType);
				NGUITools.AdjustDepth(gameObject, 100);
				return;
			}
		}
		BuildingsHUD.Get()?.CreateCollectAnim(rewardCurrency.CurrencyType, rewardIcon.gameObject, rewardCurrency.Amount);
	}
}
