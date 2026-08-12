using System;
using System.Collections;
using TWDModel;
using UnityEngine;

public class FakeTrainingGroundsHudButton : MonoBehaviour
{
	[SerializeField]
	private UILabel labelAmountOne;

	[SerializeField]
	private UILabel labelAmountTwo;

	[SerializeField]
	private AnimateNumberFromTo labelAmountOneRunning;

	[SerializeField]
	private GameObject flyTarget;

	[SerializeField]
	private UISprite currencyIconSprite;

	private int ownedAmount;

	private int ownedBeforeAmount;

	private int upgradeTargetAmount;

	private int rewardedAmount;

	private CurrencyType currencyType;

	private SelectSurvivorsPopup.SelectedRewardType rewardType;

	public void Init(RewardCurrency currency)
	{
		if (currency != null)
		{
			currencyType = currency.CurrencyType;
			ownedAmount = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
			rewardedAmount = currency.Amount;
			ownedBeforeAmount = ownedAmount - rewardedAmount;
			HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(currencyType));
			HelpersUI.SetContentToLabel(labelAmountOne, ownedBeforeAmount.ToString());
			HelpersUI.SetContentToLabel(labelAmountTwo, "/" + upgradeTargetAmount, upgradeTargetAmount > 0);
			ShowRewardCollect();
		}
	}

	public void Init(LootEntry entry, SelectSurvivorsPopup.SelectedRewardType selectedRewardType)
	{
		if (entry == null)
		{
			return;
		}
		rewardType = selectedRewardType;
		string text = "";
		text = ((selectedRewardType != SelectSurvivorsPopup.SelectedRewardType.ClassToken || entry.GeneratedSurvivor == null) ? SurvivorToken.GetHeroId(entry.RewardedCurrency) : entry.GeneratedSurvivor.ActorDefinitionID);
		if (text != "")
		{
			bool num = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(text)?.ID.ToLower().Contains("hero_") ?? false;
			switch (selectedRewardType)
			{
			case SelectSurvivorsPopup.SelectedRewardType.HeroToken:
				currencyType = entry.RewardedCurrency;
				ownedAmount = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
				rewardedAmount = entry.RewardedAmount;
				ownedBeforeAmount = ownedAmount - rewardedAmount;
				break;
			case SelectSurvivorsPopup.SelectedRewardType.ClassToken:
				if (entry.GeneratedSurvivor != null)
				{
					currencyType = SurvivorToken.GetClassAsCurrency(entry.GeneratedSurvivor.SurvivorClass);
					SurvivorModel generatedSurvivor = entry.GeneratedSurvivor;
					ownedAmount = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
					rewardedAmount = generatedSurvivor.GetDemoteCashier().GetTotalCost(currencyType);
					ownedBeforeAmount = ownedAmount - rewardedAmount;
				}
				break;
			}
			if (num)
			{
				if (GameManager.Instance.playerModel.SurvivorContainer.HasHero(text))
				{
					SurvivorModel generatedSurvivor = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(text);
					if (generatedSurvivor != null && (generatedSurvivor.CanUpgradeSurvivorRarity() || generatedSurvivor.CanUpgradeTraitRarity()))
					{
						Cashier upgradeTraitCashier = generatedSurvivor.GetUpgradeTraitCashier();
						upgradeTargetAmount = upgradeTraitCashier.GetTotalCost(entry.RewardedCurrency);
					}
				}
				else
				{
					Cashier upgradeTraitCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(entry.RewardedCurrency);
					upgradeTargetAmount = upgradeTraitCashier.GetTotalCost(entry.RewardedCurrency);
				}
			}
			else
			{
				upgradeTargetAmount = 0;
			}
			HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(currencyType));
			HelpersUI.SetContentToLabel(labelAmountOne, ownedBeforeAmount.ToString());
			HelpersUI.SetContentToLabel(labelAmountTwo, "/" + upgradeTargetAmount, upgradeTargetAmount > 0);
			StartCoroutine(WaitForOneFrame(delegate
			{
				labelAmountTwo.GetComponent<UIWidget>().MakePixelPerfect();
			}));
		}
		else
		{
			Debug.LogError("Could not create flying currency for empty ActorDefinition ID");
		}
	}

	public void ShowRewardCollect()
	{
		base.gameObject.SetActive(value: true);
		TweenManager.PlayTweenGroup(base.gameObject, 42, forward: true, OnShowRewardComplete);
	}

	public void OnShowRewardComplete()
	{
		if (labelAmountOneRunning != null)
		{
			labelAmountOneRunning.Animate(ownedBeforeAmount, ownedAmount);
			StartCoroutine(DelayedHide());
		}
	}

	public void Empty()
	{
	}

	private IEnumerator DelayedHide()
	{
		yield return new WaitForSeconds(2f);
		TweenManager.PlayTweenGroup(base.gameObject, 42, forward: false, Empty);
	}

	public void ShowCollect()
	{
		if (rewardType == SelectSurvivorsPopup.SelectedRewardType.ClassToken || rewardType == SelectSurvivorsPopup.SelectedRewardType.HeroToken)
		{
			base.gameObject.SetActive(value: true);
			TweenManager.PlayTweenGroup(base.gameObject, 3, forward: true, TweenComplete);
		}
	}

	public void HideCollect()
	{
		TweenManager.PlayTweenGroup(base.gameObject, 4);
	}

	public GameObject GetIconTarget()
	{
		if (!(flyTarget != null))
		{
			return base.gameObject;
		}
		return flyTarget;
	}

	public int GetRewardAmount()
	{
		return rewardedAmount;
	}

	public CurrencyType GetCurrencyType()
	{
		return currencyType;
	}

	private void TweenComplete()
	{
		if (labelAmountOneRunning != null)
		{
			labelAmountOneRunning.Animate(ownedBeforeAmount, ownedAmount);
		}
	}

	private IEnumerator WaitForOneFrame(Action callback)
	{
		yield return new WaitForEndOfFrame();
		callback();
	}
}
