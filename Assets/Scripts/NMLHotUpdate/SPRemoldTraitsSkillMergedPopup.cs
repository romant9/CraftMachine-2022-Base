using TWDModel;
using UnityEngine;

public class SPRemoldTraitsSkillMergedPopup : HUDElement
{
	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel traitDesc;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UITableList starList;

	public void Setup(string modSkillId)
	{
		SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillId);
		if (sPTraitsRemodeDefinition != null)
		{
			savedModSkillType = sPTraitsRemodeDefinition.Type;
			if (FavoriteToggle && newPhonePopup) FavoriteToggle.value = newPhonePopup.FavoriteModSkillList.Contains(savedModSkillType);

			level.text = LocalizationManager.GetText("System.EquipInfo.Remold.LevelX") + sPTraitsRemodeDefinition.Level;
			HelpersUI.SetTraitsIconOnSprite(traitIcon, sPTraitsRemodeDefinition.SPTraitsIcon, sPTraitsRemodeDefinition.SPTraitsIconOnCloud);
			traitName.text = LocalizationManager.GetText(sPTraitsRemodeDefinition.SPTraitsName);
			starList.Setup(sPTraitsRemodeDefinition.Star);
			classIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(sPTraitsRemodeDefinition.AvailableClass);
			UILabel uILabel = traitDesc;
			string sPTraitsDesc = sPTraitsRemodeDefinition.SPTraitsDesc;
			object[] arguments = sPTraitsRemodeDefinition.SPTraitsLcValue.ToArray();
			uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
		}
	}

	#region myparams
	private NewPhonePopup newPhonePopup => NewPhonePopup.Instance;
	public UIToggle FavoriteToggle;
	private string savedModSkillType;
	private GoldRadioCallDetailPopupItemItemItem detailItem;
	#endregion

	#region mycode
	public void SetItemData(GoldRadioCallDetailPopupItemItemItem item)
	{
		detailItem = item;
	}

	public void SetFavourite(UIToggle tg)
	{
		bool isFavorite = tg.value;
		if (!string.IsNullOrEmpty(savedModSkillType) && newPhonePopup)
		{
			if (isFavorite)
			{
				if (!newPhonePopup.FavoriteModSkillList.Contains(savedModSkillType)) newPhonePopup.FavoriteModSkillList.Add(savedModSkillType);
			}
			else
			{
				if (newPhonePopup.FavoriteModSkillList.Contains(savedModSkillType)) newPhonePopup.FavoriteModSkillList.Remove(savedModSkillType);
			}
			if (detailItem)
			{
				detailItem.SetFavorite(isFavorite);
			}
		}
	}
	#endregion
}
