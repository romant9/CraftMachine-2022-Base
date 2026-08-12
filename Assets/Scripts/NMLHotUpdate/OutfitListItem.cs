using TWDModel;
using UnityEngine;

public class OutfitListItem : UIListCard<OutfitDefinition>
{
	[SerializeField]
	private UILabel NameLabel;

	[SerializeField]
	private UILabel SeasonNameLabel;

	[SerializeField]
	private UISprite LockedSprite;

	[SerializeField]
	private UISprite ActiveBackground;

	[SerializeField]
	private UISprite DeactiveBackground;

	private bool selected;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (NameLabel != null)
		{
			NameLabel.text = LocalizationManager.GetText(base.Item.TitleLocalizationKey);
		}
		if (SeasonNameLabel != null)
		{
			SeasonNameLabel.text = LocalizationManager.GetText(base.Item.SeasonLocalizationKey);
		}
		if (LockedSprite != null)
		{
			bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasOutfit(base.Item.ID);
			LockedSprite.gameObject.SetActive(!flag);
		}
		if (ActiveBackground != null && DeactiveBackground != null)
		{
			if (selected)
			{
				ActiveBackground.gameObject.SetActive(value: true);
				DeactiveBackground.gameObject.SetActive(value: false);
			}
			else
			{
				ActiveBackground.gameObject.SetActive(value: false);
				DeactiveBackground.gameObject.SetActive(value: true);
			}
		}
	}

	public OutfitDefinition GetItemDefinition()
	{
		return base.Item;
	}

	public void OnCardClicked()
	{
		if (selected)
		{
			UIEvent.Send("OnNewOutfitDeseleted");
		}
		else
		{
			UIEvent.Send("OnNewOutfitSeleted", base.Item);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
	}

	public void Select()
	{
		selected = true;
	}

	public void Deselect()
	{
		selected = false;
	}
}
