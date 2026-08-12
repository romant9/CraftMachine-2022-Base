using TWDModel;
using UnityEngine;

public class StatisticPanel : MonoBehaviour
{
	public const string StatisticTypeExploration = "Exploration";

	public const string StatisticTypeStorageCapacity = "StorageCapacity";

	public const string StatisticTypeMaxProduction = "MaxProduction";

	public const string StatisticTypeProduction = "Production";

	public const string StatisticTypeMeleeDamage = "MeleeDamage";

	public const string StatisticTypeRangedDamage = "RangedDamage";

	public const string StatisticTypeHealth = "Health";

	public const string StatisticTypeMoveRange = "MoveRange";

	public const string StatisticTypeHealingTime = "HealingTime";

	public const string StatisticTypeHealingSlots = "HealingSlots";

	public const string StatisticTypeBasic = "StatisticTypeBasic";

	public const string StatisticTypeTrainingGround = "StatisticTypeTrainingGround";

	public const string StatisticTypeWorkshop = "StatisticTypeWorkshop";

	public const string StatisticTypeRadioTent = "StatisticTypeRadioTent";

	public const string StatisticTypeBuildingPoints = "BuildingPoints";

	public const string StatisticTypeCraftsman = "StatisticTypeCraftsman";

	[SerializeField]
	private UILabel statisticName;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel valueLabel;

	[SerializeField]
	private UILabel gainValueLabel;

	private string type;

	public void SetStatistic(string type, int value)
	{
		SetStatistic(type, null, value);
	}

	public void SetStatistic(string type, int value, int gainValue)
	{
		string iconName = null;
		if (!(type == "RangedDamage"))
		{
			if (type == "MeleeDamage")
			{
				iconName = HelpersGfx.GetEquipmentCategoryIconName(EquipmentCategory.MeleeWeapon);
			}
		}
		else
		{
			iconName = HelpersGfx.GetEquipmentCategoryIconName(EquipmentCategory.RangeWeapon);
		}
		SetStatistic(type, iconName, value, gainValue);
	}

	public void SetStatistic(string type, string iconName, int value, int gainValue = 0)
	{
		this.type = type;
		if (statisticName != null)
		{
			statisticName.text = LocalizationManager.GetText("Statistic." + type);
		}
		if (icon != null)
		{
			if (string.IsNullOrEmpty(iconName))
			{
				icon.enabled = false;
			}
			else
			{
				icon.enabled = true;
				icon.spriteName = iconName;
			}
		}
		if (valueLabel != null)
		{
			valueLabel.text = value.ToString();
			if (type == "HealingTime")
			{
				if (value == 0)
				{
					valueLabel.text = "0";
				}
				else
				{
					valueLabel.text = Helpers.FormatTime(value * 1000);
				}
			}
			else if (type == "BuildingPoints")
			{
				if (value > 0)
				{
					valueLabel.text = Helpers.FormatNumber(value);
				}
				else
				{
					valueLabel.text = "";
				}
			}
			else
			{
				valueLabel.text = Helpers.FormatNumber(value);
			}
		}
		if (gainValueLabel != null)
		{
			gainValueLabel.gameObject.SetActive(gainValue > 0);
			string text = "";
			if (gainValue > 0)
			{
				text = "+";
			}
			text = ((!(type == "HealingTime")) ? (text + Helpers.FormatNumber(gainValue)) : (text + Helpers.FormatTime(gainValue * 1000)));
			gainValueLabel.text = text;
		}
	}

	private void OnClick()
	{
		NGTooltip.Show(LocalizationManager.GetText("Toolitp.Statistic." + type));
	}
}
