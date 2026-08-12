using System;
using TWDModel;
using UnityEngine;

public class UIDataRowRadioCall : MonoBehaviourExtended
{
	[Tooltip("Needed to know the size height of this object")]
	[SerializeField]
	private UIWidget cachedWidget;

	[SerializeField]
	public UILabel labelTokenType;

	[SerializeField]
	public UILabel probabilityLabel;

	[SerializeField]
	public UIGridExtended iconsParent;

	[SerializeField]
	public GameObject tokenIconPrefab;

	public bool IsVisible
	{
		get
		{
			if (base.gameObject != null)
			{
				return base.gameObject.activeSelf;
			}
			return false;
		}
	}

	public UIWidget widget
	{
		get
		{
			if (cachedWidget == null)
			{
				cachedWidget = GetComponent<UIWidget>();
				if (cachedWidget == null)
				{
					Debug.LogError("Cant find UI widget on UIDataRow! Needed to calculate the size of the object!");
				}
			}
			return cachedWidget;
		}
	}

	public void Show()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
	}

	public void Hide()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	public void SetData(ItemAmountProbabilityData data)
	{
		HelpersUI.SetContentToLabel(content: (data.Rarity < 0) ? LocalizationManager.GetText("Building.TrainingGrounds.Title") : LocalizationManager.GetText("Droptype.{Rarity}Hero", HelpersLocalization.GetRarityLevel(data.Rarity)), label: labelTokenType);
		HelpersUI.SetContentToLabel(probabilityLabel, $"{(float)data.Probability * 100f:0.#}%");
		SetTokenIcons(data.Name);
	}

	private void SetTokenIcons(string iconData)
	{
		if (!(iconsParent != null) || !(tokenIconPrefab != null))
		{
			return;
		}
		string[] array = iconData.Split(',');
		for (int i = 0; i < ((array != null) ? array.Length : 0); i++)
		{
			CurrencyType currencyType = CurrencyType.None;
			if (Enum.IsDefined(typeof(CurrencyType), array[i]))
			{
				currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array[i]);
			}
			else if (Enum.IsDefined(typeof(CurrencyType), array[i] + "Token"))
			{
				currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array[i] + "Token");
			}
			if (currencyType != CurrencyType.None)
			{
				Helpers.InstantiateToParent(tokenIconPrefab, iconsParent.gameObject).GetComponent<UISprite>().spriteName = HelpersGfx.GetTokenCurrencyIconName(currencyType);
			}
		}
	}
}
