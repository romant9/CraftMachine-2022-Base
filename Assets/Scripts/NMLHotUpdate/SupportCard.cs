using System;
using TWDModel;
using UnityEngine;

public abstract class SupportCard : UIListCard<SupportModel>
{
	[SerializeField]
	protected UILabel nameLabel;

	[SerializeField]
	protected UITexture iconTexture;

	[SerializeField]
	protected UISprite skillIconSprite;

	[SerializeField]
	protected GameObject[] levelRepresentations;

	[SerializeField]
	protected GameObject infoButton;

	[SerializeField]
	protected GameObject CanPromoteOverlay;

	[SerializeField]
	protected GameObject LockedOverlay;

	[SerializeField]
	protected GameObject CanUnlockOverlay;

	[SerializeField]
	private GameObject unavailableContainer;

	private Action clicked;

	private Action infoButtonClicked;

	private MapCategory currentMapCategory;

	public void Initialize(SupportModel model, Action onClick, Action onInfoClick = null, MapCategory mapCategory = MapCategory.None)
	{
		clicked = onClick;
		infoButtonClicked = onInfoClick;
		currentMapCategory = mapCategory;
		base.Item = model;
		if ((bool)infoButton)
		{
			infoButton.SetActive(onInfoClick != null);
		}
		Refresh();
	}

	protected virtual void InitializeEmpty()
	{
	}

	protected virtual void InitializeRegular()
	{
		string supportId = base.Item.SupportId;
		nameLabel.text = HelpersLocalization.GetSupportName(supportId);
		iconTexture.mainTexture = HelpersGfx.LoadSupportIcon(supportId);
		skillIconSprite.spriteName = HelpersGfx.GetSupportSkillIconName(supportId);
		for (int i = 0; i < levelRepresentations.Length; i++)
		{
			levelRepresentations[i].SetActive(i < base.Item.Level);
		}
		bool canUpgrade = base.Item.CanUpgrade;
		bool unlocked = base.Item.Unlocked;
		if (unlocked)
		{
			if (canUpgrade)
			{
				Helpers.GameObjectSetActive(CanPromoteOverlay, value: true);
				Helpers.GameObjectSetActive(CanUnlockOverlay, value: false);
			}
			if (!canUpgrade)
			{
				Helpers.GameObjectSetActive(CanPromoteOverlay, value: false);
				Helpers.GameObjectSetActive(LockedOverlay, value: false);
				Helpers.GameObjectSetActive(CanUnlockOverlay, value: false);
			}
		}
		if (!unlocked && canUpgrade)
		{
			Helpers.GameObjectSetActive(CanUnlockOverlay, value: true);
			Helpers.GameObjectSetActive(LockedOverlay, value: false);
		}
		else if (!unlocked)
		{
			Helpers.GameObjectSetActive(LockedOverlay, value: true);
			Helpers.GameObjectSetActive(CanUnlockOverlay, value: false);
		}
		unavailableContainer.SetActive(!base.Item.CheckCanUse(currentMapCategory));
	}

	public void Click()
	{
		if (base.Item == null || base.Item.CheckCanUse(currentMapCategory))
		{
			clicked?.Invoke();
		}
	}

	public void InfoButtonClick()
	{
		infoButtonClicked?.Invoke();
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void SetItem(SupportModel supportModel)
	{
		base.Item = supportModel;
		Refresh();
	}

	public void Refresh()
	{
		if (base.Item == null)
		{
			InitializeEmpty();
		}
		else
		{
			InitializeRegular();
		}
	}
}
