using TWDModel;
using UnityEngine;

public class NewBieQuestListCard : UIListCard<DailyQuestItemModel>
{
	public delegate void SelectedCardDelegate(NewBieQuestListCard card);

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel progressBarLabel;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UIButton claimRewardButton;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UILabel rewardAmountLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UISprite timedRewardIcon;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private GameObject rewardContainer;

	[SerializeField]
	private GameObject complete;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color claimedColor;

	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private UISprite backgroundSprite;

	public bool IsSelected { get; set; }

	public event SelectedCardDelegate OnCardSelected;

	public string GetLocalizedDisplayName()
	{
		return LocalizationManager.GetText(base.Item.DisplayName, base.Item.CompletionTotalCap);
	}

	public string GetLocalizedDisplayDescription()
	{
		return LocalizationManager.GetText(base.Item.DisplayDescription, base.Item.CompletionTotalCap);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		if (descriptionLabel != null)
		{
			descriptionLabel.text = GetLocalizedDisplayName();
		}
		progressBarLabel.text = $"{base.Item.CompletedCount}/{base.Item.CompletionTotalCap}";
		progressBar.gameObject.SetActive(!base.Item.Claimed);
		float value = ((base.Item.CompletionTotalCap == 0) ? 1f : ((float)base.Item.CompletedCount / (float)base.Item.CompletionTotalCap));
		progressBar.value = value;
		if (claimRewardButton != null)
		{
			Helpers.GameObjectSetActive(claimRewardButton.gameObject, base.Item.IsCompleted);
		}
		bool flag = base.Item.Rewards?.GetRewardAt(0) is RewardEquipment;
		bool flag2 = base.Item.Rewards?.GetRewardAt(0) is RewardTimedBonus;
		GetRewardIconAndAmount(out var rewardIconName, out var rewardAmount);
		if (flag)
		{
			weaponTexture.mainTexture = UnityUtils.LoadFromAssetBundle<Texture>(rewardIconName, "itemgraphics");
		}
		else if (flag2)
		{
			timedRewardIcon.spriteName = rewardIconName;
		}
		else
		{
			rewardIcon.spriteName = rewardIconName;
		}
		if (rewardAmount <= 0)
		{
			rewardAmount = 1;
		}
		rewardAmountLabel.gameObject.SetActive(rewardAmount > 1);
		rewardAmountLabel.text = Helpers.FormatNumber(rewardAmount, 0, 1);
		if (backgroundSprite != null)
		{
			if (IsSelected)
			{
				backgroundSprite.color = selectedColor;
			}
			else if (base.Item.Claimed)
			{
				backgroundSprite.color = claimedColor;
			}
			else
			{
				backgroundSprite.color = normalColor;
			}
		}
		Helpers.GameObjectSetActive(complete, base.Item.Claimed);
		Helpers.GameObjectSetActive(rewardAmountLabel, !base.Item.Claimed);
		Helpers.GameObjectSetActive(rewardIcon, !base.Item.Claimed && !flag && !flag2);
		Helpers.GameObjectSetActive(timedRewardIcon, !base.Item.Claimed && !flag && flag2);
		Helpers.GameObjectSetActive(weaponTexture, !base.Item.Claimed && flag);
		Helpers.GameObjectSetActive(progressBar, !base.Item.Claimed);
		Helpers.GameObjectSetActive(progressBarLabel, !base.Item.Claimed);
	}

	public void SetBgGray()
	{
		backgroundSprite.color = claimedColor;
	}

	public void GetRewardIconAndAmount(out string rewardIconName, out int rewardAmount)
	{
		rewardIconName = null;
		rewardAmount = 0;
		if (base.Item.Rewards == null)
		{
			rewardIconName = "Ui_Icon_Quest_NewBie";
			rewardAmount = base.Item.DetermineQuestPointsFromComplete();
			return;
		}
		IReward rewardAt = base.Item.Rewards.GetRewardAt(0);
		if (rewardAt != null)
		{
			if (rewardAt is RewardEquipment)
			{
				RewardEquipment rewardEquipment = rewardAt as RewardEquipment;
				rewardIconName = HelpersGfx.GetTextureNameForEquipmentReward(rewardEquipment);
			}
			else
			{
				string spriteName = "";
				HelpersGfx.GetIconNameForIReward(rewardAt, out spriteName, null, null, null, base.Item.manager.Player);
				rewardIconName = spriteName;
			}
			rewardAmount = HelpersGfx.GetAmountForIReward(rewardAt);
		}
	}

	public void OnClicked()
	{
		if (this.OnCardSelected != null)
		{
			this.OnCardSelected(this);
		}
	}

	public override int GetSortValue()
	{
		return base.Item.SortOrder;
	}

	public RewardCurrency GetCurrencyRewardAt(int index)
	{
		if (base.Item.Rewards == null)
		{
			return null;
		}
		return base.Item.Rewards.GetRewardAt(index) as RewardCurrency;
	}

	public void OnClaimRewardClick()
	{
		if (base.Item.IsCompleted)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
			Helpers.ExecuteCommand(new NewbieSevenQuestCailmRewardCommand(base.Item.SlotIndex));
			UpdateUI();
		}
	}
}
