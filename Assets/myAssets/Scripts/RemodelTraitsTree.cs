using BaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class RemodelTraitsTree : MonoBehaviour
{
    private bool IsOpenRemodelTree;

    public TraitsRemodelItem mainTraitItem;

    [SerializeField]
    private TraitsRemodelItem traitContainerPrefab;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private UIButtonExtended exitButton;

    public EquipmentItemModel equipmentItemModel;
    public UpgradeTraitsData upgradeTraitsData;
    public string upgradeTraitsDataIdentifierOrigin;

    private List<TraitsRemodelItem> AllTreeItems = new List<TraitsRemodelItem>();

    public int AllTreeItemsCount
    {
        get { return AllTreeItems != null ? AllTreeItems.Count : 0; }
    }

    public Dictionary<string, EqupmentWithTraitsClass> EqipmentRemodelTraitsList = new Dictionary<string, EqupmentWithTraitsClass>();

    public List<int> RemodelLevelPath;
    public int Levels;
    public int oldLevels { get; private set; }

    //индекс черты в списке всех черт модели
    public int traitIndex;

    public int offsetX1 = 720;
    public int offsetX2 = 240;
    public int offsetX3 = 60;
    public int offsetX4 = 30;

    public int offsetY1 = 100;
    public int offsetY2 = 80;
    public int offsetY3 = 120;
    public int offsetY4 = 60;

    public RemodelPopup remodelPopup { get; private set; }

    private void Awake()
    {
        exitButton.SetClickCallback(ClosePanel);
    }

    public void SetRemodelPopup(RemodelPopup remodel)
    {
        remodelPopup = remodel;
    }

    public void SwitchFourLevel(UIToggle tg)
    {
        if (remodelPopup.IsBatchRemodelTree)
        {
            Levels = 1;
        }
        else
        {
            Levels = tg.value ? 4 : 3;
        }
    }

    public IEnumerator UpdateWith(UpgradeTraitsData traitData, TraitsRemodelItem root)
    {
        string lastTraitSpriteName = HelpersGfx.GetEquipmentTraitIconName(traitData);
        var traitLevel = lastTraitSpriteName.Split('_').Last();

        DebugTWD.Log("traitsItemOrigin : " + lastTraitSpriteName);

        UpgradeTraitsData upgradeTraitsDataNew = new UpgradeTraitsData();
        upgradeTraitsDataNew.RemodelIng = true;
        upgradeTraitsDataNew.Identifier = traitData.ThisRemodeIds[0];
        upgradeTraitsDataNew.ThisRemodeIds = traitData.ThisRemodeIds;
        upgradeTraitsDataNew.ThisRemodeValues = traitData.ThisRemodeValues;
        upgradeTraitsDataNew.ThisRemodeParamIndex = traitData.ThisRemodeParamIndex;
        upgradeTraitsDataNew.RarityLevel = traitData.RarityLevel;

        TraitsRemodelItem traitsItemFirst = Instantiate(traitContainerPrefab, root.transform);
        traitsItemFirst.gameObject.SetActive(true);
        traitsItemFirst.name = "First";
        traitsItemFirst.TreeLevel = root.TreeLevel + 1;
        traitsItemFirst.remodelTraitPosition = RemodelTraitPosition.First;
        traitsItemFirst.parent = root;
        traitsItemFirst.SetTraitDataToButton(upgradeTraitsDataNew, traitLevel, this);
        traitsItemFirst.transform.localPosition = MoveButton(traitsItemFirst);
        traitsItemFirst.SetLines();
        AllTreeItems.Add(traitsItemFirst);

        TraitsRemodelItem traitsItemSecond = Instantiate(traitContainerPrefab, root.transform);
        traitsItemSecond.gameObject.SetActive(true);
        traitsItemSecond.name = "Second";
        traitsItemSecond.TreeLevel = root.TreeLevel + 1;
        traitsItemSecond.remodelTraitPosition = RemodelTraitPosition.Second;
        traitsItemSecond.parent = root;
        upgradeTraitsDataNew.Identifier = traitData.ThisRemodeIds[1];

        traitsItemSecond.SetTraitDataToButton(upgradeTraitsDataNew, traitLevel, this);
        traitsItemSecond.transform.localPosition = MoveButton(traitsItemSecond);
        traitsItemSecond.SetLines();
        AllTreeItems.Add(traitsItemSecond);

        TraitsRemodelItem traitsItemBack = Instantiate(traitContainerPrefab, root.transform);
        traitsItemBack.gameObject.SetActive(true);
        traitsItemBack.name = "Back";
        traitsItemBack.TreeLevel = root.TreeLevel + 1;
        traitsItemBack.remodelTraitPosition = RemodelTraitPosition.Back;
        traitsItemBack.parent = root;
        traitsItemBack.SetTraitDataToButton(traitData, traitLevel, this);
        traitsItemBack.transform.localPosition = MoveButton(traitsItemBack);
        traitsItemBack.SetLines();
        AllTreeItems.Add(traitsItemBack);

        yield return null;

        root.childItems = new List<TraitsRemodelItem>() { traitsItemFirst, traitsItemSecond, traitsItemBack };
        root.modelRandom = new ModelRandom(DataManager.Instance.Player.PlayerRandom);

        if (root.TreeLevel + 1 >= Levels)
        {
            yield break;
        }

        for (int i = 0; i < root.childItems.Count; i++)
        {
            var child = root.childItems[i];
            DataManager.Instance.Player.PlayerRandom = new ModelRandom(root.modelRandom);
            var traitsData = child.upgradeTraitsData;
            SelectBtn(traitsData, i);
            child.upgradeTraitsData = CopyTraitsData(traitsData);

            OnClickRemodelBtn(child);
            yield return null;
        }

        yield return null;
    }

    private Vector3 MoveButton(TraitsRemodelItem btn)
    {
        RemodelTraitPosition index = btn.remodelTraitPosition;
        int sign;
        switch (index)
        {
            case RemodelTraitPosition.First: sign = -1; break;
            case RemodelTraitPosition.Second: sign = 1; break;
            case RemodelTraitPosition.Back: sign = 0; break;
            default: sign = 0; break;
        }

        Vector3 offset;
        switch (btn.TreeLevel)
        {
            case 1:
                offset = new Vector3(offsetX1 * sign, -offsetY1, 0);               
                break;
            case 2:
                offset = new Vector3(offsetX2 * sign, -offsetY1, 0);
                break;
            case 3:
                offset = new Vector3(offsetX3 * sign, -offsetY1, 0);
                break;
            case 4:
                offset = new Vector3(offsetX4 * sign, btn.parent.remodelTraitPosition == RemodelTraitPosition.Back ? -offsetY3 : -offsetY2, 0);
                btn.transform.localScale = Vector3.one * .7f;
                break;
            default:
                offset = new Vector3(offsetX3 * sign, -offsetY1, 0);
                break;
        }
        btn.Line_Horiz_Left.width = Math.Abs((int)offset.x);
        btn.Line_Horiz_Right.width = Math.Abs((int)offset.x);

        if (btn.TreeLevel == 2)
        {
            btn.Line_Vert_Down.height = Levels == 4 ? offsetY2 + offsetY3 - offsetY4 : 20;
        }
        if (btn.TreeLevel == 3)
        {
            btn.Line_Vert_Down.transform.localScale = Vector3.one * .7f;
        }
        if (btn.TreeLevel == 4)
        {
            btn.Line_Horiz_Left.width = btn.Line_Horiz_Left.width * 10/7;
            btn.Line_Horiz_Right.width = btn.Line_Horiz_Right.width * 10 / 7;
        }
        return offset;
    }

    private void ClosePanel(UIButtonExtended button)
    {
        DestroyAll();
        DataManager.Instance.SurvivorManagementPopUp.ResetTraitsData(true);

        gameObject.SetActive(false);
    }

    public void DestroyAll()
    {
        int count = AllTreeItemsCount;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Destroy(AllTreeItems[i].gameObject);
            }
            AllTreeItems.Clear();
        }
    }

    public void SelectBtn(UpgradeTraitsData upgradeTraitsData, int i_selectIndex)
    {
        TWDModelResult tWDModelResult = SelectRemodeId(upgradeTraitsData, i_selectIndex);
        if (tWDModelResult == TWDModelResult.OK)
        {
            DebugTWD.Log("SelectRemodeId is OK for " + upgradeTraitsData.Identifier + " " + i_selectIndex);
            //UIEvent.Send("EquipmentRemodelSelectioned");
        }
    }

    //Выбрать нужный Trait после кнопки Remodel
    public TWDModelResult SelectRemodeId(UpgradeTraitsData upgradeTraitsData, int selectIndex)
    {
        if (upgradeTraitsData.ThisRemodeIds == null)
        {
            return TWDModelResult.Error;
        }
        if (selectIndex == 2)
        {
            upgradeTraitsData.ThisRemodeIds.Clear();
            upgradeTraitsData.RemodelIng = false;
            upgradeTraitsData.RemodelEd = true;
            return TWDModelResult.OK;
        }       
        upgradeTraitsData.Identifier = upgradeTraitsData.ThisRemodeIds[selectIndex];
        if (upgradeTraitsData.ThisRemodeValues.TryGetValue(upgradeTraitsData.Identifier, out var value) && upgradeTraitsData.ThisRemodeParamIndex.TryGetValue(upgradeTraitsData.Identifier, out var _))
        {
            upgradeTraitsData.RemodeValues = value;
        }       
        upgradeTraitsData.ThisRemodeIds.Clear();
        upgradeTraitsData.RemodelIng = false;
        upgradeTraitsData.RemodelEd = true;
        return TWDModelResult.OK;
    }

    //Результат после нажатия на Remodel
    public TWDModelResult EquipmentRemodel(UpgradeTraitsData upgradeTraitsData)
    {
        var gameEconomyData = DataManager.Instance.GameData;
        int upgradeTraitsDataIndex = traitIndex;

        int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(upgradeTraitsData.Identifier);
        List<string> expectTratIdList = GetExpectTratIdList(traitLevelIdentifier);
        List<EquipTraitsDefinition> equipTraitsDefinitions = gameEconomyData.getEquipTraitsDefinitions(equipmentItemModel.Definition.SurvivorClass, equipmentItemModel.Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, expectTratIdList);
        if (equipTraitsDefinitions.Count < 2)
        {
            return TWDModelResult.OK;
        }
        
        if (upgradeTraitsData.RemodeValues == null)
        {
            upgradeTraitsData.ThisRemodeValues = new Dictionary<string, List<int>>();
            upgradeTraitsData.ThisRemodeParamIndex = new Dictionary<string, List<int>>();
        }
        List<EquipTraitsDefinition> list = DataManager.Instance.Player.PlayerRandom.WeightedRandomList(equipTraitsDefinitions, 2, (EquipTraitsDefinition x) => 1L, isRepeat: false);
        foreach (EquipTraitsDefinition item in list)
        {
            if (!upgradeTraitsData.ThisRemodeValues.TryGetValue(item.TraitsGroup, out var value))
            {
                value = item.MinConstructionParameters;
            }
            List<int> list2 = new List<int>();
            for (int i = 0; i < value.Count; i++)
            {
                list2.Add(DataManager.Instance.Player.PlayerRandom.GetRandomInRange(value[i], item.MaxConstructionParameters[i]));
            }
            upgradeTraitsData.ThisRemodeValues[item.TraitsGroup] = list2;
            upgradeTraitsData.ThisRemodeParamIndex[item.TraitsGroup] = item.ConstructionParametersNumber;
        }
        upgradeTraitsData.ThisRemodeIds = list.Select((EquipTraitsDefinition x) => x.TraitsGroup).ToList();
        upgradeTraitsData.RemodelIng = true;
        return TWDModelResult.OK;
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

    public bool IsFineConstruct(UpgradeTraitsData traitsData, string contruct, out List<int> Max)
    {
        Max = new List<int>() { 0 };
        List<string> contructArray = contruct.Contains("/") ? contruct.Split('/').ToList() : new List<string>() { contruct };

        var equipTraitsDefinition = DataManager.Instance.GameData.GetEquipTraitsDefinition(equipmentItemModel.Definition.SurvivorClass, equipmentItemModel.Definition.Category, traitsData.Identifier);

        bool IsFineConstruct = false;
        if (equipTraitsDefinition != null)
        {
            Max = equipTraitsDefinition.MaxConstructionParameters;

            IsFineConstruct = true;
            for (int i = 0; i < contructArray.Count; i++)
            {
                if (int.TryParse(contructArray[i], out int result) && result < Max[i]) IsFineConstruct = false;
            }
        }
        return IsFineConstruct;
    }

    public void OnClickRemodelBtn(TraitsRemodelItem root)
    {
        DebugTWD.Log("Need to Initiate Survivors - Turn Off", DebugType.System);

        UpgradeTraitsData data = root.upgradeTraitsData;
        int equipmentRemodelRarity = DataManager.Instance.GameData.ConfigData.EquipmentRemodelRarity;
        if (equipmentItemModel.RarityLevel >= equipmentRemodelRarity && data.UnlockingLevel <= equipmentItemModel.Level)
        {
            TWDModelResult tWDModelResult = EquipmentRemodel(data);
            DebugTWD.Log("Remodel result is " + tWDModelResult, DebugType.Craft);
            if (tWDModelResult != TWDModelResult.OK) 
            {
                DebugTWD.Log("Что-то пошло не так " + tWDModelResult, DebugType.Craft);
                return;
            }
            else
            {
                StartCoroutine(UpdateWith(data, root));
            }
        }
        else
        {
            DebugTWD.Log("Что-то пошло не так ", DebugType.Craft);
        }
    }

    public void SwitchRerollType()
    {
        remodelPopup.IsOpenRemodelTree = IsOpenRemodelTree;
    }

    public void SwitchRerollTypeToggle(UIToggle tg)
    {
        IsOpenRemodelTree = tg.value;
        remodelPopup.IsOpenRemodelTree = tg.value;
    }

    public IEnumerator Main(bool IsBackup = false)
    {
        if (!remodelPopup.IsBatchRemodelTree)
        {
            if (Levels != oldLevels && oldLevels > 1)
            {
                Levels = oldLevels;
            }
        }
        else
        {
            if (Levels > 1)
            {
                oldLevels = Levels;
                Levels = 1;
            }
        }

        if (IsBackup) BackupTraitsData(equipmentItemModel, upgradeTraitsData);

        mainTraitItem.TreeLevel = 0;
        string lastTraitSpriteName = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsData);
        var traitLevel = lastTraitSpriteName.Split('_').Last();

        mainTraitItem.SetTraitDataToButton(upgradeTraitsData, traitLevel, this, true);
        mainTraitItem.SetLines();

        OnClickRemodelBtn(mainTraitItem);

        int x = 1;
        var g1 = 3 * x;
        var g2 = 3 * g1;
        var g3 = 3 * g2;
        var g4 = 3 * g3;
        
        int count = g1 + (Levels > 1 ? g2 : 0) + (Levels > 2 ? g3 : 0) + (Levels > 3 ? g4 : 0);

        DebugTWD.Log("Calculate Count is " + count);
        yield return new WaitUntil(() => AllTreeItemsCount >= count);

        ResetTraitsData();

        DebugTWD.Log("All rerolled!!!");
    }

    public void BackupTraitsData(EquipmentItemModel model, UpgradeTraitsData traitsData)
    {
        if (EqipmentRemodelTraitsList == null) { return; }

        string name = model.IdForAnalytics;
        if (!EqipmentRemodelTraitsList.ContainsKey(name))
        {
            var item = new EqupmentWithTraitsClass(model, traitsData);
            EqipmentRemodelTraitsList.Add(name, item);
        }
    }

    public UpgradeTraitsData CopyTraitsData(UpgradeTraitsData traitsData)
    {
        var OriginDataSerialised = OfflineManager.JsonSerializer.Serialize(traitsData);
        return OfflineManager.JsonSerializer.Deserialize<UpgradeTraitsData>(OriginDataSerialised);
    }

    public void ResetTraitsData()
    {
        if (EqipmentRemodelTraitsList != null && EqipmentRemodelTraitsList.Count > 0)
        {
            var model = EqipmentRemodelTraitsList.First();
            var equipmentDeser = OfflineManager.JsonSerializer.Deserialize<EquipmentItemModel>(model.Value.OriginModelSerialised);
            var upgradeTraitsDeser = OfflineManager.JsonSerializer.Deserialize<UpgradeTraitsData>(model.Value.OriginDataSerialised);

            DataManager.Instance.Player.PlayerRandom = new ModelRandom(model.Value.PlayerRandom);

            DebugTWD.LogWarning("Restore Random is : " + DataManager.Instance.Player.PlayerRandom.State, DebugType.Random);

            equipmentItemModel = DataManager.Instance.Player.Equipment.ChangeEqupmentModel(equipmentDeser, out bool isWeapon);
            upgradeTraitsData = equipmentItemModel.UpgradeTraits.First(x=>x.Identifier == upgradeTraitsDeser.Identifier);
            var survivor = equipmentItemModel.Owner;

            if (survivor != null)
            {
                survivor.EquipmentItems.Models[isWeapon ? 1 : 0] = equipmentItemModel;
            }
            EqipmentRemodelTraitsList.Clear();
        }

        if (remodelPopup.IsBatchRemodelTree)
        {
            remodelPopup.batchCount++;
        }
    }

    public enum RemodelTraitPosition
    {
        First,
        Second,
        Back
    }
}

