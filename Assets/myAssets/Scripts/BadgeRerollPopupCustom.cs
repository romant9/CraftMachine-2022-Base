using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class BadgeRerollPopupCustom : MonoBehaviour
{
	[SerializeField]
	private UILabel rerollHeaderLabel;
	public UIPopupList uIPopupList;

    public GameObject BadgeCardReroll;
    public GameObject Content;

    public UITable BadgeTable;

	private BadgeReroll reroll = BadgeReroll.Bonus;

	public const string RerolledSlot = "Popup.Badges.Reroll.RerolledSlot";

	public const string RerolledSet = "Popup.Badges.Reroll.RerolledSet";

	public const string RerolledBonus = "Popup.Badges.Reroll.RerolledBonus";

	public const string RerollSlot = "Popup.Badges.Reroll.RerollSlot";

	public const string RerollSet = "Popup.Badges.Reroll.RerollSet";

	public const string RerollBonus = "Popup.Badges.Reroll.RerollBonus";

	private readonly Dictionary<BadgeReroll, string> rerollHeaderLocalizationKeys = new Dictionary<BadgeReroll, string>
	{
		{
			BadgeReroll.Slot,
			"Popup.Badges.Reroll.RerolledSlot"
		},
		{
			BadgeReroll.Set,
			"Popup.Badges.Reroll.RerolledSet"
		},
		{
			BadgeReroll.Bonus,
			"Popup.Badges.Reroll.RerolledBonus"
		}
	};

	private readonly Dictionary<BadgeReroll, string> rerollButtonLocalizationKeys = new Dictionary<BadgeReroll, string>
	{
		{
			BadgeReroll.Slot,
			"Popup.Badges.Reroll.RerollSlot"
		},
		{
			BadgeReroll.Set,
			"Popup.Badges.Reroll.RerollSet"
		},
		{
			BadgeReroll.Bonus,
			"Popup.Badges.Reroll.RerollBonus"
		}
	};

	public BadgeModel currentBadgeModel { get; protected set; }
	private SurvivorBadgesIcon _SurvivorBadgesIcon;


    private void Start()
    {
    }

    public void Open(BadgeModel badgeModel, int badgeReRollCost)
	{
        currentBadgeModel = badgeModel;

        rerollHeaderLabel.text = LocalizationManager.GetText(rerollHeaderLocalizationKeys[reroll]);

        _SurvivorBadgesIcon = Helpers.InstantiateToList(BadgeCardReroll, BadgeTable.gameObject, BadgeCraft.Instance.RerollBadgeIcons);
        BadgeTable.Reposition();

        BadgeTable.transform.parent.GetComponent<UIScrollView>().ResetPosition();

		UpdateUI(badgeReRollCost);
        TweenManager.PlayTweenGroup(_SurvivorBadgesIcon.gameObject, 2);
    }

    public void OpenMulti(List<BadgeModel> badgeModels)
    {
        rerollHeaderLabel.text = LocalizationManager.GetText(rerollHeaderLocalizationKeys[reroll]);

		var RerollMultiBadgeIcons = BadgeCraft.Instance.RerollMultiBadgeIcons;
		int count = RerollMultiBadgeIcons.Count;

        for (int i = 0; i < badgeModels.Count; i++)
		{
			SurvivorBadgesIcon survivorBadgesIcon;
            if (count > 0)
			{
                survivorBadgesIcon = RerollMultiBadgeIcons[i];
            }
			else
			{
                survivorBadgesIcon = Helpers.InstantiateToList(BadgeCardReroll, BadgeTable.gameObject, BadgeCraft.Instance.RerollMultiBadgeIcons);
            }

            BadgeInfo badgeInfo = new BadgeInfo(badgeModels[i]);
            survivorBadgesIcon.SetData(badgeInfo);
            survivorBadgesIcon.rerollIndex.text = (i+1).ToString();
            survivorBadgesIcon.UpdateUI();
        }

        BadgeTable.Reposition();
        BadgeTable.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    public void SetRerollType(BadgeReroll reroll)
	{
		this.reroll = reroll;
	}

    public void GetRerollType()
    {
        int index = uIPopupList.items.IndexOf(uIPopupList.value);
        index = index >= 0 ? index : 0;

        BadgeCraft.Instance.rerollLast = (BadgeReroll)index;
        SetRerollType((BadgeReroll)index);

        BadgeCraft.Instance.RerollMultiCommand();
    }

    public void UpdateUI(int price)
	{
        BadgeInfo badgeInfo = new BadgeInfo(currentBadgeModel);
        _SurvivorBadgesIcon.SetData(badgeInfo);
		_SurvivorBadgesIcon.rerollIndex.text = BadgeCraft.Instance.RerollBadgeIcons.Count.ToString();
        //_SurvivorBadgesIcon.rerollPrice.text = DataManager.Instance.Player.LootManager.GetBadgeReRollCost(currentBadgeModel.ModelId, reroll).ToString();
        _SurvivorBadgesIcon.rerollPrice.text = price.ToString();
        _SurvivorBadgesIcon.UpdateUI();
    }

    private void OnDisable()
    {
        if (BadgeCraft.Instance.RerollBadgeIcons.Count > 0)
		{
			DebugTWD.Log("Delete All Badges");
			Helpers.DestroyAllChildren(BadgeTable.gameObject);
			BadgeCraft.Instance.RerollBadgeIcons.Clear();

        }

		if (BadgeCraft.Instance.rerolledBadgeOrigin != null)
		{
            BadgeCraft.Instance.rerollLast = reroll;
        }
    }
}
