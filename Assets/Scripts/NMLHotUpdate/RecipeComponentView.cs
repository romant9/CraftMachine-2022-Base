using TWDModel;
using UnityEngine;

public class RecipeComponentView : NUIListItem<CurrencyModel>
{
	public UISprite Background;

	public UISprite Icon;

	public UILabel Amount;

	public GameObject UnavailableIndicator;

	public GameObject UpgradeableIndicator;

	[Header("New component selected")]
	public int TweenGroupNewSeletion = -1;

	[Header("Craft Completed")]
	public int TweenGroupPostCraft = -1;

	[Header("Selection is empty")]
	public int TweenGroupEmptySlot = -1;

	[SerializeField]
	protected UIButtonExtended button;

	private CurrencyType currencyType;

	public CurrencyType SelectedCurrency => currencyType;

	public override void Clear()
	{
		base.Clear();
		Helpers.GameObjectSetActive(Background, value: false);
		Helpers.GameObjectSetActive(Icon, value: false);
		Helpers.GameObjectSetActive(Amount, value: false);
		currencyType = CurrencyType.None;
		if (button != null)
		{
			button.Clear();
		}
	}

	public void Initialize(CurrencyType type, int amount, bool allowed = true, string id = "", UIButtonExtended.OnClickCallback onClickCallback = null, bool showIndicator = false)
	{
		Helpers.GameObjectSetActive(UnavailableIndicator, !allowed);
		if (UpgradeableIndicator != null)
		{
			Helpers.GameObjectSetActive(UpgradeableIndicator, showIndicator);
		}
		if (currencyType != type && type != CurrencyType.None && base.gameObject != null)
		{
			TriggerNewSelectionEffect();
		}
		currencyType = type;
		if (Helpers.GameObjectSetActive(Icon, value: true))
		{
			Icon.spriteName = HelpersGfx.GetCurrencyIconName(type);
		}
		if (Helpers.GameObjectSetActive(Amount, value: true))
		{
			Amount.text = amount.ToString();
		}
		int componentRarityLevel = ComponentHelper.GetComponentRarityLevel(type);
		if (Helpers.GameObjectSetActive(Background, value: true) && componentRarityLevel >= 0)
		{
			Background.spriteName = HelpersGfx.GetEquipmentRaritySprite(componentRarityLevel);
		}
		if (button != null)
		{
			if (onClickCallback != null)
			{
				button.id = id;
				button.SetClickCallback(onClickCallback);
			}
			else
			{
				button.Clear();
			}
		}
		SetEnabled(amount > 0);
	}

	public void TriggerNewSelectionEffect()
	{
		if (base.gameObject != null)
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupNewSeletion);
		}
	}

	public void TriggerPostCraftEffect()
	{
		if (base.gameObject != null)
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupPostCraft);
		}
	}

	public void TriggerEmptyEffect()
	{
		if (IsActive())
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupEmptySlot);
		}
	}

	public bool IsActive()
	{
		if (!(base.gameObject != null))
		{
			return false;
		}
		return base.gameObject.activeInHierarchy;
	}

	public void SetEnabled(bool enabled)
	{
		Color color = (enabled ? Color.white : new Color(1f, 1f, 1f, 0.2f));
		if (Amount != null)
		{
			Amount.color = color;
		}
		if (Background != null)
		{
			Background.color = color;
		}
		if (Icon != null)
		{
			Icon.color = color;
		}
	}
}
