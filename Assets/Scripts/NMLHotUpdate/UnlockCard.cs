using TWDModel;
using UnityEngine;

public class UnlockCard : UIListCard<UnlockItem>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite icon;

	private BuildingModel buildingModel;

	public override void UpdateUI()
	{
		if (nameLabel != null)
		{
			nameLabel.text = HelpersLocalization.GetBuildingName(base.Item.BuildingType.Name);
		}
		_ = amountLabel != null;
		icon.spriteName = HelpersGfx.GetBuildingIconName(base.Item.BuildingType.Name);
	}

	private void OnClick()
	{
		if (base.Item != null)
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetBuildingDescription(base.Item.BuildingType.Name));
		}
	}
}
