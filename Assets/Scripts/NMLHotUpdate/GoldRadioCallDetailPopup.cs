using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GoldRadioCallDetailPopup : HUDElement
{
	[Header("UP Section")]
	[SerializeField]
	private GameObject upSectionRoot;

	[SerializeField]
	private GameObject EntryContainerUp;

	private GameObject EntryPrefabUp;

	private readonly List<GameObject> EntriesUp = new List<GameObject>();

	[Header("Detail Text")]
	[SerializeField]
	private UILabel detailTextLabel;

	[SerializeField]
	private GameObject EntryContainer;

	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	[Header("Other")]
	[SerializeField]
	private UILabel otherTitleLabel;

	[SerializeField]
	private GameObject EntryContainerOther;

	private GameObject EntryPrefabOther;

	private readonly List<GameObject> EntriesOther = new List<GameObject>();

	[SerializeField]
	[Header("Close")]
	private UIButton closeButton;

	private EquipPrizeWheelDefinition _equipDefinition;

	private GoldRadioCallDenifition _goldRadioDenifition;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void Awake()
	{
		closeButton.onClick.Add(new EventDelegate(OnClickClose));
		EntryPrefabUp = Helpers.GameObjectChildItem(EntryContainerUp);
		EntryPrefab = Helpers.GameObjectChildItem(EntryContainer);
		EntryPrefabOther = Helpers.GameObjectChildItem(EntryContainerOther);
	}

	public void Open(EquipPrizeWheelDefinition equipDef, GoldRadioCallDenifition goldRadioDef)
	{
		base.Open();
		_equipDefinition = equipDef;
		_goldRadioDenifition = goldRadioDef;
		if (_equipDefinition == null || _goldRadioDenifition == null)
		{
			Debug.LogError("[GoldRadioCallDetailPopup] Open failed: definition is null");
			return;
		}
		BuildDetailText();
		BuildUPSection();
		BuildStarTables();
		BuildOtherTable();
	}

	private void BuildDetailText()
	{
		string text = LocalizationManager.GetText(_equipDefinition.NameLocKey);
		string text2 = BuildClassDisplayText();
		int num = (GameManager.Instance.gameEconomyData?.ConfigData?.EquipPrizeWheelLuckPoint_GoldRadio).GetValueOrDefault() + 1;
		ParseRateList(_goldRadioDenifition.Star4Rate, out var baseRate, out var totalRate);
		string text3 = LocalizationManager.GetText(_goldRadioDenifition.DetailText, text, text2, num, baseRate, totalRate);
		HelpersUI.SetContentToLabel(detailTextLabel, text3);
	}

	private string BuildClassDisplayText()
	{
		List<string> list = _goldRadioDenifition.Class;
		if (list == null || list.Count == 0)
		{
			return "";
		}
		List<string> values = (from c in list
			where !string.IsNullOrEmpty(c)
			select HelpersLocalization.GetSurvivorClassName(c)).ToList();
		return string.Join(",", values);
	}

	private void BuildUPSection()
	{
		if (_goldRadioDenifition.Type != 2)
		{
			Helpers.GameObjectSetActive(upSectionRoot, value: false);
			return;
		}
		Helpers.GameObjectSetActive(upSectionRoot, value: true);
		ClearEntries(EntriesUp);
		Dictionary<int, List<ModSkillMode>> dictionary = new Dictionary<int, List<ModSkillMode>>();
		foreach (ModSkillMode item in EquipSkillRecommendEquipModel.LoadConfigModSkillModes(_goldRadioDenifition.UPShow, playerModel))
		{
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = item.GetSpTraitsDefaultTrait();
			if (spTraitsDefaultTrait != null)
			{
				if (!dictionary.TryGetValue(spTraitsDefaultTrait.Star, out var value))
				{
					value = new List<ModSkillMode>();
					dictionary.Add(spTraitsDefaultTrait.Star, value);
				}
				value.Add(item);
			}
		}
		HashSet<string> upShowTypes = GetUpShowTypes();
		foreach (KeyValuePair<int, List<ModSkillMode>> item2 in dictionary)
		{
			EntryContainerUp.AddChild(EntryPrefabUp).TryGetComponent<GoldRadioCallDetailPopupItem>(out var component);
			component.Initialize();
			string text = LocalizationManager.GetText($"GoldRadioCall.{item2.Key}StarUPCallRate");
			component.SetInfo(item2.Key, text);
			component.FreshListData(item2.Value, upShowTypes);
		}
		EntryContainerUp.GetComponent<UITable>().Reposition();
	}

	private void ClearEntries(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
	}

	private void BuildStarTables()
	{
		ClearEntries(Entries);
		HashSet<string> upShowTypes = GetUpShowTypes();
		int[] array = new int[4] { 4, 3, 2, 1 };
		foreach (int num in array)
		{
			List<string> starRate = GetStarRate(num);
			if (starRate != null && starRate.Count > 0)
			{
				List<string> starShow = GetStarShow(num);
				CreateStarTable(num, starRate, starShow, upShowTypes);
			}
		}
		EntryContainer.GetComponent<UITable>().Reposition();
	}

	private void CreateStarTable(int starLevel, List<string> rateList, List<string> showList, HashSet<string> upShowTypes)
	{
		EntryContainer.AddChild(EntryPrefab).TryGetComponent<GoldRadioCallDetailPopupItem>(out var component);
		component.Initialize();
		ParseRateList(rateList, out var baseRate, out var totalRate);
		component.SetInfo(starLevel, LocalizationManager.GetText(GetStarSkillRateText(starLevel), baseRate, totalRate));
		component.FreshListData(showList, upShowTypes);
	}

	private string GetStarSkillRateText(int star)
	{
		return $"GoldRadioCall.{star}StarSkillRate";
	}

	private void BuildOtherTable()
	{
		List<string> otherRate = _goldRadioDenifition.OtherRate;
		ParseRateList(otherRate, out var baseRate, out var totalRate);
		HelpersUI.SetContentToLabel(otherTitleLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GoldRadioCall.OtherRate", baseRate, totalRate));
		BuildOtherShowContent();
	}

	private void BuildOtherShowContent()
	{
		ClearEntries(EntriesOther);
		List<string> otherShow = _goldRadioDenifition.OtherShow;
		if (otherShow == null || otherShow.Count == 0)
		{
			return;
		}
		foreach (string item in otherShow)
		{
			if (int.TryParse(item, out var result))
			{
				var (textId, textId2) = GetOtherItemInfo(result);
				EntryContainerOther.AddChild(EntryPrefabOther).TryGetComponent<GoldRadioCallDetailPopupOtherItem>(out var component);
				string text = LocalizationManager.GetText(textId);
				string text2 = LocalizationManager.GetText(textId2);
				component.SetInfo(text, text2);
			}
		}
		EntryContainerOther.GetComponent<UITable>().Reposition();
	}

	private (string nameKey, string typeKey) GetOtherItemInfo(int type)
	{
		if (type == 5)
		{
			return (nameKey: "Currency.SPTraitsUpgradeToken", typeKey: "BundleClass.Consumables");
		}
		return (nameKey: $"GoldRadioCall.{type}StarSkillToken", typeKey: "System.EquipRemold.ItemTips.Fucn3");
	}

	private List<string> GetStarRate(int star)
	{
		return star switch
		{
			4 => _goldRadioDenifition.Star4Rate,
			3 => _goldRadioDenifition.Star3Rate,
			2 => _goldRadioDenifition.Star2Rate,
			1 => _goldRadioDenifition.Star1Rate,
			_ => null,
		};
	}

	private List<string> GetStarShow(int star)
	{
		return star switch
		{
			4 => _goldRadioDenifition.Star4Show,
			3 => _goldRadioDenifition.Star3Show,
			2 => _goldRadioDenifition.Star2Show,
			1 => _goldRadioDenifition.Star1Show,
			_ => null,
		};
	}

	private HashSet<string> GetUpShowTypes()
	{
		List<string> list = _goldRadioDenifition?.UPShow;
		if (list == null || list.Count == 0)
		{
			return null;
		}
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!string.IsNullOrEmpty(list[i]))
			{
				hashSet.Add(list[i]);
			}
		}
		if (hashSet.Count <= 0)
		{
			return null;
		}
		return hashSet;
	}

	private void ParseRateList(List<string> rateList, out string baseRate, out string totalRate)
	{
		baseRate = "0";
		totalRate = "0";
		if (rateList != null && rateList.Count != 0)
		{
			baseRate = rateList[0]?.Trim() ?? "0";
			if (rateList.Count > 1)
			{
				totalRate = rateList[1]?.Trim() ?? "0";
			}
		}
	}
}
