using TWDModel;
using UnityEngine;

public class ReturnLoginShopRewardModSkillItem : MonoBehaviour
{
	[SerializeField]
	private GameObject content;

	[SerializeField]
	private UISprite skillBg;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UIButton button;

	private SPTraitsRemoldDefinitions _definition;

	private void Awake()
	{
		if (button != null)
		{
			EventDelegate.Set(button.onClick, OnClicked);
		}
	}

	public bool Setup(RewardRemoldSkill reward)
	{
		_definition = ((reward != null) ? Helpers.GetMinRemoldDefinitionForGroup(reward.SpRemoldSkillType) : null);
		if (_definition == null)
		{
			Hide();
			return false;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Helpers.GameObjectSetActive(content, value: true);
		if (skillBg != null)
		{
			skillBg.color = Helpers.HexToColor(_definition.Color);
		}
		if (traitIcon != null)
		{
			HelpersUI.SetTraitsIconOnSprite(traitIcon, _definition.SPTraitsIcon, _definition.SPTraitsIconOnCloud);
		}
		if (classIcon != null)
		{
			classIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(_definition.AvailableClass);
		}
		if (starList != null)
		{
			starList.Setup(_definition.Star);
		}
		HelpersUI.SetContentToLabel(levelLabel, LocalizationManager.GetText("System.EquipSPRemold.TraitLv", _definition.Level));
		return true;
	}

	public void Hide()
	{
		_definition = null;
		Helpers.GameObjectSetActive(content, value: false);
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnClicked()
	{
		if (_definition != null)
		{
			SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
			if (sPRemoldTraitsSkillMergedPopup != null)
			{
				sPRemoldTraitsSkillMergedPopup.Setup(_definition.ID);
				sPRemoldTraitsSkillMergedPopup.Open();
			}
		}
	}
}
