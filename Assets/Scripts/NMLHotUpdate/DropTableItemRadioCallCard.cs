using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DropTableItemRadioCallCard : NUIListItem<RadioCallTableItem>
{
	[SerializeField]
	private UILabel labelTableHeader;

	[SerializeField]
	private UILabel labelTableDescription;

	[SerializeField]
	private UIDataRowsRadioCall tokenRows;

	[SerializeField]
	private DropTableItemNormalCard survivorRarities;

	[SerializeField]
	private DropTableItemNormalCard heroRarities;

	[SerializeField]
	private UIGridExtended specialHeroParent;

	[SerializeField]
	private UILabel labelSpecialChance;

	[SerializeField]
	private GameObject specialCallParent;

	[SerializeField]
	private GameObject titleContainer;

	[SerializeField]
	private GameObject tokenIconPrefab;

	[SerializeField]
	private UILabel labelSpecialCallHeader;

	[SerializeField]
	private UILabel labelSpecialCallProbability;

	[SerializeField]
	private int TableMarginPx = 10;

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
		if ((data.SpecialCallProbabilities == null || data.SpecialCallProbabilities.Count == 0) && !data.GuarateedHero)
		{
			if (specialCallParent != null)
			{
				BoxCollider componentInChildren = specialCallParent.GetComponentInChildren<BoxCollider>();
				if (componentInChildren != null)
				{
					Vector3 localPosition = titleContainer.transform.localPosition;
					titleContainer.transform.localPosition = new Vector3(localPosition.x, localPosition.y - componentInChildren.size.y, localPosition.z);
				}
			}
			Helpers.GameObjectSetActive(specialCallParent, value: false);
		}
		else if ((data.SpecialCallProbabilities != null && data.SpecialCallProbabilities.Count > 0) || data.GuarateedHero)
		{
			Helpers.GameObjectSetActive(specialCallParent, value: true);
			if (data.SpecialCallProbabilities != null && data.SpecialCallProbabilities.Count > 0)
			{
				SetHeroTokenIcons(data.SpecialCallProbabilities);
				if (labelSpecialChance != null)
				{
					labelSpecialChance.text = $"{(float)data.SpecialCallProbabilities[0].Probability * 100f:0.##}%";
				}
				Helpers.GameObjectSetActive(labelSpecialCallProbability, value: false);
				HelpersUI.SetContentToLabel(labelSpecialCallHeader, data.FeaturedHero ? LocalizationManager.GetText("Droprate.Table.Header.FeaturedHero") : LocalizationManager.GetText("Droprate.Table.Header.SpecialChance"));
			}
			else if (data.GuarateedHero)
			{
				labelSpecialChance.text = "100 %";
				HelpersUI.SetContentToLabel(labelSpecialCallProbability, LocalizationManager.GetText("Droprate.Table.Description.SpecialCall"));
				HelpersUI.SetContentToLabel(labelSpecialCallHeader, LocalizationManager.GetText("Droprate.Table.Header.GuaranteedHero"));
			}
		}
		SetAndPositionProbabilityRows(tokenRows, data.Probabilities);
		SetAndPositionSurvivorRarityRows(survivorRarities.GetComponent<UIDataRows>(), data.SurvivorRarityAmounts);
		SetAndPositionHeroRarityRows(heroRarities.GetComponent<UIDataRows>(), data.HeroRarityAmounts);
	}

	private void SetAndPositionProbabilityRows(UIDataRowsRadioCall dataRowsComponent, List<ItemAmountProbabilityData> probabilities)
	{
		if (!(dataRowsComponent != null) || probabilities == null)
		{
			return;
		}
		for (int i = 0; i < probabilities.Count; i++)
		{
			if (probabilities[i].Probability * 100.0 > 0L)
			{
				tokenRows.SetDataToIndex(i, probabilities[i]);
			}
		}
		dataRowsComponent.PositionRows();
	}

	private void SetAndPositionSurvivorRarityRows(UIDataRows dataRowsComponent, List<ItemAmountProbabilityData> probabilities)
	{
		if (probabilities == null || probabilities.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < probabilities.Count; i++)
		{
			FixedPoint fixedPoint = probabilities[i].Probability * 100.0;
			if (fixedPoint > 0L)
			{
				dataRowsComponent.SetDataToIndex(i, new string[2]
				{
					"",
					$"{(float)fixedPoint:0.##}%"
				}, probabilities[i].Rarity);
			}
		}
		Vector3 localPosition = dataRowsComponent.transform.localPosition;
		dataRowsComponent.transform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)tokenRows.GetRowsHeight() - (float)TableMarginPx, localPosition.z);
		dataRowsComponent.PositionRows();
	}

	private void SetAndPositionHeroRarityRows(UIDataRows dataRowsComponent, List<ItemAmountProbabilityData> probabilities)
	{
		if (probabilities == null || probabilities.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < probabilities.Count; i++)
		{
			FixedPoint fixedPoint = probabilities[i].Probability * 100.0;
			if (fixedPoint > 0L)
			{
				dataRowsComponent.SetDataToIndex(i, new string[2]
				{
					probabilities[i].Amount,
					$"{(float)fixedPoint:0.##}%"
				});
			}
		}
		Vector3 localPosition = dataRowsComponent.transform.localPosition;
		dataRowsComponent.transform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)tokenRows.GetRowsHeight() - (float)TableMarginPx, localPosition.z);
		dataRowsComponent.PositionRows();
	}

	private void SetHeroTokenIcons(List<ItemAmountProbabilityData> data)
	{
		if (!(specialCallParent != null) || !(tokenIconPrefab != null))
		{
			return;
		}
		for (int i = 0; i < (data?.Count ?? 0); i++)
		{
			CurrencyType currencyType = CurrencyType.None;
			if (Enum.IsDefined(typeof(CurrencyType), data[i].Name))
			{
				currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), data[i].Name);
			}
			if (currencyType != CurrencyType.None)
			{
				Helpers.InstantiateToParent(tokenIconPrefab, specialHeroParent.gameObject).GetComponent<UISprite>().spriteName = HelpersGfx.GetTokenCurrencyIconName(currencyType);
			}
		}
	}
}
