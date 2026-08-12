using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ReturnLoginShopPanel : ScrollableListPanel<ReturnExchangeStoreDefinition>
{
	[SerializeField]
	private ReturnLoginShopFixedDetailPanel fixedDetailPanel;

	[SerializeField]
	private ReturnLoginShopRefreshDetailPanel refreshDetailPanel;

	private long _nextRefreshTimeLeft;

	private ReturnExchangeStoreModel _model;

	private int _selectedExchangeId = -1;

	private int _selectedIndex = -1;

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		RefreshList();
		SubscribeModelChanges(subscribe: true);
	}

	public void Close()
	{
		SubscribeModelChanges(subscribe: false);
		ClearCards();
		HideDetails();
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		SubscribeModelChanges(subscribe: false);
		HideDetails();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginShopItemSelectedEvent" && parameter is ReturnExchangeStoreDefinition definition)
		{
			ShowDetail(definition);
		}
	}

	private void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "ReturnExchangeStoreChanged" && base.gameObject.activeInHierarchy)
		{
			RefreshCardsInPlace();
		}
	}

	private void SubscribeModelChanges(bool subscribe)
	{
		if (_model != null)
		{
			_model.Changed -= OnModelChanged;
			if (subscribe)
			{
				_model.Changed += OnModelChanged;
			}
		}
	}

	protected override void SetCard(UIListCard<ReturnExchangeStoreDefinition> card)
	{
		if (card is ReturnLoginShopItem returnLoginShopItem)
		{
			returnLoginShopItem.SetContext(_model);
		}
	}

	private void RefreshList()
	{
		ReturnExchangeStoreModel model = GetModel();
		if (_model != model)
		{
			SubscribeModelChanges(subscribe: false);
			_model = model;
			SubscribeModelChanges(subscribe: true);
		}
		if (_model == null)
		{
			ClearCards();
			HideDetails();
			return;
		}
		List<ReturnExchangeStoreDefinition> exchangeDefinitions = _model.ExchangeDefinitions;
		Helpers.GameObjectSetActive(cardPrefab, value: true);
		SetCards(exchangeDefinitions);
		Helpers.GameObjectSetActive(cardPrefab, value: false);
		long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
		_nextRefreshTimeLeft = Math.Max(_model.NextRefreshTimestamp - valueOrDefault, 0L);
		if (exchangeDefinitions != null && exchangeDefinitions.Count > 0)
		{
			UIEvent.Send("ReturnLoginShopItemSelectedEvent", exchangeDefinitions[0]);
		}
		else
		{
			HideDetails();
		}
	}

	private void RefreshCardsInPlace()
	{
		ReturnExchangeStoreModel model = GetModel();
		if (_model != model)
		{
			SubscribeModelChanges(subscribe: false);
			_model = model;
			SubscribeModelChanges(subscribe: true);
		}
		if (_model == null)
		{
			ClearCards();
			HideDetails();
			return;
		}
		List<ReturnExchangeStoreDefinition> exchangeDefinitions = _model.ExchangeDefinitions;
		if (exchangeDefinitions == null || exchangeDefinitions.Count == 0)
		{
			ClearCards();
			HideDetails();
			return;
		}
		if (cards.Count != exchangeDefinitions.Count)
		{
			Helpers.GameObjectSetActive(cardPrefab, value: true);
			SetCards(exchangeDefinitions, resetScrollView: false);
			Helpers.GameObjectSetActive(cardPrefab, value: false);
		}
		else
		{
			for (int i = 0; i < cards.Count; i++)
			{
				cards[i].Item = exchangeDefinitions[i];
				if (cards[i] is ReturnLoginShopItem returnLoginShopItem)
				{
					returnLoginShopItem.SetContext(_model);
				}
				cards[i].UpdateUI();
			}
		}
		long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
		_nextRefreshTimeLeft = Math.Max(_model.NextRefreshTimestamp - valueOrDefault, 0L);
		ReturnExchangeStoreDefinition returnExchangeStoreDefinition = FindDefinitionById(exchangeDefinitions, _selectedExchangeId);
		if (returnExchangeStoreDefinition == null && _selectedIndex >= 0 && _selectedIndex < exchangeDefinitions.Count)
		{
			returnExchangeStoreDefinition = exchangeDefinitions[_selectedIndex];
		}
		if (returnExchangeStoreDefinition == null)
		{
			returnExchangeStoreDefinition = exchangeDefinitions[0];
		}
		UIEvent.Send("ReturnLoginShopItemSelectedEvent", returnExchangeStoreDefinition);
	}

	private void ShowDetail(ReturnExchangeStoreDefinition definition)
	{
		if (_model == null || definition == null)
		{
			HideDetails();
			return;
		}
		_selectedExchangeId = definition.Id;
		_selectedIndex = FindDefinitionIndex(_model.ExchangeDefinitions, definition.Id);
		if (definition.Type == ReturnExchangeStoreType.Fixed)
		{
			refreshDetailPanel?.Hide();
			fixedDetailPanel?.Show(definition, _model);
		}
		else
		{
			fixedDetailPanel?.Hide();
			refreshDetailPanel?.Show(definition, _model);
		}
	}

	private void HideDetails()
	{
		_selectedExchangeId = -1;
		_selectedIndex = -1;
		fixedDetailPanel?.Hide();
		refreshDetailPanel?.Hide();
	}

	private ReturnExchangeStoreDefinition FindDefinitionById(List<ReturnExchangeStoreDefinition> definitions, int exchangeId)
	{
		int num = FindDefinitionIndex(definitions, exchangeId);
		if (num < 0)
		{
			return null;
		}
		return definitions[num];
	}

	private int FindDefinitionIndex(List<ReturnExchangeStoreDefinition> definitions, int exchangeId)
	{
		if (definitions == null || exchangeId < 0)
		{
			return -1;
		}
		for (int i = 0; i < definitions.Count; i++)
		{
			if (definitions[i] != null && definitions[i].Id == exchangeId)
			{
				return i;
			}
		}
		return -1;
	}

	public static void ShowRewardPopup(Rewards rewards)
	{
		if (rewards != null && rewards.Count > 0)
		{
			RecycleWeaponRewardsPopup recycleWeaponRewardsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RecycleWeaponRewardsPopup) as RecycleWeaponRewardsPopup;
			if (recycleWeaponRewardsPopup != null)
			{
				recycleWeaponRewardsPopup.SetupRewards(rewards);
				recycleWeaponRewardsPopup.Open();
			}
		}
	}

	public static bool IsCurrencyInsufficient(ReturnExchangeStoreDefinition definition)
	{
		if (definition?.CostRewardEntries?.RewardsList == null)
		{
			return false;
		}
		for (int i = 0; i < definition.CostRewardEntries.RewardsList.Count; i++)
		{
			if (definition.CostRewardEntries.RewardsList[i] is RewardCurrency rewardCurrency)
			{
				CurrencyModel currencyModel = GameManager.Instance?.playerModel?.GetCurrency(rewardCurrency.CurrencyType);
				if (currencyModel == null || currencyModel.Value < rewardCurrency.Amount)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void Apply(IReward reward, UISprite icon, EquipmentButton equipButton, ReturnLoginShopRewardModSkillItem modSkillItem)
	{
		bool flag = false;
		bool flag2 = false;
		if (reward is RewardEquipment rewardEquipment && equipButton != null)
		{
			Helpers.GameObjectSetActive(equipButton.gameObject, value: true);
			equipButton.Setup(rewardEquipment);
			flag = true;
		}
		else if (reward is RewardRemoldSkill reward2 && modSkillItem != null)
		{
			flag2 = modSkillItem.Setup(reward2);
		}
		if (!flag && equipButton != null)
		{
			Helpers.GameObjectSetActive(equipButton.gameObject, value: false);
		}
		if (!flag2 && modSkillItem != null)
		{
			modSkillItem.Hide();
		}
		if (!(icon == null))
		{
			if (reward == null || flag || flag2)
			{
				Helpers.GameObjectSetActive(icon.gameObject, value: false);
				return;
			}
			Helpers.GameObjectSetActive(icon.gameObject, value: true);
			HelpersGfx.GetIconNameForIReward(reward, out var spriteName, null, null, null);
			HelpersUI.SetSprite(icon, spriteName);
		}
	}

	private static ReturnExchangeStoreModel GetModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnQuestAndExchange?.ExchangeStore;
	}
}
