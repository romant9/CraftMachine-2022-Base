using UnityEngine;

public class HeroSkinListItem : UIListCard<HeroSkinInfo>
{
	[SerializeField]
	private UILabel NameLabel;

	[SerializeField]
	private UILabel SeasonNameLabel;

	[SerializeField]
	private UISprite ActiveBackground;

	[SerializeField]
	private UISprite LockedSprite;

	[SerializeField]
	private UISprite DeactiveBackground;

	private bool selected;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (NameLabel != null)
		{
			NameLabel.text = LocalizationManager.GetText(base.Item.SkinNameLocalizationKey);
		}
		if (SeasonNameLabel != null)
		{
			SeasonNameLabel.text = LocalizationManager.GetText(base.Item.SeasonLocalizationKey);
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
		if (LockedSprite != null)
		{
			bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasHeroSkin(base.Item.PrefabId);
			LockedSprite.gameObject.SetActive(!flag);
		}
	}

	public void OnCardClicked()
	{
		if (!selected)
		{
			UIEvent.Send("OnNewOutfitSeleted", base.Item);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_equip");
		}
	}

	public string GetPrefabId()
	{
		return base.Item.PrefabId;
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
