using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DropTableItemRadioCallCard2 : NUIListItem<RadioCallTableItem>
{
	[SerializeField]
	private UILabel labelTableHeader;

	[SerializeField]
	private UILabel labelTableDescription;

	[SerializeField]
	private GameObject specialCallParent;

	[SerializeField]
	private UIGridExtended specialHeroParent;

	[SerializeField]
	private GameObject titleContainer;

	[SerializeField]
	private GameObject tokenIconPrefab;

	[Header("HERO UP!")]
	[SerializeField]
	private GameObject heroUpSection;

	[SerializeField]
	private UIGridExtended heroUpTokenGrid;

	[SerializeField]
	private GameObject includedHeroesSection;

	[SerializeField]
	private UIGridExtended includedHeroesGrid;

	[Header("CALL INFO")]
	[SerializeField]
	private GameObject Tooltip;

	[SerializeField]
	private UIButtonWithLabelAndIcon btnTooltip;

	[SerializeField]
	private UIButton btnCloseTooltip;

	[SerializeField]
	private UILabel labelCostValue;

	[SerializeField]
	private UILabel labelHeroesOnlyValue;

	[SerializeField]
	private UILabel labelRerollValue;

	[SerializeField]
	private UILabel labelRewardValue;

	[Header("AMOUNT & CHANCE")]
	[SerializeField]
	private UITable amountGrid;

	private GameObject amountChanceItemTemplate;

	private readonly List<GameObject> amountChanceItems = new List<GameObject>();

	private void Awake()
	{
		btnTooltip.onClick.Add(new EventDelegate(OnbtnTooltip));
		btnCloseTooltip.onClick.Add(new EventDelegate(OnbtnCloseTooltip));
		amountChanceItemTemplate = Helpers.GameObjectChildItem(amountGrid.gameObject);
	}

	public override void SetData(RadioCallTableItem data)
	{
		base.SetData(data);
		if (labelTableHeader != null)
		{
			labelTableHeader.text = data.DropName;
		}
		if (labelTableDescription != null)
		{
			labelTableDescription.text = data.Description;
		}
		SetupHeroUpSection(data);
		SetupAmountChanceSection(data);
		SetCallInfo(data);
	}

	private void SetupHeroUpSection(RadioCallTableItem data)
	{
		bool flag = !string.IsNullOrEmpty(data.HeroUp);
		Helpers.GameObjectSetActive(heroUpSection, flag);
		Helpers.GameObjectSetActive(includedHeroesSection, flag);
		Helpers.GameObjectSetActive(specialCallParent, !flag);
		if (flag)
		{
			ClearGridChildren(heroUpTokenGrid);
			CreateTokenIcon(data.HeroUp, heroUpTokenGrid);
			if (heroUpTokenGrid != null)
			{
				heroUpTokenGrid.Reposition();
			}
			SetupIncludedHeroesSection(data);
		}
		else
		{
			SetHeroTokenIcons(data.SpecialCallProbabilities);
		}
	}

	private void SetupIncludedHeroesSection(RadioCallTableItem item)
	{
		List<ItemAmountProbabilityData> specialCallProbabilities = item.SpecialCallProbabilities;
		ClearGridChildren(includedHeroesGrid);
		if (includedHeroesGrid == null || tokenIconPrefab == null || specialCallProbabilities == null)
		{
			return;
		}
		for (int i = 0; i < specialCallProbabilities.Count; i++)
		{
			if (!(specialCallProbabilities[i].Name == item.HeroUp))
			{
				CreateTokenIcon(specialCallProbabilities[i].Name, includedHeroesGrid);
			}
		}
		includedHeroesGrid.Reposition();
	}

	private void SetHeroTokenIcons(List<ItemAmountProbabilityData> data)
	{
		ClearGridChildren(specialHeroParent);
		for (int i = 0; i < (data?.Count ?? 0); i++)
		{
			CreateTokenIcon(data[i].Name, specialHeroParent);
		}
		specialHeroParent.Reposition();
	}

	private void SetupAmountChanceSection(RadioCallTableItem data)
	{
		ClearAmountChanceList();
		List<ItemAmountProbabilityData> heroRarityAmounts = data.HeroRarityAmounts;
		if (heroRarityAmounts == null || heroRarityAmounts.Count == 0)
		{
			return;
		}
		for (int i = 0; i < heroRarityAmounts.Count; i++)
		{
			ItemAmountProbabilityData itemAmountProbabilityData = heroRarityAmounts[i];
			if (itemAmountProbabilityData != null)
			{
				GameObject gameObject = amountGrid.gameObject.AddChild(amountChanceItemTemplate);
				if (gameObject.TryGetComponent<DropTableItemRadioCallCard2Item>(out var component))
				{
					string amount = itemAmountProbabilityData.Amount;
					FixedPoint fixedPoint = itemAmountProbabilityData.Probability * 100.0;
					string chance = $"{(float)fixedPoint:0.##}%";
					bool hasBonus = IsAmountEffectEnabled(data.AmountEffect, i);
					component.Setup(amount, chance, hasBonus);
					amountChanceItems.Add(gameObject);
				}
			}
		}
		amountGrid.Reposition();
	}

	private bool IsAmountEffectEnabled(List<string> amountEffect, int index)
	{
		if (amountEffect == null || index < 0 || index >= amountEffect.Count)
		{
			return false;
		}
		return amountEffect[index] == "1";
	}

	private void ClearAmountChanceList()
	{
		for (int i = 0; i < amountChanceItems.Count; i++)
		{
			if (amountChanceItems[i] != null)
			{
				NGUITools.Destroy(amountChanceItems[i]);
			}
		}
		amountChanceItems.Clear();
	}

	public void SetCallInfo(RadioCallTableItem item)
	{
		PhoneCallDefinition callDefinition = item.CallDefinition;
		labelCostValue.text = ((callDefinition.Price > 0) ? callDefinition.Price.ToString() : "--");
		labelHeroesOnlyValue.text = (callDefinition.HeroGuaranteed ? "√" : "×");
		labelRerollValue.text = ((callDefinition.Rerolls > 0) ? callDefinition.Rerolls.ToString() : "--");
		labelRewardValue.text = "3";
	}

	private void CreateTokenIcon(string currencyTypeStr, UIGridExtended parentGrid)
	{
		if (parentGrid == null || tokenIconPrefab == null)
		{
			return;
		}
		CurrencyType currencyType = CurrencyType.None;
		if (Enum.IsDefined(typeof(CurrencyType), currencyTypeStr))
		{
			currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), currencyTypeStr);
		}
		if (currencyType != CurrencyType.None)
		{
			UISprite component = Helpers.InstantiateToParent(tokenIconPrefab, parentGrid.gameObject).GetComponent<UISprite>();
			if (component != null)
			{
				component.spriteName = HelpersGfx.GetTokenCurrencyIconName(currencyType);
			}
		}
	}

	private void ClearGridChildren(UIGridExtended grid)
	{
		if (!(grid == null))
		{
			Transform transform = grid.transform;
			while (transform.childCount > 0)
			{
				Transform child = transform.GetChild(0);
				child.parent = null;
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	private void OnbtnTooltip()
	{
		Helpers.GameObjectSetActive(Tooltip, value: true);
	}

	private void OnbtnCloseTooltip()
	{
		Helpers.GameObjectSetActive(Tooltip, value: false);
	}
}
