using BaseModel;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static RemodelTraitsTree;


public class TraitsRemodelItem : MonoBehaviour
{
    public UIButtonExtended currentTraitButton;
    public UpgradeTraitsData upgradeTraitsData;
    public ModelRandom modelRandom;
    public List<TraitsRemodelItem> childItems;

    public UISprite traitIcon;
    public UILabel traitLabel;

    public UISprite Line_Horiz_Left;
    public UISprite Line_Horiz_Right;
    public UISprite Line_Vert_Up;
    public UISprite Line_Vert_Down;

    public bool IsFineConstruct;
    public UISprite IsFineUI;

    public TraitDefinition currentTrait;

    public bool IsFinished;

    public int TreeLevel;

    public TraitsRemodelItem parent;

    public RemodelTraitPosition remodelTraitPosition;

    public List<int> MaxConstructParameters;

    public RemodelTraitsTree remodelTraitsTree { get; private set; }


    public void SetTraitDataToButton(UpgradeTraitsData traitsData, string traitLevel, RemodelTraitsTree tree, bool isFirst = false)
    {
        remodelTraitsTree = tree;
        if (remodelTraitPosition != RemodelTraitPosition.Back)
        {
            string FirstTraitDescr;
            string traitReSprite1Name;
            string constructParams = "";
            if (isFirst)
            {
                FirstTraitDescr = HelpersLocalization.GetLastInstantiatedTraitDescription(traitsData, out constructParams);
            }
            else
            {
                FirstTraitDescr = HelpersLocalization.GetInstantiatedTraitDescription(traitsData, out constructParams);
            }
            traitReSprite1Name = HelpersGfx.GetEquipmentTraitIconName(traitsData);
            var level1 = traitReSprite1Name.Split('_').Last();
            if (traitLevel == "Highest" && traitReSprite1Name.Contains("Tactical"))
                traitReSprite1Name = traitReSprite1Name.Replace("Tactical", "ArmorTactical");
            var FirstTraitSpriteName = traitReSprite1Name.Replace(level1, traitLevel);

            //DebugTWD.Log("traitsItem " + (isFirst ? "Original " : remodelTraitPosition == RemodelTraitPosition.First ? "First " : "Second ") + traitReSprite1Name + " __ " + FirstTraitSpriteName);

            UIButtonExtended traitButton = this.currentTraitButton;
            var treeLevel = this.TreeLevel;

            traitLabel.text = constructParams;
            List<string> descrList = FirstTraitDescr.Split(':').ToList();

            if (constructParams != "0")
            {
                IsFineConstruct = remodelTraitsTree.IsFineConstruct(traitsData, constructParams, out MaxConstructParameters);
                if (IsFineConstruct) IsFineUI.gameObject.SetActive(true);

                if (TreeLevel == 1 && remodelTraitsTree.remodelPopup.IsBatchRemodelTree && IsFineConstruct &&
                    !string.IsNullOrEmpty(remodelTraitsTree.remodelPopup.TraintInstanceID))
                {
                    if (remodelTraitsTree.remodelPopup.TraintInstanceID.Contains("Unknown") || FirstTraitSpriteName == remodelTraitsTree.remodelPopup.TraintInstanceID)
                        remodelTraitsTree.remodelPopup.IsFineConstructFinded = true;
                }
                if (MaxConstructParameters.Count > 0 && MaxConstructParameters.First() != 0)
                {
                    descrList.Insert(1, " Max : " + (MaxConstructParameters.Count == 1 ? MaxConstructParameters[0].ToString() : string.Join("/", MaxConstructParameters)));                
                }
            }

            traitIcon.spriteName = FirstTraitSpriteName;

            traitButton.SetClickCallback(delegate
            {
                TooltipManager.OpenTextBoxWithText(traitButton.gameObject, string.Join("\n", descrList));
            });
        }
        else
        {
            traitIcon.transform.parent.gameObject.SetActive(false);
        }

        //SetLines();

        this.upgradeTraitsData = CopyTraitsData(traitsData);       
    }

    public void SetLines()
    {
        if (this.TreeLevel == 0)
        {
            Line_Vert_Up.gameObject.SetActive(false);
            Line_Horiz_Left.gameObject.SetActive(false);
            Line_Horiz_Right.gameObject.SetActive(false);
        }
        else
        {
            switch (remodelTraitPosition)
            {
                case RemodelTraitPosition.First:
                    Line_Horiz_Left.gameObject.SetActive(false);
                    break;

                case RemodelTraitPosition.Second:
                    Line_Horiz_Right.gameObject.SetActive(false);
                    break;

                case RemodelTraitPosition.Back:
                    Line_Horiz_Left.gameObject.SetActive(false);
                    Line_Horiz_Right.gameObject.SetActive(false);
                    break;
            }
        } 
        if (this.TreeLevel == 3)
        {
            if (remodelTraitsTree.Levels == 4)
            {
                if (this.remodelTraitPosition == RemodelTraitPosition.Back)
                {
                    Line_Vert_Down.gameObject.SetActive(false);
                }
            }
            else
            {
                if (this.remodelTraitPosition == RemodelTraitPosition.Back)
                {
                    Line_Vert_Up.gameObject.SetActive(false);
                }
                Line_Vert_Down.gameObject.SetActive(false);
            }
        }
        if (this.TreeLevel == 4)
        {
            if (this.remodelTraitPosition == RemodelTraitPosition.Back)
            {
                Line_Vert_Up.gameObject.SetActive(false);
            }
            Line_Vert_Down.gameObject.SetActive(false);
        }
    }

    private UpgradeTraitsData CopyTraitsData(UpgradeTraitsData traitsData)
    {
        var OriginDataSerialised = OfflineManager.JsonSerializer.Serialize(traitsData);
        return OfflineManager.JsonSerializer.Deserialize<UpgradeTraitsData>(OriginDataSerialised);
    }
}
