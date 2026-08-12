using TWDModel;
using UnityEngine;

public class SurvivorDamageHealthPanel : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel maxLabel;

	[SerializeField]
	private UILabel maxAmountLabel;

	[SerializeField]
	private GameObject apocalypticPlus;

	[SerializeField]
	private UITexture apocalypticIcon;

	[SerializeField]
	private Color apocalypticColor;

	[SerializeField]
	private Color normalColor;

	public void setInfo(string nameText, string amount, string baseText, string baseAmount, EquipmentItemModel equipmentItemModel = null)
	{
		bool num = amountLabel.text != amount;
		setText(nameLabel, nameText);
		setText(amountLabel, amount);
		setText(maxLabel, baseText);
		setText(maxAmountLabel, baseAmount);
		SetApocalyptic(equipmentItemModel);
		if (num && amountLabel != null)
		{
			TweenManager.PlayTweenGroup(amountLabel.gameObject, 5);
		}
	}

	public void setAmount(string amount)
	{
		bool num = amountLabel.text != amount;
		setText(amountLabel, amount);
		if (num && amountLabel != null)
		{
			TweenManager.PlayTweenGroup(amountLabel.gameObject, 5);
		}
	}

	private void setText(UILabel label, string content)
	{
		if (label != null)
		{
			label.text = content;
		}
	}

	private void SetApocalyptic(EquipmentItemModel equipmentItemModel)
	{
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		Helpers.GameObjectSetActive(apocalypticPlus, value: false);
		nameLabel.color = normalColor;
		if (equipmentItemModel != null && HelpersGfx.IsApocalypticRarity(equipmentItemModel.RarityLevel))
		{
			nameLabel.color = apocalypticColor;
			HelpersGfx.SetApocalypticEffectActive(apocalypticPlus, equipmentItemModel.RarityLevel);
			HelpersGfx.SetApocalypticEffectSprite(apocalypticIcon, equipmentItemModel.GetEquipmentActiveTraits(), equipmentItemModel.GetEquipmentPassiveTraits(), equipmentItemModel.RarityLevel);
		}
	}
}
