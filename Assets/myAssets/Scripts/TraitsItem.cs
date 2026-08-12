using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;
using static SurvivorManagementPopUp;

public class TraitsItem : MonoBehaviour
{
    public List<UIButtonWithLabelAndIcon> currentTraitButtons;

    public Transform middleRoot;
    public Transform middleLine;

    public UIGrid grid;

    public SurvivorTraits traits;

    public TraitDefinition[] currentTraitDefinitions = new TraitDefinition[3];

    public bool IsFinished = false;

    public int Level = 1;


    void Start()
    {
        
    }

}
