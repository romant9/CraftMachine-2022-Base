using TWDModel;
using UnityEngine;

public class PlayerHubActivityCard : UIListCard<ActiveInformationDefinition>
{
	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UISprite selectBg;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private GameObject newContainer;

	public override void UpdateUI()
	{
		HelpersUI.SetContentToLabel(title, LocalizationManager.GetText(base.Item.Title));
		HelpersUI.SetContentToLabel(description, LocalizationManager.GetText(base.Item.TitleSecond));
		bool value = TWDPlayerPrefs.GetInt(PlayerHubManager.ActivityRedDotKey + base.Item.ID, 1) == 1;
		Helpers.GameObjectSetActive(newContainer, value);
	}

	public override int GetSortValue()
	{
		return base.Item.Order;
	}

	public void OnClick()
	{
		if (TWDPlayerPrefs.GetInt(PlayerHubManager.ActivityRedDotKey + base.Item.ID, 1) == 1)
		{
			TWDPlayerPrefs.SetInt(PlayerHubManager.ActivityRedDotKey + base.Item.ID, 0);
			GameManager.Instance.PlayerHubManager.ActivityRedDotNum--;
			TWDPlayerPrefs.Save();
		}
		UIEvent.Send("PlayerHubActivitySelectedEvent", base.Item);
		UpdateUI();
	}

	public void SetSelected(bool selected)
	{
		Helpers.GameObjectSetActive(bg, !selected);
		Helpers.GameObjectSetActive(selectBg, selected);
	}
}
