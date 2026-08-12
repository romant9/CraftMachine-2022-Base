using TWDModel;
using UnityEngine;

public class SupportTalentPointCard : MonoBehaviour
{
	[SerializeField]
	private UISprite talentBg;

	[SerializeField]
	private UISprite talentGreyBg;

	[SerializeField]
	private UISprite talentIcon;

	[SerializeField]
	private UISprite talentSelectedIcon;

	[SerializeField]
	private UISprite talentUpdateIcon;

	[SerializeField]
	private UISprite talentMaxIcon;

	[SerializeField]
	private UISprite talentLockedIcon;

	[SerializeField]
	private UILabel talentLevelLabel;

	[SerializeField]
	private GameObject talentLine;

	[SerializeField]
	private GameObject talentLeftLine;

	[SerializeField]
	private GameObject talentRightLine;

	[SerializeField]
	public UIButton talentButton;

	public SupportTalentNodeAbstract talentModel;

	public void SetContent(SupportTalentNodeAbstract model, bool canUpdate, bool isLock)
	{
		talentModel = model;
		SetSelect(isSelect: false);
		if (model.Level == 0)
		{
			Helpers.GameObjectSetActive(talentGreyBg.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(talentGreyBg.gameObject, value: false);
		}
		if (canUpdate)
		{
			Helpers.GameObjectSetActive(talentUpdateIcon.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(talentUpdateIcon.gameObject, value: false);
		}
		if (isLock)
		{
			Helpers.GameObjectSetActive(talentLockedIcon.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(talentLockedIcon.gameObject, value: false);
		}
		if (isLock || model.Level == model.GetMaxLevel())
		{
			Helpers.GameObjectSetActive(talentMaxIcon.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(talentMaxIcon.gameObject, value: true);
		}
		talentIcon.spriteName = talentModel.GetTalentIcon();
		talentLevelLabel.text = model.Level + "/" + model.GetMaxLevel();
	}

	public void SetBranchTalentLine(SupportTalentTreeBranchDirection direction)
	{
		switch (direction)
		{
		case SupportTalentTreeBranchDirection.Left:
			Helpers.GameObjectSetActive(talentLeftLine.gameObject, value: true);
			Helpers.GameObjectSetActive(talentRightLine.gameObject, value: false);
			break;
		case SupportTalentTreeBranchDirection.Right:
			Helpers.GameObjectSetActive(talentRightLine.gameObject, value: true);
			Helpers.GameObjectSetActive(talentLeftLine.gameObject, value: false);
			break;
		}
	}

	public void SetTrunkTalentLine(bool haveRequirePoint)
	{
		Helpers.GameObjectSetActive(talentLine, haveRequirePoint);
	}

	public void SetSelect(bool isSelect)
	{
		Helpers.GameObjectSetActive(talentSelectedIcon.gameObject, isSelect);
	}
}
