using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeActivityReward : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private CampReward campReward;

	[SerializeField]
	private UILabel limitLabel;

	[SerializeField]
	private UISprite consumeIcon;

	[SerializeField]
	private UILabel consumeLabel;

	[SerializeField]
	private UIButton rewardButton;

	[SerializeField]
	private UIButton redDotButton;

	[SerializeField]
	private GameObject checkObj;

	[SerializeField]
	private GameObject soldOutObj;

	private ClassTeamExchangeDefinition _exchangeDefinition;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "WeeklyChallengeActivityRewardEvent")
		{
			UpdateUI(_exchangeDefinition);
		}
	}

	public void UpdateUI(ClassTeamExchangeDefinition definition)
	{
		_exchangeDefinition = definition;
		campReward.Bind(definition.ContentRewards.RewardsList[0]);
		if (definition.Limit != -1)
		{
			Helpers.GameObjectSetActive(limitLabel.gameObject, value: true);
			int boughtCount = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Shop.GetBoughtCount(definition.ID);
			HelpersUI.SetContentToLabel(limitLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.ExchangeLimit", definition.Limit - boughtCount, definition.Limit));
			if (boughtCount >= definition.Limit)
			{
				Helpers.GameObjectSetActive(soldOutObj, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(soldOutObj, value: false);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(limitLabel.gameObject, value: false);
		}
		HelpersUI.SetContentToLabel(titleLabel, HelpersLocalization.GetRewardLocalizedName(_exchangeDefinition.ContentRewards.RewardsList[0], 0));
		if (_exchangeDefinition.CostRewards.RewardsList[0] is RewardCurrency rewardCurrency)
		{
			consumeIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			HelpersUI.SetContentToLabel(consumeLabel, rewardCurrency.Amount.ToString());
			if (rewardCurrency.Amount > GameManager.Instance.playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType))
			{
				rewardButton.isEnabled = false;
			}
			else
			{
				rewardButton.isEnabled = true;
			}
		}
		GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Shop.ExchangeReminderStates.TryGetValue(_exchangeDefinition.ID, out var value);
		Helpers.GameObjectSetActive(checkObj, value);
	}

	public void OnRewardClick()
	{
		if (!(_exchangeDefinition.CostRewards.RewardsList[0] is RewardCurrency rewardCurrency) || rewardCurrency.Amount > GameManager.Instance.playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType) || (_exchangeDefinition.Limit != -1 && GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Shop.GetBoughtCount(_exchangeDefinition.ID) >= _exchangeDefinition.Limit))
		{
			return;
		}
		BuyWeeklyChallengeClassTeamExchangeCommand buyWeeklyChallengeClassTeamExchangeCommand = new BuyWeeklyChallengeClassTeamExchangeCommand();
		buyWeeklyChallengeClassTeamExchangeCommand.ExchangeId = _exchangeDefinition.ID;
		if (Helpers.ExecuteCommand(buyWeeklyChallengeClassTeamExchangeCommand) == TWDModelResult.OK)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(buyWeeklyChallengeClassTeamExchangeCommand.Rewards.RewardsList);
			}
			UIEvent.Send("WeeklyChallengeActivityRewardEvent");
		}
	}

	public void OnRedDotClick()
	{
		Dictionary<int, bool> exchangeReminderStates = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.Shop.ExchangeReminderStates;
		exchangeReminderStates.TryGetValue(_exchangeDefinition.ID, out var value);
		if (Helpers.ExecuteCommand(new SetWeeklyChallengeClassTeamExchangeReminderCommand
		{
			ExchangeId = _exchangeDefinition.ID,
			Enabled = !value
		}) == TWDModelResult.OK)
		{
			exchangeReminderStates.TryGetValue(_exchangeDefinition.ID, out value);
			Helpers.GameObjectSetActive(checkObj, value);
		}
	}
}
