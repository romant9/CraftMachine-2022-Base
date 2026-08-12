using BaseModel;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTraitSelector : TooltipBox, IPointerDownHandler, IPointerUpHandler
{   
    [SerializeField]
    private NUIScrollableList scrollableList;

    private int currenctIndex = -1;

    public const string defaultComponentResourcePath = "Trait_Icon";

    private List<string> UpgradeTraitsDataList;

    public UpgradeTraitsData RootData;
    public EquipmentItemModel equipmentItemModel;

    public bool IsClickOver;

    public void UpdateWithParams(UpgradeTraitsData data, EquipmentItemModel model)
    {
        currenctIndex = 1;
        RootData = SetUpgradeTraitData(data);
        equipmentItemModel = model;
    }

    public void SetLayer(int layer)
    {
        NGUITools.SetLayer(gameObject, layer);
    }

    public override void Show()
    {
        var playerModel = TwdCustomMod.DataManager.Instance.Player;
        if (scrollableList == null) return;

        base.Show();
        if (currenctIndex == -1)
        {
            return;
        }

        string lastTraitSpriteName = HelpersGfx.GetEquipmentTraitIconName(RootData);
        var traitLevel = lastTraitSpriteName.Split('_').Last();

        var first = "Ui_Icon_Trait_Unknown";
        string text2 = "";
        switch (RootData.RarityLevel)
        {
            case 0:
                text2 = "_Low";
                break;
            case 1:
                text2 = "_Mid";
                break;
            case 2:
            case 3:
            case 4:
            case 5:
                text2 = "_High";
                break;
            default:
                text2 = "";
                break;
        }
        first += text2;

        UpgradeTraitsDataList = new List<string>() { first };

        UpgradeTraitsDataList.AddRange(GetTraitsIdsList());
      
        scrollableList.UpdateWithList(UpgradeTraitsDataList, defaultComponentResourcePath, null);
        if (currenctIndex == 0)
        {
            scrollableList.RepositionItemsHorizontal();
        }
        else
        {
            scrollableList.RepositionItemsFillDownwards();
        }
        scrollableList.ResetScrollPosition();
        for (int j = 0; j < scrollableList.currentItemsList.Count; j++)
        {
            var item = scrollableList.currentItemsList[j];
            if (item != null && item is TraitSelectorButton)
            {
                item.UpdateUI();
                (item as TraitSelectorButton).Initialize(UpgradeTraitsDataList[j], OnClickBadges);
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
        if (scrollableList != null)
        {
            scrollableList.Clear();
        }
        if (UpgradeTraitsDataList != null)
        {
            UpgradeTraitsDataList.Clear();
            UpgradeTraitsDataList = null;
        }
        currenctIndex = -1;
    }

    private void OnClickBadges(UIButtonExtended button)
    {
        if (button == null)
        {
            return;
        }
        NUIListItem<TraitSelectorButton> component = button.GetComponent<NUIListItem<TraitSelectorButton>>();
        if (component != null)
        {
            UIEvent.Send("UpdateTraitInstance", component);

            Hide();
        }
    }

    public UpgradeTraitsData SetUpgradeTraitData(UpgradeTraitsData upgradeTraitsData)
    {
        var rootData = new UpgradeTraitsData();
        //RootData.RemodelIng = true;
        rootData.Identifier = upgradeTraitsData.Identifier;
        rootData.ThisRemodeIds = upgradeTraitsData.ThisRemodeIds;
        rootData.ThisRemodeValues = upgradeTraitsData.ThisRemodeValues;
        rootData.ThisRemodeParamIndex = upgradeTraitsData.ThisRemodeParamIndex;
        rootData.RarityLevel = upgradeTraitsData.RarityLevel;
        return rootData;
    }

    public List<string> GetTraitsIdsList()
    {
        var gameEconomyData = DataManager.Instance.GameData;
        List<string> TraitIconList = new List<string>();
        int upgradeTraitsDataIndex = equipmentItemModel.GetUpgradeTraitsDataIndex(RootData.Identifier);

        int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(RootData.Identifier);
        List<string> expectTratIdList = GetExpectTratIdList(traitLevelIdentifier);
        List<EquipTraitsDefinition> equipTraitsDefinitions = gameEconomyData.getEquipTraitsDefinitions(equipmentItemModel.Definition.SurvivorClass, equipmentItemModel.Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, expectTratIdList);

        //if (RootData.RemodeValues == null)
        //{
        //    RootData.ThisRemodeValues = new Dictionary<string, List<int>>();
        //    RootData.ThisRemodeParamIndex = new Dictionary<string, List<int>>();
        //}
        //List<EquipTraitsDefinition> list = DataManager.Instance.Player.PlayerRandom.WeightedRandomList(equipTraitsDefinitions, 2, (EquipTraitsDefinition x) => 1L, isRepeat: false);
        //foreach (EquipTraitsDefinition item in list)
        //{
        //    if (!RootData.ThisRemodeValues.TryGetValue(item.TraitsGroup, out var value))
        //    {
        //        value = item.MinConstructionParameters;
        //    }
        //    List<int> list2 = new List<int>();
        //    for (int i = 0; i < value.Count; i++)
        //    {
        //        list2.Add(DataManager.Instance.Player.PlayerRandom.GetRandomInRange(value[i], item.MaxConstructionParameters[i]));
        //    }
        //    RootData.ThisRemodeValues[item.TraitsGroup] = list2;
        //    RootData.ThisRemodeParamIndex[item.TraitsGroup] = item.ConstructionParametersNumber;
        //    //RootData.RemodelIng = true;
        //}

        for (int i = 0; i < equipTraitsDefinitions.Count; i++)
        {
            var trait = equipTraitsDefinitions[i];
            TraitIconList.Add(HelpersGfx.GetEquipmentTraitIconNameUsingTraitDefinition(trait));
        }
        return TraitIconList;
        //return equipTraitsDefinitions.Select((EquipTraitsDefinition x) => x.TraitsGroup).ToList();
    }

    public List<string> GetExpectTratIdList(int level)
    {
        var UpgradeTraits = equipmentItemModel.UpgradeTraits;
        List<string> list = new List<string>();
        for (int i = 0; i < UpgradeTraits.Count; i++)
        {
            int num = UpgradeTraits[i].Identifier.LastIndexOf('.');
            if (num != -1)
            {
                string item = UpgradeTraits[i].Identifier.Substring(0, num) + ".Level" + level;
                list.Add(item);
            }
            EquipTraitsMutualExclusion equipTraitsMutualExclusion = DataManager.Instance.GameData.getEquipTraitsMutualExclusion(UpgradeTraits[i].Identifier);
            if (equipTraitsMutualExclusion != null)
            {
                list.AddRange(equipTraitsMutualExclusion.MutualExclusionTraits);
            }
        }
        return list.Distinct().ToList();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsClickOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsClickOver = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsClickOver = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsClickOver = false;
    }
}
