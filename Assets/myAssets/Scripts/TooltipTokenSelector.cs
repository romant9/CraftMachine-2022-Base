using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTokenSelector : TooltipBox, IPointerDownHandler, IPointerUpHandler
{   
    [SerializeField]
    private NUIScrollableList scrollableList;

    private int currenctIndex = -1;

    public const string defaultComponentResourcePath = "Token_Icon";

    private List<string> TokensDataList;

    private bool IsClickOver;


    public void UpdateWithParams()
    {
        currenctIndex = 1;
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

        //string lastTraitSpriteName = HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency);

        TokensDataList = new List<string>() { string.Empty };

        var tokenDefinitions = GameManager.Instance.gameEconomyData.ItemDefinitions.Where(x => x.Type == "Tokens").Select(x=>x.ItemName).ToList();

        TokensDataList.AddRange(tokenDefinitions.GetRange(7, tokenDefinitions.Count - 7));
        TokensDataList.Add("GauntletAaronToken");
        scrollableList.UpdateWithList(TokensDataList, defaultComponentResourcePath, null);
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
            if (item != null && item is TokenSelectorButton)
            {
                item.UpdateUI();
                (item as TokenSelectorButton).Initialize(TokensDataList[j], OnClickToken);
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
        if (TokensDataList != null)
        {
            TokensDataList.Clear();
            TokensDataList = null;
        }
        currenctIndex = -1;
    }

    private void OnClickToken(UIButtonExtended button)
    {
        if (button == null)
        {
            return;
        }
        if (button.TryGetComponent<NUIListItem<TokenSelectorButton>>(out var component))
        {
            UIEvent.Send("UpdateTokenInstance", component);
            Hide();
        }
    }

    //public List<string> GetTraitsIdsList()
    //{
    //    var gameEconomyData = DataManager.Instance.GameData;
    //    List<string> TraitIconList = new List<string>();
    //    int upgradeTraitsDataIndex = equipmentItemModel.GetUpgradeTraitsDataIndex(RootData.Identifier);

    //    int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(RootData.Identifier);
    //    List<string> expectTratIdList = GetExpectTratIdList(traitLevelIdentifier);
    //    List<EquipTraitsDefinition> equipTraitsDefinitions = gameEconomyData.getEquipTraitsDefinitions(equipmentItemModel.Definition.SurvivorClass, equipmentItemModel.Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, expectTratIdList);

    //    for (int i = 0; i < equipTraitsDefinitions.Count; i++)
    //    {
    //        var trait = equipTraitsDefinitions[i];
    //        TraitIconList.Add(HelpersGfx.GetEquipmentTraitIconNameUsingTraitDefinition(trait));
    //    }
    //    return TraitIconList;
    //}

    //public List<string> GetExpectTratIdList(int level)
    //{
    //    var UpgradeTraits = equipmentItemModel.UpgradeTraits;
    //    List<string> list = new List<string>();
    //    for (int i = 0; i < UpgradeTraits.Count; i++)
    //    {
    //        int num = UpgradeTraits[i].Identifier.LastIndexOf('.');
    //        if (num != -1)
    //        {
    //            string item = UpgradeTraits[i].Identifier.Substring(0, num) + ".Level" + level;
    //            list.Add(item);
    //        }
    //        EquipTraitsMutualExclusion equipTraitsMutualExclusion = DataManager.Instance.GameData.getEquipTraitsMutualExclusion(UpgradeTraits[i].Identifier);
    //        if (equipTraitsMutualExclusion != null)
    //        {
    //            list.AddRange(equipTraitsMutualExclusion.MutualExclusionTraits);
    //        }
    //    }
    //    return list.Distinct().ToList();
    //}

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
