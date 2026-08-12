using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject EntryContainerRewardPic;

	private GameObject EntryContainerRewardPicItem;

	private readonly List<GameObject> EntryContainerRewardPics = new List<GameObject>();

	[SerializeField]
	private GameObject noSelect;

	[SerializeField]
	private UILabel Limit1;

	[SerializeField]
	private UIButtonWithLabel BtnChoose;

	[SerializeField]
	private GameObject haveSelect;

	[SerializeField]
	private UILabel Limit2;

	[SerializeField]
	private GameObject EntryContainerRewardPicCan;

	private GameObject EntryContainerRewardPicCanItem;

	private readonly List<GameObject> EntryContainerRewardPicCans = new List<GameObject>();

	[SerializeField]
	private UIButton BtnChooseAgain;

	[SerializeField]
	private UIButton BtnRecyle;

	[SerializeField]
	[Header("Type 1: Blueprints")]
	private RecycleWeaponPopupBlueprints blueprints;

	[SerializeField]
	private UIButton Btnblueprints;

	[SerializeField]
	[Header("Type 2: Weapon")]
	private RecycleWeaponPopupWeapon weapon;

	[SerializeField]
	private UIButton Btnweapon;

	private RecycleWeaponActivityModel _activityModel;

	private RecycleWeaponDefinition _definition;

	private bool IsBlueprintType
	{
		get
		{
			RecycleWeaponDefinition definition = _definition;
			if (definition == null)
			{
				return false;
			}
			return definition.Type == 1;
		}
	}

	private bool IsWeaponType
	{
		get
		{
			RecycleWeaponDefinition definition = _definition;
			if (definition == null)
			{
				return false;
			}
			return definition.Type == 2;
		}
	}

	private bool IsLimitReached
	{
		get
		{
			if (_activityModel != null)
			{
				return !_activityModel.CanRecycle();
			}
			return true;
		}
	}

	private bool HasSelection
	{
		get
		{
			if (!IsBlueprintType)
			{
				return weapon?.HasSelection ?? false;
			}
			return blueprints?.HasSelection ?? false;
		}
	}

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void Awake()
	{
		BtnChoose.onClick.Add(new EventDelegate(OnClickChoose));
		BtnChooseAgain.onClick.Add(new EventDelegate(OnClickChoose));
		BtnRecyle.onClick.Add(new EventDelegate(OnClickRecycle));
		Btnblueprints.onClick.Add(new EventDelegate(OnClickChoose));
		Btnweapon.onClick.Add(new EventDelegate(OnClickChoose));
		EntryContainerRewardPicItem = Helpers.GameObjectChildItem(EntryContainerRewardPic);
		EntryContainerRewardPicCanItem = Helpers.GameObjectChildItem(EntryContainerRewardPicCan);
	}

	public void SetInfo(RecycleWeaponActivityModel activityModel)
	{
		_activityModel = activityModel;
		_definition = activityModel?.CurrentDefinition;
		if (IsBlueprintType)
		{
			blueprints.SetInfo(_definition);
			blueprints.OnSelectionChanged = BuildRightSection;
		}
		if (IsWeaponType)
		{
			weapon.SetInfo(_definition);
			weapon.OnSelectionChanged = BuildRightSection;
		}
	}

	public void Open()
	{
		if (_activityModel != null && _definition != null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: true);
			Helpers.GameObjectSetActive(blueprints?.gameObject, IsBlueprintType);
			Helpers.GameObjectSetActive(weapon?.gameObject, IsWeaponType);
			BuildRewardPicPreview();
			BuildLimit(Limit1);
			BuildLimit(Limit2);
			BuildBtnChooseLabel();
			BuildRightSection();
		}
	}

	public void Close()
	{
		blueprints?.CloseSelectPanel();
		weapon?.CloseSelectPanel();
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void BuildRewardPicPreview()
	{
		ClearEntries(EntryContainerRewardPics);
		List<RewardPicEntry> list = _definition?.RewardPicEntries;
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (RewardPicEntry item in list)
		{
			if (item != null)
			{
				GameObject gameObject = EntryContainerRewardPic.AddChild(EntryContainerRewardPicItem);
				if (gameObject.TryGetComponent<RecycleWeaponPopupRewardPicItem>(out var component))
				{
					component.Setup(item);
					EntryContainerRewardPics.Add(gameObject);
				}
			}
		}
		EntryContainerRewardPic.GetComponent<UITable>()?.Reposition();
	}

	private void BuildLimit(UILabel label)
	{
		if (!(label == null) && _activityModel != null && _definition != null)
		{
			int limit = _definition.Limit;
			string text = (limit - _activityModel.RecycledCount).ToString();
			if (IsLimitReached)
			{
				text = "[FF0000]" + text + "[-]";
			}
			string text2 = LocalizationManager.GetText("RecycleWeapons.Limit", text, limit);
			HelpersUI.SetContentToLabel(label, text2);
		}
	}

	private void BuildBtnChooseLabel()
	{
		if (!(BtnChoose == null))
		{
			if (IsBlueprintType)
			{
				BtnChoose.SetContentToLabelOne(LocalizationManager.GetText("RecycleBlueprints.ChooseBlueprints.Btn"));
			}
			else
			{
				BtnChoose.SetContentToLabelOne(LocalizationManager.GetText("RecycleWeapons.ChooseWeapon.Btn"));
			}
		}
	}

	private void BuildRightSection()
	{
		if (_activityModel == null)
		{
			return;
		}
		if (IsLimitReached)
		{
			Helpers.GameObjectSetActive(noSelect, value: true);
			Helpers.GameObjectSetActive(haveSelect, value: false);
			if (IsBlueprintType)
			{
				blueprints?.ClearSelection();
			}
			else
			{
				weapon?.ClearSelection();
			}
			BtnChoose.isEnabled = false;
			return;
		}
		BtnChoose.isEnabled = true;
		if (HasSelection)
		{
			Helpers.GameObjectSetActive(noSelect, value: false);
			Helpers.GameObjectSetActive(haveSelect, value: true);
			BuildSelectedRewardCan();
		}
		else
		{
			Helpers.GameObjectSetActive(noSelect, value: true);
			Helpers.GameObjectSetActive(haveSelect, value: false);
		}
	}

	private void BuildSelectedRewardCan()
	{
		ClearEntries(EntryContainerRewardPicCans);
		int blueprintCount = 1;
		List<RewardShowPicEntry> list = ((!IsBlueprintType) ? weapon?.GetSelectedRewardsPic(out blueprintCount) : blueprints?.GetSelectedRewardsPic(out blueprintCount));
		for (int i = 0; i < list?.Count; i++)
		{
			RewardShowPicEntry rewardShowPicEntry = list[i];
			if (rewardShowPicEntry != null)
			{
				GameObject gameObject = EntryContainerRewardPicCan.AddChild(EntryContainerRewardPicCanItem);
				if (gameObject.TryGetComponent<RecycleWeaponPopupRewardPicCanItem>(out var component))
				{
					component.Setup(rewardShowPicEntry, blueprintCount, IsBlueprintType ? _definition.Object : "");
					EntryContainerRewardPicCans.Add(gameObject);
				}
			}
		}
		Rewards rewards = (IsBlueprintType ? blueprints.GetSelectedRewards(out blueprintCount) : weapon.GetSelectedRewards(out blueprintCount));
		if (IsWeaponType)
		{
			if (rewards == null)
			{
				rewards = new Rewards();
			}
			rewards.AddRewardCurrency(CurrencyType.SurvivalPoints, weapon.GetSurvivalPoints(), isDiamondExchange: false, canOverflowMax: false);
		}
		for (int j = 0; j < rewards?.RewardsList?.Count; j++)
		{
			IReward reward = rewards.RewardsList[j];
			if (reward != null)
			{
				GameObject gameObject2 = EntryContainerRewardPicCan.AddChild(EntryContainerRewardPicCanItem);
				if (gameObject2.TryGetComponent<RecycleWeaponPopupRewardPicCanItem>(out var component2))
				{
					component2.Setup(reward, blueprintCount);
					EntryContainerRewardPicCans.Add(gameObject2);
				}
			}
		}
		EntryContainerRewardPicCan.GetComponent<UITable>()?.Reposition();
	}

	private void OnClickChoose()
	{
		if (_definition != null)
		{
			int remainLimit = _definition.Limit - (_activityModel?.RecycledCount ?? 0);
			if (IsBlueprintType)
			{
				blueprints?.OpenSelectPanel(remainLimit);
			}
			else
			{
				weapon?.OpenSelectPanel();
			}
		}
	}

	private void OnClickRecycle()
	{
		if (HasSelection && _activityModel != null)
		{
			RecycleWeaponConfirmPopup recycleWeaponConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RecycleWeaponConfirmPopup) as RecycleWeaponConfirmPopup;
			if (!(recycleWeaponConfirmPopup == null))
			{
				recycleWeaponConfirmPopup.SetInfo(_activityModel, OnRecycleConfirmed, EntryContainerRewardPicCans);
				recycleWeaponConfirmPopup.Open();
			}
		}
	}

	private void OnRecycleConfirmed()
	{
		if (_activityModel != null && HasSelection && (IsBlueprintType ? blueprints.ExecuteRecycle(_activityModel.ModelId) : weapon.ExecuteRecycle(_activityModel.ModelId)) == TWDModelResult.OK)
		{
			ShowRewardPopup();
			if (IsBlueprintType)
			{
				blueprints?.ClearSelection();
			}
			else
			{
				weapon?.ClearSelection();
			}
			BuildLimit(Limit1);
			BuildLimit(Limit2);
			BuildRightSection();
		}
	}

	private void ShowRewardPopup()
	{
		if (_activityModel?.LastRecycleRewards != null)
		{
			RecycleWeaponRewardsPopup recycleWeaponRewardsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RecycleWeaponRewardsPopup) as RecycleWeaponRewardsPopup;
			if (recycleWeaponRewardsPopup != null && _activityModel.LastRecycleRewards.Count > 0)
			{
				recycleWeaponRewardsPopup.SetupRewards(_activityModel.LastRecycleRewards);
				recycleWeaponRewardsPopup.Open();
			}
		}
	}

	private void ClearEntries(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
	}
}
