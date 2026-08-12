using System;
using System.Collections.Generic;
using Driller.Models;
using TWDModel;
using UnityEngine;

public class EquipSkillRecommendEquipItem : MonoBehaviour
{
	[SerializeField]
	private UILabel TextDefault;

	[SerializeField]
	private UILabel TextData;

	[SerializeField]
	private GameObject EntryContainer;

	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	[SerializeField]
	private UITable Grid;

	[SerializeField]
	private GameObject User;

	[SerializeField]
	private PlayerEmblemIcon UserHead;

	[SerializeField]
	private UIButton UserDiscord;

	[SerializeField]
	private UILabel TextName;

	[SerializeField]
	private UIButton UserPraise;

	[SerializeField]
	private GameObject UserPraiseLike;

	[SerializeField]
	private UILabel TextPraise;

	[SerializeField]
	private UIButton SpriteSet;

	[SerializeField]
	private GameObject EntryContainerSelect;

	private GameObject EntryPrefabSelect;

	private readonly List<GameObject> EntriesSelect = new List<GameObject>();

	private EquipmentItemModel equipmentItemModel;

	private EquipSkillRecommendEquipModel equipSkillRecommendEquipModel;

	private string selectedTag;

	private EquipmentSkillSuggestion equipmentSkillSuggestion => equipSkillRecommendEquipModel?.equipmentSkillSuggestion;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public void Initialize()
	{
		UserDiscord.onClick.Add(new EventDelegate(OnClickDiscord));
		SpriteSet.onClick.Add(new EventDelegate(OnClickSet));
		UserPraise.onClick.Add(new EventDelegate(OnClickPraise));
		EntryPrefab = Helpers.GameObjectChildItem(EntryContainer.gameObject);
		EntryPrefabSelect = Helpers.GameObjectChildItem(EntryContainerSelect.gameObject);
	}

	private void OnDestroy()
	{
		ClearCall();
	}

	private void ClearCall()
	{
		if (equipSkillRecommendEquipModel != null)
		{
			EquipSkillRecommendEquipModel obj = equipSkillRecommendEquipModel;
			obj.OnRefreshCall = (Action<EquipSkillRecommendEquipModel>)Delegate.Remove(obj.OnRefreshCall, new Action<EquipSkillRecommendEquipModel>(OnRefreshCall));
		}
	}

	public void SetInfo(EquipmentItemModel model, EquipSkillRecommendEquipModel conf, string select)
	{
		equipmentItemModel = model;
		selectedTag = select;
		SetInfo(conf);
		if (!equipmentSkillSuggestion.Default)
		{
			conf.RequestLikeStatus();
			conf.RequestPlayerData();
		}
	}

	public void SetInfo(EquipSkillRecommendEquipModel conf)
	{
		ClearCall();
		equipSkillRecommendEquipModel = conf;
		ClearCall();
		conf.OnRefreshCall = (Action<EquipSkillRecommendEquipModel>)Delegate.Combine(conf.OnRefreshCall, new Action<EquipSkillRecommendEquipModel>(OnRefreshCall));
		UpdateUI();
	}

	private void OnRefreshCall(EquipSkillRecommendEquipModel conf)
	{
		if (equipSkillRecommendEquipModel == conf)
		{
			UpdateUIUser();
		}
	}

	private void UpdateUIUser()
	{
		SkillSuggestionLikeStatus currentLikeStatus = equipSkillRecommendEquipModel.CurrentLikeStatus;
		if (currentLikeStatus != null)
		{
			Helpers.GameObjectSetActive(UserPraiseLike, currentLikeStatus.HasLiked);
			TextPraise.text = currentLikeStatus.Count.ToString();
		}
		TextName.text = equipSkillRecommendEquipModel.PlayerName;
		UserHead.SetEmblem(equipSkillRecommendEquipModel.PlayerData);
	}

