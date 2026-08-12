using BaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorTraitTree : MonoBehaviour
{
    private bool IsOpenTraitsTree;

    public UIButtonWithLabelAndIcon mainTrait;

    [SerializeField]
    private TraitsItem traitContainerPrefab;
    [SerializeField]
    private TraitsItem traitContainerPrefabLast;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private UIButtonExtended exitButton;

    public SurvivorModel survivorModel;

    private List<TraitsItem> containersAll = new List<TraitsItem>();

    private List<TraitsItem> containers1 = new List<TraitsItem>();
    private List<TraitsItem> containers2 = new List<TraitsItem>();
    private List<TraitsItem> containers3 = new List<TraitsItem>();
    private List<TraitsItem> containers4 = new List<TraitsItem>();

    int level = 1;

    public int offset1 = 720;
    public int offset2 = 240;
    public int offset3 = 60;
    public int offset4 = 30;

    public int offsetY1 = 100;
    public int offsetY2 = 190;

    public bool IsFoursLevel;

    private TraitDefinition traitDefinitionOriginal;
    private int indexButtonSaved;

    private void Awake()
    {
        exitButton.SetClickCallback(ClosePanel);
    }

    public void SwitchFourLevel(UIToggle tg)
    {
        IsFoursLevel = tg.value;

        if (containersAll.Count > 10)
        {
            DestroyAll();
            DataManager.Instance.SurvivorManagementPopUp.rerollTraitIndexCurrent = indexButtonSaved;
            StartCoroutine(Main(traitDefinitionOriginal));
        }
    }

    public IEnumerator UpdateWith(List<TraitsItem> containers, TraitDefinition traitDefinition, Transform root)
    {
        if (containers.Count > 40)
        {
            DataManager.Instance.SurvivorManagementPopUp.ResetTraitsData(true);
            yield break;
        }

        var data = DataManager.Instance.GameData;

        int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitDefinition.Identifier);

        TraitsItem traitsItem = Instantiate(traitContainerPrefab, root);
        traitsItem.transform.localPosition = new Vector3(0, level == 4 && root.name == "TraitChoice.Back" ? -offsetY2 : -offsetY1, 0);
        traitsItem.Level = level;
        traitsItem.traits = BackupTraitsData(traitDefinition);

        //Debug.Log("traitDefinition.Identifier : " + traitDefinition.Identifier);
        //Debug.Log("traitsItem.traits.RandomTraitsFromReroll.Count : " + traitsItem.traits.RandomTraitsFromReroll.Count);

        traitsItem.gameObject.SetActive(true);
        containers.Add(traitsItem);
        containersAll.Add(traitsItem);
        yield return null;

        foreach (var bt in traitsItem.currentTraitButtons)
        {
            MoveButton(bt.transform, traitsItem.currentTraitButtons.IndexOf(bt));
            yield return null;
        }

        //traitsItem.currentTraitDefinitions[0] = data.GetTraitDefinition(UpgradeTraitsData.CompileUpgradeTraitIdentifier(traitsItem.traits.RandomTraitsFromReroll[0], traitLevelIdentifier, isLocked: false));
        //SetTraitDataToButton(traitsItem.currentTraitButtons[0], traitsItem.currentTraitDefinitions[0], IsWithChoose: true, false);

        //ChooseTrait(0);
        //OnTraitRerollButtonClicked(traitsItem.currentTraitDefinitions[0].Identifier, 0, false, traitsItem.currentTraitButtons[0].transform);

        for (int i = 0; i < traitsItem.traits.RandomTraitsFromReroll.Count; i++)
        {
            traitsItem.currentTraitDefinitions[i] = data.GetTraitDefinition(UpgradeTraitsData.CompileUpgradeTraitIdentifier(traitsItem.traits.RandomTraitsFromReroll[i], traitLevelIdentifier, isLocked: false));
            SetTraitDataToButton(traitsItem.currentTraitButtons[i], traitsItem.currentTraitDefinitions[i]);
        }
        traitsItem.currentTraitDefinitions[2] = data.GetTraitDefinition(traitDefinition.Identifier);
        traitsItem.middleRoot.localPosition = Vector3.zero;

        //yield return null;

        if (!IsFoursLevel)
        {
            if (traitsItem.Level == 3 && containers2.IndexOf(traitsItem) != 2)
            {
                traitsItem.middleRoot.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            if (traitsItem.Level == 3 || traitsItem.Level == 4)
            {
                if (traitsItem.Level == 3)
                {
                    traitsItem.middleLine.gameObject.SetActive(true);
                }
                traitsItem.middleRoot.GetChild(0).gameObject.SetActive(false);
            }
        }
       
        yield return null;

        traitsItem.IsFinished = true;
    }

    private void MoveButton(Transform btn, int index)
    {
        int sign = index == 0 ? -1 : 1;
        Vector3 offset;
        switch (level)
        {
            case 1:
                offset = new Vector3(offset1 * sign, 0, 0);
                break;
            case 2:
                offset = new Vector3(offset2 * sign, 0, 0);
                break;
            case 3:
                offset = new Vector3(offset3 * sign, 0, 0);
                break;
            case 4:
                offset = new Vector3(offset4 * sign, 0, 0);
                btn.localScale = Vector3.one * .7f;
                break;
            default:
                offset = new Vector3(offset3 * sign, 0, 0);
                break;
        }
        btn.localPosition = offset;
    }

    private void ClosePanel(UIButtonExtended button)
    {
        DestroyAll();
        DataManager.Instance.SurvivorManagementPopUp.ResetTraitsData(true);

        gameObject.SetActive(false);
    }

    public void DestroyAll()
    {
        int count = containersAll.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Destroy(containersAll[i].gameObject);
            }
            containersAll.Clear();
            containers1.Clear();
            containers2.Clear();
            containers3.Clear();
            containers4.Clear();

        }

    }

    private int BtIndex(string lastChar)
    {
        switch (lastChar)
        {
            case "l":
                return 0;
            case "r":
                return 1;
            default:
                return -1;
        }
    }

	public void SetTraitDataToButton(UIButtonWithLabelAndIcon traitButton, TraitDefinition traitDefinition)
	{
        if (level != 4)
            traitButton.SetContentToLabelTwo((UpgradeTraitsData.GetTraitLevelIdentifier(traitDefinition.Identifier) + 1).ToString());
        else 
            traitButton.secondLabel.gameObject.SetActive(false);
		traitButton.SetContentToIconOne(HelpersGfx.GetSurvivorTraitIconName(traitDefinition));
		traitButton.SetClickCallback(delegate
		{
			TooltipManager.OpenTextBoxWithText(traitButton.gameObject, HelpersLocalization.GetTraitName(traitDefinition) + '\n' + HelpersLocalization.GetTraitDescription(traitDefinition), CraftSettings.Instance.tooltipPrefab);
		});
    }

    private TWDModelResult ChooseTrait(int btIndex)
    {
        TWDModelResult tWDModelResult;
        if ((btIndex == -1 || btIndex == 0 || btIndex == 1) && !string.IsNullOrEmpty(survivorModel.TraitToBeRerolledCandidate) && survivorModel.RandomTraitsFromReroll != null && survivorModel.RandomTraitsFromReroll.Count == 2)
        {
            //Debug.Log("survivorModel.TraitToBeRerolledCandidate " + survivorModel.TraitToBeRerolledCandidate + "  " + btIndex);

            tWDModelResult = !survivorModel.ChooseRerolledTrait(btIndex) ? TWDModelResult.Error : TWDModelResult.OK;
            if (tWDModelResult != TWDModelResult.OK)
            {
               DebugTWD.LogError("reroll error");
            }
        }

        return TWDModelResult.OK;
    }

    public void OnTraitRerollButtonClicked(string traitIdentifier, Transform root, List<TraitsItem> containers)
    {
        TraitDefinition traitDefinition = DataManager.Instance.GameData.GetTraitDefinition(traitIdentifier);
        if (survivorModel != null && survivorModel.HasUpgradeTrait(traitIdentifier) && traitDefinition != null && !traitDefinition.HasTag("FactionBuffTrait") && !traitDefinition.Identifier.Equals("Overwatch", StringComparison.Ordinal))
        {
            TWDModelResult tWDModelResult = !survivorModel.RerollTrait(traitIdentifier) ? TWDModelResult.Error : TWDModelResult.OK;

            if (tWDModelResult == TWDModelResult.OK)
            {
                StartCoroutine(UpdateWith(containers, traitDefinition, root));
            }
            else 
            {
                DebugTWD.LogError("reroll error");
            }
        }
        else
        {
            DebugTWD.LogError("reroll error");
        }
    }

    public void SwitchRerollType()
    {
        DataManager.Instance.SurvivorManagementPopUp.IsOpenTraitsTree = IsOpenTraitsTree;
    }

    public void SwitchRerollTypeToggle(UIToggle tg)
    {
        IsOpenTraitsTree = tg.value;
        DataManager.Instance.SurvivorManagementPopUp.IsOpenTraitsTree = tg.value;
    }

    public void ResetTraitsData(SurvivorTraits survivorTraitsTree)
    {   
        List<UpgradeTraitsData> originUpgradeTraits = new List<UpgradeTraitsData>();
        foreach (var trait in survivorTraitsTree.UpgradeTraits)
        {
            originUpgradeTraits.Add(trait);
        }
        survivorModel.UpgradeTraits = originUpgradeTraits;
        survivorModel.TraitRandom = new ModelRandom(survivorTraitsTree.random);

        survivorModel.RandomTraitsFromReroll = survivorTraitsTree.RandomTraitsFromReroll != null ? new List<string>(survivorTraitsTree.RandomTraitsFromReroll) : null;
        survivorModel.PreviousRandomRolledTraits = survivorTraitsTree.PreviousRandomRolledTraits != null ? new List<string>(survivorTraitsTree.PreviousRandomRolledTraits) : null;

        survivorModel.TraitToBeRerolledCandidate = survivorTraitsTree.TraitToBeRerolledCandidate != null ? survivorTraitsTree.TraitToBeRerolledCandidate : null;
        
    }

    public SurvivorTraits BackupTraitsData(TraitDefinition traitDefinition)
    {
        //string name = survivorModel.IsHero ? survivorModel.FullName : survivorModel.SurvivorName;
     
        List<UpgradeTraitsData> originUpgradeTraits = new List<UpgradeTraitsData>();
        foreach (var trait in survivorModel.UpgradeTraits)
        {
            originUpgradeTraits.Add(trait);
        }

        SurvivorTraits survivorTraitsTree = new SurvivorTraits()
        {
            Survivor = survivorModel,
            UpgradeTraits = originUpgradeTraits,
            random = new ModelRandom(survivorModel.TraitRandom),
            traitDefinitionCurrent = traitDefinition,

            RandomTraitsFromReroll = survivorModel.RandomTraitsFromReroll != null ? new List<string>(survivorModel.RandomTraitsFromReroll) : null,
            PreviousRandomRolledTraits = survivorModel.PreviousRandomRolledTraits != null ? new List<string>(survivorModel.PreviousRandomRolledTraits) : null,

            TraitToBeRerolledCandidate = survivorModel.TraitToBeRerolledCandidate != null ? survivorModel.TraitToBeRerolledCandidate : null
        };
        return survivorTraitsTree;
    }

    public IEnumerator Main(TraitDefinition traitDefinition)
    {
        traitDefinitionOriginal = traitDefinition;
        int index = DataManager.Instance.SurvivorManagementPopUp.rerollTraitIndexCurrent - 1;
        indexButtonSaved = index;
        DataManager.Instance.SurvivorManagementPopUp.BackupTraitsData(survivorModel, traitDefinition, index, true);

        level = 1;
        SetTraitDataToButton(mainTrait, traitDefinition);
        OnTraitRerollButtonClicked(traitDefinition.Identifier, mainTrait.transform, containers1);

        

        yield return new WaitUntil(() => containers1.Count == 1 && containers1.Last().IsFinished);

        //DataManager.Instance.SurvivorManagementPopUp.ResetTraitsData(true);

        //var traitItem = containers1.Last();
        //ResetTraitsData(traitItem.traits);
        //ChooseTrait(0);
        //OnTraitRerollButtonClicked(traitItem.currentTraitDefinitions[0].Identifier, traitItem.currentTraitButtons[0].transform, containers2);
        //yield break;

        level = 2;
        foreach (var traitItem in containers1)
        {
            foreach (var traitDef in traitItem.currentTraitDefinitions)
            {
                int count = containers2.Count;
                int index1 = GetTraitDefinitionIndex(traitItem.currentTraitDefinitions.ToList().IndexOf(traitDef));

                ResetTraitsData(traitItem.traits);
                DebugTWD.Log("traitItem.traits.TraitToBeRerolledCandidate : " + traitItem.traits.TraitToBeRerolledCandidate);

                ChooseTrait(index1);

                OnTraitRerollButtonClicked(traitDef.Identifier, index1 != -1 ? traitItem.currentTraitButtons[index1].transform : traitItem.middleRoot, containers2);

                yield return new WaitUntil(() => containers2.Count == count + 1);
            }
        }

        yield return new WaitUntil(() => containers2.Count == 3 && containers1.Last().IsFinished);
        
        level = 3;
        foreach (var traitItem in containers2)
        {
            foreach (var traitDef in traitItem.currentTraitDefinitions)
            {
                int count = containers3.Count;
                int index1 = GetTraitDefinitionIndex(traitItem.currentTraitDefinitions.ToList().IndexOf(traitDef));

                ResetTraitsData(traitItem.traits);

                ChooseTrait(index1);

                OnTraitRerollButtonClicked(traitDef.Identifier, index1 != -1 ? traitItem.currentTraitButtons[index1].transform : traitItem.middleRoot, containers3);

                yield return new WaitUntil(() => containers3.Count == count + 1);
            }
        }

        yield return new WaitUntil(() => containers3.Count == 9 && containers2.Last().IsFinished);

        if (IsFoursLevel)
        {
            level = 4;
            foreach (var traitItem in containers3)
            {
                foreach (var traitDef in traitItem.currentTraitDefinitions)
                {
                    int count = containers4.Count;
                    int index1 = GetTraitDefinitionIndex(traitItem.currentTraitDefinitions.ToList().IndexOf(traitDef));

                    ResetTraitsData(traitItem.traits);

                    ChooseTrait(index1);

                    OnTraitRerollButtonClicked(traitDef.Identifier, index1 != -1 ? traitItem.currentTraitButtons[index1].transform : traitItem.middleRoot, containers4);

                    yield return new WaitUntil(() => containers4.Count == count + 1);
                }
            }

            yield return new WaitUntil(() => containers4.Count == 27 && containers3.Last().IsFinished);
        }
        
        DataManager.Instance.SurvivorManagementPopUp.ResetTraitsData(true);

        yield return null;

        DebugTWD.Log("all rerolled!!!");

    }

    private int GetTraitDefinitionIndex(int index)
    {
        if (index < 2) return index;
        else return -1;
    }
}