	public void UpdateUI()
	{
		bool flag = !equipmentSkillSuggestion.Default;
		Helpers.GameObjectSetActive(TextDefault, !flag);
		Helpers.GameObjectSetActive(EntryContainerSelect, flag);
		TextData.text = LocalizationManager.GetText(equipmentSkillSuggestion.Data);
		Helpers.GameObjectSetActive(User, flag);
		Grid.Reposition();
		UpdateUIUser();
		FreshListData();
		FreshSeletListData();
	}

	public void HideGrid()
	{
		Helpers.GameObjectSetActive(Grid, value: false);
	}

	private void FreshListData()
	{
		ClearEntries(Entries);
		ModSkillSlot[] resultModSkillSlots = GetResultModSkillSlots();
		for (int i = 0; i < resultModSkillSlots?.Length; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<EquipSkillRecommendEquipItemItem>(out var component))
			{
				component.Setup(i, resultModSkillSlots[i].ModSkillMode);
				Entries.Add(gameObject);
			}
		}
		EntryContainer.GetComponent<UITable>().Reposition();
		EntryContainer.GetComponentInParent<UIScrollView>()?.ResetPosition();
	}

	private ModSkillSlot[] GetResultModSkillSlots()
	{
		ModSkillSlot[] resultModSkillSlots = equipSkillRecommendEquipModel.GetResultModSkillSlots(playerModel);
		for (int i = 0; i < resultModSkillSlots?.Length; i++)
		{
			if (resultModSkillSlots[i].ModSkillMode.EquipmentItemModel == null)
			{
				resultModSkillSlots[i].ModSkillMode.EquipmentItemModel = equipmentItemModel;
			}
		}
		return resultModSkillSlots;
	}

	private void FreshSeletListData()
	{
		ClearEntries(EntriesSelect);
		if (equipmentSkillSuggestion == null)
		{
			return;
		}
		UITable component = EntryContainerSelect.GetComponent<UITable>();
		List<string> tags = equipmentSkillSuggestion.Tags;
		for (int i = 0; i < tags?.Count; i++)
		{
			if (!(tags[i] == "Tag0"))
			{
				GameObject gameObject = EntryContainerSelect.AddChild(EntryPrefabSelect);
				if (gameObject.TryGetComponent<EquipSkillRecommendEquipItemSelect>(out var component2))
				{
					component2.SetInfo(tags[i]);
					component2.SetSelect(selectedTag);
					EntriesSelect.Add(gameObject);
				}
			}
		}
		component.Reposition();
	}

	private void ClearEntries(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
	}

	private void OnClickDiscord()
	{
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.Hyperlink_Discord_EquipSkillSuggestion);
	}

	private void OnClickSet()
	{
		if (equipmentItemModel != null)
		{
			EquipSkillRecommendEquipApply equipSkillRecommendEquipApply = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EquipSkillRecommendEquipApply) as EquipSkillRecommendEquipApply;
			if (!(equipSkillRecommendEquipApply == null))
			{
				equipSkillRecommendEquipApply.SetInfo(equipmentItemModel, equipSkillRecommendEquipModel, base.gameObject);
				equipSkillRecommendEquipApply.Open();
			}
		}
	}

	public void OnClickDetailInfo()
	{
		if (equipSkillRecommendEquipModel != null)
		{
			SPRemoldTraitsSkillDetailInfoPopup sPRemoldTraitsSkillDetailInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillDetailInfoPopup) as SPRemoldTraitsSkillDetailInfoPopup;
			if (!(sPRemoldTraitsSkillDetailInfoPopup == null))
			{
				ModSkillSlot[] resultModSkillSlots = GetResultModSkillSlots();
				sPRemoldTraitsSkillDetailInfoPopup.FreshListData(resultModSkillSlots);
				sPRemoldTraitsSkillDetailInfoPopup.Open();
			}
		}
	}

	private void OnClickPraise()
	{
		equipSkillRecommendEquipModel.ToggleLikeLocal();
	}
}
